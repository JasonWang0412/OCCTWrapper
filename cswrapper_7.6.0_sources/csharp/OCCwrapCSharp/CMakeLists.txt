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

project(OCCwrapCSharp)

set(OCCT_WRAPPERS_DIR "${CMAKE_SOURCE_DIR}/../swig")
set(SWIG_INCLUDE_DIRS "-I${CMAKE_CURRENT_SOURCE_DIR}" "-I${OCCT_WRAPPERS_DIR}")
set(SWIG_FLAGS -c++ -csharp -Wextra -w309 -fcompact -fvirtual -DSWIG2_CSHARP -D_WIN32)

function(get_file_size var filename)
  if(EXISTS "${filename}")
    file(READ "${filename}" content HEX)
    string(LENGTH "${content}" content_length)
    math(EXPR content_length "${content_length} / 2")
    set(${var} ${content_length} PARENT_SCOPE)
  else()
    set(${var} 0 PARENT_SCOPE)
  endif()
endfunction()

function(update_shortcut_headers_occt STANDARD_INC_DIR STANDARD_ROOT_DIR SWIG_AUX_INCLUDE_DIRS)
  set(${SWIG_AUX_INCLUDE_DIRS} "" PARENT_SCOPE)

  get_file_size(STANDARD_FILE_SIZE "${STANDARD_INC_DIR}/Standard_Type.hxx")
  if (${STANDARD_FILE_SIZE} GREATER 2000)
    return()
  endif()

  if(NOT EXISTS "${STANDARD_ROOT_DIR}/adm/UDLIST" OR NOT EXISTS "${STANDARD_ROOT_DIR}/src/Standard/Standard_Type.hxx")
    if(NOT EXISTS "${STANDARD_INC_DIR}/Standard_Type.hxx")
      message(FATAL_ERROR "Header file \"Standard_Type.hxx\" is not found in \"${STANDARD_INC_DIR}\"")
    else()
      message(FATAL_ERROR "It seems that OCCT header files are short-cuts at \"${STANDARD_INC_DIR}\". "
                          "Unable finding OCCT source folder for wrapping")
    endif()
    return()
  endif()

  message(WARNING "It seems that OCCT header files are short-cuts at \"${STANDARD_INC_DIR}\". "
                   "Generator will wrap headers directly from source folder \"${STANDARD_ROOT_DIR}/src\"")
  set(SWIG_INCLUDE_DIRS_TMP)
  file (STRINGS "${STANDARD_ROOT_DIR}/adm/UDLIST" LOCAL_FILE_CONTENT)
  foreach (FILE_CONTENT_LINE ${LOCAL_FILE_CONTENT})
    string (REGEX MATCH "^n ([^ ]*)" TK_FOUND ${FILE_CONTENT_LINE})
    if (NOT "${TK_FOUND}" STREQUAL "")
      set(SWIG_INCLUDE_DIRS_TMP ${SWIG_INCLUDE_DIRS_TMP} "-Isrc/${CMAKE_MATCH_1}")
    endif()
  endforeach()
  set(${SWIG_AUX_INCLUDE_DIRS} ${SWIG_INCLUDE_DIRS_TMP} PARENT_SCOPE)
endfunction()

function(update_shortcut_headers STANDARD_FILE STANDARD_INC_DIR STANDARD_SRC_DIR SWIG_AUX_INCLUDE_DIRS)
  set(${SWIG_AUX_INCLUDE_DIRS} "" PARENT_SCOPE)
  if(NOT EXISTS "${STANDARD_INC_DIR}/${STANDARD_FILE}")
    message(FATAL_ERROR "Header file \"${STANDARD_FILE}\" is not found in \"${STANDARD_INC_DIR}\"")
    return()
  endif()

  get_file_size(STANDARD_FILE_SIZE "${STANDARD_INC_DIR}/${STANDARD_FILE}")
  if (${STANDARD_FILE_SIZE} GREATER 2000)
    return()
  endif()

  message(WARNING "It seems that OCC Product header files are short-cuts at \"${STANDARD_INC_DIR}\"")
  message(WARNING "Generator will wrap headers directly from source folder \"${STANDARD_SRC_DIR}\"")

  file(GLOB_RECURSE SourceHeaderFiles "${STANDARD_SRC_DIR}/*.hxx")
  list(LENGTH SourceHeaderFiles HEADERS_NB)
  if (${HEADERS_NB} EQUAL 0)
    message(FATAL_ERROR "OCC Product header files are missing at \"${STANDARD_SRC_DIR}\"")
  endif()

  set(HEADERS_DIRECTORIES)
  foreach (HEADER_FILE ${SourceHeaderFiles})
    get_filename_component(HEADER_PATH ${HEADER_FILE} DIRECTORY)
    list(APPEND HEADERS_DIRECTORIES "${HEADER_PATH}")
  endforeach()

  set(SWIG_INCLUDE_DIRS_TMP)
  list(REMOVE_DUPLICATES HEADERS_DIRECTORIES)
  foreach (HEADER_FILE ${HEADERS_DIRECTORIES})
    set(SWIG_INCLUDE_DIRS_TMP ${SWIG_INCLUDE_DIRS_TMP} "-I${HEADER_FILE}")
  endforeach()

  set(${SWIG_AUX_INCLUDE_DIRS} ${SWIG_INCLUDE_DIRS_TMP} PARENT_SCOPE)
