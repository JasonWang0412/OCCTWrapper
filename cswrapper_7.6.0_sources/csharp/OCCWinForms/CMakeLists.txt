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

project (OCCWinForms LANGUAGES CSharp)
include (CSharpUtilities)
set(CMAKE_CSharp_FLAGS "/platform:${TARGET_PLATFORM}")

configure_file("${CMAKE_SOURCE_DIR}/cmake/AssemblyInfo.cs.in" "AssemblyInfo.cs" @ONLY)

set (GuiFiles
  OCCWinForms_ViewControl.cs
  OCCWinForms_ViewControl.Designer.cs
  
  OCCWinForms_Viewer.cs
  OCCWinForms_Viewer.Designer.cs
  OCCWinForms_Viewer.resx
)

set (PropertyFiles
  AssemblyInfo.cs
  Properties/Resources.resx
  Properties/Resources.Designer.cs
  
  Properties/Settings.settings
  Properties/Settings.Designer.cs
)

add_library(${PROJECT_NAME} SHARED
  ${GuiFiles}
  ${PropertyFiles}
)

set (DEPENDENCIES_LIST OCCCSharp)
add_dependencies(${PROJECT_NAME} ${DEPENDENCIES_LIST})

set_property(TARGET ${PROJECT_NAME} PROPERTY VS_DOTNET_TARGET_FRAMEWORK_VERSION "${BUILD_dotNetFrameworkVersion}")
set_property(TARGET ${PROJECT_NAME} PROPERTY WIN32_EXECUTABLE FALSE)
set_property(TARGET ${PROJECT_NAME} PROPERTY VS_GLOBAL_ROOTNAMESPACE ${PROJECT_NAME})
set_property(TARGET ${PROJECT_NAME} PROPERTY VS_DOTNET_REFERENCES
  System
  System.Drawing
  System.Windows.Forms
  ${DEPENDENCIES_LIST}
)

csharp_set_designer_cs_properties(${PropertyFiles} ${GuiFiles})
csharp_set_windows_forms_properties(${GuiFiles})

install(FILES $<TARGET_PDB_FILE:${PROJECT_NAME}> CONFIGURATIONS Debug                   DESTINATION "${CMAKE_RUNTIME_INSTALL_DIRECTORY_DEBUG}" OPTIONAL)
install(TARGETS ${PROJECT_NAME}                  CONFIGURATIONS Debug   RUNTIME         DESTINATION "${CMAKE_RUNTIME_INSTALL_DIRECTORY_DEBUG}")
install(TARGETS ${PROJECT_NAME}                  CONFIGURATIONS Release RUNTIME         DESTINATION "${CMAKE_RUNTIME_INSTALL_DIRECTORY_RELEASE}")
