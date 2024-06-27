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

/* ----------------------------------------------------------------------------
 * This class demonstrates direct usage of wrapped Open CASCADE classes from 
 * C# code. Compare it with the C++ OCCViewer class in standard C# sample.
 * ----------------------------------------------------------------------------- */

using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using OCC.AIS;
using OCC.V3d;
using OCC.Graphic3d;
using OCC.OpenGl;
using OCC.TCollection;
using OCC.Quantity;
using OCC.IGESControl;
using OCC.IFSelect;
using OCC.STEPControl;
using OCC.WNT;
using OCC.TopoDS;
using OCC.BRep;
using OCC.Message;
using OCC.Interface;
using OCC.StlAPI;
using OCC.VrmlAPI;
using OCC.Standard;
using OCC.BRepTools;
using OCC.TopAbs;
using OCC.Aspect;
using OCC.Transfer;
using OCC.XSControl;

namespace OCCWinForms
{
  // "using static" requires C# 6+ compiler (.NET Framework 4.6+ / Visual Studio 2015+)
  //using static OCC.SWIG.OCCwrapCSharp; // Aspect_VKeyMouse

  public enum CurrentAction3d {
    CurAction3d_Nothing,
    CurAction3d_DynamicZooming,
    CurAction3d_WindowZooming,
    CurAction3d_DynamicPanning,
    CurAction3d_DynamicRotation
  }

  public partial class OCCWinForms_Viewer : Control, IDisposable {

    // compatibility with old C# compilers not supporting "using static"
    public static readonly int Aspect_VKeyMouse_NONE         = OCC.SWIG.OCCwrapCSharp.Aspect_VKeyMouse_NONE;
    public static readonly int Aspect_VKeyMouse_LeftButton   = OCC.SWIG.OCCwrapCSharp.Aspect_VKeyMouse_LeftButton;
    public static readonly int Aspect_VKeyMouse_MiddleButton = OCC.SWIG.OCCwrapCSharp.Aspect_VKeyMouse_MiddleButton;
    public static readonly int Aspect_VKeyMouse_RightButton  = OCC.SWIG.OCCwrapCSharp.Aspect_VKeyMouse_RightButton;
    public static readonly int Aspect_VKeyFlags_NONE  = OCC.SWIG.OCCwrapCSharp.Aspect_VKeyFlags_NONE;
    public static readonly int Aspect_VKeyFlags_CTRL  = OCC.SWIG.OCCwrapCSharp.Aspect_VKeyFlags_CTRL;
    public static readonly int Aspect_VKeyFlags_SHIFT = OCC.SWIG.OCCwrapCSharp.Aspect_VKeyFlags_SHIFT;
    public static readonly int Aspect_VKeyFlags_ALT   = OCC.SWIG.OCCwrapCSharp.Aspect_VKeyFlags_ALT;
    public static readonly int Aspect_VKeyFlags_META  = OCC.SWIG.OCCwrapCSharp.Aspect_VKeyFlags_META;

    private V3d_Viewer myViewer;
    private V3d_View myView;
    private AIS_InteractiveContext myAISContext;
    private Graphic3d_GraphicDriver myGraphicDriver;
    private AIS_ViewController myViewController;
    private AIS_MouseGestureMap myDefaultGestures;

    protected CurrentAction3d myCurrentMode = CurrentAction3d.CurAction3d_Nothing;

    public CurrentAction3d Mode {
      get { return myCurrentMode; }
      set
      {
        this.myCurrentMode = value;
        defineMouseGestures();
      }
    }

