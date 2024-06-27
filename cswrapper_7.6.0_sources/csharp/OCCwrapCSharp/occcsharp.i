// Created: 2006-06-21
//
// Copyright (c) 2006-2021 OPEN CASCADE SAS
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

// Top-level interface definition file for wrapping OCC classes to CSharp with SWIG

%module(directors="1") OCCwrapCSharp

// Basic definitions
%include "occtypes.i"
%include "occcol.i"
%include "occbase.i"

%exception {
  try {
    $action
  }
  catch (const Standard_Failure& theException) {
    SWIG_CSharpSetPendingExceptionCustom(theException.DynamicType()->Name());
    return $null;
  }
}

// Foundation classes
%include "TKernel.i"
%include "TKMath.i"

// Modeling data
%include "TKG2d.i"
%include "TKG3d.i"
%include "TKGeomBase.i"
%include "TKBRep.i"

// Modeling algorithms
%include "TKGeomAlgo.i"
%include "TKTopAlgo.i"
%include "TKMesh.i"
%include "TKHLR.i"
%include "TKPrim.i"
%include "TKBO.i"
%include "TKBool.i"
%include "TKShHealing.i"
%include "TKOffset.i"
%include "TKFillet.i"
%include "TKFeat.i"

// Visualization
%include "TKService.i"
%include "TKV3d.i" 
%include "TKOpenGL.i"

// TKD3DHost is wrapped only if available, see makewrapper.bat
#ifdef WRAP_D3DHOST
%include "TKD3DHost.i"
#endif

// Application Framework
%include "TKCDF.i"
%include "TKLCAF.i" 
%include "TKCAF.i" 
%include "TKTObj.i"
%include "TKBinL.i"
%include "TKBin.i"
%include "TKBinXCAF.i"

// Data Exchange
%include "TKXSBase.i"
%include "TKIGES.i" 
%include "TKRWMesh.i" 
%include "TKSTEPBase.i" 
%include "TKSTEPAttr.i"
%include "TKSTEP.i" 
%include "TKSTL.i"
%include "TKVRML.i"
%include "TKXDESTEP.i"
%include "TKXDEIGES.i"

// XCAF
%include "TKXCAF.i"

#ifdef WRAP_TKOCCLicense
%include "TKOCCLicense.i"
#endif

#ifdef WRAP_TKJT
%include "TKJT.i"
%include "TKXDEJT.i"
#endif

/**/
