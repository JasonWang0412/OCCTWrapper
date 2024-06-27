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

/* ----------------------------------------------------------------------------
 * This class demonstrates direct usage of wrapped Open CASCADE classes from C# code.
 * ----------------------------------------------------------------------------- */

using ImportExportWPF.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OCC.Message;
using OCC.TCollection;
using OCC.TDF;
using OCC.TDocStd;
using OCC.XCAFApp;
using OCC.XCAFDoc;
using OCC.Quantity;
using OCC.Aspect;
using OCC.V3d;
using OCCWpf.Utils;
using OCCWpf;

namespace ImportExportWPF
{

  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private XCAFApp_Application _CafApp;
    private TDocStd_Document _document;
    private MessagePrinter _Printer;
    private bool _isSelectingViewerFromTreeAllowed = true;

    public Object FlyByShapeData {
      get { return (Object)GetValue(FlyByShapeDataProperty); }
      set { SetValue(FlyByShapeDataProperty, value); }
    }

    // Using a DependencyProperty as the backing store for FlyByShapeData.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty FlyByShapeDataProperty =
        DependencyProperty.Register("FlyByShapeData", typeof(Object), typeof(MainWindow), 
          new PropertyMetadata(null));

    /// <summary>
    /// list of tree nodes representing root shapes of last loaded document.
    /// </summary>
    public List<ShapeTreeNode> RootShapes {
      get { return (List<ShapeTreeNode>)GetValue(RootShapesProperty); }
      set { SetValue(RootShapesProperty, value); }
    }

    public static readonly DependencyProperty RootShapesProperty =
        DependencyProperty.Register("RootShapes", typeof(List<ShapeTreeNode>),
          typeof(MainWindow), new PropertyMetadata(null));
    

    //! Default constructor
    public MainWindow() {
      DataContext = this;

      InitializeComponent();
      
      if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) {
        return; // avoid calling native code within Designer
      }

      _CafApp = XCAFApp_Application.GetApplication();

      newDocument();

      // redirect OCCT messenger to User Interface
      _Printer = new MessagePrinter((msg, msgType) =>
      {
        if (msgType == Message_Gravity.Message_Fail
        || msgType == Message_Gravity.Message_Alarm)
        {
          MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
      });
      Message.DefaultMessenger().RemovePrinters(Message_PrinterOStream.TypeOf());
      Message.DefaultMessenger().RemovePrinters(MessagePrinter.TypeOf());
      Message.DefaultMessenger().AddPrinter(_Printer);

      // initialize the viewer
      myOcctView.InitViewer();
      Quantity_Color aColor1 = new Quantity_Color(0.62, 0.64, 0.67, Quantity_TypeOfColor.Quantity_TOC_RGB);
      Quantity_Color aColor2 = new Quantity_Color(0.91, 0.93, 0.94, Quantity_TypeOfColor.Quantity_TOC_RGB);
      myOcctView.View.SetBgGradientColors(aColor1, aColor2, Aspect_GradientFillMethod.Aspect_GFM_VER, false);
      myOcctView.View.TriedronDisplay(Aspect_TypeOfTriedronPosition.Aspect_TOTP_RIGHT_LOWER,
                                       new Quantity_Color (Quantity_NameOfColor.Quantity_NOC_BLACK),
                                       0.08, V3d_TypeOfVisualization.V3d_ZBUFFER);

      // show box
/*
      BRepPrimAPI.BRepPrimAPI_MakeBox aBuilder = new BRepPrimAPI.BRepPrimAPI_MakeBox(1.0, 2.0, 3.0);
      AIS.AIS_Shape aShapePrs = new AIS.AIS_Shape(aBuilder.Shape());
      aShapePrs.SetMaterial (new Graphic3d.Graphic3d_MaterialAspect (Graphic3d.Graphic3d_NameOfMaterial.Graphic3d_NOM_ALUMINIUM));
      myOcctView.Context.Display(aShapePrs, 1, 0, false);
      myOcctView.View.FitAll(0.01, false);
*/
      myOcctView.AisSelectionChanged += (s, e) => {
        updateTreeSelection(OcctExtensions.getSelectionId(e.Selected.FirstOrDefault()));
      };

      myOcctView.MouseMovedOverShape += updateFlyByTooltip;
    }

