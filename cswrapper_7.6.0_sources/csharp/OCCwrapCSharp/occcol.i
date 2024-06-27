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

// Definition of macros for wrapping instantiations of template classes from 
// NCollection package to CSharp using SWIG.

// Macro for wrapping simple template instantiations (without nested classes)
%define WRAP_AS_TEMPLATE(Class,Template...)
  // suppress warning on unknown base types, we do not need them to be wrapped
  %warnfilter(401);

  // ignore STL iterators
  %ignore Template::begin;
  %ignore Template::cbegin;
  %ignore Template::end;
  %ignore Template::cend;

  WRAP_CLASS_NOCOPY(Template)
  %template(Class) Template;
  %warnfilter(+401);
%enddef

// Macro for wrapping simple template instantiations (without nested classes)
%define WRAP_AS_TEMPLATE_INCLUDE(Class,Template...)
  WRAP_AS_TEMPLATE(Class,Template)
  WRAP_INCLUDE(Class)
%enddef

// Macro extending NCollection Iteraror template class with IEnumerator interface.
%define EXTEND_NCOLLECTION_ITERATOR_AS_ENUMERATOR(Class_Iterator,CSType)
  %typemap(csbase) Class_Iterator "global::System.Collections.Generic.IEnumerator<CSType>"
  %typemap(cscode) Class_Iterator %{
  #region IEnumerator interface
  //! Auxiliary variable for handling different semantic of OCCT and C# iterators.
  private bool myIsPreFirst = true;

  //! IEnumerator interface implementation: moves enumerator to the next position.
  [global::System.Obsolete("Internal method for IEnumerator interface.", true)]
  public virtual bool MoveNext()
  {
    if (myIsPreFirst)
    {
      myIsPreFirst = false;
      return More();
    }
    Next();
    return More();
  }

  //! IEnumerator interface implementation: reset enumerator; NOT implemented.
  [global::System.Obsolete("Internal method for IEnumerator interface.", true)]
  public virtual void Reset() { throw new global::System.InvalidOperationException ("Not implemented"); }

  //! IEnumerator interface implementation: current enumerator value.
  [global::System.Obsolete("Internal method for IEnumerator interface.", true)]
  object global::System.Collections.IEnumerator.Current { get { if (myIsPreFirst) { throw new global::System.InvalidOperationException ("Enumerator at invalid position"); } else { return Value(); } } }

  //! IEnumerator interface implementation: current enumerator value.
  [global::System.Obsolete("Internal method for IEnumerator interface.", true)]
  public CSType Current { get { if (myIsPreFirst) { throw new global::System.InvalidOperationException ("Enumerator at invalid position"); } else { return Value(); } } }
  #endregion

  //! Handy property for accessing Value() from Debugger.
  public CSType value { get { return Value(); } }
  %}
%enddef

// Macro extending NCollection template class with IEnumerable interface.
%define EXTEND_NCOLLECTION_AS_ENUMERABLE(Class_Collection,Class_Iterator,CSType)
  %typemap(csbase) Class_Collection "global::System.Collections.Generic.IEnumerable<CSType>"
  %typemap(cscode) Class_Collection %{
  #region IEnumerable interface
  //! IEnumerable interface implementation.
  public global::System.Collections.Generic.IEnumerator<CSType> GetEnumerator() { return new Class_Iterator (this); }

  //! IEnumerable interface implementation.
  global::System.Collections.IEnumerator global::System.Collections.IEnumerable.GetEnumerator() { return new Class_Iterator (this); }

  //! Handy property for accessing entities list from Debugger.
  //! Class_Iterator should be used instead for efficient access (without creating a temporary List).
  public global::System.Collections.Generic.List<CSType> elementsAsList {
    get {
      global::System.Collections.Generic.List<CSType> aList = new global::System.Collections.Generic.List<CSType>();
      foreach (CSType aChildIter in this) { aList.Add (aChildIter); }
      return aList;
    }
  }
  #endregion
  %}
%enddef

