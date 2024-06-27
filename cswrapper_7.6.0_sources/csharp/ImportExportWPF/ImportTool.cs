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

using OCC.TDocStd;
using OCC.XCAFDoc;
using OCC.STEPCAFControl;
using OCC.IGESCAFControl;
using OCC.Message;
using OCC.IFSelect;
using OCC.TopoDS;
using OCC.BRep;
using OCC.BRepTools;

using System.Diagnostics;
using System.Threading;

namespace ImportExportWPF
{
  //! Class demonstrating OCCT import operation.
  class ImportTool
  {
    private TDocStd_Document Document { get; set; }

    //! Empty constructor.
    public ImportTool(TDocStd_Document document) {
      Document = document;
    }

    //! Perform the import operation.
    public bool Perform(string theFilePath, Message_ProgressIndicator theProgress) {
# if DEBUG
      return perform(theFilePath, theProgress);
#else
      try {
        return perform(theFilePath, theProgress);
      } catch (Exception theException) {
        Message.DefaultMessenger().Send("Error - exception has been raised during import operation:\n" + theException.Message,
                                        Message_Gravity.Message_Fail);
        return false;
      }
#endif
    }

    //! Perform the import operation.
    protected bool perform (string                    theFilePath,
                            Message_ProgressIndicator theProgress)
    {
      bool result;

      if (theFilePath.EndsWith(".brep", true, null)
       || theFilePath.EndsWith(".rle", true, null)) {
        result = importBREP(theFilePath, theProgress);
      } else if (theFilePath.EndsWith(".step", true, null)
              || theFilePath.EndsWith(".stp", true, null)) {
        result = importSTEP(theFilePath, theProgress);
      } else if (theFilePath.EndsWith(".iges", true, null)
              || theFilePath.EndsWith(".igs", true, null)) {
        result = importIGES(theFilePath, theProgress);
      } else {
        result = false;
      }

      return result;
    }
    
    //! Import OCCT BREP file.
    protected bool importBREP(string theFilePath, Message_ProgressIndicator theProgress) {
      TopoDS_Shape aShape = new TopoDS_Shape();
      BRep_Builder aBuilder = new BRep_Builder();
      using (var aRootRange = theProgress.Start())
      {
        using (var aPS = new Message_ProgressScope (aRootRange, "Loading", 10))
        {
          aPS.SetName ("Loading");
          using (var aReadRange = aPS.Next (9))
          {
            if (!BRepTools.Read (ref aShape, theFilePath, aBuilder, aReadRange))
            {
              return false;
            }
          }

          aPS.SetName ("Creating document");
          XCAFDoc_DocumentTool.ShapeTool (Document.Main()).AddShape (aShape);
          return true;
        }
      }
    }

    private bool importSTEP(string theFilePath, Message_ProgressIndicator theProgress)
    {
      bool success = true;

      var aReader = new STEPCAFControl_Reader();
      var ws = aReader.Reader().WS();
      using (var aRootRange = theProgress.Start())
      {
        using (var aPS = new Message_ProgressScope (aRootRange, "Loading", 100))
        {
          aPS.SetName("Loading");
          success = success && (aReader.ReadFile(theFilePath) == IFSelect_ReturnStatus.IFSelect_RetDone);
          aPS.SetName("Translating geometry");
          using (var aTransfRange1 = aPS.Next (20))
          {
            success = success && aReader.Reader().TransferRoots (aTransfRange1) > 0;
          }
          aPS.SetName("Translating attributes");
          using (var aTransfRange2 = aPS.Next (60))
          {
            success = success && aReader.Transfer(Document, aTransfRange2);
          }
        }
      }

      return success;
    }
    
    //! Import IGES file.
    protected bool importIGES(string theFilePath, Message_ProgressIndicator theProgress)
    {
      bool success = true;

      var aReader = new IGESCAFControl_Reader();
      using (var aRootRange = theProgress.Start())
      {
        using (var aPS = new Message_ProgressScope (aRootRange, "Loading", 100))
        {
          aPS.SetName ("Loading");
          using (var aReadRange = aPS.Next (20))
          {
            success = success && (aReader.ReadFile (theFilePath) == IFSelect_ReturnStatus.IFSelect_RetDone);
          }
          aPS.SetName ("Translating");
          using (var aTranslateRange = aPS.Next (80))
          {
            success = success && aReader.Transfer (Document, aTranslateRange);
          }
        }
      }

      return success;
    }

  }
}
