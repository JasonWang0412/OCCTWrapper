# Created: 2019-06-05

# Copyright (c) 2019-2021 OPEN CASCADE SAS

# This file is part of commercial software by OPEN CASCADE SAS.

# This software is furnished in accordance with the terms and conditions
# of the contract and with the inclusion of this copyright notice.
# This software or any other copy thereof may not be provided or otherwise
# be made available to any third party.

# No ownership title to the software is transferred hereby.

# OPEN CASCADE SAS makes no representation or warranties with respect to the
# performance of this software, and specifically disclaims any responsibility
# for any damages, special or consequential, connected with its use.

cmake_minimum_required (VERSION 3.13.0 FATAL_ERROR)

project (OCCCSharp)

# Version number. To be updated manually.
set   (VERSION_MAJOR       1)
set   (VERSION_MINOR       0)
set   (VERSION_MAINTENANCE 0)
set   (VERSION_CORRECTION  0)
set   (PRODUCT_NAME        ${PROJECT_NAME})
set   (COM_VISIBLE         false)
set   (WPF_THEME_INFO_TMPL "[assembly: System.Windows.ThemeInfo (System.Windows.ResourceDictionaryLocation.None, System.Windows.ResourceDictionaryLocation.SourceAssembly)]")
string(TIMESTAMP YEAR "%Y")

list(APPEND CMAKE_MODULE_PATH "${CMAKE_SOURCE_DIR}/cmake")
list(APPEND CMAKE_MODULE_PATH "${CMAKE_SOURCE_DIR}/../../adm/cmake")
set (CMAKE_CONFIGURATION_TYPES Release Debug RelWithDebInfo MinSizeRel CACHE INTERNAL "" FORCE)

# Get all used variables: OS_WITH_BIT, COMPILER
include(occt_defs_flags)
include(occt_macros)

OCCT_MAKE_OS_WITH_BITNESS()
OCCT_MAKE_COMPILER_SHORT_NAME()

if (OS_WITH_BIT STREQUAL "win32")
  set(TARGET_PLATFORM "x86")
else()
  set(TARGET_PLATFORM "x64")
endif()

# =========================================================
# Choose sub-projects
# =========================================================
if (NOT DEFINED WRAP_PRODUCTS)
  set(WRAP_PRODUCTS OFF CACHE BOOL "WRAP products")
endif()
# =========================================================

find_package(SWIG REQUIRED)
find_package(TCL  REQUIRED)

set(BUILD_dotNetFrameworkVersion "v4.0" CACHE STRING "Version of .Net Framework")
set_property(CACHE BUILD_dotNetFrameworkVersion PROPERTY STRINGS v3.5 v4.0 v4.5 v4.5.1 v4.5.2 v4.6 v4.6.1 v4.6.2 v4.7 v4.7.1 v4.7.2)
if (BUILD_dotNetFrameworkVersion STREQUAL "")
  message (FATAL_ERROR "Version of .Net Framework is not specified, please set BUILD_dotNetFrameworkVersion variable" )
else()
  message (STATUS "Using .Net Framework ${BUILD_dotNetFrameworkVersion}" )
endif()

# Find OCCT root dir
find_package(OpenCASCADE REQUIRED)
if (NOT OpenCASCADE_FOUND)
  message (FATAL_ERROR "could not find OpenCASCADE, please set OpenCASCADE_DIR variable" )
else()
  message (STATUS "Using OpenCASCADE from \"${OpenCASCADE_INSTALL_PREFIX}\"" )
endif()

# Set flag to build wrapper with enabled COM interface 
set (USE_CSHARP2COM FALSE CACHE BOOL "Flag to build wrapper with enabled COM interface")

if (USE_CSHARP2COM)
  set (USED_CSHARP2COM "1")
endif()

