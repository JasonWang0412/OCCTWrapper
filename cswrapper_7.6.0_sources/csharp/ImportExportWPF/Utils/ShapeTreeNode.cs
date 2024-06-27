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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

using OCC.TCollection;
using OCC.TDataStd;
using OCC.TDF;
using OCC.TDocStd;
using OCC.TopLoc;
using OCC.TopoDS;
using OCC.XCAFDoc;
using OCC.XCAFPrs;
using OCCWpf.Utils;
using ImportExportWPF.Utils;

namespace ImportExportWPF.Utils {
  public class ShapeTreeNode : PropertyChangedNotifier {
    private Func<ShapeTreeNode,IEnumerable<ShapeTreeNode>> _readChildrenFunc;
    //private Func<ShapeTreeNode, bool> _hasChildrenFunc;
    private ShapeTreeNode _parent;
    private String _uniqueElementId;
    private bool _isExpanded = false;
    private bool _isSelected = false;

    //private static IEnumerable<ShapeTreeNode> _dummyChildren
    //  = new ReadOnlyCollection<ShapeTreeNode>(new[] { new ShapeTreeNode() });
    private ObservableCollection<ShapeTreeNode> _children = new ObservableCollection<ShapeTreeNode>();
    private static ShapeTreeNode DummyChild = new ShapeTreeNode();

    private ShapeTreeNode() { }

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="shapeId"></param>
    /// <param name="uniqueId"></param>
    /// <param name="label"></param>
    /// <param name="parentNode"></param>
    /// <param name="hasChildrenFunc"></param>
    /// <param name="readChildrenFunc"></param>
    public ShapeTreeNode(String shapeId, String uniqueId,  String label, ShapeTreeNode parentNode, 
      //Func<ShapeTreeNode, bool> hasChildrenFunc,
      bool hasChildren,
      Func<ShapeTreeNode, IEnumerable<ShapeTreeNode>> readChildrenFunc) {

      Label = label;
      Id = shapeId;
      _parent = parentNode;
      _uniqueElementId = uniqueId;
      _readChildrenFunc = readChildrenFunc;

      // check if have children
      if (hasChildren) {
        _children.Add(DummyChild);
      }
    }

    public String Id { get; private set; }
    public String Label { get; private set; }

    /// <summary>
    /// sequence of IDs of this node in its root assembly
    /// </summary>
    public IEnumerable<String> TreePath {
      get {
        var thisPath = new[] { _uniqueElementId };
        return _parent == null ? thisPath : _parent.TreePath.Concat(thisPath);
      }
    }
    
    public IList<ShapeTreeNode> Children {
      get {
        return _children;
        //Debug.WriteLine("Query children of " + Id);
        //return _readChildrenFunc == null ? new List<ShapeTreeNode>() :
        //   _readChildrenFunc(this).ToList();
      }
    }

    public bool IsSelected {
      get { return _isSelected; }
      set {
        if (value != _isSelected) {
          _isSelected = value;
          this.OnPropertyChanged("IsSelected");
        }
      }
    }

    public bool IsExpanded {
      get { return _isExpanded; }
      set {
        if (value != _isExpanded) {
          _isExpanded = value;
          this.OnPropertyChanged("IsExpanded");
        }

        // Expand all the way up to the root.
        if (_isExpanded && _parent != null)
          _parent.IsExpanded = true;

        if (this.HasDummyChild) {
          this.Children.Remove(DummyChild);
          this.loadChildren();
        }
      }
    }

    private bool HasDummyChild {
      get {
        return _children.Count == 1 && _children[0] == DummyChild;
      }
    }

    /// <summary>
    /// last element of TreePath
    /// </summary>
    public string TreePathId { get { return _uniqueElementId; } }

    private void loadChildren() {
      Debug.WriteLine("load children of " + this);
      foreach (var newNode in _readChildrenFunc(this)) {
        _children.Add(newNode);
      }
    }

    public override string ToString() {
      return String.Format("ShapeTreeNode '{0}' [id={1}, compoId={2}]", Label, Id,
        String.Join(" / ", TreePath));
    }

    #region static methods

