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
using OCC.AIS;
using OCC.TCollection;
using OCC.TDataStd;
using OCC.TDF;
using OCC.TDocStd;

namespace OCCWpf.Utils {
  /// <summary>
  /// Convenience extensions for OCC and OCAF functionality
  /// </summary>
  public static class OcctExtensions {

    /// <summary>
    /// Converts into IEnumerable for use in LINQ expressions
    /// </summary>
    /// <param name="childIterator"></param>
    /// <returns></returns>
    public static IEnumerable<TDF_Label> ToEnumerable(this TDF_ChildIterator childIterator) {
      for (TDF_ChildIterator aChildIter = childIterator; aChildIter.More(); aChildIter.Next()) {
        yield return aChildIter.Value();
      }
    }

    /// <summary>
    /// returns value of TDataStd_Name attribute of the label argument
    /// </summary>
    /// <param name="label"></param>
    /// <returns></returns>
    public static String getShapeName(this TDF_Label label) {
      string shapeName = null;
      TDataStd_Name aNodeName = new TDataStd_Name();
      if (label.FindAttribute(TDataStd_Name.GetId(), aNodeName)) {
        shapeName = aNodeName.Get().ToString();
      }
      return shapeName;
    }

    /// <summary>
    /// Retrieves OCAF label string of given TDF_Label
    /// </summary>
    /// <param name="tdfLabel"></param>
    /// <returns>out parameter of TDF_Tool::Entry(tdfLabel, out labelString) call</returns>
    public static String getOcafLabelId(this TDF_Label tdfLabel) {
      TCollection_AsciiString aNewEntry = new TCollection_AsciiString();
      TDF_Tool.Entry(tdfLabel, ref aNewEntry);
      return aNewEntry.ToString();
    }

    /// <summary>
    /// Finds in TDocStd_Document a TDF_Label instance by given entry id 
    /// </summary>
    /// <param name="document"></param>
    /// <param name="entryId"></param>
    /// <returns></returns>
    public static TDF_Label findLabel(this TDocStd_Document document, String entryId) {
      TDF_Label aLabel = new TDF_Label();
      TDF_Tool.Label(document.GetData(), entryId, ref aLabel);
      return aLabel;
    }


    /// <summary>
    /// Retrieves ID binding the displayed shape to its node in a tree.
    /// This ID is sequence of TDF_Label names separated by two chars, '.\n'
    /// </summary>
    /// <param name="aisObjectOfShape"></param>
    /// <returns></returns>
    public static String getSelectionId(AIS_InteractiveObject aisObjectOfShape) {
      String tdfLabel;

      if (aisObjectOfShape == null) {
        tdfLabel = null;
      } else {
        var hasciiStr = TCollection_HAsciiString.DownCast(aisObjectOfShape.GetOwner());
        if (hasciiStr == null || hasciiStr.IsNull()) {
          tdfLabel = null;
        } else {
          tdfLabel = hasciiStr.ToString();
        }
      }

      return tdfLabel;
    }

    /// <summary>
    /// Splits selectionId encoded as \n-separated sequence of '.'-terminated IDs
    /// </summary>
    /// <param name="selectionId">selection ID used to bind displayed shape and its node in a tree</param>
    /// <returns></returns>
    public static String[] splitSelectionId(String selectionId) {
      return selectionId.Split(new[] { '\n' }).Select(
                 s => s.EndsWith(".") ? s.Substring(0, s.Length - 1) : s).ToArray();
    }
  }

}