endfunction()

macro (WRAP_PRODUCT PRODUCT_NAME)
  set(PRODUCTS_WRAPPERS_ROOT_FILE "${PRODUCT_NAME}.i")
  if(EXISTS "${PRODUCTS_WRAPPERS_DIR}/${PRODUCTS_WRAPPERS_ROOT_FILE}")
    set(SWIG_INCLUDE_DIRS ${SWIG_INCLUDE_DIRS} "-I${PRODUCTS_WRAPPERS_DIR}")
    set(SWIG_FLAGS ${SWIG_FLAGS} -DWRAP_${PRODUCT_NAME})
    
    update_shortcut_headers("OCCLicense_Activate.hxx" "${PRODUCTS_INCLUDE_DIR}" "${PRODUCTS_RESOURCE_DIR}" SWIG_AUX_INCLUDE_DIRS)
    if (NOT "${SWIG_AUX_INCLUDE_DIRS}" STREQUAL "")
      set(SWIG_INCLUDE_DIRS ${SWIG_INCLUDE_DIRS} ${SWIG_AUX_INCLUDE_DIRS})
    else()
      set(SWIG_INCLUDE_DIRS ${SWIG_INCLUDE_DIRS} "-I${PRODUCTS_INCLUDE_DIR}")
    endif()
  endif()
endmacro()

macro (ADD_PRODUCT PRODUCT_NAME)
  set(PRODUCTS_WRAPPERS_ROOT_FILE "${PRODUCT_NAME}.i")
  get_filename_component(PRODUCTS_LIBRARY_NAME ${PRODUCTS_WRAPPERS_ROOT_FILE} NAME_WE)
  target_link_libraries (${PROJECT_NAME} PRIVATE optimized "${PRODUCTS_LIBRARY_DIR}/${PRODUCTS_LIBRARY_NAME}.lib")
  target_link_libraries (${PROJECT_NAME} PRIVATE debug     "${PRODUCTS_LIBRARY_DIR}d/${PRODUCTS_LIBRARY_NAME}.lib")

  if(EXISTS "${CMAKE_SOURCE_DIR}/${PRODUCTS_LIBRARY_NAME}")  
    add_subdirectory("${CMAKE_SOURCE_DIR}/${PRODUCTS_LIBRARY_NAME}" ${PRODUCTS_LIBRARY_NAME})
  endif()
endmacro()

update_shortcut_headers_occt("${OpenCASCADE_INCLUDE_DIR}" "${OpenCASCADE_DIR}" SWIG_AUX_INCLUDE_DIRS)
if (NOT "${SWIG_AUX_INCLUDE_DIRS}" STREQUAL "")
  set(SWIG_INCLUDE_DIRS ${SWIG_INCLUDE_DIRS} ${SWIG_AUX_INCLUDE_DIRS})
else()
  set(SWIG_INCLUDE_DIRS ${SWIG_INCLUDE_DIRS} "-I${OpenCASCADE_INCLUDE_DIR}")
endif()

include_directories(${OpenCASCADE_INCLUDE_DIR})

if(EXISTS "${OpenCASCADE_INCLUDE_DIR}/D3DHost_GraphicDriver.hxx")
  set(SWIG_FLAGS ${SWIG_FLAGS} -DWRAP_D3DHOST)
