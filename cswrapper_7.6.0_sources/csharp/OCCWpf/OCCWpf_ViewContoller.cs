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
using System.Windows;

using OCC.AIS;
using OCC.V3d;

namespace OCCWpf
{

  //! Arguments for selection change event.
  public class AisSelectionChangedEventArgs : EventArgs
  {
    public List<AIS_InteractiveObject> Selected { get; set; }
  }

  //! Arguments for mouse over shape moved event.
  public class MouseMovedOverShapeEventArgs : EventArgs
  {
    public Point Location { get; internal set; }
    public AIS_InteractiveObject ObjectUnderCursor { get; set; }
  }

  public delegate void AisSelectionChangedEventHandler(object sender, AisSelectionChangedEventArgs e);
  public delegate void MouseMovedOverShapeHandler(object sender, MouseMovedOverShapeEventArgs e);

  //! View controller specialization.
  public class OCCWpf_ViewContoller : AIS_ViewController
  {
    public event AisSelectionChangedEventHandler AisSelectionChanged;

    //! Redirect selection changed callback to event.
    public override void OnSelectionChanged (AIS_InteractiveContext theCtx, V3d_View theView)
    {
      if (AisSelectionChanged != null)
      {
        List<AIS_InteractiveObject> aSelected = new List<AIS_InteractiveObject>();
        for (theCtx.InitSelected(); theCtx.MoreSelected(); theCtx.NextSelected())
        {
          aSelected.Add (theCtx.SelectedInteractive());
        }
        AisSelectionChanged (this, new AisSelectionChangedEventArgs() { Selected = aSelected });
      }
    }

    //! Override object dragging.
    public override void OnObjectDragged (AIS_InteractiveContext theCtx,
                                          V3d_View theView,
                                          AIS_DragAction theAction)
    {
      base.OnObjectDragged (theCtx, theView, theAction);
    }
  }

}
