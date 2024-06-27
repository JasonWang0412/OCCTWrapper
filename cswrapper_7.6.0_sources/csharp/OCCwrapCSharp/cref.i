// Created: 2009-03-18
//
// Copyright (c) 2009-2021 OPEN CASCADE SAS
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

// This file provides alternative definitions for C# wrapper classes, 
// similar to default ones provided by SWIG but extended by reference 
// to "parent" object, so as to make wrappers safe for possible case 
// of returning C++ references to class fields or other temporary objects.
//
// By default SWIG in this case creates wrapper object of the same type as 
// for newly allocated object, but with flag owner=false, so as not
// to delete referred C++ object upon wrapper destruction.
//
// This works well if returned value is reference to C++ object
// which lives longer than the wrapper to the reference obtained.
// The problem occurs if reference to field of the class is returned:
// in this case it may happen that the main object is destroyed while
// the reference to its field is still in use; this cannot be controlled
// properly on C# level since this reference is C++, hence "unmanaged".
// 
// This file provides macros that redefine typemaps for wrapper classes
// so as to have additional field for reference to parent object on C# level, 
// and typemaps for returning references to C++ class which store 
// reference to parent (calling) object in C#.
// In effect, resulting C# wrapper for the object returned by reference 
// will store C# reference to parent object; this guarantees that parent
// will not be restored until reference to the field is still in use.
//
// See SWIG documentation section 18.6.1 for description of the problem and
// idea behind this solution.
// 
// Note that while this solution provides safe treatment of returning C++ 
// reference to class field in C# wrapper, it has some drawbacks:
//
// - It works if returned reference is to the field or other object controlled
//   by the calling class and living until this "parent" object is alive.
//   If it is not the case, specific treatment will be necessary.
//
// - It does not work for static methods (since there is no instance of the 
//   "parent"). Such cases will result in compilation error, and the problem
//   shall be fixed specifically.
// 
// - It may have unintended effect of providing longer than expected life time  
//   for objects, references to whose fields were stored in C# variables.
//   For instance, if tool object used to construct some object returns its result 
//   as reference to its field, then the instance of the tool object is kept along
//   with result in C#, and it will not be garbage collected until this result is 
//   in use. This may cause memory overconsumption.
// 
// In order to provide solution to the last problem, we need to have a means
// to make copies of C++ objects on C# level. Note that this is not provided by 
// SWIG by default. 
// 
// This file provides additional macro for wrapping copyable classes that solves
// this problem. In addition to keeping reference to parent object, it redefines 
// "out" typemaps for references to so as to return special wrapper object,
// that cannot be used directly but requires explicit conversion to usable
// wrapper either as reference to existing C++ object (field, in which case
// reference to parent is stored), or as a copy (without reference to parent).
// In this way the user is warned and can (have to) choose appropriate
// handling in each case.
//
// Original definitions taken from SWIG 2.0.12, Lib/csharp/csharp.swg

// Macro for wrapping non-copyable classes
%define WRAP_CLASS_NOCOPY(Class...)

  // Redefine body for C# wrapper class (add parent field)
  %typemap(csbody) Class %{
  
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  protected bool swigCMemOwn;

  protected object swigParent;

  public $csclassname(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  public $csclassname(global::System.IntPtr cPtr, bool cMemoryOwn, object Parent) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
    if ( ! cMemoryOwn ) swigParent = Parent; // keep reference to assumed parent object
  }

  public static global::System.Runtime.InteropServices.HandleRef getCPtr($csclassname obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }
  %}

  // Redefine body for C# wrapper class for derived classes 
  %typemap(csbody_derived) Class %{

  private global::System.Runtime.InteropServices.HandleRef swigCPtr;

  public $csclassname(global::System.IntPtr cPtr, bool cMemoryOwn)
  : base($imclassname.$csclazznameSWIGUpcast(cPtr), cMemoryOwn) {
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  public $csclassname(global::System.IntPtr cPtr, bool cMemoryOwn, object Parent)
  : base($imclassname.$csclassname_SWIGUpcast(cPtr), cMemoryOwn, Parent) {
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  public static global::System.Runtime.InteropServices.HandleRef getCPtr($csclassname obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }
  %}
  
  // redefine input type for 'non-const &'
  %typemap(cstype,out="$csclassname") Class & %{ ref $csclassname %};

  // restore default input type for const & 
  %typemap(cstype,out="$csclassname") const Class & %{ $csclassname %};

  // redefine wrapping of returning reference to class
  %typemap(csout, excode=SWIGEXCODE) Class & {
    $csclassname ret = new $csclassname($imcall, $owner, this);$excode
    return ret;
  }
  
%enddef

// Macro for wrapping copyable class
%define WRAP_CLASS_COPYABLE(Class)

  // Redefine body for C# wrapper class (add parent field)
  %typemap(csbody) Class %{
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  protected bool swigCMemOwn;

  protected object swigParent;

  public $csclassname(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  public $csclassname(global::System.IntPtr cPtr, bool cMemoryOwn, object Parent) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
    if ( ! cMemoryOwn ) swigParent = Parent; // keep reference to assumed parent object
  }

  public static global::System.Runtime.InteropServices.HandleRef getCPtr($csclassname obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  // Auxiliary structure for holding wrapped C reference to object 
  // requiring explicit decision to use it either as reference or a copy
  public struct CRef {
    public CRef ($csclassname ptr) { Ptr = ptr; }
    public $csclassname AsReference () { return Ptr; }
    public $csclassname AsCopy () { return Ptr.MakeCopy(); }
    public $csclassname Ptr;
  }
  %}

  // Redefine body for C# wrapper class for derived classes 
  %typemap(csbody_derived) Class %{
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;

  public $csclassname(global::System.IntPtr cPtr, bool cMemoryOwn)
  : base($imclassname.$csclazznameSWIGUpcast(cPtr), cMemoryOwn) {
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  public $csclassname(global::System.IntPtr cPtr, bool cMemoryOwn, object Parent)
  : base($imclassname.$csclassname_SWIGUpcast(cPtr), cMemoryOwn, Parent) {
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  public static global::System.Runtime.InteropServices.HandleRef getCPtr($csclassname obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  // Auxiliary structure for holding wrapped C reference to object
  // requiring explicit decision to use it either as a reference or as a copy
  #pragma warning disable 0109
  public new struct CRef {
    public CRef ($csclassname ptr) { Ptr = ptr; }
    public $csclassname AsReference () { return Ptr; }
    public $csclassname AsCopy () { return Ptr.MakeCopy(); }
    public $csclassname Ptr;
  }
  #pragma warning restore 0109
  %}

  // redefine input and output type for 'non-const &'
  %typemap(cstype,out="$csclassname.CRef") Class & %{ ref Class %};

  // restore default input and output types for const &
  %typemap(cstype,out="$csclassname") const Class & %{ Class %};

  // redefine wrapping of returning reference to class
  %typemap(csout, excode=SWIGEXCODE) Class & {
    $csclassname ret = new $csclassname($imcall, $owner, this);$excode
    return new $csclassname.CRef (ret);
  }
  
  // redefine wrapping of returning const reference to class
  %typemap(csout, excode=SWIGEXCODE) const Class & {
    $csclassname ret = new $csclassname($imcall, true);$excode
    return ret;
  }
  %typemap(out) const Class & %{
    $result = new Class ( *$1 );
  %}
  
  // provide method Copy for usage by CRef class
  %extend Class {
    Class MakeCopy () { return *$self; }
  }

%enddef
