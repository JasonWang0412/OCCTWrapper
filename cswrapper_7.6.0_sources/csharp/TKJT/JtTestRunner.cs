// Created: 2019-11-15
//
// Copyright (c) 2019-2021 OPEN CASCADE SAS
//
// This file is part of commercial software by OPEN CASCADE SAS.
//
// This software is furnished in accordance with the terms and conditions
// of the contract and with the inclusion of this copyright notice.
// This software or any other copy thereof may not be provided or otherwise
// be made available to any third party.
//
// No ownership title to the software is transferred hereby.
//
// OPEN CASCADE SAS makes no representation or warranties with respect to the
// performance of this software, and specifically disclaims any responsibility
// for any damages, special or consequential, connected with its use.

using System;
using System.Collections.Generic;
using System.Linq;
using OCC.Jt;
using OCC.JtNode;
using OCC.Standard;
using OCC.TCollection;
using OCC.JTCAFControl;
using OCC.JtAttribute;
using OCC.TDocStd;
using OCC.TNaming;
using OCC.TDF;
using OCC.BinXCAFDrivers;
using OCC.XCAFApp;
using OCC.XCAFDoc;
using OCC.UnitsMethods;
using OCC.JtData;
using OCC.Quantity;
using OCC.Precision;
using OCC.XCAFPrs;
namespace JtTools
{
  public interface IPrinter
  {
    void Print(string theMessage);
  }

  public class JtTestRunner
  {
    private static void initLicense()
    {
      //OCC.SWIG.OCCwrapCSharp.OCCLicense_Activate();
    }

    public static void RunObj(
      string   theInputFile,
      string   theOutputFile,
      IPrinter thePrinter)
    {
      if (thePrinter == null)
      {
        return;
      }

      initLicense();

      Jt_SceneGraph aLSG = new Jt_SceneGraph();

      thePrinter.Print("Try to load JT file");

      TCollection_ExtendedString aFileName = new TCollection_ExtendedString(theInputFile);

      aLSG.LoadFile(aFileName);

      ObjWriter aWriter = new ObjWriter(theOutputFile);

      thePrinter.Print("Traversing LSG");

      ObjWriter.Traverse(aWriter, aLSG.Tree(), new TraverseState());

      thePrinter.Print("Conversion finished");

      aWriter.Close();
    }

    public static void RunXde(
      string   theInputFile,
      string   theOutputFileXde,
      string   theOutputFileJt,
      IPrinter thePrinter)
    {
      if (thePrinter == null)
      {
        return;
      }

      initLicense();

      // 1.Test reading to XDE
      TCollection_ExtendedString aFileName = new TCollection_ExtendedString(theInputFile);
      TCollection_ExtendedString aFormat = new TCollection_ExtendedString("BinXCAF");
      TDocStd_Document aXdeDoc = new TDocStd_Document(aFormat);
      XCAFApp_Application anApp = XCAFApp_Application.GetApplication();
      BinXCAFDrivers.DefineFormat(anApp);
      anApp.Open(aXdeDoc);
      JTCAFControl_Reader aXCafReader = new JTCAFControl_Reader();
      int aLodIndex = 0;
      bool toParallel = true;
      bool toSkipDegenerateTriangles = true;
      bool toDeduplicatePrims = true;
      bool toCacheFileContent = false;
      aXCafReader.SetParallel(toParallel);
      aXCafReader.SetTriangulationLod(aLodIndex);
      aXCafReader.SetSkipDegenerateTriangles(toSkipDegenerateTriangles);
      aXCafReader.SetDeduplicatePrimitives(toDeduplicatePrims);
      aXCafReader.SetCacheFileContent(toCacheFileContent);
      double aUnitFactor = UnitsMethods.GetCasCadeLengthUnit() * 0.001;
      aXCafReader.SetSystemLengthUnit(aUnitFactor);

      if (aXCafReader.Perform(aXdeDoc, aFileName.ToString()))
      {
        if (aXdeDoc != null)
        {
          XCAFDoc_DocumentTool aDocTool = XCAFDoc_DocumentTool.Set(aXdeDoc.Main(), true);
          XCAFDoc_ShapeTool aShTool = XCAFDoc_DocumentTool.ShapeTool(aXdeDoc.Main());
          XCAFDoc_ColorTool aColTool = XCAFDoc_DocumentTool.ColorTool(aXdeDoc.Main());
          XCAFDoc_MaterialTool aMatTool = XCAFDoc_DocumentTool.MaterialTool(aXdeDoc.Main());

          TDF_LabelSequence aLabels = new TDF_LabelSequence();
          aShTool.GetFreeShapes(ref aLabels);
          bool isEmpty = true;

          //2.1. Access to shapes
          for (int i = 1; i <= aLabels.Length(); i++)
          {
            TDF_Label aLabel = aLabels.Value(i);
            TNaming_NamedShape aShapeAtt = new TNaming_NamedShape();
            if (aLabel.FindAttribute(TNaming_NamedShape.GetId(), aShapeAtt))
            {
              TCollection_AsciiString anEntry = new TCollection_AsciiString();
              TDF_Tool.Entry(aLabel, ref anEntry);
              thePrinter.Print("Found a free Shape at the Label = " + anEntry.ToString());
              isEmpty = false;
            }
          }

          //2.2. Access to materials
          XCAFPrs_DocumentExplorer aDocExplorer = new XCAFPrs_DocumentExplorer(aXdeDoc, 0);
          for (;aDocExplorer.More(); aDocExplorer.Next())
          {
            XCAFDoc_VisMaterial aMaterial = aDocExplorer.Current().Style.Material();
            TCollection_AsciiString anEntry = new TCollection_AsciiString();
            TDF_Tool.Entry(aDocExplorer.Current().Label, ref anEntry);
            if (!aMaterial.IsNull())
            {
              thePrinter.Print("Label = " + anEntry.ToString());
              TCollection_HAsciiString aName = aMaterial.RawName();
              Quantity_ColorRGBA aColor = aMaterial.BaseColor();
              thePrinter.Print("Material: " + aName.ToString());
              if ((1.0 - aColor.Alpha()) < Precision.Confusion())
              {
                thePrinter.Print("Material Color: " + aColor.GetRGB().Name());
              }
              else
              {
                thePrinter.Print("Material Color: " + aColor.GetRGB().Name() + " (" + aColor.Alpha() + ")");
              }
            }
          }

          // Keep to XDE Doc
          if (!isEmpty)
          {
            TCollection_ExtendedString aDocName = new TCollection_ExtendedString(theOutputFileXde);
            anApp.SaveAs(aXdeDoc, aDocName);
            thePrinter.Print("Jt file successfully converted to XDE document and kept in file " + aDocName);
          }

          // 3.Converting CAF to Jt and keeping in external file
          Jt_GUID.Value(true);
          JTCAFControl_XcafToJT aConverter = new JTCAFControl_XcafToJT();
          JtNode_Partition aRootPartition = aConverter.Convert(aXdeDoc);
          anApp.Close(aXdeDoc);
          if (!aRootPartition.IsNull())
          {
            TCollection_AsciiString aJtName = new TCollection_AsciiString(theOutputFileJt);
            JtData_Model aJtModel = new JtData_Model(aJtName);
            if (!JtData_Model.Store(aJtModel, aRootPartition))
              thePrinter.Print("Error: failed to write file " + aJtName);
            else
              thePrinter.Print("XDE document successfully converted to Jt and kept in file " + aJtName);
          }
          else
            thePrinter.Print("Root Partition is Null");
        }
      }
    }
  }
}
