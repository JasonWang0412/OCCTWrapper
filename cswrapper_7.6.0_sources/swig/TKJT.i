// Created: 2019-06-05
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
#pragma comment(lib, "TKJT.lib")
#endif
%}

%include "std_string.i"

WRAP_AS_STRUCT_INCLUDE(Jt_Box)
WRAP_AS_HANDLE_INCLUDE(Jt_Element)
WRAP_AS_HANDLE_INCLUDE(Jt_Attribute)
WRAP_AS_HANDLE_INCLUDE(Jt_Material)
WRAP_AS_HANDLE_INCLUDE(Jt_Transform)

WRAP_AS_NCOLLECTION_INCLUDE(Jt_ArrayOfAttributes, NCollection_Vector, Handle(Jt_Attribute))

WRAP_AS_HANDLE_INCLUDE(Jt_Node)

WRAP_AS_NCOLLECTION_INCLUDE(Jt_ArrayOfNodes, NCollection_Vector, Handle(Jt_Node))

%ignore Jt_RangeLOD::Ranges () const;
%ignore Jt_SceneGraph::LoadFile (const std::string&);

WRAP_AS_HANDLE_INCLUDE(Jt_Group)
WRAP_AS_HANDLE_INCLUDE(Jt_LOD)
WRAP_AS_HANDLE_INCLUDE(Jt_Part)
WRAP_AS_HANDLE_INCLUDE(Jt_Partition)
WRAP_AS_HANDLE_INCLUDE(Jt_RangeLOD)
WRAP_AS_HANDLE_INCLUDE(Jt_Instance)
WRAP_AS_HANDLE_INCLUDE(Jt_Shape)

WRAP_AS_HANDLE_INCLUDE(Jt_Triangulation)
WRAP_AS_HANDLE_INCLUDE(Jt_Mesh)
WRAP_AS_HANDLE_INCLUDE(Jt_SceneGraph)


// XDE to JT
WRAP_AS_STRUCT_INCLUDE(JtData_ElementData)
WRAP_AS_HANDLE_INCLUDE(JtData_Object)
WRAP_AS_HANDLE_INCLUDE(JtNode_Base)
WRAP_AS_HANDLE_INCLUDE(JtNode_GroupOrInstance)
WRAP_AS_HANDLE_INCLUDE(JtNode_Group)
%ignore JtNode_Partition::ElementData(void) const;
WRAP_AS_HANDLE_INCLUDE(JtNode_Partition)

WRAP_AS_CLASS_INCLUDE(JtData_Types)
%ignore JtData_Model::SegmentsWriter;
%ignore JtData_Model::IsLittleEndianHost;
%ignore JtData_Model::ObjectToId;
WRAP_AS_HANDLE_INCLUDE(JtData_Model)
WRAP_AS_HANDLE_INCLUDE(JtAttribute_Base)
WRAP_AS_HANDLE_INCLUDE(JtAttribute_Material)
