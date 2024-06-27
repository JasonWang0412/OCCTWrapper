using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace OCCWinForms {
  partial class OCCWinForms_Viewer
  {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;


    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      myCurrentMode = CurrentAction3d.CurAction3d_Nothing;

      this.SuspendLayout();
      // 
      // OcctViewerWinFormsUserControl
      // 
      this.Name = "OcctViewerWinFormsUserControl";
      this.ResumeLayout(false);

    }

    private void OcctView_Paint(object sender, System.Windows.Forms.PaintEventArgs e) {
      RedrawView();
      UpdateView();
    }

    #endregion
  }
}
