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

// Define list of necessary libraries to link with
%{
#ifdef _MSC_VER
#pragma comment(lib, "TKXDEJT.lib")
#endif
%}

//JT to XDE document
WRAP_AS_HANDLE_INCLUDE(JTCAFControl_Reader)
WRAP_AS_HANDLE_INCLUDE(JTCAFControl_Triangulation)
WRAP_AS_CLASS_INCLUDE(JTCAFControl_XcafToJT)

%ignore JTCAFControl_Reader::getNodeFullName (const Handle(JtNode_Base)&);
%ignore JTCAFControl_Reader::getNodeShortName (const Handle(JtNode_Base)&);
%ignore JTCAFControl_Reader::getNodeTransformation (const Handle(JtNode_Base)&);
%ignore JTCAFControl_Reader::getNodeMaterial (const Handle(JtNode_Base)&);
