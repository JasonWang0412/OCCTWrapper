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

// Definitions for wrapping Open CASCADE classes from TKService toolkit to CSharp using SWIG.
// This file is to be %included into SWIG interface definition files that wrap OCC classes
// after %include occtypes.i

// Note that windowing API is platform-dependent, you need to define one of the
// following macros using SWIG preprocessor to have corresponding API available:
// - _WIN32: Windows API

// Define list of necessary libraries to link with
%{
#ifdef _MSC_VER
#pragma comment(lib, "TKService.lib")
#endif
%}

// Service Exceptions
WRAP_AS_EXCEPTION(Aspect_AspectFillAreaDefinitionError)
WRAP_AS_EXCEPTION(Aspect_AspectLineDefinitionError)
WRAP_AS_EXCEPTION(Aspect_AspectMarkerDefinitionError)
WRAP_AS_EXCEPTION(Aspect_DisplayConnectionDefinitionError)
WRAP_AS_EXCEPTION(Aspect_GraphicDeviceDefinitionError)
WRAP_AS_EXCEPTION(Aspect_IdentDefinitionError)
WRAP_AS_EXCEPTION(Aspect_WindowDefinitionError)
WRAP_AS_EXCEPTION(Aspect_WindowError)
WRAP_AS_EXCEPTION(Graphic3d_GroupDefinitionError)
WRAP_AS_EXCEPTION(Graphic3d_MaterialDefinitionError)
WRAP_AS_EXCEPTION(Graphic3d_PriorityDefinitionError)
WRAP_AS_EXCEPTION(Graphic3d_StructureDefinitionError)
WRAP_AS_EXCEPTION(WNT_ClassDefinitionError)

// Aspect enumerations
WRAP_AS_ENUM_INCLUDE(Aspect_FillMethod)
WRAP_AS_ENUM_INCLUDE(Aspect_GradientFillMethod)
WRAP_AS_ENUM_INCLUDE(Aspect_GridDrawMode)
WRAP_AS_ENUM_INCLUDE(Aspect_GridType)
WRAP_AS_ENUM_INCLUDE(Aspect_HatchStyle)
WRAP_AS_ENUM_INCLUDE(Aspect_InteriorStyle)
WRAP_AS_ENUM_INCLUDE(Aspect_PolygonOffsetMode)
WRAP_AS_ENUM_INCLUDE(Aspect_TypeOfDeflection)
WRAP_AS_ENUM_INCLUDE(Aspect_TypeOfMarker)
WRAP_AS_ENUM_INCLUDE(Aspect_TypeOfColorScaleData)
WRAP_AS_ENUM_INCLUDE(Aspect_TypeOfColorScalePosition)
WRAP_AS_ENUM_INCLUDE(Aspect_TypeOfDisplayText)
WRAP_AS_ENUM_INCLUDE(Aspect_TypeOfFacingModel)
WRAP_AS_ENUM_INCLUDE(Aspect_TypeOfHighlightMethod)
WRAP_AS_ENUM_INCLUDE(Aspect_TypeOfLine)
WRAP_AS_ENUM_INCLUDE(Aspect_TypeOfMarker)
WRAP_AS_ENUM_INCLUDE(Aspect_TypeOfResize)
WRAP_AS_ENUM_INCLUDE(Aspect_TypeOfStyleText)
WRAP_AS_ENUM_INCLUDE(Aspect_TypeOfTriedronPosition)

WRAP_INCLUDE(Aspect_VKey)
WRAP_INCLUDE(Aspect_VKeyFlags)
WRAP_AS_ENUM(Aspect_VKeyBasic)
WRAP_AS_STRUCT_INCLUDE(Aspect_ScrollDelta)
WRAP_AS_HANDLE_INCLUDE(Aspect_VKeySet)
WRAP_AS_CLASS_INCLUDE(Aspect_WindowInputListener)

// Aspect typedefs
WRAP_AS_VOID_PTR(Aspect_Handle)
WRAP_AS_VOID_PTR(Aspect_RenderingContext)
WRAP_AS_VOID_PTR(Aspect_Drawable)

// Aspect classes
WRAP_AS_NCOLLECTION_INCLUDE(Aspect_SequenceOfColor,NCollection_Sequence,Quantity_Color)
WRAP_AS_STRUCT_INCLUDE(Aspect_Background)
%ignore Aspect_GradientBackground::SetBgGradientFillMethod;
WRAP_AS_STRUCT_INCLUDE(Aspect_GradientBackground)

%ignore Aspect_DisplayConnection::GetDisplay;
%ignore Aspect_DisplayConnection::GetDisplayName;
%ignore Aspect_DisplayConnection::Aspect_DisplayConnection(const TCollection_AsciiString& theDisplayName);
%ignore Aspect_DisplayConnection::Aspect_DisplayConnection(Display* theDisplay);
%ignore Aspect_DisplayConnection::GetAtom (const Aspect_XAtom theAtom) const;
%ignore Aspect_DisplayConnection::IsOwnDisplay;
%ignore Aspect_DisplayConnection::Init;
WRAP_AS_HANDLE(Aspect_DisplayConnection)
WRAP_INCLUDE(Aspect_DisplayConnection)

%ignore Aspect_Window::NativeFBConfig; // method returning low-level data
WRAP_AS_HANDLE_INCLUDE(Aspect_Window)
WRAP_AS_HANDLE_INCLUDE(Aspect_Grid)

// Image enumerations
WRAP_AS_ENUM_INCLUDE(Image_Format)

// Image classes
WRAP_AS_HANDLE_INCLUDE(Image_PixMap)
WRAP_AS_HANDLE(Image_AlienPixMap)
WRAP_INCLUDE(Image_AlienPixMap)

// Interfaces to window system is wrapped if corresponding 
// preprocessor macro is set (_WIN32 or OCC_XLIB)
#if defined(_WIN32)

// WNT classes -- assume we are on Windows unless macro XW is defined
WRAP_INCLUDE(WNT_Dword)
WRAP_INCLUDE(WNT_WindowPtr)

typedef unsigned int UINT; // to avoid including windows.h
typedef unsigned long DWORD; // to avoid including windows.h
WRAP_AS_HANDLE_INCLUDE(WNT_WClass)
%ignore WNT_Window::WndProc;
%ignore WNT_Window::WNT_Window(const Standard_CString aTitle,const Handle(WNT_WClass)& aClass,const WNT_Dword& aStyle = 0,const Quantity_Parameter Xc = 0.5,const Quantity_Parameter Yc = 0.5,const Quantity_Parameter aWidth = 0.5,const Quantity_Parameter aHeight = 0.5,const Quantity_NameOfColor aBackColor = Quantity_NOC_MATRAGRAY,const Aspect_Handle& aParent = 0,const Aspect_Handle& aMenu = 0,const Standard_Address aClientStruct = 0);
%ignore WNT_Window::MapColors;
WRAP_AS_HANDLE_INCLUDE(WNT_Window)

// eliminate defines made by windows.h which gets #included with WNT_Window
%{ 
#ifdef SetForm
#undef SetForm
#endif
%}

#else

WRAP_AS_HANDLE_INCLUDE(Xw_Window)

// undefine known macros defined by X11 and conflicting with method names
%{
#ifdef Convex
#undef Convex
#endif
%}

#endif

// Font enumerations
WRAP_AS_ENUM_INCLUDE(Font_FontAspect)
