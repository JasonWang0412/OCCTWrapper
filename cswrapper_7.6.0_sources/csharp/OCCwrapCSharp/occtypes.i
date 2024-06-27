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

// Basic definitions for wrapping Open CASCADE classes to SWIG.
// This file is to be %included into SWIG interface definition files that
// wrap OCC classes

#if SWIG_VERSION < 0x020000 
  #error This version of SWIG is known to break OCC wrapper for nested Iterator classes
#endif
#if SWIG_VERSION < 0x030000 
  #error This version of SWIG is known to fail to parse nested template instantiations
#endif
#if SWIG_VERSION < 0x030006
  #error This version of SWIG is known to fail to parse OpenGL_Window.hxx due to directive #import
#endif
#if (SWIG_VERSION > 0x030012)
  #warning This version of SWIG was not tested for using with OCC wrapper
  #warning Be sure to execute tests to check if it works well
#endif

// In SWIG 1.3.29, typemap(csdestruct_derived) always generates public method;
// thus we cannot make the same method in the parent protected...
#if SWIG_VERSION <= 0x010329 
  #define CSDESTRUCT_UNUSED_ACCESS public
#else
  #define CSDESTRUCT_UNUSED_ACCESS protected
#endif

// disable warning on absence of exception handling
%warnfilter(844);

%insert(runtime) %{
// Deprecated functions are automatically wrapped and generate a lot of useless C++ compiler warnings.
// But deprecated status is propagated to C# classes by auxiliary TCL script, hence, suppress these warnings on C++ level.
#define OCCT_NO_DEPRECATED
%}

// Define default typemaps
%include "typemaps.i"
%include "cref.i"

// See Standard_Macro.hxx
#define Standard_EXPORT
#define Standard_OVERRIDE
#define Standard_NODISCARD

// Include definition of Handle
//%include <Standard_Handle.hxx>
namespace opencascade { template <class T> class handle; }
#define Handle(Class) opencascade::handle<Class>

// Ignore macro used to declare OCC Handle classes and RTTI;
// relevant functionality is handled in C# in specific way
#undef DEFINE_STANDARD_HANDLE
#define DEFINE_STANDARD_HANDLE(T1,T2)

// Ignore new / delete macros
#define _Standard_DefineAlloc_HeaderFile
#define DEFINE_STANDARD_ALLOC
#define DEFINE_STANDARD_ALLOC_PLACEMENT
#define _NCollection_DefineAlloc_HeaderFile
#define DEFINE_NCOLLECTION_ALLOC
#define NCOLLECTION_HSEQUENCE(T1,T2)

// See Standard_TypeDef.hxx
typedef int Standard_Integer;
typedef double Standard_Real;
typedef float Standard_ShortReal;
typedef char  Standard_Character;
typedef short Standard_ExtCharacter; // defined as short here to use default SWIG typemaps to C# short
typedef char16_t Standard_Utf16Char;
typedef char32_t Standard_Utf32Char;
typedef unsigned char  Standard_Byte;
typedef const char* Standard_CString;
typedef char* Standard_PCharacter;
typedef void* Standard_Address;
typedef size_t Standard_Size;
typedef wchar_t Standard_WideChar;
//typedef const Standard_ExtCharacter* Standard_ExtString;
typedef bool Standard_Boolean;

// Define typemaps and supporting code to map wchar_t* to C# string with 
// appropriate conversions supporting Unicode symbols

%insert(runtime) %{
// Callback for returning wide strings to C# without leaking memory
typedef wchar_t * (SWIGSTDCALL* SWIG_CSharpWideStringHelperCallback)(const wchar_t *);
static SWIG_CSharpWideStringHelperCallback SWIG_csharp_widestring_callback = NULL;
extern "C" SWIGEXPORT void SWIGSTDCALL SWIGRegisterWideStringCallback_$module(SWIG_CSharpWideStringHelperCallback theCallback)
{
  SWIG_csharp_widestring_callback = theCallback;
}
%}

