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
using System.Net.NetworkInformation;

using OCC.Standard;

namespace IE
{
	/// <summary>
	/// Summary description for AboutDialog.
	/// </summary>
	public class AboutDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label myVersion;
        private System.Windows.Forms.Label myLicenseID;
        private TextBox myLicensesInfo;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public AboutDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			//
			// TODO: Add any constructor code after InitializeComponent call
			//

            String aVersion = Standard_Version.String();
            this.myVersion.Text = this.myVersion.Text + aVersion + "." + Standard_Version.Maintenance();
            String anID = "";
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                // Only consider Ethernet network interfaces
                if (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet &&
                    nic.OperationalStatus == OperationalStatus.Up)
                    {
                        anID = nic.GetPhysicalAddress().ToString();
                        anID = anID.Trim(new Char[] { ':', '-', ' ' });
                        anID = anID.Remove(0, anID.Length - 8);
                        anID = anID.TrimStart(new Char[] { '0' });
                        anID = anID.ToUpper();
                        break;
                    }
            }
            this.myLicenseID.Text = "Your copy ID: " + anID;

            String aLicenseInfo = "";
            String aKeyFile = Environment.GetEnvironmentVariable("OCCLICENSE_FILE");
            bool hasLicense = false;
            if (aKeyFile == null)
            {
                aLicenseInfo += "Warning! Could not find license file since OCCLICENSE_FILE environment variable is not defined.";
            }
            else
            {
                String aLine;
                try
                {
                    System.IO.StreamReader aFile = new System.IO.StreamReader(aKeyFile);
                    while ((aLine = aFile.ReadLine()) != null)
                    {
                        String aCleanString = System.Text.RegularExpressions.Regex.Replace(aLine, @"\s+", " ");
                        String[] aTokens = aCleanString.Split(null);
                        if (aTokens.Length < 2)
                            continue;

                        String aProd  = aTokens[0];
                        String aLicID = aTokens[1];
                        aLicID = aLicID.ToUpper();

                        if (aProd.Contains(aVersion) && anID == aLicID)
                        {
                            if (!hasLicense)
                            {
                                hasLicense = true;
                                aLicenseInfo += "License file ";
                                aLicenseInfo += aKeyFile;
                                aLicenseInfo += " specified in OCCLICENSE_FILE environment variable activates the following products:\r\n - ";
                            }
                            else
                            {
                                aLicenseInfo += "\r\n - ";
                            }
                            aLicenseInfo += aProd;
                        }
                    }
                    aFile.Close();
                }
                catch (System.IO.FileNotFoundException)
                {
                    aLicenseInfo += "Warning! Could not open license file ";
                    aLicenseInfo += aKeyFile;
                    aLicenseInfo += " specified in OCCLICENSE_FILE environment variable.";
                }
            }
            this.myLicensesInfo.Text = aLicenseInfo;
            this.myLicensesInfo.Visible = true;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutDialog));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.myVersion = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.myLicenseID = new System.Windows.Forms.Label();
            this.myLicensesInfo = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(59, 64);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(196, 102);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(95, 326);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(128, 24);
            this.button1.TabIndex = 1;
            this.button1.Text = "OK";
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(16, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(288, 24);
            this.label1.TabIndex = 2;
            this.label1.Text = "Import/Export Sample,";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // myVersion
            // 
            this.myVersion.Location = new System.Drawing.Point(16, 32);
            this.myVersion.Name = "myVersion";
            this.myVersion.Size = new System.Drawing.Size(288, 16);
            this.myVersion.TabIndex = 3;
            this.myVersion.Text = "Open CASCADE Technology ";
            this.myVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(24, 168);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(280, 23);
            this.label3.TabIndex = 4;
            this.label3.Text = "Copyright (C) 2019-2021, Open CASCADE SAS";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(8, 200);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(296, 24);
            this.label4.TabIndex = 5;
            this.label4.Text = "http://opencascade.com marketing.contact@opencascade.com";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // myLicenseID
            // 
            this.myLicenseID.Location = new System.Drawing.Point(8, 226);
            this.myLicenseID.Name = "myLicenseID";
            this.myLicenseID.Size = new System.Drawing.Size(296, 24);
            this.myLicenseID.TabIndex = 6;
            this.myLicenseID.Text = "License ID";
            this.myLicenseID.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // myLicensesInfo
            // 
            this.myLicensesInfo.Location = new System.Drawing.Point(19, 253);
            this.myLicensesInfo.Multiline = true;
            this.myLicensesInfo.Name = "myLicensesInfo";
            this.myLicensesInfo.ReadOnly = true;
            this.myLicensesInfo.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.myLicensesInfo.Size = new System.Drawing.Size(281, 67);
            this.myLicensesInfo.TabIndex = 7;
            // 
            // AboutDialog
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(312, 362);
            this.ControlBox = false;
            this.Controls.Add(this.myLicensesInfo);
            this.Controls.Add(this.myLicenseID);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.myVersion);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutDialog";
            this.Text = "About Import/Export Sample";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void button1_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}
	}
}
