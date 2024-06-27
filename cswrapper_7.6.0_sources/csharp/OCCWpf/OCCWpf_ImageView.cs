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
using System.Security.Permissions;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Input;
using System.Windows.Interop;
using System.Runtime.InteropServices;

using OCC.AIS;
using OCC.Graphic3d;
using OCC.V3d;
using OCC.TCollection;
using OCC.WNT;
using OCC.D3DHost;

namespace OCCWpf
{

  //! The image control holding OCCT 3D View.
  public class OCCWpf_ImageView : System.Windows.Controls.Image
  {

    //! Define view update mode.
    public enum UpdateMode
    {
      Soft, //!< redraw only immediate view content
      Hard  //!< redraw whole view content
    };

    //! Empty constructor.
    //! Please call InitViewer() to initialize the viewer.
    public OCCWpf_ImageView() : base() {}

    //! Destroy the image.
    ~OCCWpf_ImageView()
    {
      System.Windows.Media.CompositionTarget.Rendering -= new EventHandler (onBeforeRender);
      this.SizeChanged -= new SizeChangedEventHandler (onSizeChanged);
      //Dispose();
    }

    //! Redeclare property to hide it from Designer
    [System.ComponentModel.Browsable(false)]
    public new ImageSource Source
    {
      get { return base.Source; }
    }

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

    //! View Controller.
    public OCCWpf_ViewContoller ViewController
    {
      get { return myViewController; }
      set { myViewController = value; }
    }

    //! Return off-screen window instance.
    public WNT_Window Window
    {
      get { return myWindow; }
    }

    //! Flag to control image cache on WPF side.
    public bool ToUseWpfCache
    {
      get { return myUseWpfCache; }
      set { myUseWpfCache = value; }
    }

    //! Flag to control image cache on OCCT / OpenGL side.
    public bool ToUseGlCache
    {
      get { return myUseGlCache; }
      set { myUseGlCache = value; }
    }

    //! Request view update.
    //!
    //! Actual update will be performed later right before new WPF frame rendering,
    //! thus it is relatively safe to ask update multiple times.
    //!
    //! WPF implements retain-mode rendering, e.g. WPF caches content of the elements
    //! to reuse it during frame rendering without recomputing the presentation for unchanged elements.
    //! At the same time OCCT viewer implements its own retain-mode
    //! which results in additional layer of caches.
    //!
    //! It is possible to ignore WPF cache and update it at each frame from OCCT cache.
    //! However it might be still reasonable to eliminate extra copies.
    //!
    //! This method invalidates WPF cache within any option and in addition
    //! invalidates entire view content within UpdateMode.Hard flag.
    public void Update (UpdateMode theMode)
    {
      myToUpdate = true;
      if (theMode == UpdateMode.Hard
       && myView != null)
      {
        myView.Invalidate();
      }
    }

    //! Release the view, should be called explicitly from GUI thread to release unmanaged resources.
    public void Release()
    {
      System.Windows.Media.CompositionTarget.Rendering -= new EventHandler (onBeforeRender);
      this.SizeChanged -= new SizeChangedEventHandler (onSizeChanged);

      base.Source  = null;
      myViewController = null;
      myView       = null;
      myWindow     = null;
      myAISContext = null;
    }

    //! Initialize viewer using default parameters.
    //! Method Release() should be called explicitly from GUI thread to release unmanaged resources.
    public bool InitViewer()
    {
      D3DHost_GraphicDriver aDriver = new D3DHost_GraphicDriver();
      aDriver.ChangeOptions().buffersNoSwap = true;
      aDriver.ChangeOptions().useSystemBuffer = false;
      return InitViewer (aDriver);
    }

    //! Initialize viewer within specified graphic driver instance.
    //! Method Release() should be called explicitly from GUI thread to release unmanaged resources.
    public bool InitViewer (D3DHost_GraphicDriver theDriver)
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
      aContext.SetDisplayMode((int)AIS_DisplayMode.AIS_Shaded, false);
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

      if (myViewController == null)
      {
        myViewController = new OCCWpf_ViewContoller();
      }

      V3d_Viewer            aViewer = theContext.CurrentViewer();
      D3DHost_GraphicDriver aDriver = D3DHost_GraphicDriver.DownCast (aViewer.Driver());
      if (aDriver == null)
      {
        return false;
      }