// Macro for wrapping NCollection template classes
%define WRAP_AS_NCOLLECTION(Class,Collection,Type)

  // skip wrapping of iterator inside collection to prevent name collision
  %ignore Collection< Type >::Iterator;
  %ignore Collection< Type >::Node;

  EXTEND_NCOLLECTION_ITERATOR_AS_ENUMERATOR(Class##_Iterator,$typemap(cstype,Type))
  EXTEND_NCOLLECTION_AS_ENUMERABLE(Collection<Type>,Class##_Iterator,$typemap(cstype,Type))

  // provide standard typemaps for iterator
  WRAP_CLASS_NOCOPY(Class##_Iterator)

  // define iterator class as global one
  class Class##_Iterator
  {
  public:
    Class##_Iterator (const Collection< Type >&);
    Standard_Boolean More () const;
    void Next ();
    const Type& Value () const;
    Type& ChangeValue () const;
  };
  %{
  typedef Collection< Type >::Iterator Class##_Iterator;
  %}

  // define typemaps to map nested iterator class in C++ to global in C# 
  %typemap(cstype) Collection< Type >::Iterator & %{ Class##_Iterator %};
  %typemap(csin) Collection< Type >::Iterator & %{ Class##_Iterator.getCPtr($csinput) %}

  %ignore Collection< Type >::First() const;
  %ignore Collection< Type >::Last() const;

  // wrap main template class
  WRAP_AS_TEMPLATE(Class,Collection< Type >)

%enddef

// Macro for wrapping class defined as typedef to NCollection template
%define WRAP_AS_NCOLLECTION_INCLUDE(Class,Collection,Type)
  WRAP_AS_NCOLLECTION(Class,Collection,Type)
  WRAP_INCLUDE(Class)
%enddef

// Macro for wrapping NCollection template hash map classes
%define WRAP_AS_MAP(Class,Collection,Type,Hasher)

  // skip wrapping of iterator and map node inside collection to prevent name collision
  %ignore Collection< Type, Hasher >::Iterator;
  %ignore Collection< Type, Hasher >::MapNode;
  
  // do not wrap methods returning C pointer
  %ignore Collection< Type, Hasher >::Seek;
  %ignore Collection< Type, Hasher >::ChangeSeek;
  
  // provide standard typemaps for iterator
  WRAP_CLASS_NOCOPY(Class##_Iterator)

  // define iterator class as global one
  class Class##_Iterator
  {
  public:
    Class##_Iterator (const Collection< Type, Hasher >&);
    Standard_Boolean More () const;
    void Next ();
    const Type& Value () const;
  };
  %{
  typedef Collection< Type, Hasher >::Iterator Class##_Iterator;
  %}

  // define typemaps to map nested iterator class in C++ to global in C# 
  %typemap(cstype) Collection< Type, Hasher >::Iterator & %{ Class##_Iterator %};
  %typemap(csin) Collection< Type, Hasher >::Iterator & %{ Class##_Iterator.getCPtr($csinput) %}

  // wrap main template class
  WRAP_AS_TEMPLATE(Class,Collection< Type, Hasher >)

%enddef

// Macro for wrapping class defined as typedef to NCollection template
%define WRAP_AS_MAP_INCLUDE(Class,Collection,Type,Hasher)
  WRAP_AS_MAP(Class,Collection,Type,Hasher)
  WRAP_INCLUDE(Class)
%enddef

// Macro for wrapping NCollection template data map classes
%define WRAP_AS_DATAMAP(Class,Collection,TypeKey,TypeValue,Hasher)

  // skip wrapping of iterator and map node inside collection to prevent name collision
  %ignore Collection< TypeKey, TypeValue, Hasher >::Iterator;
  %ignore Collection< TypeKey, TypeValue, Hasher >::DataMapNode;
  %ignore Collection< TypeKey, TypeValue, Hasher >::IndexedDataMapNode;
  
  // do not wrap methods returning C pointer
  %ignore Collection< TypeKey, TypeValue, Hasher >::Bound;
  %ignore Collection< TypeKey, TypeValue, Hasher >::Seek;
  %ignore Collection< TypeKey, TypeValue, Hasher >::ChangeSeek;
  
  // provide standard typemaps for iterator
  WRAP_CLASS_NOCOPY(Class##_Iterator)

  // define iterator class as global one
  class Class##_Iterator
  {
  public:
    Class##_Iterator (const Collection< TypeKey, TypeValue, Hasher >&);
    Standard_Boolean More () const;
    void Next ();
    const TypeKey& Key () const;
    const TypeValue& Value () const;
    TypeValue& ChangeValue () const;
  };
  %{
  typedef Collection< TypeKey, TypeValue, Hasher >::Iterator Class##_Iterator;
  %}

  // define typemaps to map nested iterator class in C++ to global in C# 
  %typemap(cstype) Collection< TypeKey, TypeValue, Hasher >::Iterator & %{ Class##_Iterator %};
  %typemap(csin) Collection< TypeKey, TypeValue, Hasher >::Iterator & %{ Class##_Iterator.getCPtr($csinput) %}

  // wrap main template class
  WRAP_AS_TEMPLATE(Class,Collection< TypeKey, TypeValue, Hasher >)

%enddef

// Macro for wrapping class defined as typedef to NCollection template
%define WRAP_AS_DATAMAP_INCLUDE(Class,Collection,TypeKey,TypeValue,Hasher)
  WRAP_AS_DATAMAP(Class,Collection,TypeKey,TypeValue,Hasher)
  WRAP_INCLUDE(Class)
%enddef
