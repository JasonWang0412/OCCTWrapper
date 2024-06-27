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
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Input;
using OCC.D3DHost;
using OCC.TDocStd;
using OCC.AIS;
using OCC.XCAFDoc;
using OCC.TopoDS;
using OCC.BRep;
using OCC.IMeshTools;
using OCC.XCAFPrs;
using OCC.Quantity;
using OCC.TDF;
using OCC.TopLoc;
using OCC.TCollection;
using OCC.Prs3d;
using OCC.BRepTools;
using OCC.BRepMesh;
using OCCWpf.Utils;

namespace OCCWpf {
  // "using static" requires C# 6+ compiler (.NET Framework 4.6+ / Visual Studio 2015+)
  //using static OCC.SWIG.OCCwrapCSharp; // Aspect_VKeyMouse

  public class OCCWpf_Viewer : OCCWpf_ImageView {

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

    private TDocStd_Document OcafDocument { get; set; }


    /// <summary>
    /// ctor
    /// </summary>
    public OCCWpf_Viewer() {
      MouseMove += onMouseMove;
      MouseWheel += onMouseWheel;
      MouseDown += onMouseDown;
      MouseUp += onMouseUp;
    }

    #region Mouse events

    /// <summary>
    /// Convert WPF mouse button into Aspect_VKeyMouse.
    /// </summary>
    private static uint getMouseButtonFromWpf (MouseButtonEventArgs theArgs)
    {
      switch (theArgs.ChangedButton)
      {
        case MouseButton.Left:   return (uint )Aspect_VKeyMouse_LeftButton;
        case MouseButton.Middle: return (uint )Aspect_VKeyMouse_MiddleButton;
        case MouseButton.Right:  return (uint )Aspect_VKeyMouse_RightButton;
      }
      return (uint )Aspect_VKeyMouse_NONE;
    }

