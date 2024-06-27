// Created: 2019-06-05
//
// Copyright (c) 2019-2021 OPEN CASCADE SAS
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
using System.Linq;
using System.Windows.Forms;

namespace JtTools
{
  class ConsolePrinter : IPrinter
  {
    public void Print(string theMessage)
    {
      Console.WriteLine(theMessage);
    }
  }

  static class Program
  {
    
    //! The main entry point for the application.
    static void Main()
    {
      IPrinter aPrinter = new ConsolePrinter();
      string aInputJt   = "cam.jt";
      string aOutputXde = "jt2xde.xbf";
      string aOutputJt  = "xde2jt.jt";
      string aOutputObj = "xde2jt.obj";

      JtTestRunner.RunXde(aInputJt, aOutputXde, aOutputJt, aPrinter);
      JtTestRunner.RunObj(aOutputJt, aOutputObj, aPrinter);
    } 
  }
}
