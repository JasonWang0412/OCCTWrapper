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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using OCC.Graphic3d;
using OCC.Jt;

namespace JtTools
{
  //! Simple OBJ exporter.
  public class ObjWriter
  {
    //! Traverses LSG in order to convert the model to OBJ format.
    public static void Traverse(ObjWriter theWriter, Jt_Node theNode, TraverseState theState)
    {
      int aTransIndex = -1;
      int aMaterIndex = -1;

      for (int anIdx = 0; anIdx < theNode.Attributes.Size(); ++anIdx)
      {
        if (theNode.Attributes.Value(anIdx).IsKind(Jt_Transform.TypeOf()))
        {
          aTransIndex = anIdx;
        }
        else if (theNode.Attributes.Value(anIdx).IsKind(Jt_Material.TypeOf()))
        {
          aMaterIndex = anIdx;
        }
      }

      if (aTransIndex >= 0) // Material found
      {
        theState.Transform = theState.Transform.Multiplied(Jt_Transform.DownCast(theNode.Attributes.Value(aTransIndex)).Transform());
      }

      if (aMaterIndex >= 0) // Transform found
      {
        theState.Material = Jt_Material.DownCast(theNode.Attributes.Value(aMaterIndex));
      }

      if (theNode.IsKind(Jt_RangeLOD.TypeOf()))
      {
        theState.RangeLOD = Jt_RangeLOD.DownCast(theNode);
      }
      else if (theNode.IsKind(Jt_Mesh.TypeOf())) // Extract scene graph leaf nodes
      {
        Jt_Mesh aMesh = Jt_Mesh.DownCast(theNode);

        theWriter.Append(aMesh, theState);
      }

      if (theNode.IsKind(Jt_Group.TypeOf()))
      {
        Jt_Group aGroup = Jt_Group.DownCast(theNode);

        for (int aChildIdx = 0; aChildIdx < aGroup.Children.Size(); ++aChildIdx)
        {
          Traverse(theWriter, aGroup.Children.Value(aChildIdx), theState.DeepClone());
        }
      }
    }

    public ObjWriter (string theFileName)
    {
      myObjFile = new System.IO.StreamWriter (theFileName);

      string aDir = Path.GetDirectoryName(theFileName);
      if (string.IsNullOrEmpty(aDir))
        aDir = Directory.GetCurrentDirectory();

      myMtlFile = new System.IO.StreamWriter (aDir + "/" + Path.GetFileNameWithoutExtension (theFileName) + ".mtl");

      myObjFile.WriteLine ("mtllib " + Path.GetFileNameWithoutExtension (theFileName) + ".mtl");
    }

    //! Applies the given transformation matrix to the given point.
    private Graphic3d_Vec4 Transform (Graphic3d_Mat4 theMatrix, Graphic3d_Vec4 thePoint)
    {
      Graphic3d_Vec4 aResult = new Graphic3d_Vec4 (
        theMatrix.GetValue (0, 0) * thePoint.x () +
        theMatrix.GetValue (0, 1) * thePoint.y () +
        theMatrix.GetValue (0, 2) * thePoint.z () +
        theMatrix.GetValue (0, 3) * thePoint.w (),
        theMatrix.GetValue (1, 0) * thePoint.x () +
        theMatrix.GetValue (1, 1) * thePoint.y () +
        theMatrix.GetValue (1, 2) * thePoint.z () +
        theMatrix.GetValue (1, 3) * thePoint.w (),
        theMatrix.GetValue (2, 0) * thePoint.x () +
        theMatrix.GetValue (2, 1) * thePoint.y () +
        theMatrix.GetValue (2, 2) * thePoint.z () +
        theMatrix.GetValue (2, 3) * thePoint.w (),
        theMatrix.GetValue (3, 0) * thePoint.x () +
        theMatrix.GetValue (3, 1) * thePoint.y () +
        theMatrix.GetValue (3, 2) * thePoint.z () +
        theMatrix.GetValue (3, 3) * thePoint.w ());

      return aResult;
    }

    //! Current vertex offset in the OBJ file.
    private int myOffset = 1;