    /// <summary>
    /// Setup mouse gestures.
    /// </summary>
    protected virtual void defineMouseGestures()
    {
      if (myViewController == null)
      {
        return;
      }

      myViewController.ChangeMouseGestureMap().Clear();
      switch (myCurrentMode)
      {
        case CurrentAction3d.CurAction3d_Nothing:
        {
          myViewController.ChangeMouseGestureMap().Assign (myDefaultGestures);
          break;
        }
        case CurrentAction3d.CurAction3d_DynamicZooming:
        {
          AIS_MouseGesture aZoom = AIS_MouseGesture.AIS_MouseGesture_Zoom;
          myViewController.ChangeMouseGestureMap().Bind ((uint )Aspect_VKeyMouse_LeftButton, ref aZoom);
          break;
        }
        case CurrentAction3d.CurAction3d_WindowZooming:
        {
          AIS_MouseGesture aZoomWin = AIS_MouseGesture.AIS_MouseGesture_ZoomWindow;
          myViewController.ChangeMouseGestureMap().Bind ((uint )Aspect_VKeyMouse_LeftButton, ref aZoomWin);
          break;
        }
        case CurrentAction3d.CurAction3d_DynamicPanning:
        {
          AIS_MouseGesture aPan = AIS_MouseGesture.AIS_MouseGesture_Pan;
          myViewController.ChangeMouseGestureMap().Bind ((uint )Aspect_VKeyMouse_LeftButton, ref aPan);
          break;
        }
        case CurrentAction3d.CurAction3d_DynamicRotation:
        {
          AIS_MouseGesture aRot = AIS_MouseGesture.AIS_MouseGesture_RotateOrbit;
          myViewController.ChangeMouseGestureMap().Bind ((uint )Aspect_VKeyMouse_LeftButton, ref aRot);
          break;
        }
      }
    }

    public bool InitViewer() {
      return InitViewer (null);
    }

    public bool InitViewer(AIS_InteractiveContext theCtx)
    {
      if (myViewController == null)
      {
        myViewController = new AIS_ViewController();
      }
      if (myDefaultGestures == null)
      {
        myDefaultGestures = new AIS_MouseGestureMap();
        myDefaultGestures.Assign (myViewController.MouseGestureMap());
      }
      if (theCtx != null)
      {
        myAISContext = theCtx;
        myViewer = theCtx.CurrentViewer();
        myGraphicDriver = myViewer.Driver();
      }
      else
      {
        Aspect_DisplayConnection aDisplayConnection = new Aspect_DisplayConnection();
        myGraphicDriver = new OpenGl_GraphicDriver(aDisplayConnection);
        myViewer = new V3d_Viewer(myGraphicDriver);
        myViewer.SetDefaultLights();
        myViewer.SetLightOn();
        myAISContext = new AIS_InteractiveContext(myViewer);
      }

      myView = myViewer.CreateView();
      WNT_Window aWNTWindow = new WNT_Window(this.Handle);
      myView.SetWindow(aWNTWindow);
      if (!aWNTWindow.IsMapped())
        aWNTWindow.Map();

      myView.MustBeResized();
      myView.Redraw();
      return true;
    }

    public OCCWinForms_Viewer() {
      InitializeComponent();

      this.Paint += new System.Windows.Forms.PaintEventHandler(this.OcctView_Paint);
    }

    ~OCCWinForms_Viewer() {
      Dispose();
    }

    /// <summary>
    /// Clean up resources being used
    /// </summary>
    public virtual new void Dispose()
    {
      int aNbViews = 0;
      if (myViewer != null)
      {
        for (myViewer.InitDefinedViews(); myViewer.MoreDefinedViews(); myViewer.NextDefinedViews())
        {
          ++aNbViews;
        }
      }

      if (myAISContext != null
       && aNbViews == 1)
      {
        myAISContext.RemoveAll(false);
        myAISContext.Dispose();
      }
      myAISContext = null;
      if (myView != null)
      {
        myView.Remove();
        myView.Dispose();
      }
      myView = null;
      if (myViewer != null
       && aNbViews == 1)
      {
        myViewer.Remove();
        myViewer.Dispose();
      }
      myViewer = null;
      base.Dispose();
    }

    public void UpdateView() {
      if (myView != null)
        myView.MustBeResized();
    }

    public void RedrawView() {
      if (myView != null)
        myView.Redraw();
    }

    public void WindowFitAll(int Xmin, int Ymin, int Xmax, int Ymax) {
      if (myView != null)
        myView.WindowFitAll(Xmin, Ymin, Xmax, Ymax);
    }

    public void Place(int x, int y, float zoomFactor) {
      if (myView != null)
        myView.Place(x, y, zoomFactor);
    }