    /// <summary>
    /// replaces existing document with new one
    /// </summary>
    private void newDocument() {
      if (_document != null) {
        if (_document.HasOpenCommand()) {
          _document.AbortCommand();
        }

        _document.Main().Root().ForgetAllAttributes(true);
        _CafApp.Close(_document);
      }

      _document = new TDocStd_Document(new TCollection_ExtendedString(""));
      _CafApp.NewDocument(new TCollection_ExtendedString("BinXCAF"), _document);
    }

    /// <summary>
    /// called when user moves mouse over viewer.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void updateFlyByTooltip(object sender, OCCWpf.MouseMovedOverShapeEventArgs e) {
      String idUnderCursor = e.ObjectUnderCursor == null ? null : OcctExtensions.getSelectionId(e.ObjectUnderCursor);

      if (idUnderCursor == null) {
        FlyByShapeData = null;
      } else {
        var shapePath = OcctExtensions.splitSelectionId(OcctExtensions.getSelectionId(e.ObjectUnderCursor));
        String shapeId = shapePath.LastOrDefault();
        var tdfLabel = _document.findLabel(shapeId);
        if (!tdfLabel.IsNull())
        {
          if (XCAFDoc_ShapeTool.IsReference(tdfLabel))
          {
            var aRefLabel = new TDF_Label();
            if (XCAFDoc_ShapeTool.GetReferredShape(tdfLabel, ref aRefLabel))
            {
              tdfLabel = aRefLabel;
            }
          }

          FlyByShapeData = new
          {
            Text = tdfLabel.getShapeName(),
            Location = new Point(e.Location.X + 10, e.Location.Y - 30),
          };
        }
      }
    }

    /// <summary>
    /// Called when selection is changed in viewer
    /// </summary>
    /// <param name="selectedId"></param>
    private void updateTreeSelection(String selectedId) {
      try {
        // protect from triggering tree-to-viewer selection
        // when selected tree node changes
        _isSelectingViewerFromTreeAllowed = false;

        if (selectedId != null) {
          ShapeTreeNode.selectNode(RootShapes, selectedId);
        }
      } finally {
        _isSelectingViewerFromTreeAllowed = true;
      }
    }

    //! Close view correctly in GUI thread
    private void Window_Closed (object    theSender,
                                EventArgs theArgs)
    {
      // remove printers before closing viewer to avoid showing any errors occurred on close
      Message.DefaultMessenger().RemovePrinters(MessagePrinter.TypeOf());

      if (myOcctView != null)
      {
        myOcctView.Release();
      }
      myOcctView = null;
    }

    #region Mouse events


    #endregion // Mouse events

    //! Open file dialog
    private void OnImport_Click (object          theSender,
                                 RoutedEventArgs theArgs)
    {
      Microsoft.Win32.OpenFileDialog aDialog = new Microsoft.Win32.OpenFileDialog();
      aDialog.FileName = "Document";
      aDialog.Filter = "CAD files (BREP, STEP, IGES)|*.brep;*.rle;*.step;*.stp;*.iges;*.igs"
                     + "|Open CASCADE files (.brep)|*.brep;*.rle"
                     + "|STEP files (.step)|*.step;*.stp"
                     + "|IGES files (.iges)|*.iges;*.igs"
                     + "|All files (*.*)|*.*";

      if (aDialog.ShowDialog() == true) {
        using (ProgressIndicator aProgress = new ProgressIndicator(myTaskbarInfo)) {
        newDocument();
        myOcctView.clearShapes();
        RootShapes = new List<ShapeTreeNode>();

          if (new ImportTool(_document).Perform(aDialog.FileName, aProgress)) {
            this.Title = System.IO.Path.GetFileName(aDialog.FileName) + " - C# OCCT WPF Viewer";
            RootShapes = ShapeTreeNode.readChildNodes(_document, null);
          } else {
            MessageBox.Show("The file '" + System.IO.Path.GetFileName(aDialog.FileName)
              + "' cannot be imported", "Import error", MessageBoxButton.OK, MessageBoxImage.Error);
            this.Title = "C# OCCT WPF Viewer";
          }

          myOcctView.displayDocument(_document);
      }
    }
    }

    //! Save file dialog
    private void OnExport_Click (object          theSender,
                                 RoutedEventArgs theArgs)
    {
      Microsoft.Win32.SaveFileDialog aDialog = new Microsoft.Win32.SaveFileDialog();
      aDialog.FileName = "Document";
      aDialog.Filter = "Open CASCADE file (.brep)|*.brep"
                     + "|STEP file (.step)|*.step;*.stp"
                     + "|IGES file (.iges)|*.iges;*.igs";

      if (aDialog.ShowDialog() == true) {
        using (ProgressIndicator aProgress = new ProgressIndicator(myTaskbarInfo)) {
          ExportTool anExportTool = new ExportTool();
          anExportTool.Perform(_document, aDialog.FileName, aProgress);
      }
    }
    }

    /// <summary>
    /// Fit ALL
    /// </summary>
    /// <param name="theSender"></param>
    /// <param name="theArgs"></param>
    private void OnFitAll_Click (object          theSender,
                                 RoutedEventArgs theArgs)
    {
      myOcctView.fitAll();
    }

    /// <summary>
    /// About dialog
    /// </summary>
    /// <param name="theSender"></param>
    /// <param name="theArgs"></param>
    private void OnAbout_Click (object          theSender,
                                RoutedEventArgs theArgs)
    {
      bool isBasic = true;
      OCC.TColStd.TColStd_IndexedDataMapOfStringString aGlCapsDict = new OCC.TColStd.TColStd_IndexedDataMapOfStringString();
      myOcctView.View.DiagnosticInformation (ref aGlCapsDict, isBasic
                                                            ? OCC.Graphic3d.Graphic3d_DiagnosticInfo.Graphic3d_DiagnosticInfo_Basic
                                                            : OCC.Graphic3d.Graphic3d_DiagnosticInfo.Graphic3d_DiagnosticInfo_Complete);
      string anInfo = "";
      for (OCC.TColStd.TColStd_IndexedDataMapOfStringString_Iterator aValueIter =
             new OCC.TColStd.TColStd_IndexedDataMapOfStringString_Iterator (aGlCapsDict); aValueIter.More(); aValueIter.Next())
      {
        if (!aValueIter.Value().IsEmpty())
        {
          if (anInfo != "")
          {
            anInfo += "\n";
          }
          anInfo += aValueIter.Key() + ": " + aValueIter.Value();
        }
      }

      MessageBox.Show ("This small application demonstrates OCCT 3D Viewer integration into WPF using automated C# wrapper.\n"
                     + "For more information please contact Open Cascade:\n"
                     + "https://opencascade.com\n"
                     + "marketing.contact@opencascade.com\n"
                     + "\n"
                     + "Copyright (C) 2019-2021, Open CASCADE SAS\n"
                     + "\n"
                     + "System info:\n"
                     + anInfo,
                       "About C# OCCT WPF Viewer");
    }

    private void ShapeTree_OnSelection(object sender, RoutedPropertyChangedEventArgs<Object> e) {
      if (_isSelectingViewerFromTreeAllowed && sender is TreeView) {
        var item = (sender as TreeView).SelectedItem as ShapeTreeNode;

        if (item != null) {
          myOcctView.selectSubTreeOfShapes(item.TreePath);
        }
      }
    }
  }

  namespace designtime {
    public class DesignTimeData {

      public DesignTimeData() {
        RootShapes = createDesignTimeNodes();
      }

      private List<ShapeTreeNode> createDesignTimeNodes() {
        return Enumerable.Range(1, 10).Select(i => new ShapeTreeNode(
          "sh" + i, "sh" + i, "TestLabel " + i, null, false, null
          )).ToList();
      }

      public List<ShapeTreeNode> RootShapes { get; set; }
    }
  }
}
