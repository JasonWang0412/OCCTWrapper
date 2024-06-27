// Created: 2017-02-15
//
// Copyright (c) 2017-2021 OPEN CASCADE SAS
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
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ImportExportWPF.Utils {
  /// <summary>
  /// inheriting classes may call OnPropertyChanged 
  /// to update bound view properties
  /// </summary>
  public class PropertyChangedNotifier : INotifyPropertyChanged {
    public event PropertyChangedEventHandler PropertyChanged;

    public void OnPropertyChanged(string propname) {
      if (PropertyChanged != null) {
        PropertyChanged(this, new PropertyChangedEventArgs(propname));
      }
    }
  }
}