    public void Zoom(int x1, int y1, int x2, int y2) {
      if (myView != null)
        myView.Zoom(x1, y1, x2, y2);
    }

    public void Pan(int x, int y) {
      if (myView != null)
        myView.Pan(x, y);
    }

    public void Rotation(int x, int y) {
      if (myView != null)
        myView.Rotation(x, y);
    }

    public void StartRotation(int x, int y) {
      if (myView != null)
        myView.StartRotation(x, y);
    }

    public void Select(int x1, int y1, int x2, int y2) {
      OCC.Graphic3d.Graphic3d_Vec2i aPosMin = new OCC.Graphic3d.Graphic3d_Vec2i (x1, y1);
      OCC.Graphic3d.Graphic3d_Vec2i aPosMax = new OCC.Graphic3d.Graphic3d_Vec2i (x2, y2);
      myAISContext.SelectRectangle(aPosMin, aPosMax, myView);
      myAISContext.UpdateCurrentViewer();
    }

    public new void Select() {
      myAISContext.SelectDetected();
      myAISContext.UpdateCurrentViewer();
    }

    public void MoveTo(int x, int y) {
      myAISContext.MoveTo(x, y, myView, true);
    }

    public void ShiftSelect(int x1, int y1, int x2, int y2) {
      OCC.Graphic3d.Graphic3d_Vec2i aPosMin = new OCC.Graphic3d.Graphic3d_Vec2i (x1, y1);
      OCC.Graphic3d.Graphic3d_Vec2i aPosMax = new OCC.Graphic3d.Graphic3d_Vec2i (x2, y2);
      myAISContext.SelectRectangle(aPosMin, aPosMax,
                                   myView, AIS_SelectionScheme.AIS_SelectionScheme_XOR);
      myAISContext.UpdateCurrentViewer();
    }

    public void ShiftSelect() {
      myAISContext.SelectDetected(AIS_SelectionScheme.AIS_SelectionScheme_XOR);
      myAISContext.UpdateCurrentViewer();
    }

    public void BackgroundColor(ref int r, ref int g, ref int b) {
      double R1 = 0, G1 = 0, B1 = 0;
      myView.BackgroundColor(Quantity_TypeOfColor.Quantity_TOC_RGB, ref R1, ref G1, ref B1);
      r = (int)(R1 * 255);
      g = (int)(G1 * 255);
      b = (int)(B1 * 255);
    }

    public void UpdateCurrentViewer() {
      myAISContext.UpdateCurrentViewer();
    }

    public void FrontView() {
      if (myView != null)
        myView.SetProj(V3d_TypeOfOrientation.V3d_Yneg);
    }

    public void TopView() {
      if (myView != null)
        myView.SetProj(V3d_TypeOfOrientation.V3d_Zpos);
    }

    public void LeftView() {
      if (myView != null)
        myView.SetProj(V3d_TypeOfOrientation.V3d_Xneg);
    }

    public void BackView() {
      if (myView != null)
        myView.SetProj(V3d_TypeOfOrientation.V3d_Ypos);
    }

    public void RightView() {
      if (myView != null)
        myView.SetProj(V3d_TypeOfOrientation.V3d_Xpos);
    }

    public void BottomView() {
      if (myView != null)
        myView.SetProj(V3d_TypeOfOrientation.V3d_Zneg);
    }

    public void AxoView() {
      if (myView != null)
        myView.SetProj(V3d_TypeOfOrientation.V3d_XposYnegZpos);
    }

    public void ZoomAllView() {
      if (myView != null) {
        myView.FitAll();
        myView.ZFitAll();
      }
    }

    public double Scale() {
      if (myView != null)
        return myView.Scale();
      else
        return -1;
    }

    public void ResetView() {
      if (myView != null)
        myView.Reset();
    }

    public void SetDisplayMode(int aMode) {
      AIS_DisplayMode CurrentMode;
      if (aMode == 0)
        CurrentMode = AIS_DisplayMode.AIS_WireFrame;
      else
        CurrentMode = AIS_DisplayMode.AIS_Shaded;

      myAISContext.InitSelected();
      if (myAISContext.MoreSelected())
      {
        for (; myAISContext.MoreSelected(); myAISContext.NextSelected())
        {
          myAISContext.SetDisplayMode(myAISContext.SelectedInteractive(), aMode, false);
        }
      }
      else
      {
        myAISContext.SetDisplayMode((int)CurrentMode, false);
      }

      myAISContext.UpdateCurrentViewer();
    }

