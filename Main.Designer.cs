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

namespace AeroShot
{
	sealed partial class AeroShot
	{
		private System.ComponentModel.IContainer components = null;

		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AeroShot));
			this.ssButton = new System.Windows.Forms.Button();
			this.windowList = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.rButton = new System.Windows.Forms.Button();
			this.folderTextBox = new System.Windows.Forms.TextBox();
			this.bButton = new System.Windows.Forms.Button();
			this.folderSelection = new System.Windows.Forms.FolderBrowserDialog();
			this.SuspendLayout();
			// 
			// ssButton
			// 
			resources.ApplyResources(this.ssButton, "ssButton");
			this.ssButton.Name = "ssButton";
			this.ssButton.UseVisualStyleBackColor = true;
			this.ssButton.Click += new System.EventHandler(this.ssButton_Click);
			// 
			// windowList
			// 
			this.windowList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.windowList.FormattingEnabled = true;
			resources.ApplyResources(this.windowList, "windowList");
			this.windowList.Name = "windowList";
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			// 
			// rButton
			// 
			resources.ApplyResources(this.rButton, "rButton");
			this.rButton.Name = "rButton";
			this.rButton.UseVisualStyleBackColor = true;
			this.rButton.Click += new System.EventHandler(this.rButton_Click);
			// 
			// folderTextBox
			// 
			resources.ApplyResources(this.folderTextBox, "folderTextBox");
			this.folderTextBox.Name = "folderTextBox";
			// 
			// bButton
			// 
			resources.ApplyResources(this.bButton, "bButton");
			this.bButton.Name = "bButton";
			this.bButton.UseVisualStyleBackColor = true;
			this.bButton.Click += new System.EventHandler(this.bButton_Click);
			// 
			// folderSelection
			// 
			resources.ApplyResources(this.folderSelection, "folderSelection");
			// 
			// AeroShot
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.folderTextBox);
			this.Controls.Add(this.bButton);
			this.Controls.Add(this.rButton);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.windowList);
			this.Controls.Add(this.ssButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "AeroShot";
			this.Shown += new System.EventHandler(this.AeroShot_Shown);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AeroShot_Closing);
			this.ResumeLayout(false);
			this.PerformLayout();

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

	}
}

