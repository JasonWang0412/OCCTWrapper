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

project (OCCCSharp LANGUAGES CSharp)
include (CSharpUtilities)

set(DOC_FILE "obj/${PROJECT_NAME}.xml")
set(CMAKE_CSharp_FLAGS "/platform:${TARGET_PLATFORM} -nowarn:1591 -doc${DOC_FILE}")

set(COM_VISIBLE_TMP ${COM_VISIBLE})
set(COM_VISIBLE true)
configure_file("${CMAKE_SOURCE_DIR}/cmake/AssemblyInfo.cs.in" "AssemblyInfo.cs" @ONLY)
set(COM_VISIBLE ${COM_VISIBLE_TMP})

set (PropertyFiles
  AssemblyInfo.cs
)

set(WRAPPERS_DIR "${CMAKE_BINARY_DIR}/Wrappers")
if(NOT EXISTS "${WRAPPERS_DIR}")
  file(MAKE_DIRECTORY "${WRAPPERS_DIR}")
endif()

file(GLOB SourceFiles "${WRAPPERS_DIR}/*.cs")
add_library(${PROJECT_NAME} SHARED
  ${PropertyFiles}
  ${SourceFiles}
)

set (DEPENDENCIES_LIST OCCwrapCSharp)
add_dependencies(${PROJECT_NAME} ${DEPENDENCIES_LIST})

set_property(TARGET ${PROJECT_NAME} PROPERTY VS_DOTNET_TARGET_FRAMEWORK_VERSION "${BUILD_dotNetFrameworkVersion}")
set_property(TARGET ${PROJECT_NAME} PROPERTY WIN32_EXECUTABLE FALSE)
set_property(TARGET ${PROJECT_NAME} PROPERTY VS_GLOBAL_ROOTNAMESPACE ${PROJECT_NAME})
set_property(TARGET ${PROJECT_NAME} PROPERTY VS_DOTNET_REFERENCES
  System
)

install(FILES $<TARGET_PDB_FILE:${PROJECT_NAME}>          CONFIGURATIONS Debug                   DESTINATION "${CMAKE_RUNTIME_INSTALL_DIRECTORY_DEBUG}" OPTIONAL)
install(TARGETS ${PROJECT_NAME}                           CONFIGURATIONS Debug   RUNTIME         DESTINATION "${CMAKE_RUNTIME_INSTALL_DIRECTORY_DEBUG}")
install(TARGETS ${PROJECT_NAME}                           CONFIGURATIONS Release RUNTIME         DESTINATION "${CMAKE_RUNTIME_INSTALL_DIRECTORY_RELEASE}")
install(TARGETS ${PROJECT_NAME}                           CONFIGURATIONS Debug   LIBRARY ARCHIVE DESTINATION "${CMAKE_LIBRARY_INSTALL_DIRECTORY_DEBUG}")
install(TARGETS ${PROJECT_NAME}                           CONFIGURATIONS Release LIBRARY ARCHIVE DESTINATION "${CMAKE_LIBRARY_INSTALL_DIRECTORY_RELEASE}")
install(FILES   "${CMAKE_CURRENT_BINARY_DIR}/${DOC_FILE}" CONFIGURATIONS Debug                   DESTINATION "${CMAKE_RUNTIME_INSTALL_DIRECTORY_DEBUG}")
install(FILES   "${CMAKE_CURRENT_BINARY_DIR}/${DOC_FILE}" CONFIGURATIONS Release                 DESTINATION "${CMAKE_RUNTIME_INSTALL_DIRECTORY_RELEASE}")