%pragma(csharp) imclasscode=%{
  protected class SWIGWideStringHelper {
    [return: global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPWStr)]
    public delegate string SWIGWideStringDelegate([global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPWStr)] string message);
    static SWIGWideStringDelegate myStringDelegate = new SWIGWideStringDelegate(CreateWideString);

    [global::System.Runtime.InteropServices.DllImport("$dllimport", EntryPoint="SWIGRegisterWideStringCallback_$module", CharSet = global::System.Runtime.InteropServices.CharSet.Unicode)]
    public static extern void SWIGRegisterWideStringCallback_$module(SWIGWideStringDelegate theCallback);

    static string CreateWideString(string theCString) {
      return theCString;
    }

    static SWIGWideStringHelper() {
      SWIGRegisterWideStringCallback_$module(myStringDelegate);
    }
  }
  static protected SWIGWideStringHelper swigWideStringHelper = new SWIGWideStringHelper();
%}

// wrap Standard_WideChar as C# string preserving Unicode characters
%typemap(ctype) const wchar_t *  "wchar_t *"
%typemap(imtype, inattributes="[global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPWStr)]", outattributes="[return: global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPWStr)]", directorinattributes="[global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPWStr)]") const wchar_t * "string"
%typemap(cstype) const wchar_t *  "string"
%typemap(in) const wchar_t * %{ $1 = $input; %}
%typemap(out) const wchar_t * %{ $result = SWIG_csharp_widestring_callback($1); %}
%typemap(csin) const wchar_t * "$csinput"
%typemap(csout, excode=SWIGEXCODE) const wchar_t * {
    string ret = $imcall;$excode
    return ret;
  }
%typemap(csvarin, excode=SWIGEXCODE2) const wchar_t * %{
    set {
      $imcall;$excode
    } %}
%typemap(csvarout, excode=SWIGEXCODE2) const wchar_t * %{
    get {
      string ret = $imcall;$excode
      return ret;
    } %}
%typemap(directorin) const wchar_t * %{ 
    $input = SWIG_csharp_widestring_callback($1);
  %}

// Map Standard_CString to C# string via wchar_t, to support Unicode symbols (in UTF-8)
%{
#include <TCollection_AsciiString.hxx>
#include <TCollection_ExtendedString.hxx>
%}
%typemap(ctype) Standard_CString "wchar_t *"
%typemap(imtype, inattributes="[global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPWStr)]", outattributes="[return: global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPWStr)]", directorinattributes="[global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPWStr)]") Standard_CString "string"
%typemap(cstype) Standard_CString "string"
%typemap(in) Standard_CString %{ 
    TCollection_AsciiString aStr_$1 ($input); 
    $1 = (char*)(aStr_$1).ToCString(); 
  %}
%typemap(out) Standard_CString %{ 
    TCollection_ExtendedString anExtStr ($1, Standard_True);
    $result = SWIG_csharp_widestring_callback(anExtStr.ToWideString()); 
  %}
%typemap(csin) Standard_CString "$csinput"
%typemap(csout, excode=SWIGEXCODE) Standard_CString {
    string ret = $imcall; $excode
    return ret;
  }
%typemap(csvarin, excode=SWIGEXCODE2) Standard_CString %{ set { 
    $imcall; $excode
  } %}
%typemap(csvarout, excode=SWIGEXCODE2) Standard_CString %{ get { 
    string ret = $imcall; $excode 
    return ret; 
  } %}
%typemap(directorin) Standard_CString %{ 
    TCollection_ExtendedString a_$1 ($1, Standard_True);
    $input = SWIG_csharp_widestring_callback((a_$1).ToWideString()); 
  %}

// Ignore redundant methods commonly found in OCC classes
%ignore ShallowDump;
%ignore ShallowCopy;
%ignore operator new;
%ignore operator delete;
%ignore get_type_descriptor(); // internal method, use TypeOf() instead

