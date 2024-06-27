// Created: 2015-07-03
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
using System.Linq;
using System.Text;
using System.Windows;
using OCC.Message;
using OCC.TCollection;

namespace ImportExportWPF
{

  //! Example of Message Printer implementing OCCT C++ interface.
  public class MessagePrinter : Message_Printer
  {
    private Action<String, Message_Gravity> myPrinter;

    //! Main constructor
    public MessagePrinter(Action<String, Message_Gravity> thePrinter) {
      myPrinter = thePrinter;
    }

    //! Interface method - redirect to send().
    protected override void send (TCollection_AsciiString theString,
                                  Message_Gravity theGravity)
    {
      myPrinter(theString.ToString(), theGravity);
      //Console.WriteLine (theString.ToString());
    }
  }

}