    public void SetColor(int r, int g, int b) {
      Quantity_Color col = new Quantity_Color(r / 255.0, g / 255.0, b / 255.0, Quantity_TypeOfColor.Quantity_TOC_RGB);
      for (; myAISContext.MoreSelected(); myAISContext.NextSelected())
      {
        myAISContext.SetColor(myAISContext.SelectedInteractive(), col, false);
      }
      myAISContext.UpdateCurrentViewer();
    }

    public void ObjectColor(ref int r, ref int g, ref int b) {
      r = 255;
      g = 255;
      b = 255;
      myAISContext.InitSelected();
      if (!myAISContext.MoreSelected())
      {
        return;
      }

      AIS_InteractiveObject aCurrent = myAISContext.SelectedInteractive();
      if (!aCurrent.HasColor())
      {
        return;
      }

      Quantity_Color ObjCol = new Quantity_Color();
      myAISContext.Color(aCurrent, ref ObjCol);
      r = (int)ObjCol.Red() * 255;
      g = (int)ObjCol.Green() * 255;
      b = (int)ObjCol.Blue() * 255;
    }

    public void SetBackgroundColor(int r, int g, int b) {
      if (myView != null)
        myView.SetBackgroundColor(Quantity_TypeOfColor.Quantity_TOC_RGB, r / 255.0, g / 255.0, b / 255.0);
    }

    public void EraseObjects() {
      myAISContext.EraseSelected(false);
      myAISContext.ClearSelected(false);
      myAISContext.UpdateCurrentViewer();
    }

    public double GetVersion() {
      return Standard_Version.Number();
    }

    public void SetMaterial(int theMaterial) {
      Graphic3d_MaterialAspect aMaterial = new Graphic3d_MaterialAspect((Graphic3d_NameOfMaterial)theMaterial);
      for (myAISContext.InitSelected(); myAISContext.MoreSelected(); myAISContext.NextSelected())
      {
        myAISContext.SetMaterial(myAISContext.SelectedInteractive(), aMaterial, false);
      }
      myAISContext.UpdateCurrentViewer();
    }

    public void SetTransparency(int theTrans) {
      for (myAISContext.InitSelected(); myAISContext.MoreSelected(); myAISContext.NextSelected())
      {
        myAISContext.SetTransparency(myAISContext.SelectedInteractive(), ((double)theTrans) / 10.0, false);
      }
      myAISContext.UpdateCurrentViewer();
    }

    public bool ImportBRep(string filename) {
      TopoDS_Shape aShape = new TopoDS_Shape();
      BRep_Builder aBuilder = new BRep_Builder();
      bool result = BRepTools.Read(ref aShape, filename, aBuilder);
      if (!result)
        return false;
      myAISContext.Display(new AIS_Shape(aShape), true);
      return true;
    }

    public bool ImportIges(string filename) {
      bool res = false;
      IGESControl_Reader aReader = new IGESControl_Reader();
      IFSelect_ReturnStatus aStatus = aReader.ReadFile(filename);
      if (aStatus == IFSelect_ReturnStatus.IFSelect_RetDone) {
        res = true;
        aReader.TransferRoots();
        TopoDS_Shape aShape = aReader.OneShape();
        myAISContext.Display(new AIS_Shape(aShape), true);
      }
      clearSession (aReader.WS());
      return res;
    }

