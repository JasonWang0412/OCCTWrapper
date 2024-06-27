// Created: 2006-07-19
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

// Definitions for wrapping Open CASCADE classes from TKHLR toolkit to CSharp using SWIG.
// This file is to be %included into SWIG interface definition files that wrap OCC classes
// after %include occtypes.i

// Define list of necessary libraries to link with
%{
#ifdef _MSC_VER
#pragma comment(lib, "TKHLR.lib")
#endif
%}

// HLRAlgo classes
WRAP_AS_STRUCT_INCLUDE(HLRAlgo_Projector)
WRAP_AS_HANDLE(HLRBRep_Algo)
WRAP_AS_HANDLE(HLRBRep_PolyAlgo)
WRAP_AS_HANDLE_INCLUDE(HLRBRep_InternalAlgo)
WRAP_INCLUDE(HLRBRep_Algo)
WRAP_INCLUDE(HLRBRep_PolyAlgo)
WRAP_AS_CLASS_INCLUDE(HLRBRep_HLRToShape)
WRAP_AS_CLASS_INCLUDE(HLRBRep_PolyHLRToShape)