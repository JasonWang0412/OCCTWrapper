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

project (CSharpTest LANGUAGES CSharp)
include (CSharpUtilities)
set(CMAKE_CSharp_FLAGS "/platform:${TARGET_PLATFORM} /win32icon:${CMAKE_CURRENT_SOURCE_DIR}\\res\\App.ico")

configure_file("${CMAKE_SOURCE_DIR}/cmake/AssemblyInfo.cs.in" "AssemblyInfo.cs" @ONLY)

set (PropertyFiles
  AssemblyInfo.cs
)

file(GLOB SourceFiles "${CMAKE_CURRENT_SOURCE_DIR}/*.cs")
file(GLOB Resources   "res/*")
add_executable(${PROJECT_NAME}
  ${PropertyFiles}
  ${SourceFiles}
  ${Resources}
)

set (DEPENDENCIES_LIST OCCCSharp)
add_dependencies(${PROJECT_NAME} ${DEPENDENCIES_LIST})

set_property(TARGET ${PROJECT_NAME} PROPERTY VS_DOTNET_TARGET_FRAMEWORK_VERSION "${BUILD_dotNetFrameworkVersion}")
set_property(TARGET ${PROJECT_NAME} PROPERTY WIN32_EXECUTABLE FALSE)
set_property(TARGET ${PROJECT_NAME} PROPERTY VS_GLOBAL_ROOTNAMESPACE ${PROJECT_NAME})
set_property(TARGET ${PROJECT_NAME} PROPERTY VS_DOTNET_REFERENCES
  System
  ${DEPENDENCIES_LIST}
)

csharp_set_designer_cs_properties(${PropertyFiles} ${Resources})
set_source_files_properties(${Resources} PROPERTIES VS_TOOL_OVERRIDE "Resource")

configure_file("${CMAKE_SOURCE_DIR}/cmake/run.bat.in" "${CMAKE_BINARY_DIR}/${PROJECT_NAME}.bat" @ONLY)

install(FILES $<TARGET_PDB_FILE:${PROJECT_NAME}> CONFIGURATIONS Debug                   DESTINATION "${CMAKE_RUNTIME_INSTALL_DIRECTORY_DEBUG}" OPTIONAL)
install(TARGETS ${PROJECT_NAME}                  CONFIGURATIONS Debug   RUNTIME         DESTINATION "${CMAKE_RUNTIME_INSTALL_DIRECTORY_DEBUG}")
install(TARGETS ${PROJECT_NAME}                  CONFIGURATIONS Release RUNTIME         DESTINATION "${CMAKE_RUNTIME_INSTALL_DIRECTORY_RELEASE}")
install(FILES   "${CMAKE_BINARY_DIR}/${PROJECT_NAME}.bat"                               DESTINATION "${CMAKE_INSTALL_PREFIX}")
