// Created: 2006-06-22
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

// Definitions for wrapping Open CASCADE classes from TKSTEP toolkit to CSharp using SWIG.
// This file is to be %included into SWIG interface definition files that wrap OCC classes
// after %include occtypes.i

// Define list of necessary libraries to link with
%{
#ifdef _MSC_VER
#pragma comment(lib, "TKSTEP.lib")
#endif
%}

// package STEPControl
WRAP_AS_ENUM_INCLUDE(STEPControl_StepModelType)

WRAP_AS_HANDLE_INCLUDE(STEPControl_Controller)
WRAP_AS_CLASS_INCLUDE(STEPControl_Reader)
WRAP_AS_CLASS_INCLUDE(STEPControl_Writer)

//package StepToGeom
WRAP_AS_PACKAGE(StepToGeom)

//package StepToTopoDS
WRAP_AS_ENUM_INCLUDE(StepToTopoDS_BuilderError)
WRAP_AS_ENUM_INCLUDE(StepToTopoDS_TranslateShellError)
WRAP_AS_ENUM_INCLUDE(StepToTopoDS_TranslateFaceError)
WRAP_AS_ENUM_INCLUDE(StepToTopoDS_TranslateEdgeLoopError)
WRAP_AS_ENUM_INCLUDE(StepToTopoDS_TranslateEdgeError)
WRAP_AS_ENUM_INCLUDE(StepToTopoDS_TranslateVertexError)
WRAP_AS_ENUM_INCLUDE(StepToTopoDS_TranslateVertexLoopError)
WRAP_AS_ENUM_INCLUDE(StepToTopoDS_TranslatePolyLoopError)
WRAP_AS_ENUM_INCLUDE(StepToTopoDS_GeometricToolError)

WRAP_AS_DATAMAP_INCLUDE(StepToTopoDS_DataMapOfRI,NCollection_DataMap,Handle(StepRepr_RepresentationItem),TopoDS_Shape,TColStd_MapTransientHasher)
WRAP_AS_DATAMAP_INCLUDE(StepToTopoDS_DataMapOfRINames,NCollection_DataMap,TCollection_AsciiString,TopoDS_Shape,TCollection_AsciiString)
WRAP_AS_CLASS_INCLUDE(StepToTopoDS_Root)
WRAP_AS_CLASS_INCLUDE(StepToTopoDS_TranslateShell)
WRAP_AS_CLASS_INCLUDE(StepToTopoDS_TranslateFace)
WRAP_AS_CLASS_INCLUDE(StepToTopoDS_TranslateEdgeLoop)
WRAP_AS_CLASS_INCLUDE(StepToTopoDS_TranslateEdge)
WRAP_AS_CLASS_INCLUDE(StepToTopoDS_TranslateVertex)
WRAP_AS_CLASS_INCLUDE(StepToTopoDS_TranslatePolyLoop)
WRAP_AS_CLASS_INCLUDE(StepToTopoDS_TranslateVertexLoop)
WRAP_AS_CLASS_INCLUDE(StepToTopoDS_TranslateCompositeCurve)
WRAP_AS_CLASS_INCLUDE(StepToTopoDS_TranslateCurveBoundedSurface)
%ignore StepToTopoDS_Builder::StepToTopoDS_Builder(const Handle(StepShape_GeometricSet)& S,const Handle(Transfer_TransientProcess)& TP);
WRAP_AS_CLASS_INCLUDE(StepToTopoDS_Builder)
WRAP_AS_CLASS_INCLUDE(StepToTopoDS_MakeTransformed)
WRAP_AS_CLASS_INCLUDE(StepToTopoDS_GeometricTool)
WRAP_AS_CLASS_INCLUDE(StepToTopoDS_NMTool)
WRAP_AS_CLASS_INCLUDE(StepToTopoDS_Tool)
WRAP_AS_CLASS_INCLUDE(StepToTopoDS_CartesianPointHasher)
WRAP_AS_CLASS_INCLUDE(StepToTopoDS_PointPair)
WRAP_AS_CLASS_INCLUDE(StepToTopoDS_PointPairHasher)

WRAP_AS_DATAMAP_INCLUDE(StepToTopoDS_DataMapOfTRI,NCollection_DataMap,Handle(StepShape_TopologicalRepresentationItem),TopoDS_Shape,TColStd_MapTransientHasher)
WRAP_AS_DATAMAP_INCLUDE(StepToTopoDS_PointEdgeMap,NCollection_DataMap,StepToTopoDS_PointPair,TopoDS_Edge,StepToTopoDS_PointPairHasher)
WRAP_AS_DATAMAP_INCLUDE(StepToTopoDS_PointVertexMap,NCollection_DataMap,Handle(StepGeom_CartesianPoint),TopoDS_Vertex,StepToTopoDS_CartesianPointHasher)
