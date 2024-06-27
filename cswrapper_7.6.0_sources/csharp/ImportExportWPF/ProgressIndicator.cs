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
using OCC.Message;

namespace ImportExportWPF
{

  //! Example of Progress Indicator implementing OCCT C++ interface.
  public class ProgressIndicator : Message_ProgressIndicator
  {

    //! Main constructor.
    public ProgressIndicator (System.Windows.Shell.TaskbarItemInfo theTaskbarInfo)
    {
      myTaskbarInfo = theTaskbarInfo;
      if (myTaskbarInfo != null)
      {
        myTaskbarInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Normal;
        myTaskbarInfo.ProgressValue = 0;
      }
    }

    //! Release progress state.
    public override void Dispose()
    {
      if (myTaskbarInfo != null)
      {
        myTaskbarInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.None;
      }
      base.Dispose();
    }

    //! Update progress indicator.
    protected override void Show (Message_ProgressScope theScope, bool theIsForced)
    {
      double aPos = GetPosition();
      if (myTaskbarInfo != null && aPos - myTaskbarInfo.ProgressValue > 0.01)
      {
        myTaskbarInfo.ProgressValue = aPos;
      }
    }

    //! Should return true when user aborts operation.
    protected override bool UserBreak() { return false; }

    protected System.Windows.Shell.TaskbarItemInfo myTaskbarInfo;
  }

}
