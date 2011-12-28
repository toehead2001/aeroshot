/*  AeroShot - Transparent screenshot utility for Windows
	Copyright (C) 2011 Caleb Joseph

	AeroShot is free software: you can redistribute it and/or modify
	it under the terms of the GNU General Public License as published by
	the Free Software Foundation, either version 3 of the License, or
	(at your option) any later version.

	AeroShot is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU General Public License for more details.

	You should have received a copy of the GNU General Public License
	along with this program.  If not, see <http://www.gnu.org/licenses/>. */

using System.Drawing;

namespace AeroShot {
	sealed partial class MainForm {
		private System.Windows.Forms.Button bButton;
		private System.Windows.Forms.NumericUpDown checkerValue;
		private System.Windows.Forms.ColorDialog colorDialog;
		private AeroShot.ColorDisplay colorDisplay;
		private System.Windows.Forms.TextBox colorHexBox;
		private System.Windows.Forms.FolderBrowserDialog folderSelection;
		private System.Windows.Forms.TextBox folderTextBox;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.CheckBox opaqueCheckBox;
		private System.Windows.Forms.ComboBox opaqueType;
		private System.Windows.Forms.Button rButton;
		private System.Windows.Forms.Panel ssButton;
		private System.Windows.Forms.CheckBox resizeCheckBox;
		private System.Windows.Forms.NumericUpDown windowHeight;
		private System.Windows.Forms.ComboBox windowList;
		private System.Windows.Forms.NumericUpDown windowWidth;

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.windowList = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.rButton = new System.Windows.Forms.Button();
			this.folderTextBox = new System.Windows.Forms.TextBox();
			this.bButton = new System.Windows.Forms.Button();
			this.folderSelection = new System.Windows.Forms.FolderBrowserDialog();
			this.resizeCheckBox = new System.Windows.Forms.CheckBox();
			this.windowHeight = new System.Windows.Forms.NumericUpDown();
			this.label4 = new System.Windows.Forms.Label();
			this.windowWidth = new System.Windows.Forms.NumericUpDown();
			this.opaqueCheckBox = new System.Windows.Forms.CheckBox();
			this.opaqueType = new System.Windows.Forms.ComboBox();
			this.label6 = new System.Windows.Forms.Label();
			this.checkerValue = new System.Windows.Forms.NumericUpDown();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.colorHexBox = new System.Windows.Forms.TextBox();
			this.colorDisplay = new AeroShot.ColorDisplay();
			this.label7 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label3 = new System.Windows.Forms.Label();
			this.colorDialog = new System.Windows.Forms.ColorDialog();
			this.ssButton = new System.Windows.Forms.Panel();
			((System.ComponentModel.ISupportInitialize)(this.windowHeight)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.windowWidth)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.checkerValue)).BeginInit();
			this.groupBox2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// windowList
			// 
			this.windowList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.windowList.FormattingEnabled = true;
			this.windowList.Location = new System.Drawing.Point(12, 25);
			this.windowList.Name = "windowList";
			this.windowList.Size = new System.Drawing.Size(309, 21);
			this.windowList.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(9, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(130, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Select window to capture:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(9, 54);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(107, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Save screenshots to:";
			// 
			// rButton
			// 
			this.rButton.Location = new System.Drawing.Point(327, 23);
			this.rButton.Name = "rButton";
			this.rButton.Size = new System.Drawing.Size(90, 23);
			this.rButton.TabIndex = 2;
			this.rButton.Text = "Refresh List";
			this.rButton.UseVisualStyleBackColor = true;
			this.rButton.Click += new System.EventHandler(this.RefreshButtonClick);
			// 
			// folderTextBox
			// 
			this.folderTextBox.Location = new System.Drawing.Point(12, 70);
			this.folderTextBox.Name = "folderTextBox";
			this.folderTextBox.Size = new System.Drawing.Size(309, 20);
			this.folderTextBox.TabIndex = 4;
			// 
			// bButton
			// 
			this.bButton.Location = new System.Drawing.Point(327, 68);
			this.bButton.Name = "bButton";
			this.bButton.Size = new System.Drawing.Size(90, 23);
			this.bButton.TabIndex = 5;
			this.bButton.Text = "Browse";
			this.bButton.UseVisualStyleBackColor = true;
			this.bButton.Click += new System.EventHandler(this.BrowseButtonClick);
			// 
			// folderSelection
			// 
			this.folderSelection.Description = "Please select the folder you wish screenshots to be saved to.\r\nFilenames are dete" +
				"rmined by the title of the window you are capturing.";
			// 
			// resizeCheckBox
			// 
			this.resizeCheckBox.AutoSize = true;
			this.resizeCheckBox.Location = new System.Drawing.Point(19, 97);
			this.resizeCheckBox.Name = "resizeCheckBox";
			this.resizeCheckBox.Size = new System.Drawing.Size(96, 17);
			this.resizeCheckBox.TabIndex = 6;
			this.resizeCheckBox.Text = "Resize window";
			this.resizeCheckBox.UseVisualStyleBackColor = true;
			this.resizeCheckBox.CheckedChanged += new System.EventHandler(this.ResizeCheckboxStateChange);
			// 
			// windowHeight
			// 
			this.windowHeight.Location = new System.Drawing.Point(309, 25);
			this.windowHeight.Maximum = new decimal(new int[] {
            16777215,
            0,
            0,
            0});
			this.windowHeight.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
			this.windowHeight.Name = "windowHeight";
			this.windowHeight.Size = new System.Drawing.Size(90, 20);
			this.windowHeight.TabIndex = 11;
			this.windowHeight.Value = new decimal(new int[] {
            480,
            0,
            0,
            0});
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(291, 27);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(12, 13);
			this.label4.TabIndex = 10;
			this.label4.Text = "x";
			// 
			// windowWidth
			// 
			this.windowWidth.Location = new System.Drawing.Point(195, 25);
			this.windowWidth.Maximum = new decimal(new int[] {
            16777215,
            0,
            0,
            0});
			this.windowWidth.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
			this.windowWidth.Name = "windowWidth";
			this.windowWidth.Size = new System.Drawing.Size(90, 20);
			this.windowWidth.TabIndex = 9;
			this.windowWidth.Value = new decimal(new int[] {
            640,
            0,
            0,
            0});
			// 
			// opaqueCheckBox
			// 
			this.opaqueCheckBox.AutoSize = true;
			this.opaqueCheckBox.Location = new System.Drawing.Point(19, 158);
			this.opaqueCheckBox.Name = "opaqueCheckBox";
			this.opaqueCheckBox.Size = new System.Drawing.Size(123, 17);
			this.opaqueCheckBox.TabIndex = 12;
			this.opaqueCheckBox.Text = "Opaque background";
			this.opaqueCheckBox.UseVisualStyleBackColor = true;
			this.opaqueCheckBox.CheckedChanged += new System.EventHandler(this.OpaqueCheckboxStateChange);
			// 
			// opaqueType
			// 
			this.opaqueType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.opaqueType.FormattingEnabled = true;
			this.opaqueType.Items.AddRange(new object[] {
            "Checkerboard",
            "Solid color"});
			this.opaqueType.Location = new System.Drawing.Point(195, 19);
			this.opaqueType.Name = "opaqueType";
			this.opaqueType.Size = new System.Drawing.Size(204, 21);
			this.opaqueType.TabIndex = 15;
			this.opaqueType.SelectedIndexChanged += new System.EventHandler(this.OpaqueTypeItemChange);
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(192, 48);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(29, 13);
			this.label6.TabIndex = 16;
			this.label6.Text = "VAR";
			// 
			// checkerValue
			// 
			this.checkerValue.Location = new System.Drawing.Point(327, 46);
			this.checkerValue.Maximum = new decimal(new int[] {
            256,
            0,
            0,
            0});
			this.checkerValue.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
			this.checkerValue.Name = "checkerValue";
			this.checkerValue.Size = new System.Drawing.Size(48, 20);
			this.checkerValue.TabIndex = 17;
			this.checkerValue.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.colorHexBox);
			this.groupBox2.Controls.Add(this.colorDisplay);
			this.groupBox2.Controls.Add(this.label7);
			this.groupBox2.Controls.Add(this.label6);
			this.groupBox2.Controls.Add(this.checkerValue);
			this.groupBox2.Controls.Add(this.opaqueType);
			this.groupBox2.Controls.Add(this.label5);
			this.groupBox2.Controls.Add(this.label8);
			this.groupBox2.Location = new System.Drawing.Point(12, 158);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(405, 76);
			this.groupBox2.TabIndex = 13;
			this.groupBox2.TabStop = false;
			// 
			// colorHexBox
			// 
			this.colorHexBox.Font = new System.Drawing.Font("Lucida Console", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.colorHexBox.Location = new System.Drawing.Point(265, 46);
			this.colorHexBox.MaxLength = 6;
			this.colorHexBox.Name = "colorHexBox";
			this.colorHexBox.Size = new System.Drawing.Size(56, 20);
			this.colorHexBox.TabIndex = 20;
			this.colorHexBox.Text = "FFFFFF";
			this.colorHexBox.TextChanged += new System.EventHandler(this.ColorTextboxTextChange);
			// 
			// colorDisplay
			// 
			this.colorDisplay.Color = System.Drawing.Color.White;
			this.colorDisplay.Location = new System.Drawing.Point(327, 46);
			this.colorDisplay.Name = "colorDisplay";
			this.colorDisplay.Size = new System.Drawing.Size(72, 19);
			this.colorDisplay.TabIndex = 21;
			this.colorDisplay.Click += new System.EventHandler(this.ColorDisplayClick);
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(381, 48);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(18, 13);
			this.label7.TabIndex = 18;
			this.label7.Text = "px";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(6, 20);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(153, 39);
			this.label5.TabIndex = 14;
			this.label5.Text = "Screenshots are saved with an\r\nopaque background, rather\r\nthan a transparent one." +
				"";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(250, 48);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(14, 13);
			this.label8.TabIndex = 19;
			this.label8.Text = "#";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.windowHeight);
			this.groupBox1.Controls.Add(this.windowWidth);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Location = new System.Drawing.Point(12, 97);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(405, 55);
			this.groupBox1.TabIndex = 7;
			this.groupBox1.TabStop = false;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(6, 20);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(141, 26);
			this.label3.TabIndex = 8;
			this.label3.Text = "Sets the exact resolution\r\nscreenshots are captured in.";
			// 
			// colorDialog
			// 
			this.colorDialog.Color = System.Drawing.Color.White;
			this.colorDialog.FullOpen = true;
			// 
			// ssButton
			// 
			this.ssButton.BackColor = System.Drawing.Color.Transparent;
			this.ssButton.Location = new System.Drawing.Point(115, 240);
			this.ssButton.Name = "ssButton";
			this.ssButton.Size = new System.Drawing.Size(200, 30);
			this.ssButton.TabIndex = 14;
			this.ssButton.MouseLeave += new System.EventHandler(this.ScreenshotButtonPlaceholderMouseLeave);
			this.ssButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ScreenshotButtonPlaceholderMouseDown);
			this.ssButton.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ScreenshotButtonPlaceholderMouseUp);
			this.ssButton.MouseEnter += new System.EventHandler(this.ScreenshotButtonPlaceholderMouseEnter);
			// 
			// MainForm
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(430, 275);
			this.Controls.Add(this.resizeCheckBox);
			this.Controls.Add(this.opaqueCheckBox);
			this.Controls.Add(this.folderTextBox);
			this.Controls.Add(this.bButton);
			this.Controls.Add(this.rButton);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.windowList);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.ssButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "MainForm";
			this.Text = "AeroShot 1.2.1";
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.OnPaint);
			this.SizeChanged += new System.EventHandler(this.FormSizeChange);
			this.Shown += new System.EventHandler(this.FormShown);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormClose);
			((System.ComponentModel.ISupportInitialize)(this.windowHeight)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.windowWidth)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.checkerValue)).EndInit();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion
	}
}