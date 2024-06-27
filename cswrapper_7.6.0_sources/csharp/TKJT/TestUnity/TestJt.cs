// Created: 2019-06-05
//
// Copyright (c) 2019-2021 OPEN CASCADE SAS
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

using UnityEngine;
using System.Collections;
using System.IO;
using System;
using OCC.Jt;
using OCC.TCollection;
using JtTools;

public class TestJt : MonoBehaviour, JtTools.IPrinter
{
  public void Print(string theMessage)
  {
    print(theMessage);
  }

  // Use this for initialization
  void Start ()
  {
    // NOTE: CHANGE PATH TO YOUR JT FILE
    string aAssetsDir = Directory.GetCurrentDirectory() + "/Assets/";
    string aInputJt   = aAssetsDir + "cam.jt";
    string aOutputXde = aAssetsDir + "jt2xde.xbf";
    string aOutputJt  = aAssetsDir + "xde2jt.jt";
    string aOutputObj = aAssetsDir + "xde2jt.obj";

    JtTestRunner.RunXde(aInputJt, aOutputXde, aOutputJt, this);
    JtTestRunner.RunObj(aOutputJt, aOutputObj, this);
  }

  // Update is called once per frame
  void Update ()
  {
    print ("You can close the scene");
  }
}