      base.Source = new D3DImage();
      myAISContext = theContext;
      myView       = aViewer.CreateView();
      if (myView == null)
      {
        return false;
      }

      uint CS_OWNDC = 0x0020;
      uint WS_POPUP = 0x80000000;
      WNT_WClass aWClass = new WNT_WClass (new TCollection_AsciiString ("OCC_Viewer"), IntPtr.Zero, CS_OWNDC);
      myWindow = new WNT_Window ("OCC_Viewer", aWClass, WS_POPUP, 64, 64, 64, 64);
      myWindow.SetVirtual (true);
      Size aSize = this.ActualSizePixels;
      if (aSize.Width > 0 && aSize.Height > 0)
      {
        myWindow.SetPos(0, 0, (int)aSize.Width, (int)aSize.Height);
      }

      myView.SetWindow (myWindow);
      myView.SetImmediateUpdate (false); // prevent implicit viewer updates
      myView.MustBeResized();

      // update OCCT view right before WPF performs frame rendering
      System.Windows.Media.CompositionTarget.Rendering += new EventHandler (onBeforeRender);

      // implicitly handle resize events
      this.SizeChanged += new SizeChangedEventHandler (onSizeChanged);
      return true;
    }

    //! Straightforward wrapper for AIS_InteractiveContext.MoveTo() method.
    public virtual void MoveTo (Point thePosition)
    {
      if (myView == null
       || myAISContext == null)
      {
        return;
      }

      myAISContext.MoveTo ((int )thePosition.X, (int )thePosition.Y, myView, false);
      myToUpdate = true;
    }

    //! Straightforward wrapper for AIS_InteractiveContext.Select() method.
    public virtual void Select()
    {
      if (myView == null
       || myAISContext == null)
      {
        return;
      }

      myAISContext.SelectDetected (AIS_SelectionScheme.AIS_SelectionScheme_Replace);

      myViewController.OnSelectionChanged (myAISContext, myView);

      myView.Invalidate();
      myToUpdate = true;
    }

    #region Events

    //! Selection changed event.
    public event AisSelectionChangedEventHandler AisSelectionChanged
    {
      add    { myViewController.AisSelectionChanged += value; }
      remove { myViewController.AisSelectionChanged -= value; }
    }

    //! Occurs when shape under cursor changes or mouse is moved over same shape
    public event MouseMovedOverShapeHandler MouseMovedOverShape;

    #endregion

    //! Compute actual size converted into pixels (taking into account DPI).
    protected Size ActualSizePixels
    {
      get {
        Matrix transformToDevice;
        var source = PresentationSource.FromVisual(this);
        if (source != null)
          transformToDevice = source.CompositionTarget.TransformToDevice;
        else
        {
          source = new HwndSource(new HwndSourceParameters());
          transformToDevice = source.CompositionTarget.TransformToDevice;
        }
        Vector size = new Vector (this.ActualWidth, this.ActualHeight);
        return (Size)transformToDevice.Transform(size);
      }
    }

    //! Handle control resize event.
    private void onSizeChanged (object               theSender,
                                SizeChangedEventArgs theArgs)
    {
      D3DImage aD3dImg = this.Source as D3DImage;
      if (!this.IsInitialized
        || aD3dImg  == null
        || myWindow == null
        || myView   == null)
      {
        return;
      }

      Size aSize = this.ActualSizePixels;
      if (aSize.Width < 1 || aSize.Height < 1)
      {
        return;
      }

      int anOldSizeX = 0;
      int anOldSizeY = 0;
      myWindow.Size (ref anOldSizeX, ref anOldSizeY);
      if (aSize.Width == anOldSizeX && aSize.Height == anOldSizeY)
      {
        return;
      }

      myWindow.SetPos(0, 0, (int)aSize.Width, (int)aSize.Height);
      myToResize = true;
      myView.Invalidate();
    }

    /// <summary>
    /// Wrapper for D3DImage.SetBackBuffer() using new method available in .NET4.5 or old method otherwise.
    /// </summary>
    /// <param name="theImage"></param>
    /// <param name="theSurfacePtr"></param>
    private void setBackBuffer (D3DImage theImage, IntPtr theSurfacePtr)
    {
      _MethodInfo aMethod = typeof(D3DImage).GetMethod("SetBackBuffer", new [] {typeof(D3DResourceType), typeof(IntPtr), typeof(bool)});
      if (aMethod != null)
      {
        aMethod.Invoke (theImage, new object[] { D3DResourceType.IDirect3DSurface9, theSurfacePtr, myToAllowSoftD3DImage });
      }
      else
      {
        theImage.SetBackBuffer (D3DResourceType.IDirect3DSurface9, theSurfacePtr);
      }
    }