// Ignore operators that cannot be overloaded in C#
// Note that in general case it is safe to simple ignore operators
// (not rename them with some name), because usually in OCC classes  
// operators are shortcuts to relevant methods.
%ignore operator [];
%ignore operator ();
%ignore operator =;
%ignore operator +=;
%ignore operator -=;
%ignore operator *=;
%ignore operator /=;
%ignore operator ^=;
%ignore operator |=;
%ignore operator &=;

// Ignore operators that do not get mapped to C# automatically.
// If necessary, corresponding operators can be recovered in C# by %cscode directive.
%ignore operator ==;
%ignore operator !=;
%ignore operator <=;
%ignore operator >=;
%ignore operator >;
%ignore operator <;
%ignore operator +;
%ignore operator -;
%ignore operator *;
%ignore operator /;
%ignore operator ^;
%ignore operator <<;
%ignore operator >>;

// Simple types arguments passed by reference are in general case in-out,
// so let`s define appropriate typemaps by default
%define WRAP_PRIMITIVE_REF(CType,CSType)
  // explicitly define typemaps for 'const &' to be as by default
  %typemap(ctype) const CType & %{ CType %}
  %typemap(imtype) const CType & %{ CSType %}
  %typemap(cstype) const CType & %{ CSType %}
  %typemap(csin) const CType & %{ $csinput %}
  %typemap(csout, excode=SWIGEXCODE) const CType & { 
    CSType ret = $imcall; 
    $excode
    return ret;
  }

  // redefine typemaps for 'non-const &'
  %typemap(ctype,out="CType") CType & %{ CType * %}
  %typemap(imtype,out="CSType") CType & %{ ref CSType %}
  %typemap(cstype,out="CSType") CType & %{ ref CSType %}
  %typemap(csin) CType & %{ ref $csinput %}
  %typemap(csout, excode=SWIGEXCODE) CType & { 
    CSType ret = $imcall; 
    $excode
    return ret;
  }
  %typemap(out) CType & { $result = *$1; }
%enddef

// Apply to elementary types
WRAP_PRIMITIVE_REF(double,double)
WRAP_PRIMITIVE_REF(int,int)
WRAP_PRIMITIVE_REF(float,float)
WRAP_PRIMITIVE_REF(bool,bool)
WRAP_PRIMITIVE_REF(char,byte)
WRAP_PRIMITIVE_REF(unsigned char,byte)
WRAP_PRIMITIVE_REF(short,short)
WRAP_PRIMITIVE_REF(size_t,ulong)

// void * will be wrapped as global::System.IntPtr
%typemap(cstype) void * %{ global::System.IntPtr %};
%typemap(imtype) void * %{ global::System.IntPtr %};
%typemap(csin) void * %{ $csinput %};
%typemap(csout, excode=SWIGEXCODE) void * { 
  global::System.IntPtr ret = $imcall; 
  $excode
  return ret;
}
WRAP_PRIMITIVE_REF(void*,global::System.IntPtr)

// Macro to apply to other types typedefed to void*
%define WRAP_AS_VOID_PTR(Type)
  typedef void* Type;
  WRAP_PRIMITIVE_REF(Type,global::System.IntPtr)
%enddef

// Macro to apply to other types typedefed to void* as global::System.IntPtr
%define WRAP_AS_INT_PTR(Type)
  %typemap(cstype) Type* "global::System.IntPtr"
  %typemap(csin) Type* "new global::System.Runtime.InteropServices.HandleRef(null, $csinput)"
  %typemap(csout, excode=SWIGEXCODE) Type* "{global::System.IntPtr res = $imcall; $excode; return res;}"
  %typemap(csvarout, excode=SWIGEXCODE2) Type* "get{global::System.IntPtr res = $imcall; $excode; return res;}"
  %typemap(csdirectorin) Type* "$iminput"
  %typemap(csdirectorout) Type* "$cscall"