    public bool ImportStep(string filename) {
      bool res = false;
      STEPControl_Reader aReader = new STEPControl_Reader();
      IFSelect_ReturnStatus aStatus = aReader.ReadFile(filename);
      if (aStatus == IFSelect_ReturnStatus.IFSelect_RetDone) {
        bool failsonly = false;
        aReader.PrintCheckLoad(failsonly, IFSelect_PrintCount.IFSelect_ItemsByEntity);

        int nbr = aReader.NbRootsForTransfer();
        aReader.PrintCheckTransfer(failsonly, IFSelect_PrintCount.IFSelect_ItemsByEntity);
        for (int r = 1; r <= nbr; r++) {
          bool ok = aReader.TransferRoot(r);
          int nbs = aReader.NbShapes();
          if (nbs > 0) {
            res = true;
            for (int s = 1; s <= nbs; s++) {
              TopoDS_Shape aShape = aReader.Shape(s);
              myAISContext.Display(new AIS_Shape(aShape), false);
            }
          }
        }
        myAISContext.UpdateCurrentViewer();
      }
      clearSession (aReader.WS());
      return res;
    }

    public bool ExportBRep(string filename) {
      myAISContext.InitSelected();
      if (!myAISContext.MoreSelected())
      {
        return false;
      }

      AIS_Shape anIS = AIS_Shape.DownCast(myAISContext.SelectedInteractive());
      return !anIS.IsNull()
           && BRepTools.Write(anIS.Shape(), filename);
    }

    public bool ExportIges(string filename) {
      IGESControl_Controller.Init();
      IGESControl_Writer aWriter = new IGESControl_Writer(Interface_Static.CVal("XSTEP.iges.unit"),
                                                          Interface_Static.IVal("XSTEP.iges.writebrep.mode"));
      for (myAISContext.InitSelected(); myAISContext.MoreSelected(); myAISContext.NextSelected())
      {
        AIS_Shape anIS = AIS_Shape.DownCast(myAISContext.SelectedInteractive());
        if (anIS.IsNull())
        {
          return false;
        }

        TopoDS_Shape shape = anIS.Shape();
        aWriter.AddShape(shape);
      }
      aWriter.ComputeModel();
      return aWriter.Write(filename);
    }

    public bool ExportStep(string filename) {
      STEPControl_StepModelType type = STEPControl_StepModelType.STEPControl_AsIs;
      STEPControl_Writer aWriter = new STEPControl_Writer();
      for (myAISContext.InitSelected(); myAISContext.MoreSelected(); myAISContext.NextSelected())
      {
        AIS_Shape anIS = AIS_Shape.DownCast(myAISContext.SelectedInteractive());
        if (anIS.IsNull())
        {
          return false;
        }

        TopoDS_Shape shape = anIS.Shape();
        if (aWriter.Transfer(shape, type) != IFSelect_ReturnStatus.IFSelect_RetDone)
        {
          return false;
        }
      }
      return aWriter.Write(filename) == IFSelect_ReturnStatus.IFSelect_RetDone;
    }

    public bool ExportStl(string filename) {
      TopoDS_Compound comp = new TopoDS_Compound();
      BRep_Builder builder = new BRep_Builder();
      builder.MakeCompound(ref comp);
      TopoDS_Shape sh = comp;
      for (myAISContext.InitSelected(); myAISContext.MoreSelected(); myAISContext.NextSelected())
      {
        AIS_Shape anIS = AIS_Shape.DownCast(myAISContext.SelectedInteractive());
        if (anIS.IsNull())
        {
          return false;
        }

        TopoDS_Shape shape = anIS.Shape();
        if (shape.IsNull())
        {
          return false;
        }
        builder.Add(ref sh, shape);
      }
      StlAPI_Writer aWriter = new StlAPI_Writer();
      aWriter.ASCIIMode = false;
      aWriter.Write(comp, filename);
      return true;
    }

    public bool ExportVrml(string filename)
    {
      TopoDS_Compound res = new TopoDS_Compound();
      BRep_Builder builder = new BRep_Builder();
      builder.MakeCompound(ref res);
      TopoDS_Shape sh = res;
      for (myAISContext.InitSelected(); myAISContext.MoreSelected(); myAISContext.NextSelected())
      {
        AIS_Shape anIS = AIS_Shape.DownCast(myAISContext.SelectedInteractive());
        if (anIS.IsNull())
        {
          return false;
        }
        TopoDS_Shape shape = anIS.Shape();
        if (shape.IsNull())
        {
          return false;
        }
        builder.Add(ref sh, shape);
      }
      VrmlAPI_Writer aWriter = new VrmlAPI_Writer();
      aWriter.Write(res, filename);
      return true;
    }

