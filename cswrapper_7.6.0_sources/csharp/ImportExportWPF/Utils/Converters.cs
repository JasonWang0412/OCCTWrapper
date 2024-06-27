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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ImportExportWPF.Utils {
  [ValueConversion(typeof(Object), typeof(Visibility))]
  public class ObjToVisibilityConverter : IValueConverter {
    public bool NoCollapse { get; set; }
    public bool Invert { get; set; }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
      bool nocollapse = NoCollapse, inverse = Invert;
      string paramstr = parameter as string;
      if (!String.IsNullOrWhiteSpace(paramstr)) {
        var paramList = paramstr.Split(new char[] { ' ' });
        nocollapse = NoCollapse || paramList.Contains("nocollapse");
        inverse = Invert || paramList.Contains("inverse");
      }

      bool isVisible = value != null;

      if (isVisible) {
        if (value is string) {
          isVisible = (value as string).Length > 0;
        } else
          if (value is bool) {
          isVisible = (bool)value;
        }
      }

      if (inverse) {
        isVisible = !isVisible;
      }

      var newVis = Visibility.Visible;

      if (!isVisible) {
        //string paramstr = parameter as string;
        //        newVis = (paramstr == null || paramstr != "nocollapse") ? Visibility.Collapsed : Visibility.Hidden;
        newVis = nocollapse ? Visibility.Hidden : Visibility.Collapsed;
      }

      return newVis;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
      // just don't care
      return value;
    }
  }
}