    //! Viewer redraw callback - called just before WPF will start rendering new frame.
    private void onBeforeRender (object    theSender,
                                 EventArgs theArgs)
    {
      if (!this.IsInitialized)
      {
        return;
      }

      RenderingEventArgs anArgs = (RenderingEventArgs )theArgs;
      D3DImage aD3dImg = this.Source as D3DImage;
      if (aD3dImg == null
      || (!aD3dImg.IsFrontBufferAvailable && !myToAllowSoftD3DImage)
      ||  myLastRenderTime == anArgs.RenderingTime // WPF might call this multiple times per single frame
      ||  myView == null)
      {
        return;
      }

      D3DHost_View aView = D3DHost_View.DownCast (myView.View());
      IntPtr aSurfacePtr = aView.D3dColorSurface();
      if (aSurfacePtr == IntPtr.Zero
      || (!myToUpdate && !myView.IsInvalidated() && myUseWpfCache))
      {
        return;
      }

      aD3dImg.Lock();
      if (myToResize)
      {
        setBackBuffer (aD3dImg, IntPtr.Zero);
        myView.MustBeResized();
        aSurfacePtr = aView.D3dColorSurface();
        myToResize = false;
        if (aSurfacePtr == IntPtr.Zero)
        {
          aD3dImg.Unlock();
          return;
        }
      }
      setBackBuffer (aD3dImg, aSurfacePtr); // calling with the same pointer is a no-op
      if (myViewController != null)
      {
        myViewController.FlushViewEvents (myAISContext, myView, true);

        if (myLastMoveTo.X != myViewController.LastMousePosition().x()
         || myLastMoveTo.Y != myViewController.LastMousePosition().y())
        {
          myLastMoveTo.X = myViewController.LastMousePosition().x();
          myLastMoveTo.Y = myViewController.LastMousePosition().y();

          AIS_InteractiveObject aSelObj = null;
          var aDetOwner = myAISContext.DetectedOwner();
          if (!aDetOwner.IsNull())
          {
            aSelObj = AIS_InteractiveObject.DownCast(aDetOwner.Selectable());
            if (aSelObj.IsNull())
            {
              aSelObj = null;
            }
          }
          if (aSelObj != null || myIsLastMoveOverShape)
          {
            // emit event
            if (MouseMovedOverShape != null)
            {
              MouseMovedOverShape(this, new MouseMovedOverShapeEventArgs()
              {
                ObjectUnderCursor = aSelObj,
                Location = new Point (myViewController.LastMousePosition().x(), myViewController.LastMousePosition().y()),
              });
            }
          }
          myIsLastMoveOverShape = aSelObj != null;
        }
      }
      aD3dImg.AddDirtyRect (new System.Windows.Int32Rect (0, 0, aD3dImg.PixelWidth, aD3dImg.PixelHeight));
      aD3dImg.Unlock();

      myLastRenderTime = anArgs.RenderingTime;
      myToUpdate = false;
    }

    protected V3d_View               myView;               //!< view bound to this image
    protected WNT_Window             myWindow;             //!< hidden WNT window for D3D and OpenGL contexts
    protected AIS_InteractiveContext myAISContext;         //!< active interactive context
    protected OCCWpf_ViewContoller   myViewController;     //!< view controller

    protected TimeSpan               myLastRenderTime;     //!< timestamp of last rendered frame
    protected bool                   myUseWpfCache = true; //!< flag to ignore myToUpdate and redraw WPF image at every frame
    protected bool                   myUseGlCache = true;  //!< flag to disable OCCT viewer cache (e.g. call Redraw() instead of RedrawImmediate())
    protected bool                   myToUpdate = true;    //!< flag to update image content from 3D view content
    protected bool                   myToResize = false;   //!< flag to resize view
    protected bool                   myToAllowSoftD3DImage = true; //!< flag to allow software fallback for D3DImage (e.g. for rendering in RDP mode); requires .NET4.5+

    private bool  myIsLastMoveOverShape = false;
    private Point myLastMoveTo = new Point (-1, -1);

  }

}