%enddef

// Define typemaps for enumerations passed as output arguments to functions
// Here we rely on default marshaling performed by Platform/INVOKE
// when enum is passed as pointer to its integer representation
%define WRAP_AS_ENUM(Type)
  %typemap(cstype,out="Type") Type & %{ ref Type %}
  %typemap(ctype,out="Type") Type & "Type &"
  %typemap(imtype,out="Type") Type & "ref Type"

  %typemap(in) Type & %{ $1 = &$input; %}
  %typemap(out) Type & %{ $result = *$1; %}

  %typemap(csin) Type & "ref $csinput"
  %typemap(csout, excode=SWIGEXCODE) Type & { 
    Type ret = $imcall; 
    $excode
    return ret;
  }
%enddef

%define WRAP_AS_ENUM_WNULL(Type, NullVal)
  %typemap(cstype,out="Type") Type & %{ ref Type %}
  %typemap(ctype,out="Type") Type & "Type &"
  %typemap(imtype,out="Type") Type & "ref Type"

  %typemap(in) Type & %{ $1 = &$input; %}
  %typemap(out, null="NullVal") Type & %{ $result = *$1; %}

  %typemap(csin) Type & "ref $csinput"
  %typemap(csout, excode=SWIGEXCODE) Type & { 
    Type ret = $imcall; 
    $excode
    return ret;
  }
%enddef


%feature("smartptr", noblock=1) Standard_Transient { Handle(Standard_Transient) }

