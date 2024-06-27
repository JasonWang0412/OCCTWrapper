// Created: 2006-06-22
//
// Copyright (c) 2006-2021 OPEN CASCADE SAS
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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;


namespace IE
{

  public enum ModelFormat {
    BREP,
    STEP,
    IGES,
    VRML,
    STL,
    IMAGE
  }
  /// <summary>
  /// Summary description for Form2.
  /// </summary>
  public class Form2 : System.Windows.Forms.Form
	{
		private System.ComponentModel.IContainer components;

		public Form2()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			myHlrModeIsOn=true;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
          if ( disposing )
          {
            if ( components != null )
              components.Dispose ();
            // ensure destruction of viewer and all related objects before window gets destroyed
            if ( this.myView != null )
              this.myView.Dispose();
          }
          base.Dispose (disposing);
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
      this.components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form2));
      this.imageList1 = new System.Windows.Forms.ImageList(this.components);
      this.myPopup = new System.Windows.Forms.ContextMenu();
      this.menuItem1 = new System.Windows.Forms.MenuItem();
      this.myPopupObject = new System.Windows.Forms.ContextMenu();
      this.ContextWireframe = new System.Windows.Forms.MenuItem();
      this.ContextShading = new System.Windows.Forms.MenuItem();
      this.ContextColor = new System.Windows.Forms.MenuItem();
      this.ContextMaterial = new System.Windows.Forms.MenuItem();
      this.ContMatBrass = new System.Windows.Forms.MenuItem();
      this.ContMenBronze = new System.Windows.Forms.MenuItem();
      this.ContMenCopper = new System.Windows.Forms.MenuItem();
      this.ContMenGold = new System.Windows.Forms.MenuItem();
      this.ContMenPewt = new System.Windows.Forms.MenuItem();
      this.ContMenPlaster = new System.Windows.Forms.MenuItem();
      this.ContMenPlastic = new System.Windows.Forms.MenuItem();
      this.ContMenSilver = new System.Windows.Forms.MenuItem();
      this.ContextSelection = new System.Windows.Forms.MenuItem();
      this.ContFaceSel = new System.Windows.Forms.MenuItem();
      this.ContEdgeSel = new System.Windows.Forms.MenuItem();
      this.ContVertSel = new System.Windows.Forms.MenuItem();
      this.ContMenTranc = new System.Windows.Forms.MenuItem();
      this.ContMenDelete = new System.Windows.Forms.MenuItem();
      this.myView = new OCCWinForms.OCCWinForms_Viewer();
      this.SuspendLayout();
      // 
      // imageList1
      // 
      this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
      this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
      this.imageList1.Images.SetKeyName(0, "");
      this.imageList1.Images.SetKeyName(1, "");
      this.imageList1.Images.SetKeyName(2, "");
      this.imageList1.Images.SetKeyName(3, "");
      this.imageList1.Images.SetKeyName(4, "");
      this.imageList1.Images.SetKeyName(5, "");
      this.imageList1.Images.SetKeyName(6, "");
      this.imageList1.Images.SetKeyName(7, "");
      this.imageList1.Images.SetKeyName(8, "");
      this.imageList1.Images.SetKeyName(9, "");
      this.imageList1.Images.SetKeyName(10, "");
      this.imageList1.Images.SetKeyName(11, "");
      this.imageList1.Images.SetKeyName(12, "");
      this.imageList1.Images.SetKeyName(13, "");
      this.imageList1.Images.SetKeyName(14, "");
      this.imageList1.Images.SetKeyName(15, "");
      // 
      // myPopup
      // 
      this.myPopup.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1});
      this.myPopup.Popup += new System.EventHandler(this.myPopup_Popup);
      // 
      // menuItem1
      // 
      this.menuItem1.Index = 0;
      this.menuItem1.Text = "Change &Background";
      this.menuItem1.Click += new System.EventHandler(this.menuItem1_Click);
      // 
      // myPopupObject
      // 
      this.myPopupObject.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.ContextWireframe,
            this.ContextShading,
            this.ContextColor,
            this.ContextMaterial,
            this.ContextSelection,
            this.ContMenTranc,
            this.ContMenDelete});
      this.myPopupObject.Popup += new System.EventHandler(this.myPopupObject_Popup);
      // 
      // ContextWireframe
      // 
      this.ContextWireframe.Index = 0;
      this.ContextWireframe.Text = "Wireframe";
      this.ContextWireframe.Click += new System.EventHandler(this.ContextWireframe_Click);
      // 
      // ContextShading
      // 
      this.ContextShading.Index = 1;
      this.ContextShading.Text = "Shading";
      this.ContextShading.Click += new System.EventHandler(this.ContextShading_Click);
      // 
      // ContextColor
      // 
      this.ContextColor.Index = 2;
      this.ContextColor.Text = "Color";
      this.ContextColor.Click += new System.EventHandler(this.ContextColor_Click);
      // 
      // ContextMaterial
      // 
      this.ContextMaterial.Index = 3;
      this.ContextMaterial.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.ContMatBrass,
            this.ContMenBronze,
            this.ContMenCopper,
            this.ContMenGold,
            this.ContMenPewt,
            this.ContMenPlaster,
            this.ContMenPlastic,
            this.ContMenSilver});
      this.ContextMaterial.Text = "Material";
      // 
      // ContMatBrass
      // 
      this.ContMatBrass.Index = 0;
      this.ContMatBrass.Text = "&Brass";
      this.ContMatBrass.Click += new System.EventHandler(this.ContMatBrass_Click);
      // 
      // ContMenBronze
      // 
      this.ContMenBronze.Index = 1;
      this.ContMenBronze.Text = "&Bronze";
      this.ContMenBronze.Click += new System.EventHandler(this.ContMenBronze_Click);
      // 
      // ContMenCopper
      // 
      this.ContMenCopper.Index = 2;
      this.ContMenCopper.Text = "&Copper";
      this.ContMenCopper.Click += new System.EventHandler(this.ContMenCopper_Click);
      // 
      // ContMenGold
      // 
      this.ContMenGold.Index = 3;
      this.ContMenGold.Text = "&Gold";
      this.ContMenGold.Click += new System.EventHandler(this.ContMenGold_Click);
      // 
      // ContMenPewt
      // 
      this.ContMenPewt.Index = 4;
      this.ContMenPewt.Text = "&Pewter";
      this.ContMenPewt.Click += new System.EventHandler(this.ContMenPewt_Click);
      // 
      // ContMenPlaster
      // 
      this.ContMenPlaster.Index = 5;
      this.ContMenPlaster.Text = "&Plaster";
      this.ContMenPlaster.Click += new System.EventHandler(this.ContMenPlaster_Click);
      // 
      // ContMenPlastic
      // 
      this.ContMenPlastic.Index = 6;
      this.ContMenPlastic.Text = "&Plastic";
      this.ContMenPlastic.Click += new System.EventHandler(this.ContMenPlastic_Click);
      // 
      // ContMenSilver
      // 
      this.ContMenSilver.Index = 7;
      this.ContMenSilver.Text = "&Silver";
      this.ContMenSilver.Click += new System.EventHandler(this.ContMenSilver_Click);
      // 
      // ContextSelection
      // 
      this.ContextSelection.Index = 4;
      this.ContextSelection.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.ContFaceSel,
            this.ContEdgeSel,
            this.ContVertSel});
      this.ContextSelection.Text = "Selection";
      // 
      // ContFaceSel
      // 
      this.ContFaceSel.Index = 0;
      this.ContFaceSel.Text = "&Faces";
      this.ContFaceSel.Click += new System.EventHandler(this.ContFaceSel_Click);
      // 
      // ContEdgeSel
      // 
      this.ContEdgeSel.Index = 1;
      this.ContEdgeSel.Text = "&Edges";
      this.ContEdgeSel.Click += new System.EventHandler(this.ContEdgeSel_Click);
      // 
      // ContVertSel
      // 
      this.ContVertSel.Index = 2;
      this.ContVertSel.Text = "&Vertices";
      this.ContVertSel.Click += new System.EventHandler(this.ContVertSel_Click);
      // 
      // ContMenTranc
      // 
      this.ContMenTranc.Index = 5;
      this.ContMenTranc.Text = "&Trancparency";
      this.ContMenTranc.Click += new System.EventHandler(this.ContMenTranc_Click);
      // 
      // ContMenDelete
      // 
      this.ContMenDelete.Index = 6;
      this.ContMenDelete.Text = "&Delete";
      this.ContMenDelete.Click += new System.EventHandler(this.ContMenDelete_Click);
      // 
      // myView
      // 
      this.myView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.myView.Location = new System.Drawing.Point(0, 0);
      this.myView.Mode = OCCWinForms.CurrentAction3d.CurAction3d_Nothing;
      this.myView.Name = "myView";
      this.myView.Size = new System.Drawing.Size(320, 261);
      this.myView.TabIndex = 0;
      this.myView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form2_MouseDown);
      this.myView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Form2_MouseUp);
      // 
      // Form2
      // 
      this.AccessibleRole = System.Windows.Forms.AccessibleRole.Window;
      this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
      this.ClientSize = new System.Drawing.Size(320, 261);
      this.Controls.Add(this.myView);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "Form2";
      this.Text = "Document";
      this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
      this.Closed += new System.EventHandler(this.Form2_Closed);
      this.SizeChanged += new System.EventHandler(this.Form2_SizeChanged);
      this.ResumeLayout(false);
      this.PerformLayout();

                }
		#endregion

		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.ContextMenu myPopup;
		private System.Windows.Forms.ContextMenu myPopupObject;
		private System.Windows.Forms.MenuItem ContextWireframe;
		private System.Windows.Forms.MenuItem ContextShading;
		private System.Windows.Forms.MenuItem ContextColor;
		private System.Windows.Forms.MenuItem ContextMaterial;
		private System.Windows.Forms.MenuItem ContMatBrass;
		private System.Windows.Forms.MenuItem ContMenBronze;
		private System.Windows.Forms.MenuItem ContMenCopper;
		private System.Windows.Forms.MenuItem ContMenGold;
		private System.Windows.Forms.MenuItem ContMenPewt;
		private System.Windows.Forms.MenuItem ContMenPlaster;
		private System.Windows.Forms.MenuItem ContMenPlastic;
		private System.Windows.Forms.MenuItem ContMenSilver;
                private System.Windows.Forms.MenuItem ContextSelection;
                private System.Windows.Forms.MenuItem ContFaceSel;
                private System.Windows.Forms.MenuItem ContEdgeSel;
                private System.Windows.Forms.MenuItem ContVertSel;
		private System.Windows.Forms.MenuItem ContMenTranc;
		private System.Windows.Forms.MenuItem ContMenDelete;
		private System.Windows.Forms.MenuItem menuItem1;

		public void InitV3D()
		{

			if( !this.myView.InitViewer() )
				MessageBox.Show("Fatal Error during the graphic initialisation", "Error!", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		public bool ImportBRep(System.String filename)
		{
			return this.myView.ImportBRep(filename);
		}

		private void Form2_SizeChanged(object sender, System.EventArgs e)
		{
			this.myView.UpdateView();
		}

		protected bool myHlrModeIsOn;


		protected void Popup(int x, int y)
		{
			System.Drawing.Point p = new Point(x,y);
			if (this.myView.IsObjectSelected())
				this.myPopupObject.Show(this, p);
			else
				this.myPopup.Show(this, p);
		}

    //! Mouse click position for showing popup menu.
    private Point myClickPos = new Point(-1, -1);

    private void Form2_MouseDown(object sender, System.Windows.Forms.MouseEventArgs theArgs)
    {
      myClickPos.X = theArgs.X;
      myClickPos.Y = theArgs.Y;
    }

    private void Form2_MouseUp(object sender, System.Windows.Forms.MouseEventArgs theArgs)
    {
      if (myView.Mode != OCCWinForms.CurrentAction3d.CurAction3d_Nothing)
      {
        myView.Mode = OCCWinForms.CurrentAction3d.CurAction3d_Nothing;
      }
      IE.Form1 parent = (IE.Form1)this.ParentForm;
      parent.SelectionChanged();
      if (theArgs.Button == MouseButtons.Right
       && (Control.ModifierKeys & Keys.Control) == 0
       && Math.Abs (myClickPos.X - theArgs.X) <= 4
       && Math.Abs (myClickPos.Y - theArgs.Y) <= 4)
      {
        Popup (theArgs.X, theArgs.Y);
      }
    }

 		public void SetDisplayMode(int aMode)
		{
			this.myView.SetDisplayMode(aMode);
		}

		public void ChangeColor(bool IsObjectColor)
		{
			int r=0, g=0, b=0;
			if (IsObjectColor) 
                          this.myView.ObjectColor(ref r, ref g, ref b);
                        else 
                          this.myView.BackgroundColor(ref r, ref g, ref b);
                        System.Windows.Forms.ColorDialog ColDlg = new ColorDialog();
			ColDlg.Color=System.Drawing.Color.FromArgb(r, g, b);
			if (ColDlg.ShowDialog() == DialogResult.OK)
			{
				Color c = ColDlg.Color;
				r=c.R;
				g=c.G;
				b=c.B;
				if (IsObjectColor)
					this.myView.SetColor(r, g, b);
				else
					this.myView.SetBackgroundColor(r, g, b);
			}
			this.myView.UpdateCurrentViewer();

		}

		public void DeleteObjects()
		{
			this.myView.EraseObjects();
			IE.Form1 parent = (IE.Form1)this.ParentForm;
			parent.SelectionChanged();
		}

    public void ImportModel(IE.ModelFormat format) {
      System.Windows.Forms.OpenFileDialog openDialog = new OpenFileDialog();

      string DataDir = Environment.GetEnvironmentVariable("CSF_OCCTDataPath");

      string filter = "";

      switch (format) {
        case IE.ModelFormat.BREP:
          openDialog.InitialDirectory = (DataDir + "\\occ");
          filter = "BREP Files (*.brep *.rle)|*.brep; *.rle";
          break;
        case IE.ModelFormat.STEP:
          openDialog.InitialDirectory = (DataDir + "\\step");
          filter = "STEP Files (*.stp *.step)|*.stp; *.step";
          break;
        case IE.ModelFormat.IGES:
          openDialog.InitialDirectory = (DataDir + "\\iges");
          filter = "IGES Files (*.igs *.iges)|*.igs; *.iges";
          break;
        default:
          break;
      }
      openDialog.Filter = filter + "|All files (*.*)|*.*";
      if (openDialog.ShowDialog() == DialogResult.OK) {
        string filename = openDialog.FileName;
        if (filename == "")
          return;
        this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
        bool res;
        switch (format) {
          case IE.ModelFormat.BREP:
            res = this.myView.ImportBRep(filename);
            break;
          case IE.ModelFormat.STEP:
            res = this.myView.ImportStep(filename);
            break;
          case IE.ModelFormat.IGES:
            res = this.myView.ImportIges(filename);
            break;
          default:
            res = false;
            break;
        }
        if (!res)
          MessageBox.Show("Can't read this file", "Error!",
            MessageBoxButtons.OK, MessageBoxIcon.Warning);
        this.Cursor = System.Windows.Forms.Cursors.Default;
      }
      this.myView.ZoomAllView();
    }

    public void ExportModel(ModelFormat format) {
      System.Windows.Forms.SaveFileDialog saveDialog = new SaveFileDialog();
      string DataDir = Environment.GetEnvironmentVariable("CSF_OCCTDataPath");
      string filter = "";
      switch (format) {
        case IE.ModelFormat.BREP:
          saveDialog.InitialDirectory = (DataDir + "\\occ");
          filter = "BREP Files (*.brep *.rle)|*.brep; *.rle";
          break;
        case IE.ModelFormat.STEP:
          saveDialog.InitialDirectory = (DataDir + "\\step");
          filter = "STEP Files (*.stp *.step)|*.step; *.stp";
          break;
        case IE.ModelFormat.IGES:
          saveDialog.InitialDirectory = (DataDir + "\\iges");
          filter = "IGES Files (*.igs *.iges)| *.iges; *.igs";
          break;
        case IE.ModelFormat.VRML:
          saveDialog.InitialDirectory = (DataDir + "\\vrml");
          filter = "VRML Files (*.vrml)|*.vrml";
          break;
        case IE.ModelFormat.STL:
          saveDialog.InitialDirectory = (DataDir + "\\stl");
          filter = "STL Files (*.stl)|*.stl";
          break;
        case IE.ModelFormat.IMAGE:
          saveDialog.InitialDirectory = (DataDir + "\\images");
          filter = "Images Files (*.bmp *.gif *.xwd)| *.bmp; *.gif; *.xwd";
          break;
        default:
          break;
      }
      saveDialog.Filter = filter;
      if (saveDialog.ShowDialog() == DialogResult.OK) {
        string filename = saveDialog.FileName;
        if (filename == "")
          return;
        this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
        bool res = false;
        switch (format) {
          case IE.ModelFormat.BREP:
            res = this.myView.ExportBRep(filename);
            break;
          case IE.ModelFormat.STEP:
            res = this.myView.ExportStep(filename);
            break;
          case IE.ModelFormat.IGES:
            res = this.myView.ExportIges(filename);
            break;
          case IE.ModelFormat.VRML:
            res = this.myView.ExportVrml(filename);
            break;
          case IE.ModelFormat.STL:
            res = this.myView.ExportStl(filename);
            break;
          case IE.ModelFormat.IMAGE:
            res = this.myView.Dump(filename);
            break;
          default:
            break;
        }
        if (!res)
          MessageBox.Show("Cann't write this file", "Error!",
            MessageBoxButtons.OK, MessageBoxIcon.Warning);
        this.Cursor = System.Windows.Forms.Cursors.Default;
      }
    }

    private void ContextColor_Click(object sender, System.EventArgs e)
		{
			this.ChangeColor(true);
		}

		private void menuItem1_Click(object sender, System.EventArgs e)
		{
			this.myView.UpdateCurrentViewer();
			this.ChangeColor(false);
		}

		private void ContextWireframe_Click(object sender, System.EventArgs e)
		{
			this.SetDisplayMode(0);
			this.myView.UpdateCurrentViewer();
			IE.Form1 parent=(IE.Form1)this.ParentForm;
			parent.SelectionChanged();
		}

		private void ContextShading_Click(object sender, System.EventArgs e)
		{
			this.SetDisplayMode(1);
			this.myView.UpdateCurrentViewer();
			IE.Form1 parent=(IE.Form1)this.ParentForm;
			parent.SelectionChanged();
		}

		private void ContMenTranc_Click(object sender, System.EventArgs e)
		{
			IE.TransparencyDialog dlg = new TransparencyDialog();
			dlg.View=this.myView;
			dlg.ShowDialog(this);
		}

		private void ContMenDelete_Click(object sender, System.EventArgs e)
		{
			this.DeleteObjects();
		}

		private void ContMatBrass_Click(object sender, System.EventArgs e)
		{
			this.myView.UpdateCurrentViewer();
			this.myView.SetMaterial(0);
		}

		private void ContMenBronze_Click(object sender, System.EventArgs e)
		{
			this.myView.UpdateCurrentViewer();
			this.myView.SetMaterial(1);
		}

		private void ContMenCopper_Click(object sender, System.EventArgs e)
		{
			this.myView.UpdateCurrentViewer();
			this.myView.SetMaterial(2);
		}

		private void ContMenGold_Click(object sender, System.EventArgs e)
		{
			this.myView.UpdateCurrentViewer();
			this.myView.SetMaterial(3);
		}

		private void ContMenPewt_Click(object sender, System.EventArgs e)
		{
			this.myView.UpdateCurrentViewer();
			this.myView.SetMaterial(4);
		}

		private void ContMenPlaster_Click(object sender, System.EventArgs e)
		{
			this.myView.UpdateCurrentViewer();
			this.myView.SetMaterial(5);
		}

		private void ContMenPlastic_Click(object sender, System.EventArgs e)
		{
			this.myView.UpdateCurrentViewer();
			this.myView.SetMaterial(6);
		}

		private void ContMenSilver_Click(object sender, System.EventArgs e)
		{
			this.myView.UpdateCurrentViewer();
			this.myView.SetMaterial(7);
		}
                private void ContFaceSel_Click(object sender, System.EventArgs e)
                {
                  this.myView.OnFaces();
                }
                private void ContEdgeSel_Click(object sender, System.EventArgs e)
                {
                  this.myView.OnEdges();
                }
                private void ContVertSel_Click(object sender, System.EventArgs e)
                {
                  this.myView.OnVertices();
                }
                
		private void toolBar1_MouseHover(object sender, System.EventArgs e)
		{
			IE.Form1 parent=(IE.Form1)this.ParentForm;
			parent.StatusBar.Text="View toolbar";

		}

		private void toolBar1_MouseLeave(object sender, System.EventArgs e)
		{
			IE.Form1 parent=(IE.Form1)this.ParentForm;
			parent.StatusBar.Text="";
		}

		private void myPopupObject_Popup(object sender, System.EventArgs e)
		{
			int mode=this.myView.DisplayMode();
			switch (mode)
			{
				case -1:
					 break;
				case 0:
					this.ContextWireframe.Enabled=false;
					this.ContextShading.Enabled=true;
					this.ContMenTranc.Enabled=false;
					break;
				case 1:
					this.ContextShading.Enabled=false;
					this.ContextWireframe.Enabled=true;
					this.ContMenTranc.Enabled=true;
					break;
				case 10:
					this.ContextShading.Enabled=true;;
					this.ContextWireframe.Enabled=true;
					this.ContMenTranc.Enabled=true;
					break;
				default:
					break;

			}
		}

        public OCCWinForms.OCCWinForms_Viewer View { get { return myView; } set { myView = value; } }
        private OCCWinForms.OCCWinForms_Viewer myView;

		public void InitView()
		{
//			this.myView.InitOCCViewer();
		}

		private void Form2_Closed(object sender, System.EventArgs e)
		{
			IE.Form1 parent = (IE.Form1) this.ParentForm;
			parent.OnFileClose();
		}

          private void myPopup_Popup(object sender, System.EventArgs e)
          {
          
          }

		public OCCWinForms.CurrentAction3d Mode
		{
			get
			{
				return this.myView.Mode;
			}
			set
			{
              this.myView.Mode = value;
			}
		}

		public bool HlrMode
		{
			get
			{
				return this.myHlrModeIsOn;
			}
			set
			{
				this.myHlrModeIsOn=value;
			}
		}
	}
}