if (WRAP_PRODUCTS)
  # =========================================================
  # Choose components
  # =========================================================
  if (NOT DEFINED USE_OCC_LICENSE)
    set(USE_OCC_LICENSE OFF CACHE BOOL "Use OCCLicense to protect wrapper")
  endif()
  
  if (NOT DEFINED WRAP_TKOCCLicense)
    set(WRAP_TKOCCLicense OFF CACHE BOOL "WRAP TKOCCLicense component")
  endif()
  
  if (NOT DEFINED WRAP_TKJT)
    set(WRAP_TKJT OFF CACHE BOOL "WRAP TKJT component")
  endif()
  
  if (NOT DEFINED OCCLICENSE_FILE)
    set(OCCLICENSE_FILE "" CACHE FILEPATH "Path to OCCT products license file")
  endif()

  find_package(PRODUCTS QUIET PATHS "${PRODUCTS_DIR}" "${PRODUCTS_INSTALL_PREFIX}" NO_DEFAULT_PATH)
  if (NOT PRODUCTS_FOUND)
    message (FATAL_ERROR "could not find OpenCASCADE Products, please set PRODUCTS_DIR variable" )
  else()
    set (PRODUCTS_DIR "${PRODUCTS_INSTALL_PREFIX}" CACHE PATH "OpenCASCADE Products directory" FORCE)
    message (STATUS "Using OpenCASCADE Products from \"${PRODUCTS_DIR}\"" )
  endif()
endif()

configure_file("${CMAKE_SOURCE_DIR}/cmake/env.bat.in"     "${CMAKE_BINARY_DIR}/env.bat"     @ONLY)
configure_file("${CMAKE_SOURCE_DIR}/cmake/custom.bat.in"  "${CMAKE_BINARY_DIR}/custom.bat"  @ONLY)
configure_file("${CMAKE_SOURCE_DIR}/cmake/listbad.bat.in" "${CMAKE_BINARY_DIR}/listbad.bat" @ONLY)

set(BUILD_DIR "${CMAKE_BINARY_DIR}/${OS_WITH_BIT}/${COMPILER}")
set(CMAKE_ARCHIVE_OUTPUT_DIRECTORY_RELEASE "${BUILD_DIR}/lib")
set(CMAKE_LIBRARY_OUTPUT_DIRECTORY_RELEASE "${BUILD_DIR}/lib")
set(CMAKE_RUNTIME_OUTPUT_DIRECTORY_RELEASE "${BUILD_DIR}/bin")
set(CMAKE_ARCHIVE_OUTPUT_DIRECTORY_DEBUG   "${BUILD_DIR}/libd")
set(CMAKE_LIBRARY_OUTPUT_DIRECTORY_DEBUG   "${BUILD_DIR}/libd")
set(CMAKE_RUNTIME_OUTPUT_DIRECTORY_DEBUG   "${BUILD_DIR}/bind")

set(CMAKE_BINARY_INSTALL_DIRECTORY "${CMAKE_INSTALL_PREFIX}/${OS_WITH_BIT}/${COMPILER}")
set(CMAKE_ARCHIVE_INSTALL_DIRECTORY_RELEASE "${CMAKE_BINARY_INSTALL_DIRECTORY}/lib")
set(CMAKE_LIBRARY_INSTALL_DIRECTORY_RELEASE "${CMAKE_BINARY_INSTALL_DIRECTORY}/lib")
set(CMAKE_RUNTIME_INSTALL_DIRECTORY_RELEASE "${CMAKE_BINARY_INSTALL_DIRECTORY}/bin")
set(CMAKE_ARCHIVE_INSTALL_DIRECTORY_DEBUG   "${CMAKE_BINARY_INSTALL_DIRECTORY}/libd")
set(CMAKE_LIBRARY_INSTALL_DIRECTORY_DEBUG   "${CMAKE_BINARY_INSTALL_DIRECTORY}/libd")
set(CMAKE_RUNTIME_INSTALL_DIRECTORY_DEBUG   "${CMAKE_BINARY_INSTALL_DIRECTORY}/bind")

add_subdirectory(OCCwrapCSharp)
add_subdirectory(CSharpTest)
add_subdirectory(OCCCSharp)
add_subdirectory(OCCWinForms)
add_subdirectory(ImportExport)
add_subdirectory(OCCWpf)
add_subdirectory(ImportExportWPF)

install(FILES "${CMAKE_BINARY_DIR}/env.bat"    DESTINATION "${CMAKE_INSTALL_PREFIX}")
install(FILES "${CMAKE_BINARY_DIR}/custom.bat" DESTINATION "${CMAKE_INSTALL_PREFIX}")