    public bool Dump(string filename) {
      myView.Redraw();
      return myView.Dump(filename);
    }

    public bool IsObjectSelected() {
      myAISContext.InitSelected();
      return myAISContext.MoreSelected();
    }

    public void OnFaces() {
      myAISContext.ClearSelected(false);
      myAISContext.Deactivate(AIS_Shape.SelectionMode(TopAbs_ShapeEnum.TopAbs_VERTEX));
      myAISContext.Deactivate(AIS_Shape.SelectionMode(TopAbs_ShapeEnum.TopAbs_EDGE));
      myAISContext.Activate(AIS_Shape.SelectionMode(TopAbs_ShapeEnum.TopAbs_FACE));
    }

    public void OnEdges() {
      myAISContext.ClearSelected(false);
      myAISContext.Deactivate(AIS_Shape.SelectionMode(TopAbs_ShapeEnum.TopAbs_VERTEX));
      myAISContext.Deactivate(AIS_Shape.SelectionMode(TopAbs_ShapeEnum.TopAbs_FACE));
      myAISContext.Activate(AIS_Shape.SelectionMode(TopAbs_ShapeEnum.TopAbs_EDGE));
    }

    public void OnVertices() {
      myAISContext.ClearSelected(false);
      myAISContext.Deactivate(AIS_Shape.SelectionMode(TopAbs_ShapeEnum.TopAbs_EDGE));
      myAISContext.Deactivate(AIS_Shape.SelectionMode(TopAbs_ShapeEnum.TopAbs_FACE));
      myAISContext.Activate(AIS_Shape.SelectionMode(TopAbs_ShapeEnum.TopAbs_VERTEX));
    }

    public int DisplayMode() {
      int mode = -1;
      bool OneOrMoreInShading = false;
      bool OneOrMoreInWireframe = false;
      for (myAISContext.InitSelected(); myAISContext.MoreSelected(); myAISContext.NextSelected())
      {
        if (myAISContext.IsDisplayed(myAISContext.SelectedInteractive(), 1))
        {
          OneOrMoreInShading = true;
        }
        if (myAISContext.IsDisplayed(myAISContext.SelectedInteractive(), 0))
        {
          OneOrMoreInWireframe = true;
        }
      }
      if (OneOrMoreInShading && OneOrMoreInWireframe)
        mode = 10;
      else if (OneOrMoreInShading)
        mode = 1;
      else if (OneOrMoreInWireframe)
        mode = 0;
      return mode;
    }

    /// <summary>
    /// Return View controller.
    /// </summary>
    public AIS_ViewController GetViewController() {
      return myViewController;
    }

    /// <summary>
    /// Set View controller.
    /// </summary>
    public void SetViewController (AIS_ViewController theController) {
      myViewController = theController;
    }

    public AIS_InteractiveContext GetAISContext() {
      return myAISContext;
    }

    /// <summary>
    /// Clear working session.
    /// </summary>
    protected void clearSession (XSControl_WorkSession theWorkSession)
    {
      XSControl_TransferReader aTransferReader = theWorkSession.TransferReader();
      if (!aTransferReader.IsNull())
      {
        Transfer_TransientProcess aMapReader = aTransferReader.TransientProcess();
        if (!aMapReader.IsNull())
        {
          aMapReader.Clear();
        }
        aTransferReader.Clear (-1);
      }
    }

    /// <summary>
    /// Convert WinForms mouse button into Aspect_VKeyMouse.
    /// </summary>
    private static uint getMouseButtonFromWinForms (MouseButtons theButton)
    {
      switch (theButton)
      {
        case MouseButtons.Left:   return (uint )Aspect_VKeyMouse_LeftButton;
        case MouseButtons.Middle: return (uint )Aspect_VKeyMouse_MiddleButton;
        case MouseButtons.Right:  return (uint )Aspect_VKeyMouse_RightButton;
      }
      return (uint )Aspect_VKeyMouse_NONE;
    }