// Redefine proxy class for Standard_Transient so as to make its field
// swigCPtr protected; see also redefinition of %typemap(csbody_derived)
// for derived classes in the macro WRAP_AS_HANDLE
%typemap(csbody,noblock=1) Standard_Transient {
  protected global::System.Runtime.InteropServices.HandleRef swigCPtr;
  protected bool swigCMemOwn;

  public $csclassname(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  public static global::System.Runtime.InteropServices.HandleRef getCPtr($csclassname obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  // Define destructor (i.e. finalizer) for Standard_Transient explicitly by C# 
  // code since its automatic generation is avoided (see WRAP_AS_HANDLE macro) 
  ~Standard_Transient () { Dispose(); }

  // Unused method added to avoid generation of Dispose() methods in descendants
  CSDESTRUCT_UNUSED_ACCESS virtual void disposeUnused () {}

  // Special method added to Standard_Transient in order to be able to
  // ensure (at compile time) that wrappers of all classes inheriting it in C++ 
  // do the same in C#
  protected virtual void checkTransientInheritance () {}
}

// Wrap arithmetic operators for gp classes to C#
// It would be better if SWIG were able to wrap these operators automatically...
%define WRAP_VECTOR_OPS(Type,TypeCross)
#if (#Type == "TypeCross")
%typemap(cscode) Type %{
  // Operators
  public static Type      operator + (Type me, Type   other) { return me.Added ( other ); }
  public static TypeCross operator ^ (Type me, Type   other) { return me.Crossed ( other ); }
  public static Type      operator / (Type me, double factor){ return me.Divided ( factor ); }
  public static double    operator * (Type me, Type   other) { return me.Dot ( other ); }
  public static Type      operator * (Type me, double scale) { return me.Multiplied ( scale ); }
  public static Type      operator - (Type me, Type   other) { return me.Subtracted ( other ); }
  public override string ToString() { return X() + " " + Y() + " " + Z(); }
%}
#else
%typemap(cscode) Type %{
  // Operators
  public static Type      operator + (Type me, Type   other) { return me.Added ( other ); }
  public static TypeCross operator ^ (Type me, Type   other) { return me.Crossed ( other ); }
  public static Type      operator / (Type me, double factor){ return me.Divided ( factor ); }
  public static double    operator * (Type me, Type   other) { return me.Dot ( other ); }
  public static Type      operator * (Type me, double scale) { return me.Multiplied ( scale ); }
  public static Type      operator - (Type me, Type   other) { return me.Subtracted ( other ); }
  public override string ToString() { return X() + " " + Y(); }
%}
#endif
%enddef

// Macro to wrap OCC class inheriting Standard_Transient - first part.
%define WRAP_AS_HANDLE_1(Class,CSClass)

  %feature("nodirector") Class::DynamicType;
  %feature("nodirector") Class::This;
  %feature("nodirector") Class::Delete;
  %feature("smartptr", noblock=1) Class { Handle(Class) }

  // Define typemap for wrapping returned pointers to that class by Handle.
  // Actually used in constructor, to wrap constructed C++ object.
  %typemap(out) Class * %{
    $result = new Handle(Class) ( $1 );
  %}
  
  // Define typemap for returning references to that class, to wrap by Handle.
  // Applied in very rare cases when reference to class normally operated by 
  // Handle is returned.
  %typemap(out, canthrow=1) Class & %{
    if ($1->GetRefCount() <=0)
    {
      SWIG_CSharpSetPendingException (SWIG_CSharpInvalidOperationException, "Returning reference to statically allocated instance of Transient class");
      return 0;
    }
    else
    {
      $result = new Handle(Class) ( $1 );
    }
  %}
  
  // Define typemap for dereferencing the handle when Class * is expected
  // (used mainly for handling "this" argument of class methods)
  %typemap(in) Class * %{
    $1 = ( $input && ! ((Handle(Class)*)$input)->IsNull() ? ((Handle(Class)*)$input)->get() : 0 );
  %}

  // Define typemap for dereferencing the handle when Class & is expected (e.g. passing class as non-handle argument)
  %typemap(in, canthrow=1) Class & %{
    if (!$input || ((Handle(Class)*)$input)->IsNull()) { SWIG_CSharpSetPendingException (SWIG_CSharpInvalidOperationException, "NULL dereference"); }
    $1 = ((Handle(Class)*)$input)->get();
  %}

  // Redefine templates for constructor and destructor of proxy class
  // (see default templates in SWIG/Lib/csharp/csharp.swg):
  // we do not need to do Upcast for the pointer, as the pointer we keep is 
  // a pointer to Handle; hence we do not need to have own swigCPtr field
  // and will use this field declared in base class (Standard_Transient);
  // furthermore we do not need own destructor (finalizer) and Dispose()
  %typemap(csbody_derived) Class %{
  public $csclassname(global::System.IntPtr cPtr, bool cMemoryOwn) : base(cPtr, cMemoryOwn) {}
  %}
  %typemap(csfinalize) Class %{ %}
  %typemap(csdestruct_derived, methodname="disposeUnused", methodmodifiers="protected") Class {}

  // Define equivalent C# type for C++ Handle class to be wrapper of the
  // class itself
  %typemap(cstype) Handle(Class), const Handle(Class) & %{ CSClass %}
  %typemap(cstype) Handle(Class) & %{ CSClass %} // not "ref CSClass"!

  // Define type conversion of C# wrapper class to pointer to Handle,
  // used in passing Handle arguments to C++ functions
  %typemap(csin) Handle(Class), Handle(Class) & %{ CSClass.getCPtr($csinput) %}

  // Define type conversion of C++ Handle (when returned by some function)
  // to C# equivalent class
  %typemap(csout, excode=SWIGEXCODE) Handle(Class) {
    CSClass ret = new CSClass ( $imcall, true );
    $excode
    return ret;
  }
  %typemap(csout, excode=SWIGEXCODE) Handle(Class) & {
    CSClass ret = new CSClass ( $imcall, true ); 
    $excode
    return ret;
  }
  %typemap(out) Handle(Class) & %{
    $result = new Handle(Class) ( *$1 );
  %}

  // Convert director method arguments (called from C++ level and passing handles as global::System.IntPtr) on C# level
  %typemap(csdirectorin) Handle(Class) & %{ new CSClass ($1, false) %}

  // Provide DownCast and Type methods
  %csmethodmodifiers Class::DownCast "public new";          // 'new' keyword is required by C#
  %csmethodmodifiers Standard_Transient::DownCast "public"; // but not in base class!
%enddef

// Macro to wrap OCC class inheriting Standard_Transient - second part.
%define WRAP_AS_HANDLE_2_DOWNCAST(Class,CSClass)
  %extend Class {
    static Handle(Class) DownCast(const Handle(Standard_Transient) &T) {
      return Handle(Class)::DownCast ( T );
    }
  };
%enddef

// Macro to wrap OCC class inheriting Standard_Transient - third part.
%define WRAP_AS_HANDLE_3(Class,CSClass)
  %extend Class {
    static Handle(Standard_Type) TypeOf() { return STANDARD_TYPE(Class); }
  };

  // And now.. the final dirty trick! redefine SWIG destruction method
  %{
  // Note the dirty trick!
  #define CSharp_delete_##Class \
          CSharp_delete_##Class(void * jarg1) { delete (Handle(##Class)*)jarg1; } \
          inline void unused_CSharp_delete_##Class
  %}

  %typemap(cscode) Class %{
  // Special method added to wrapper of the class on C# level in order to
  // ensure (at compile time) that it inherits Standard_Transient wrapper.
  // If error appears here check that base class is also wrapped (before this one).
  private void ensureTransientInheritance () { checkTransientInheritance(); }
  %}
  
%enddef

// Macro to wrap OCC class inheriting Standard_Transient.
// Defines a set of typemaps ensuring that the instance of the C++ class will be
// held in C# wrapper by appropriate Handle class.
%define WRAP_AS_HANDLE_(Class,CSClass)
WRAP_AS_HANDLE_1(Class,CSClass)
WRAP_AS_HANDLE_2_DOWNCAST(Class,CSClass)
WRAP_AS_HANDLE_3(Class,CSClass)
%enddef

%define WRAP_AS_HANDLE(Class)
WRAP_AS_HANDLE_(Class,Class)
%enddef

// Shortcut: includes corresponding header file for both SWIG and C++ compiler
%define WRAP_INCLUDE_EXT(TheInclude)
  %{
  #include <TheInclude>
  %}
  %include <TheInclude>
%enddef

// Shortcut: includes corresponding header file for both SWIG and C++ compiler
%define WRAP_INCLUDE(TheClass)
WRAP_INCLUDE_EXT(TheClass.hxx)
%enddef

// Shortcut: calls WRAP_AS_HANDLE(Class) and %includes corresponding header file
%define WRAP_AS_HANDLE_INCLUDE_EXT(TheClass, TheInclude)
  %{
  #include <TheInclude>
  %}
  WRAP_AS_HANDLE(TheClass)
  %include <TheInclude>
%enddef

// Shortcut: calls WRAP_AS_HANDLE(Class) and %includes corresponding header file
%define WRAP_AS_HANDLE_INCLUDE(TheClass)
WRAP_AS_HANDLE_INCLUDE_EXT(TheClass, TheClass.hxx)
%enddef

// Shortcut: defines typemaps for passing by reference and wraps the class 
%define WRAP_AS_STRUCT_INCLUDE(Class)
  WRAP_CLASS_COPYABLE(Class)
  WRAP_INCLUDE(Class)
%enddef

// Shortcut: defines typemaps for passing by reference and wraps the class 
%define WRAP_AS_STRUCT_INCLUDE_EXT(TheClass, TheInclude)
  WRAP_CLASS_COPYABLE(TheClass)
  WRAP_INCLUDE_EXT(TheInclude)
%enddef

// Shortcut: defines typemaps for passing by reference and wraps the class
%define WRAP_AS_CLASS_INCLUDE(Class)
  WRAP_CLASS_NOCOPY(Class)
  WRAP_INCLUDE(Class)
%enddef

// Shortcut: defines typemaps for passing by reference and wraps the class
%define WRAP_AS_CLASS_INCLUDE_EXT(TheClass, TheInclude)
  WRAP_CLASS_NOCOPY(TheClass)
  WRAP_INCLUDE_EXT(TheInclude)
%enddef

// Shortcut: defines typemaps for enum and wraps it
%define WRAP_AS_ENUM_INCLUDE(Enum)
  WRAP_AS_ENUM(Enum)
  WRAP_INCLUDE(Enum)
%enddef

// Shortcut: defines typemaps for enum with NULL value and wraps it
%define WRAP_AS_ENUM_WNULL_INCLUDE(Enum, NullVal)
  WRAP_AS_ENUM_WNULL(Enum, NullVal)
  WRAP_INCLUDE(Enum)
%enddef

// Shortcut: defines typemaps for enum and wraps it
%define WRAP_AS_ENUM_INCLUDE_EXT(TheEnum, TheInclude)
  WRAP_AS_ENUM(TheEnum)
  WRAP_INCLUDE_EXT(TheInclude)
%enddef

%insert(runtime) %{
  // Code to handle throwing of OCCT Exceptions from C/C++ code.
  // The equivalent delegate to the callback, CSharpExceptionCallback_t, is CustomExceptionDelegate
  // and the equivalent customExceptionCallback instance is customDelegate
  typedef void (SWIGSTDCALL* CSharpExceptionCallback_t)(const char *);
  CSharpExceptionCallback_t customExceptionCallback = NULL;

  extern "C" SWIGEXPORT
  void SWIGSTDCALL CustomExceptionRegisterCallback(CSharpExceptionCallback_t customCallback) {
    customExceptionCallback = customCallback;
  }

  // Note that SWIG detects any method calls named starting with
  // SWIG_CSharpSetPendingException for warning 845
  static void SWIG_CSharpSetPendingExceptionCustom(const char *msg) {
    customExceptionCallback(msg);
  }
%}

%pragma(csharp) imclasscode=%{
  class CustomExceptionHelper {
    // C# delegate for the C/C++ customExceptionCallback
    public delegate void CustomExceptionDelegate(string theExceptionType);
    static CustomExceptionDelegate customDelegate =
                                   new CustomExceptionDelegate(SetPendingCustomException);

    [DllImport("$dllimport", EntryPoint="CustomExceptionRegisterCallback")]
    public static extern
           void CustomExceptionRegisterCallback(CustomExceptionDelegate customCallback);

    static void SetPendingCustomException(string theExceptionType) {
      string aPackage = theExceptionType.Remove(theExceptionType.IndexOf('_'));
      Type anExceptionType = System.Type.GetType("OCC." + aPackage + '.' + theExceptionType);
      SWIGPendingException.Set((Exception)(System.Activator.CreateInstance(anExceptionType)));
    }

    static CustomExceptionHelper() {
      CustomExceptionRegisterCallback(customDelegate);
    }
  }
  static CustomExceptionHelper exceptionHelper = new CustomExceptionHelper();
%}

// Shortcut: defines typemaps for exceptions 
%define WRAP_AS_EXCEPTION(Class)
  %warnfilter(SWIGWARN_LANG_SMARTPTR_MISSING) Class;
  %ignore Class::NewInstance;
  %ignore Class::Raise;
  WRAP_AS_CLASS_INCLUDE(Class)
%enddef

// Shortcut: defines wrapping a main class of the package 
// Note: the package name may be postfixed by underscore as a workaround
//       against possible conflict in internal SWIG names between package methods
//       (e.g. TopoDS::Face) and class names (e.g. TopoDS+Face)
%define WRAP_AS_PACKAGE(Package)
//  %rename Package Package##_;
  WRAP_AS_CLASS_INCLUDE(Package)
%enddef

WRAP_AS_INT_PTR(Standard_Byte)
WRAP_AS_INT_PTR(unsigned char)