endif()

set(PRODUCTS_WRAPPERS_DIR "${OCCT_WRAPPERS_DIR}")

if (DEFINED WRAP_PRODUCTS AND WRAP_PRODUCTS)
  include_directories(${PRODUCTS_INCLUDE_DIR})
endif()

if (DEFINED WRAP_TKOCCLicense AND WRAP_TKOCCLicense)
  WRAP_PRODUCT ("TKOCCLicense")
endif()

if (DEFINED WRAP_TKJT AND WRAP_TKJT)
  WRAP_PRODUCT ("TKJT")
endif()

set(SWIG_OUTDIR "${CMAKE_BINARY_DIR}/Wrappers")
set(SWIG_LOG_FILE "${CMAKE_CURRENT_BINARY_DIR}/makewrapper.log")
set(SWIG_TMP_LOG_FILE "${CMAKE_CURRENT_BINARY_DIR}/makewrapper.log_tmp")

# Specify relative paths to $CASROOT/src packages and execute SWIG in $CASROOT
# to workaround command line arguments length limits on Windows platform.
set(SWIG_WORKING_DIR "${SWIG_OUTDIR}")
if(EXISTS "${OpenCASCADE_DIR}/src/Standard")
  set(SWIG_WORKING_DIR "${OpenCASCADE_DIR}")
endif()

set(INTERFACE_FILE_NAME        occcsharp)
set(INTERFACE_WRAP_FILE_NAME   ${INTERFACE_FILE_NAME}_wrap)
set(INTERFACE_FILE             "${CMAKE_CURRENT_SOURCE_DIR}/${INTERFACE_FILE_NAME}.i")
set(INTERFACE_WRAP_FILE        "${SWIG_OUTDIR}/${INTERFACE_WRAP_FILE_NAME}.cxx")
set(INTERFACE_WRAP_HEADER_FILE "${SWIG_OUTDIR}/${INTERFACE_WRAP_FILE_NAME}.h")

set(UPDTOOL "${CMAKE_CURRENT_SOURCE_DIR}/updatewrappers.tcl")
if(NOT EXISTS "${UPDTOOL}")
  set(UPDTOOL "${OCCT_WRAPPERS_DIR}/updatewrappers.tcl")
endif()

macro (PUT_TO_LOG)
  file(READ   "${SWIG_TMP_LOG_FILE}" LOG_FILE_CONTENT)
  file(APPEND "${SWIG_LOG_FILE}"     ${LOG_FILE_CONTENT})
endmacro()

# =========================================================
# Generate wrappers
# =========================================================
file(GLOB GeneratedFiles "${SWIG_OUTDIR}/*.cs")
list(LENGTH GeneratedFiles WRAPPERS_NB)
if (${WRAPPERS_NB} EQUAL 0)
  message(STATUS "Building OCC CSharp wrapper...")
  execute_process(COMMAND "${CMAKE_COMMAND}" -E remove -f      "${SWIG_LOG_FILE}")
  execute_process(COMMAND "${CMAKE_COMMAND}" -E make_directory "${SWIG_OUTDIR}")
  execute_process(COMMAND "${CMAKE_COMMAND}" -E touch          "${INTERFACE_WRAP_FILE}")

  execute_process(
    COMMAND cmd /c date /t && time /t && echo "Invoking SWIG..."
    OUTPUT_FILE "${SWIG_TMP_LOG_FILE}"
    ERROR_FILE  "${SWIG_TMP_LOG_FILE}"
  )
  PUT_TO_LOG()

  message(STATUS "\"${SWIG_EXECUTABLE}\" ${SWIG_FLAGS} ${SWIG_INCLUDE_DIRS} -o \"${INTERFACE_WRAP_FILE}\" -outdir \"${SWIG_OUTDIR}\" \"${INTERFACE_FILE}\"")
  execute_process(
   COMMAND "${SWIG_EXECUTABLE}" ${SWIG_FLAGS} ${SWIG_INCLUDE_DIRS} -o "${INTERFACE_WRAP_FILE}" -outdir "${SWIG_OUTDIR}" "${INTERFACE_FILE}"
   OUTPUT_FILE "${SWIG_TMP_LOG_FILE}"
   ERROR_FILE  "${SWIG_TMP_LOG_FILE}"
   WORKING_DIRECTORY "${SWIG_WORKING_DIR}"
  )
  PUT_TO_LOG()

  execute_process(
   COMMAND cmd /c echo "Dispatching wrappers by packages..."
   OUTPUT_FILE "${SWIG_TMP_LOG_FILE}"
   ERROR_FILE  "${SWIG_TMP_LOG_FILE}"
  )
  PUT_TO_LOG()

  execute_process(
   COMMAND cmd /c call "${CMAKE_BINARY_DIR}/env.bat" && "${TCL_TCLSH}" "${UPDTOOL}" csharp "${SWIG_OUTDIR}"
   OUTPUT_FILE "${SWIG_TMP_LOG_FILE}"
   ERROR_FILE  "${SWIG_TMP_LOG_FILE}"
  )
  PUT_TO_LOG()

  execute_process(
    COMMAND cmd /c echo "Finished at" && time /t
    OUTPUT_FILE "${SWIG_TMP_LOG_FILE}"
    ERROR_FILE  "${SWIG_TMP_LOG_FILE}"
  )
  PUT_TO_LOG()

  file(READ "${SWIG_LOG_FILE}" SWIG_LOG)
  message(STATUS ${SWIG_LOG})
