// Created: 2015-12-15
//
// Copyright (c) 2015-2021 OPEN CASCADE SAS
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

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace ImportExportWPF
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    public App () : base()
    {
      string aResDir = System.Reflection.Assembly.GetExecutingAssembly().Location;
      aResDir = System.IO.Path.GetDirectoryName (aResDir) + "\\res";
      if (!System.IO.File.Exists (aResDir + "\\SHAPE.us"))
      {
        // broken setup - rely on environment variables set by batch script
        return;
      }

      System.Environment.SetEnvironmentVariable ("CSF_SHMessage",             aResDir);
      System.Environment.SetEnvironmentVariable ("CSF_MDTVTexturesDirectory", aResDir);
      System.Environment.SetEnvironmentVariable ("CSF_XSMessage",             aResDir);
      System.Environment.SetEnvironmentVariable ("CSF_StandardDefaults",      aResDir);
      System.Environment.SetEnvironmentVariable ("CSF_PluginDefaults",        aResDir);
      System.Environment.SetEnvironmentVariable ("CSF_XCAFDefaults",          aResDir);
      System.Environment.SetEnvironmentVariable ("CSF_TObjDefaults",          aResDir);
      System.Environment.SetEnvironmentVariable ("CSF_StandardLiteDefaults",  aResDir);
      System.Environment.SetEnvironmentVariable ("CSF_IGESDefaults",          aResDir);
      System.Environment.SetEnvironmentVariable ("CSF_STEPDefaults",          aResDir);
    }
  }
}
