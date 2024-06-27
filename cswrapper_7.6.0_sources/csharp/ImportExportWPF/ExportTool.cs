// Created: 2015-07-03
//
// Copyright (c) 2015-2021 OPEN CASCADE SAS
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
using System.Text;
using OCCWpf.Utils;
using OCC.Message;
using OCC.TDocStd;
using OCC.TDF;
using OCC.XCAFDoc;
using OCC.BRepTools;
using OCC.IGESCAFControl;
using OCC.STEPCAFControl;
using OCC.TopoDS;
using OCC.IFSelect;
using OCC.BRep;

namespace ImportExportWPF
{

  //! Class demonstrating OCCT import operation.
  class ExportTool
  {

    //! Perform the export operation.
    public bool Perform (TDocStd_Document          theDocument,
                         string                    theFilePath,
                         Message_ProgressIndicator theProgress)
    {
      try
      {
        return perform (theDocument, theFilePath, theProgress);
      }
      catch (Exception theException)
      {
        Message.DefaultMessenger().Send ("Error - exception has been raised during export operation:\n" + theException.Message,
                                         Message_Gravity.Message_Fail);
        return false;
      }
    }

    //! Perform the export operation.
    protected bool perform (TDocStd_Document theDocument,
                            string theFilePath,
                            Message_ProgressIndicator theProgress)
    {
      if (theFilePath.EndsWith(".brep", true, null)
       || theFilePath.EndsWith(".rle", true, null))
      {
        return exportBREP(theDocument, theFilePath, theProgress);
      }
      else if (theFilePath.EndsWith (".step", true, null)
            || theFilePath.EndsWith (".stp",  true, null))
      {
        return exportSTEP (theDocument, theFilePath, theProgress);
      }
      else if (theFilePath.EndsWith (".iges", true, null)
            || theFilePath.EndsWith (".igs",  true, null))
      {
        return exportIGES (theDocument, theFilePath, theProgress);
      }

      Message.DefaultMessenger().Send ("Error - Unsupported export type!",
                                       Message_Gravity.Message_Fail);
      return false;
    }
    
    //! Export into OCCT BREP file.
    protected bool exportBREP (TDocStd_Document theDocument,
                               string theFilePath,
                               Message_ProgressIndicator theProgress)
    {
      var aLabels = new TDF_LabelSequence();
      var aShapeTool = XCAFDoc_DocumentTool.ShapeTool(theDocument.Main());
      aShapeTool.GetFreeShapes(ref aLabels);
      if (aLabels.Length() <= 0)
      {
        Message.DefaultMessenger().Send("Error - no shapes to write!",
                                         Message_Gravity.Message_Fail);
        return false;
      }

      if (aLabels.Length() == 1)
      {
        var aShape = XCAFDoc_ShapeTool.GetShape(aLabels.Value(1));
        return BRepTools.Write(aShape, theFilePath);
      }
      else
      {
        var aCompound = new TopoDS_Compound();
        var aBuilder = new BRep_Builder();
        aBuilder.MakeCompound(ref aCompound);
        for (int i = 1; i <= aLabels.Length(); i++)
        {
          var aShape = XCAFDoc_ShapeTool.GetShape(aLabels.Value(i));
          TopoDS_Shape aCompoundShape = aCompound; // need to have TopoDS_Shape variable for a ref call
          aBuilder.Add(ref aCompoundShape, aShape);
        }
        return BRepTools.Write(aCompound, theFilePath);
      }
    }
      
    //! Export into STEP file.
    protected bool exportSTEP (TDocStd_Document theDocument,
                               string theFilePath,
                               Message_ProgressIndicator theProgress)
    {
      var aWriter = new STEPCAFControl_Writer();
      aWriter.Transfer(theDocument);
      return aWriter.Write (theFilePath) == IFSelect_ReturnStatus.IFSelect_RetDone;
    }

    //! Export into IGES file.
    protected bool exportIGES (TDocStd_Document theDocument,
                               string theFilePath,
                               Message_ProgressIndicator theProgress)
    {
      var aWriter = new IGESCAFControl_Writer();
      aWriter.Transfer(theDocument);
      return aWriter.Write (theFilePath);
    }
  }
}
