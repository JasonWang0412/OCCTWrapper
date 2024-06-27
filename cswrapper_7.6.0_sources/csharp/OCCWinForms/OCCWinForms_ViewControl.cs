// Created: 2016-02-16
//
// Copyright (c) 2016-2021 OPEN CASCADE SAS
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
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using OCC.V3d;
using OCC.WNT;
using OCC.AIS;
using OCC.OpenGl;
using OCC.Aspect;

namespace OCCWinForms
{
  /// <summary>
  /// Define the WinForms control for using as OpenGL window for OCCT 3D Viewer.
  /// </summary>
  public partial class OCCWinForms_ViewControl : UserControl
  {

    #region Component properties for OpenGL window

    /// <summary>
    /// Constructor.
    /// </summary>
    public OCCWinForms_ViewControl()
    {
      if (DesignMode)
      {
        return; // avoid calling native code within Designer
      }

      SetStyle (ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
      SetStyle (ControlStyles.OptimizedDoubleBuffer, false);
      InitializeComponent();
    }

    /// <summary>
    /// Request own device context.
    /// </summary>
    protected override CreateParams CreateParams
    {
      get
      {
        if (DesignMode)
        {
          return base.CreateParams;
        }

        const int VREDRAW = 0x00000001;
        const int HREDRAW = 0x00000002;
        const int OWNDC   = 0x00000020;

        CreateParams aParams = base.CreateParams;
        aParams.ClassStyle |= VREDRAW | HREDRAW | OWNDC;
        return aParams;
      }
    }

    #endregion Component properties for OpenGL window

    #region Properties

    //! Return view instance.
    public V3d_View View
    {
      get { return myView; }
    }

    //! Return Interactive Context.
    public AIS_InteractiveContext Context
    {
      get { return myAISContext; }
    }

    //! Return off-screen window instance.
    public WNT_Window Window
    {
      get { return myWindow; }
    }

    #endregion Properties

    #region Redirectors to V3d_View methods

    //! Invalidate the main scene content, but does not redraw the view.
    public virtual void InvalidateView()
    {
      if (myView != null)
      {
        myView.Invalidate();
      }
    }

    //! Redraw the entire view content.
    public virtual void Redraw()
    {
      if (myView != null)
      {
        myView.Redraw();
      }
    }

    //! Redraw either entire view content (if view has been invalidated) or only immediate layer.
    public virtual void RedrawImmediate()
    {
      if (myView != null)
      {
        myView.RedrawImmediate();
      }
    }

    //! Fit scene content into the view.
    public virtual void FitAll (double theMargin,
                                bool   theToUpdate)
    {
      if (myView != null)
      {
        myView.FitAll (theMargin, theToUpdate);
      }
    }

    #endregion Redirectors to V3d_View methods

    #region Viewer initialization

    //! Release the view, should be called explicitly from GUI thread to release unmanaged resources.
    public void Release()
    {
      myView       = null;
      myWindow     = null;
      myAISContext = null;
    }

    //! Initialize viewer using default parameters.
    //! Method Release() should be called explicitly from GUI thread to release unmanaged resources.
    public bool InitViewer()
    {
      Aspect_DisplayConnection aDummy = new Aspect_DisplayConnection();
      OpenGl_GraphicDriver aDriver = new OpenGl_GraphicDriver (aDummy);
      return InitViewer (aDriver);
    }

    //! Initialize viewer within specified graphic driver instance.
    //! Method Release() should be called explicitly from GUI thread to release unmanaged resources.
    public bool InitViewer (OpenGl_GraphicDriver theDriver)
    {
      if (theDriver == null)
      {
        Release();
        return false;
      }

      // create viewer and AIS context with default parameters
      V3d_Viewer aViewer = new V3d_Viewer (theDriver);
      aViewer.SetDefaultLights();
      aViewer.SetLightOn();
      AIS_InteractiveContext aContext = new AIS_InteractiveContext (aViewer);
      return InitViewer (aContext);
    }

    //! Create new view within specified AIS context.
    //! The graphic driver should be an instance of D3DHost_GraphicDriver class.
    //! Method Release() should be called explicitly from GUI thread to release unmanaged resources.
    public bool InitViewer (AIS_InteractiveContext theContext)
    {
      Release();
      if (theContext == null)
      {
        return false;
      }

      V3d_Viewer           aViewer = theContext.CurrentViewer();
      OpenGl_GraphicDriver aDriver = OpenGl_GraphicDriver.DownCast (aViewer.Driver());
      if (aDriver == null)
      {
        return false;
      }

      myAISContext = theContext;
      myView       = aViewer.CreateView();
      if (myView == null)
      {
        return false;
      }

      myWindow = new WNT_Window (this.Handle);
      myView.SetWindow (myWindow);
      myView.SetImmediateUpdate (false); // prevent implicit viewer updates
      myView.MustBeResized();
      return true;
    }

    #endregion Viewer initialization

    #region Event handlers

    //! Resize the viewer.
    protected override void OnResize (EventArgs theArgs)
    {
      base.OnResize (theArgs);
      if (myView != null)
      {
        myView.MustBeResized();
      }
    }

    //! Redraw 3D viewer.
    protected virtual void OnPaint (object         theSender,
                                    PaintEventArgs theArgs)
    {
      Redraw();
    }

    #endregion Event handlers

    protected V3d_View               myView;       //!< view bound to this image
    protected WNT_Window             myWindow;     //!< hidden WNT window for D3D and OpenGL contexts
    protected AIS_InteractiveContext myAISContext; //!< active interactive context

  }
}