    /// <summary>
    /// Convert WPF key modifiers into Aspect_VKeyFlags.
    /// </summary>
    /// <param name="theFlags"></param>
    /// <returns></returns>
    private static uint getKeyFlagsFromWpf()
    {
      uint aFlags = (uint )Aspect_VKeyFlags_NONE;
      if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
      {
        aFlags |= (uint )Aspect_VKeyFlags_SHIFT;
      }
      if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
      {
        aFlags |= (uint )Aspect_VKeyFlags_CTRL;
      }
      if ((Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
      {
        aFlags |= (uint )Aspect_VKeyFlags_ALT;
      }
      if ((Keyboard.Modifiers & ModifierKeys.Windows) == ModifierKeys.Windows)
      {
        aFlags |= (uint )Aspect_VKeyFlags_META;
      }
      return aFlags;
    }

    private void onMouseUp(object sender, MouseButtonEventArgs theArgs) {
      if (myViewController == null
       || myView == null)
      {
        return;
      }

      theArgs.Handled = true;

      Point aCurrPnt = theArgs.GetPosition(this);
      OCC.Graphic3d.Graphic3d_Vec2i aNewPos = new OCC.Graphic3d.Graphic3d_Vec2i ((int)aCurrPnt.X, (int)aCurrPnt.Y);
      if (myViewController.ReleaseMouseButton (aNewPos, getMouseButtonFromWpf (theArgs), getKeyFlagsFromWpf(), false))
      {
        this.Update(OCCWpf_ImageView.UpdateMode.Soft);
      }
      if (myViewController.PressedMouseButtons() == (uint )Aspect_VKeyMouse_NONE)
      {
        Mouse.Capture (null);
      }
    }

    private void onMouseDown(object sender, MouseButtonEventArgs theArgs) {
      if (myViewController == null
       || myView == null)
      {
        return;
      }

      theArgs.Handled = true;
      Point aCurrPnt = theArgs.GetPosition(this);
      OCC.Graphic3d.Graphic3d_Vec2i aNewPos = new OCC.Graphic3d.Graphic3d_Vec2i ((int)aCurrPnt.X, (int)aCurrPnt.Y);
      if (myViewController.PressMouseButton (aNewPos, getMouseButtonFromWpf (theArgs), getKeyFlagsFromWpf(), false))
      {
        Mouse.Capture (this);
        this.Update(OCCWpf_ImageView.UpdateMode.Soft);
      }
    }

    private void onMouseWheel(object sender, MouseWheelEventArgs theArgs) {
      if (myViewController == null
       || myView == null)
      {
        return;
      }

      theArgs.Handled = true;
      Point aCurrPnt = theArgs.GetPosition(this);
      OCC.Graphic3d.Graphic3d_Vec2i aNewPos = new OCC.Graphic3d.Graphic3d_Vec2i ((int)aCurrPnt.X, (int)aCurrPnt.Y);
      OCC.Aspect.Aspect_ScrollDelta aDelta = new OCC.Aspect.Aspect_ScrollDelta (aNewPos, (double )theArgs.Delta / 120.0);
      myViewController.UpdateMouseScroll (aDelta);
      this.Update(OCCWpf_ImageView.UpdateMode.Soft);
    }

    private void onMouseMove(object theSender, MouseEventArgs theArgs) {
      if (myViewController == null
       || myView == null)
      {
        return;
      }

      theArgs.Handled = true;
      Point aCurrPnt = theArgs.GetPosition(this);
      OCC.Graphic3d.Graphic3d_Vec2i aNewPos = new OCC.Graphic3d.Graphic3d_Vec2i ((int)aCurrPnt.X, (int)aCurrPnt.Y);
      if (myViewController.UpdateMousePosition (aNewPos, myViewController.PressedMouseButtons(), getKeyFlagsFromWpf(), false))
      {
        this.Update(OCCWpf_ImageView.UpdateMode.Soft);
      }
    }

    #endregion // Mouse events

    public void fitAll() {
      this.View.FitAll(0.01, false);
      this.Update(OCCWpf_ImageView.UpdateMode.Hard);
    }

    /// <summary>
    /// clears all displayed shapes and creates new shapes from the given document
    /// </summary>
    /// <param name="document"></param>
    public void displayDocument(TDocStd_Document document) {
      Context.RemoveAll(false);
      OcafDocument = document;
      displayDocumentShapes();
      fitAll();
    }

    /// <summary>
    /// selects in viewer all nodes whose path starts with specified tree path, 
    /// i.e. assembly and its parts
    /// </summary>
    /// <param name="treePath">Sequence of TDF label names</param>
    public void selectSubTreeOfShapes(IEnumerable<String> treePath) {
      //Debug.WriteLine("Select nodes under '{0}'".format(String.Join(" / ", treePath)));
      String threePathString = String.Join(".\n", treePath) + ".";
      Context.ClearSelected(false);

      AIS_ListOfInteractive aDispObjs = new AIS_ListOfInteractive();
      Context.DisplayedObjects(aDispObjs);

      foreach (var aisTuple in from ais in aDispObjs
                               let path = OcctExtensions.getSelectionId(ais)
                               where path != null && path.StartsWith(threePathString)
                               select new { AisObject = ais, TreePath = path }) {
        Context.AddOrRemoveSelected(aisTuple.AisObject, false);
      }

      this.View.Invalidate();
    }

    /// <summary>
    /// removes all shapes from viewer
    /// </summary>
    public void clearShapes() {
      Context.RemoveAll(true);
    }

    #region private methods
    /// <summary>
    /// creates interactive objects supporting selection in given context, for each document shape
    /// </summary>
    /// <param name="document"></param>
    /// <param name="context"></param>
    private void displayDocumentShapes() {
      TDocStd_Document document = OcafDocument;
      AIS_InteractiveContext context = Context;

      TDF_LabelSequence aLabels = new TDF_LabelSequence();
      var shapeTool = XCAFDoc_DocumentTool.ShapeTool(document.Main());
      shapeTool.GetFreeShapes(ref aLabels);

      if (!aLabels.IsEmpty()) {
        TopoDS_Compound aCompound = new TopoDS_Compound();
        BRep_Builder aBuildTool = new BRep_Builder();
        aBuildTool.MakeCompound(ref aCompound);

        for (int i = 1, len = aLabels.Size(); i <= len; i++) {
          var label = aLabels.Value(i);
          TopoDS_Shape aShape = new TopoDS_Shape();

          if (XCAFDoc_ShapeTool.GetShape(label, ref aShape)) {
            var compoShape = aCompound as TopoDS_Shape;
            aBuildTool.Add(ref compoShape, aShape);
          }
        }

        var drawer = context.DefaultDrawer();
        double deflection = OCC.StdPrs.StdPrs_ToolTriangulatedShape.GetDeflection(aCompound, drawer);

        if (!BRepTools.Triangulation(aCompound, deflection)) {
          var meshAlgo = new BRepMesh_IncrementalMesh();
          meshAlgo.ChangeParameters().AsReference().Deflection = deflection;
          meshAlgo.ChangeParameters().AsReference().Angle      = drawer.DeviationAngle();
          meshAlgo.ChangeParameters().AsReference().InParallel = true;
          meshAlgo.SetShape(aCompound);
          meshAlgo.Perform();
        }

        XCAFPrs_Style aDefStyle = new XCAFPrs_Style();
        var color = new Quantity_Color(Quantity_NameOfColor.Quantity_NOC_GRAY65);
        aDefStyle.SetColorSurf(color);
        aDefStyle.SetColorCurv(color);
        var mapOfShapes = new Dictionary<TDF_Label, AIS_InteractiveObject>();
        var colorTool = XCAFDoc_DocumentTool.ColorTool(document.Main());

        foreach (var aLabel in aLabels) {
          displayWithChildren(shapeTool, colorTool, aLabel, new TopLoc_Location(),
            aDefStyle, new TCollection_AsciiString(), mapOfShapes);
        }
      }
    }

    private void displayWithChildren(
      XCAFDoc_ShapeTool theShapeTool,
      XCAFDoc_ColorTool theColorTool,
      TDF_Label theLabel, TopLoc_Location theParentTrsf,
      XCAFPrs_Style theParentStyle,
      TCollection_AsciiString theParentId,
      Dictionary<TDF_Label, AIS_InteractiveObject> theMapOfShapes) {

      TDF_Label aRefLabel = new TDF_Label();
      if (XCAFDoc_ShapeTool.IsReference(theLabel)) {
        XCAFDoc_ShapeTool.GetReferredShape(theLabel, ref aRefLabel);
      } else {
        aRefLabel = theLabel;
      }

      TCollection_AsciiString anEntry = new TCollection_AsciiString();
      TDF_Tool.Entry(theLabel, ref anEntry);
      if (!theParentId.IsEmpty()) {
        anEntry = theParentId.Cat("\n").Cat(anEntry);
      }
      anEntry = anEntry.Cat(".");

      //var myImportMaterial = new Graphic3d_MaterialAspect(Graphic3d.Graphic3d_NameOfMaterial.Graphic3d_NOM_BRASS);

      if (!XCAFDoc_ShapeTool.IsAssembly(aRefLabel)) {
        AIS_InteractiveObject anAIS;
        if (theMapOfShapes.ContainsKey(aRefLabel)) {
          anAIS = theMapOfShapes[aRefLabel];
        } else {
          anAIS = new XCAFPrs_AISObject(aRefLabel);
          theMapOfShapes[aRefLabel] = anAIS;
        }

        TCollection_HAsciiString anId = new TCollection_HAsciiString(anEntry);
        AIS_ConnectedInteractive aConnected = new AIS_ConnectedInteractive();
        aConnected.Connect(anAIS, theParentTrsf.Transformation());
        aConnected.SetOwner(anId);
        aConnected.SetLocalTransformation(theParentTrsf.Transformation());
        Context.Display(aConnected, false);
        return;
      }

      XCAFPrs_Style aDefStyle = theParentStyle;
      Quantity_Color aColor = new Quantity_Color();

      if (theColorTool.GetColor(aRefLabel, XCAFDoc_ColorType.XCAFDoc_ColorGen, ref aColor)) {
        aDefStyle.SetColorCurv(aColor);
        aDefStyle.SetColorSurf(aColor);
      }
      if (theColorTool.GetColor(aRefLabel, XCAFDoc_ColorType.XCAFDoc_ColorSurf, ref aColor)) {
        aDefStyle.SetColorSurf(aColor);
      }
      if (theColorTool.GetColor(aRefLabel, XCAFDoc_ColorType.XCAFDoc_ColorCurv, ref aColor)) {
        aDefStyle.SetColorCurv(aColor);
      }

      foreach (var aLabel in new TDF_ChildIterator(aRefLabel).ToEnumerable()) {
        if (!aLabel.IsNull() && (aLabel.HasAttribute() || aLabel.HasChild())) {
          TopLoc_Location aTrsf = theParentTrsf.Multiplied(XCAFDoc_ShapeTool.GetLocation(aLabel));
          displayWithChildren(theShapeTool, theColorTool, aLabel, aTrsf, aDefStyle, anEntry, theMapOfShapes);
        }
      }
    }

    #endregion
  }
}