    /// <summary>
    /// Convert WinForms key modifiers into Aspect_VKeyFlags.
    /// </summary>
    /// <param name="theFlags"></param>
    /// <returns></returns>
    private uint getKeyFlagsFromWinForms()
    {
      uint aFlags = (uint )Aspect_VKeyFlags_NONE;
      if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
      {
        aFlags |= (uint )Aspect_VKeyFlags_SHIFT;
      }
      if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
      {
        aFlags |= (uint )Aspect_VKeyFlags_CTRL;
      }
      if ((Control.ModifierKeys & Keys.Alt) == Keys.Alt)
      {
        aFlags |= (uint )Aspect_VKeyFlags_ALT;
      }
      if ((Control.ModifierKeys & Keys.LWin) == Keys.LWin
       || (Control.ModifierKeys & Keys.RWin) == Keys.RWin)
      {
        aFlags |= (uint )Aspect_VKeyFlags_META;
      }
      return aFlags;
    }

    /// <summary>
    /// Handle mouse down event.
    /// </summary>
    /// <param name="theArgs"></param>
    protected override void OnMouseDown (MouseEventArgs theArgs)
    {
      if (myViewController != null
       && myView != null)
      {
        OCC.Graphic3d.Graphic3d_Vec2i aNewPos = new OCC.Graphic3d.Graphic3d_Vec2i ((int)theArgs.X, (int)theArgs.Y);
        myViewController.PressMouseButton (aNewPos, getMouseButtonFromWinForms (theArgs.Button), getKeyFlagsFromWinForms(), false);
        myViewController.FlushViewEvents (myAISContext, myView, true);
      }
      base.OnMouseDown (theArgs);
    }

    /// <summary>
    /// Handle mouse up event.
    /// </summary>
    /// <param name="theArgs"></param>
    protected override void OnMouseUp (MouseEventArgs theArgs)
    {
      if (myViewController != null
       && myView != null)
      {
        OCC.Graphic3d.Graphic3d_Vec2i aNewPos = new OCC.Graphic3d.Graphic3d_Vec2i (theArgs.X, theArgs.Y);
        myViewController.ReleaseMouseButton (aNewPos, getMouseButtonFromWinForms (theArgs.Button), getKeyFlagsFromWinForms(), false);
        myViewController.FlushViewEvents (myAISContext, myView, true);
      }
      base.OnMouseUp (theArgs);
    }

    /// <summary>
    /// Handle mouse move event.
    /// </summary>
    /// <param name="theArgs"></param>
    protected override void OnMouseMove (MouseEventArgs theArgs)
    {
      if (myViewController != null
       && myView != null)
      {
        OCC.Graphic3d.Graphic3d_Vec2i aNewPos = new OCC.Graphic3d.Graphic3d_Vec2i (theArgs.X, theArgs.Y);
        myViewController.UpdateMousePosition (aNewPos, myViewController.PressedMouseButtons(), myViewController.LastMouseFlags(), false);
        myViewController.FlushViewEvents (myAISContext, myView, true);
      }
      base.OnMouseMove (theArgs);
    }

    /// <summary>
    /// Handle mouse scroll event.
    /// </summary>
    /// <param name="theArgs"></param>
    protected override void OnMouseWheel (MouseEventArgs theArgs)
    {
      if (myViewController != null
       && myView != null)
      {
        OCC.Graphic3d.Graphic3d_Vec2i aNewPos = new OCC.Graphic3d.Graphic3d_Vec2i ((int)theArgs.X, (int)theArgs.Y);
        OCC.Aspect.Aspect_ScrollDelta aDelta = new OCC.Aspect.Aspect_ScrollDelta (aNewPos, (double )theArgs.Delta / 120.0);
        myViewController.UpdateMouseScroll (aDelta);
        myViewController.FlushViewEvents (myAISContext, myView, true);
      }
      base.OnMouseWheel (theArgs);
    }

    /// <summary>
    /// Handle focus lost event.
    /// </summary>
    /// <param name="theArgs"></param>
    protected override void OnLostFocus (EventArgs theArgs)
    {
      if (myViewController != null
       && myView != null)
      {
        myViewController.ResetViewInput();
      }
      base.OnLostFocus (theArgs);
    }

  }
}