    //! Append the given JT mesh to OBJ file.
    public void Append (Jt_Mesh theMesh, TraverseState theState)
    {
      myMtlFile.WriteLine ("newmtl M" + myOffset);

      myMtlFile.WriteLine ("Ka " + theState.Material.AmbientColor ().x () +
                             " " + theState.Material.AmbientColor ().y () +
                             " " + theState.Material.AmbientColor ().z ());

      myMtlFile.WriteLine ("Kd " + theState.Material.DiffuseColor ().x () +
                             " " + theState.Material.DiffuseColor ().y () +
                             " " + theState.Material.DiffuseColor ().z ());

      myMtlFile.WriteLine ("Ks " + theState.Material.SpecularColor ().x () +
                             " " + theState.Material.SpecularColor ().y () +
                             " " + theState.Material.SpecularColor ().z ());

      myMtlFile.WriteLine ("Ns " + theState.Material.Shininess ());

      Jt_Triangulation aTriangulation = theMesh.Triangulation ();

      aTriangulation.RequestTriangulation (0 /* LOD index */);

      myObjFile.WriteLine ("usemtl M" + myOffset);

      for (int i = 0; i < aTriangulation.Vertices ().Size (); i += 3)
      {
        Graphic3d_Vec4 aPoint = Transform (theState.Transform, new Graphic3d_Vec4 (aTriangulation.Vertices ().Value (i + 0),
                                                                                   aTriangulation.Vertices ().Value (i + 1),
                                                                                   aTriangulation.Vertices ().Value (i + 2),
                                                                                   1.0f));

        myObjFile.WriteLine ("v " + aPoint.x () +
                              " " + aPoint.y () +
                              " " + aPoint.z ());
      }

      Graphic3d_Mat4 aNormalMatrix = theState.NormalMatrix ();

      if (aTriangulation.Normals ().Size () == aTriangulation.Vertices ().Size ())
      {
        for (int i = 0; i < aTriangulation.Normals ().Size (); i += 3)
        {
          Graphic3d_Vec4 aNormal = Transform (aNormalMatrix, new Graphic3d_Vec4 (aTriangulation.Normals ().Value (i + 0),
                                                                                 aTriangulation.Normals ().Value (i + 1),
                                                                                 aTriangulation.Normals ().Value (i + 2),
                                                                                 1.0f));

          myObjFile.WriteLine ("vn " + aNormal.x () +
                                 " " + aNormal.y () +
                                 " " + aNormal.z ());
        }
      }

      for (int i = 0; i < aTriangulation.Indices ().Size (); i += 3)
      {
        myObjFile.WriteLine ("f " + (aTriangulation.Indices ().Value (i + 0) + myOffset) +
                              " " + (aTriangulation.Indices ().Value (i + 1) + myOffset) +
                              " " + (aTriangulation.Indices ().Value (i + 2) + myOffset));
      }

      myOffset += aTriangulation.Vertices ().Size () / 3;
    }

    //! Closes the file.
    public void Close ()
    {
      myObjFile.Close ();
      myMtlFile.Close ();
    }

    protected System.IO.StreamWriter myObjFile;
    protected System.IO.StreamWriter myMtlFile;
  }

  //! Scene graph traversal state.
  public class TraverseState
  {
    //! Creates new default traversal state.
    public TraverseState ()
    {
      Transform.InitIdentity ();
    }

    //! Returns deep copy of state.
    public TraverseState DeepClone ()
    {
      TraverseState aState = new TraverseState ();

      for (uint x = 0; x < 4; ++x)
      {
        for (uint y = 0; y < 4; ++y)
        {
          aState.Transform.SetValue (x, y, Transform.GetValue (x, y));
        }
      }

      aState.RangeLOD = RangeLOD;

      aState.Material = new Jt_Material (Material.AmbientColor (),
                                         Material.DiffuseColor (),
                                         Material.SpecularColor (),
                                         Material.EmissionColor (),
                                         Material.Shininess ());

      return aState;
    }

    //! Returns normal transformation matrix.
    public Graphic3d_Mat4 NormalMatrix ()
    {
      Graphic3d_Mat4 aMatrix = new Graphic3d_Mat4 ();

      Transform.Inverted (ref aMatrix);

      return aMatrix.Transposed ();
    }

    //! Nearest range LOD node.
    public Jt_RangeLOD RangeLOD = null;

    //! OpenGL material of the node.
    public Jt_Material Material = new Jt_Material ();

    //! Accumulated affine transformation.
    public Graphic3d_Mat4 Transform = new Graphic3d_Mat4 ();
  };
}