endif()
# =========================================================

set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} /bigobj")

set(DOC_FILES
  ${CMAKE_CURRENT_SOURCE_DIR}/../Readme.txt
  ${CMAKE_CURRENT_SOURCE_DIR}/ChangeLog.txt
  ${CMAKE_CURRENT_SOURCE_DIR}/OCC2CSharp.txt
)

add_library(${PROJECT_NAME} SHARED
  ${INTERFACE_WRAP_FILE}
  ${INTERFACE_WRAP_HEADER_FILE}
)

file(GLOB OCCT_LIBRARIES "${OpenCASCADE_LIBRARY_DIR}/*.lib")
foreach (OCCT_LIB ${OCCT_LIBRARIES})
  get_filename_component(OCCT_LIB_NAME ${OCCT_LIB} NAME_WE)
  target_link_libraries (${PROJECT_NAME} PRIVATE optimized "${OpenCASCADE_LIBRARY_DIR}/${OCCT_LIB_NAME}.lib")
endforeach()

file(GLOB OCCT_LIBRARIES "${OpenCASCADE_LIBRARY_DIR}d/*.lib")
foreach (OCCT_LIB ${OCCT_LIBRARIES})
  get_filename_component(OCCT_LIB_NAME ${OCCT_LIB} NAME_WE)
  target_link_libraries (${PROJECT_NAME} PRIVATE debug     "${OpenCASCADE_LIBRARY_DIR}d/${OCCT_LIB_NAME}.lib")
endforeach()

if (DEFINED USE_OCC_LICENSE AND USE_OCC_LICENSE)
  add_definitions(-DUSE_OCC_LICENSE)
  ADD_PRODUCT ("TKOCCLicense")
endif()

if (DEFINED WRAP_TKOCCLicense AND WRAP_TKOCCLicense)
  ADD_PRODUCT ("TKOCCLicense")
endif()

if (DEFINED WRAP_TKJT AND WRAP_TKJT)
  ADD_PRODUCT ("TKJT")
  ADD_PRODUCT ("TKXDEJT")
endif()

install(FILES $<TARGET_PDB_FILE:${PROJECT_NAME}> CONFIGURATIONS Debug                   DESTINATION "${CMAKE_RUNTIME_INSTALL_DIRECTORY_DEBUG}" OPTIONAL)
install(TARGETS ${PROJECT_NAME}                  CONFIGURATIONS Debug   RUNTIME         DESTINATION "${CMAKE_RUNTIME_INSTALL_DIRECTORY_DEBUG}")
install(TARGETS ${PROJECT_NAME}                  CONFIGURATIONS Release RUNTIME         DESTINATION "${CMAKE_RUNTIME_INSTALL_DIRECTORY_RELEASE}")
install(TARGETS ${PROJECT_NAME}                  CONFIGURATIONS Debug   LIBRARY ARCHIVE DESTINATION "${CMAKE_LIBRARY_INSTALL_DIRECTORY_DEBUG}")
install(TARGETS ${PROJECT_NAME}                  CONFIGURATIONS Release LIBRARY ARCHIVE DESTINATION "${CMAKE_LIBRARY_INSTALL_DIRECTORY_RELEASE}")
