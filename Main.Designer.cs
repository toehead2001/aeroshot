/*  AeroShot - Transparent screenshot utility for Windows
	Copyright (C) 2011 Caleb Joseph

	This program is free software: you can redistribute it and/or modify
	it under the terms of the GNU General Public License as published by
	the Free Software Foundation, either version 3 of the License, or
	(at your option) any later version.

	This program is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU General Public License for more details.

	You should have received a copy of the GNU General Public License
	along with this program.  If not, see <http://www.gnu.org/licenses/>. */

namespace AeroShot {
	sealed partial class AeroShot {
		private System.ComponentModel.IContainer components = null;

		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) components.Dispose();
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			var resources = new System.ComponentModel.ComponentResourceManager(typeof (AeroShot));
			ssButton = new System.Windows.Forms.Button();
			windowList = new System.Windows.Forms.ComboBox();
			label1 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			rButton = new System.Windows.Forms.Button();
			folderTextBox = new System.Windows.Forms.TextBox();
			bButton = new System.Windows.Forms.Button();
			folderSelection = new System.Windows.Forms.FolderBrowserDialog();
			resizeCheckBox = new System.Windows.Forms.CheckBox();
			windowHeight = new System.Windows.Forms.NumericUpDown();
			label3 = new System.Windows.Forms.Label();
			windowWidth = new System.Windows.Forms.NumericUpDown();
			((System.ComponentModel.ISupportInitialize) (windowHeight)).BeginInit();
			((System.ComponentModel.ISupportInitialize) (windowWidth)).BeginInit();
			SuspendLayout();
			// 
			// ssButton
			// 
			resources.ApplyResources(ssButton, "ssButton");
			ssButton.Name = "ssButton";
			ssButton.UseVisualStyleBackColor = true;
			ssButton.Click += new System.EventHandler(ssButton_Click);
			// 
			// windowList
			// 
			windowList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			windowList.FormattingEnabled = true;
			resources.ApplyResources(windowList, "windowList");
			windowList.Name = "windowList";
			// 
			// label1
			// 
			resources.ApplyResources(label1, "label1");
			label1.Name = "label1";
			// 
			// label2
			// 
			resources.ApplyResources(label2, "label2");
			label2.Name = "label2";
			// 
			// rButton
			// 
			resources.ApplyResources(rButton, "rButton");
			rButton.Name = "rButton";
			rButton.UseVisualStyleBackColor = true;
			rButton.Click += new System.EventHandler(rButton_Click);
			// 
			// folderTextBox
			// 
			resources.ApplyResources(folderTextBox, "folderTextBox");
			folderTextBox.Name = "folderTextBox";
			// 
			// bButton
			// 
			resources.ApplyResources(bButton, "bButton");
			bButton.Name = "bButton";
			bButton.UseVisualStyleBackColor = true;
			bButton.Click += new System.EventHandler(bButton_Click);
			// 
			// folderSelection
			// 
			resources.ApplyResources(folderSelection, "folderSelection");
			// 
			// resizeCheckBox
			// 
			resources.ApplyResources(resizeCheckBox, "resizeCheckBox");
			resizeCheckBox.Name = "resizeCheckBox";
			resizeCheckBox.UseVisualStyleBackColor = true;
			resizeCheckBox.CheckedChanged += new System.EventHandler(resizeCheckBox_CheckedChanged);
			// 
			// windowHeight
			// 
			resources.ApplyResources(windowHeight, "windowHeight");
			windowHeight.Maximum = new decimal(new int[] {16777215, 0, 0, 0});
			windowHeight.Minimum = new decimal(new int[] {100, 0, 0, 0});
			windowHeight.Name = "windowHeight";
			windowHeight.Value = new decimal(new int[] {480, 0, 0, 0});
			// 
			// label3
			// 
			resources.ApplyResources(label3, "label3");
			label3.Name = "label3";
			// 
			// windowWidth
			// 
			resources.ApplyResources(windowWidth, "windowWidth");
			windowWidth.Maximum = new decimal(new int[] {16777215, 0, 0, 0});
			windowWidth.Minimum = new decimal(new int[] {100, 0, 0, 0});
			windowWidth.Name = "windowWidth";
			windowWidth.Value = new decimal(new int[] {640, 0, 0, 0});
			// 
			// AeroShot
			// 
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			resources.ApplyResources(this, "$this");
			Controls.Add(windowWidth);
			Controls.Add(label3);
			Controls.Add(windowHeight);
			Controls.Add(resizeCheckBox);
			Controls.Add(folderTextBox);
			Controls.Add(bButton);
			Controls.Add(rButton);
			Controls.Add(label2);
			Controls.Add(label1);
			Controls.Add(windowList);
			Controls.Add(ssButton);
			FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			MaximizeBox = false;
			Name = "AeroShot";
			Shown += new System.EventHandler(AeroShot_Shown);
			FormClosing += new System.Windows.Forms.FormClosingEventHandler(AeroShot_Closing);
			((System.ComponentModel.ISupportInitialize) (windowHeight)).EndInit();
			((System.ComponentModel.ISupportInitialize) (windowWidth)).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private System.Windows.Forms.Button ssButton;
		private System.Windows.Forms.ComboBox windowList;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button rButton;
		private System.Windows.Forms.TextBox folderTextBox;
		private System.Windows.Forms.Button bButton;
		private System.Windows.Forms.FolderBrowserDialog folderSelection;
		private System.Windows.Forms.CheckBox resizeCheckBox;
		private System.Windows.Forms.NumericUpDown windowHeight;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.NumericUpDown windowWidth;
	}
}