    /// <summary>
    /// Selects and makes visible the tree node specified by selection ID even if it's not expanded yet.
    /// All parents are expanded if necessary
    /// </summary>
    /// <param name="rootNodes"></param>
    /// <param name="selectionId"></param>
    public static void selectNode(IEnumerable<ShapeTreeNode> rootNodes, String selectionId) {
      selectNode(rootNodes, OcctExtensions.splitSelectionId(selectionId));
    }

    /// <summary>
    /// Selects and makes visible the tree node specified by path even if it's not expanded yet.
    /// All parents are expanded if necessary
    /// </summary>
    /// <param name="rootNodes"></param>
    /// <param name="idTreePath"></param>
    public static void selectNode(IEnumerable<ShapeTreeNode> rootNodes, String[] idTreePath) {
      var foundNode = findInHierarchy(rootNodes, idTreePath, 0);
      if (foundNode != null) {
        foundNode.IsSelected = true;
      }
    }

    private static ShapeTreeNode findInHierarchy(IEnumerable<ShapeTreeNode> curLevelNodes, String[] idTreePath, int depth) {
      var foundNode = curLevelNodes.FirstOrDefault(rn => rn.TreePathId == idTreePath[depth]);

      // found but not last - iterate children of it expanding if necessary
      if (foundNode != null && depth < idTreePath.Length - 1) {
        // go deeper
        if (!foundNode.IsExpanded) {
          foundNode.IsExpanded = true;
        }
        foundNode = findInHierarchy(foundNode.Children, idTreePath, depth + 1);
      }

      return foundNode;
    }


    /// <summary>
    /// Returns children of the node or nodes of document top level shapes if parentNode is null
    /// </summary>
    /// <param name="document"></param>
    /// <param name="parentNode">Node for which the children are returned</param>
    /// <returns></returns>
    public static List<ShapeTreeNode> readChildNodes(TDocStd_Document document, ShapeTreeNode parentNode) {
      String theEntryId = parentNode == null ? "" : parentNode.Id;

      var aLabels = getChildLabels(document, theEntryId);
      var childNodes = new List<ShapeTreeNode>();

      foreach (var iterLabel in aLabels) {
        var aLabel = iterLabel;
        var uniqueShapeId = aLabel.getOcafLabelId();
        var shapeId = uniqueShapeId;

        if (XCAFDoc_ShapeTool.IsReference(aLabel)) {
          TDF_Label aRefLabel = new TDF_Label();
          if (XCAFDoc_ShapeTool.GetReferredShape(aLabel, ref aRefLabel)) {
            aLabel = aRefLabel;
            shapeId = aRefLabel.getOcafLabelId();
          }
        }

        String tdfLabelName = aLabel.getShapeName();
        String nodeId = shapeId;
        String label = tdfLabelName + " - " + nodeId;
        childNodes.Add(new ShapeTreeNode(nodeId, uniqueShapeId, label, parentNode,
          getChildLabels(document, nodeId).Length() > 0,
          pn => readChildNodes(document, pn)));
      }

      return childNodes;
    }

    /// <summary>
    /// gets child labels of TDF Label specified by ID
    /// </summary>
    /// <param name="document"></param>
    /// <param name="parentLabelId">ID of label or null to get top level shapes</param>
    /// <returns></returns>
    private static TDF_LabelSequence getChildLabels(TDocStd_Document document, String parentLabelId) {
      TDF_LabelSequence aLabels = new TDF_LabelSequence();
      var aShapeTool = XCAFDoc_DocumentTool.ShapeTool(document.Main());

      if (String.IsNullOrEmpty(parentLabelId)) {
        aShapeTool.GetFreeShapes(ref aLabels);
      } else {
        // get label with using ocaf entry
        TDF_Label aRootLabel = new TDF_Label();
        TDF_Tool.Label(document.Main().Data(), parentLabelId, ref aRootLabel, false);

        if (XCAFDoc_ShapeTool.IsAssembly(aRootLabel)) {
          XCAFDoc_ShapeTool.GetComponents(aRootLabel, ref aLabels);
        }
      }
      return aLabels;
    }

    #endregion

  }
}
