/*  AeroShot - Transparent screenshot utility for Windows
    Copyright (C) 2012 Caleb Joseph

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
using System.Windows.Forms;

namespace AeroShot {
    sealed partial class MainForm {
        private System.Windows.Forms.Button bButton;
        private System.Windows.Forms.NumericUpDown checkerValue;
        private System.Windows.Forms.ColorDialog colourDialog;
        private AeroShot.ColourDisplay colourDisplay;
        private System.Windows.Forms.TextBox colourHexBox;
        private System.Windows.Forms.FolderBrowserDialog folderSelection;
        private System.Windows.Forms.TextBox folderTextBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelResize;
        private System.Windows.Forms.Label labelX;
        private System.Windows.Forms.Label labelOpaque;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label labelHash;
        private System.Windows.Forms.CheckBox opaqueCheckbox;
        private System.Windows.Forms.ComboBox opaqueType;
        private System.Windows.Forms.CheckBox resizeCheckbox;
        private System.Windows.Forms.NumericUpDown windowHeight;
        private System.Windows.Forms.NumericUpDown windowWidth;
        private System.Windows.Forms.RadioButton diskButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton clipboardButton;
        private System.Windows.Forms.CheckBox mouseCheckbox;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label labelMouse;
        private System.Windows.Forms.CheckBox delayCheckbox;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label labelDelay;
        private System.Windows.Forms.NumericUpDown delaySeconds;
        private System.Windows.Forms.Label label6;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.label2 = new System.Windows.Forms.Label();
            this.folderTextBox = new System.Windows.Forms.TextBox();
            this.bButton = new System.Windows.Forms.Button();
            this.folderSelection = new System.Windows.Forms.FolderBrowserDialog();
            this.resizeCheckbox = new System.Windows.Forms.CheckBox();
            this.windowHeight = new System.Windows.Forms.NumericUpDown();
            this.labelX = new System.Windows.Forms.Label();
            this.windowWidth = new System.Windows.Forms.NumericUpDown();
            this.opaqueCheckbox = new System.Windows.Forms.CheckBox();
            this.opaqueType = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.checkerValue = new System.Windows.Forms.NumericUpDown();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.colourHexBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.labelOpaque = new System.Windows.Forms.Label();
            this.labelHash = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labelResize = new System.Windows.Forms.Label();
            this.colourDialog = new System.Windows.Forms.ColorDialog();
            this.diskButton = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.clipboardButton = new System.Windows.Forms.RadioButton();
            this.mouseCheckbox = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.labelMouse = new System.Windows.Forms.Label();
            this.delayCheckbox = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.labelDelay = new System.Windows.Forms.Label();
            this.delaySeconds = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.colourDisplay = new AeroShot.ColourDisplay();
            ((System.ComponentModel.ISupportInitialize)(this.windowHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.windowWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkerValue)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.delaySeconds)).BeginInit();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(9, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(108, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Save screenshots to";
            // 
            // folderTextBox
            // 
            this.folderTextBox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.folderTextBox.Location = new System.Drawing.Point(12, 25);
            this.folderTextBox.Name = "folderTextBox";
            this.folderTextBox.Size = new System.Drawing.Size(309, 22);
            this.folderTextBox.TabIndex = 7;
            // 
            // bButton
            // 
            this.bButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bButton.Location = new System.Drawing.Point(327, 24);
            this.bButton.Name = "bButton";
            this.bButton.Size = new System.Drawing.Size(90, 23);
            this.bButton.TabIndex = 8;
            this.bButton.Text = "Browse";
            this.bButton.UseVisualStyleBackColor = true;
            this.bButton.Click += new System.EventHandler(this.BrowseButtonClick);
            // 
            // folderSelection
            // 
            this.folderSelection.Description = "Please select the folder you wish screenshots to be saved to.\r\nFilenames are dete" +
    "rmined by the title of the window you are capturing.";
            // 
            // resizeCheckbox
            // 
            this.resizeCheckbox.AutoSize = true;
            this.resizeCheckbox.BackColor = System.Drawing.Color.Transparent;
            this.resizeCheckbox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.resizeCheckbox.Location = new System.Drawing.Point(19, 53);
            this.resizeCheckbox.Name = "resizeCheckbox";
            this.resizeCheckbox.Size = new System.Drawing.Size(103, 17);
            this.resizeCheckbox.TabIndex = 9;
            this.resizeCheckbox.Text = "Resize window";
            this.resizeCheckbox.UseVisualStyleBackColor = false;
            this.resizeCheckbox.CheckedChanged += new System.EventHandler(this.ResizeCheckboxStateChange);
            // 
            // windowHeight
            // 
            this.windowHeight.Location = new System.Drawing.Point(338, 25);
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
            this.windowHeight.Size = new System.Drawing.Size(60, 20);
            this.windowHeight.TabIndex = 14;
            this.windowHeight.Value = new decimal(new int[] {
            480,
            0,
            0,
            0});
            // 
            // labelX
            // 
            this.labelX.AutoSize = true;
            this.labelX.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelX.Location = new System.Drawing.Point(324, 27);
            this.labelX.Name = "labelX";
            this.labelX.Size = new System.Drawing.Size(12, 13);
            this.labelX.TabIndex = 13;
            this.labelX.Text = "x";
            // 
            // windowWidth
            // 
            this.windowWidth.Location = new System.Drawing.Point(262, 25);
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
            this.windowWidth.Size = new System.Drawing.Size(60, 20);
            this.windowWidth.TabIndex = 12;
            this.windowWidth.Value = new decimal(new int[] {
            640,
            0,
            0,
            0});
            // 
            // opaqueCheckbox
            // 
            this.opaqueCheckbox.AutoSize = true;
            this.opaqueCheckbox.BackColor = System.Drawing.Color.Transparent;
            this.opaqueCheckbox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.opaqueCheckbox.Location = new System.Drawing.Point(19, 114);
            this.opaqueCheckbox.Name = "opaqueCheckbox";
            this.opaqueCheckbox.Size = new System.Drawing.Size(134, 17);
            this.opaqueCheckbox.TabIndex = 15;
            this.opaqueCheckbox.Text = "Opaque background";
            this.opaqueCheckbox.UseVisualStyleBackColor = false;
            this.opaqueCheckbox.CheckedChanged += new System.EventHandler(this.OpaqueCheckboxStateChange);
            // 
            // opaqueType
            // 
            this.opaqueType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.opaqueType.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.opaqueType.FormattingEnabled = true;
            this.opaqueType.Items.AddRange(new object[] {
            "Checkerboard",
            "Solid colour"});
            this.opaqueType.Location = new System.Drawing.Point(195, 19);
            this.opaqueType.Name = "opaqueType";
            this.opaqueType.Size = new System.Drawing.Size(204, 21);
            this.opaqueType.TabIndex = 18;
            this.opaqueType.SelectedIndexChanged += new System.EventHandler(this.OpaqueTypeItemChange);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(192, 48);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(28, 13);
            this.label4.TabIndex = 19;
            this.label4.Text = "VAR";
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
            this.checkerValue.TabIndex = 23;
            this.checkerValue.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.colourHexBox);
            this.groupBox2.Controls.Add(this.colourDisplay);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.checkerValue);
            this.groupBox2.Controls.Add(this.opaqueType);
            this.groupBox2.Controls.Add(this.labelOpaque);
            this.groupBox2.Controls.Add(this.labelHash);
            this.groupBox2.Location = new System.Drawing.Point(12, 114);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(405, 76);
            this.groupBox2.TabIndex = 16;
            this.groupBox2.TabStop = false;
            // 
            // colourHexBox
            // 
            this.colourHexBox.Font = new System.Drawing.Font("Lucida Console", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.colourHexBox.Location = new System.Drawing.Point(265, 46);
            this.colourHexBox.MaxLength = 6;
            this.colourHexBox.Name = "colourHexBox";
            this.colourHexBox.Size = new System.Drawing.Size(56, 20);
            this.colourHexBox.TabIndex = 21;
            this.colourHexBox.Text = "FFFFFF";
            this.colourHexBox.TextChanged += new System.EventHandler(this.ColourTextboxTextChange);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(381, 48);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(18, 13);
            this.label5.TabIndex = 24;
            this.label5.Text = "px";
            // 
            // labelOpaque
            // 
            this.labelOpaque.AutoSize = true;
            this.labelOpaque.BackColor = System.Drawing.Color.Transparent;
            this.labelOpaque.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelOpaque.Location = new System.Drawing.Point(6, 20);
            this.labelOpaque.Name = "labelOpaque";
            this.labelOpaque.Size = new System.Drawing.Size(162, 39);
            this.labelOpaque.TabIndex = 17;
            this.labelOpaque.Text = "Screenshots are saved with an\r\nopaque background, rather\r\nthan a transparent one." +
    "";
            // 
            // labelHash
            // 
            this.labelHash.AutoSize = true;
            this.labelHash.BackColor = System.Drawing.Color.Transparent;
            this.labelHash.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelHash.Location = new System.Drawing.Point(250, 48);
            this.labelHash.Name = "labelHash";
            this.labelHash.Size = new System.Drawing.Size(14, 13);
            this.labelHash.TabIndex = 20;
            this.labelHash.Text = "#";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.labelResize);
            this.groupBox1.Controls.Add(this.windowHeight);
            this.groupBox1.Controls.Add(this.windowWidth);
            this.groupBox1.Controls.Add(this.labelX);
            this.groupBox1.Location = new System.Drawing.Point(12, 53);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(405, 55);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            // 
            // labelResize
            // 
            this.labelResize.AutoSize = true;
            this.labelResize.BackColor = System.Drawing.Color.Transparent;
            this.labelResize.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelResize.Location = new System.Drawing.Point(6, 20);
            this.labelResize.Name = "labelResize";
            this.labelResize.Size = new System.Drawing.Size(239, 26);
            this.labelResize.TabIndex = 11;
            this.labelResize.Text = "Sets the exact resolution screenshots are\r\ncaptured in, if the window supports re" +
    "sizing.";
            // 
            // colourDialog
            // 
            this.colourDialog.Color = System.Drawing.Color.White;
            this.colourDialog.FullOpen = true;
            // 
            // diskButton
            // 
            this.diskButton.AutoSize = true;
            this.diskButton.BackColor = System.Drawing.Color.Transparent;
            this.diskButton.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.diskButton.Location = new System.Drawing.Point(118, 7);
            this.diskButton.Name = "diskButton";
            this.diskButton.Size = new System.Drawing.Size(46, 17);
            this.diskButton.TabIndex = 4;
            this.diskButton.TabStop = true;
            this.diskButton.Text = "disk";
            this.diskButton.UseVisualStyleBackColor = false;
            this.diskButton.CheckedChanged += new System.EventHandler(this.DiskButtonStateChange);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(160, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(18, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "or";
            // 
            // clipboardButton
            // 
            this.clipboardButton.AutoSize = true;
            this.clipboardButton.BackColor = System.Drawing.Color.Transparent;
            this.clipboardButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clipboardButton.Location = new System.Drawing.Point(180, 7);
            this.clipboardButton.Name = "clipboardButton";
            this.clipboardButton.Size = new System.Drawing.Size(74, 17);
            this.clipboardButton.TabIndex = 6;
            this.clipboardButton.TabStop = true;
            this.clipboardButton.Text = "clipboard";
            this.clipboardButton.UseVisualStyleBackColor = false;
            this.clipboardButton.CheckedChanged += new System.EventHandler(this.ClipboardButtonStateChange);
            // 
            // mouseCheckbox
            // 
            this.mouseCheckbox.AutoSize = true;
            this.mouseCheckbox.BackColor = System.Drawing.Color.Transparent;
            this.mouseCheckbox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mouseCheckbox.Location = new System.Drawing.Point(19, 196);
            this.mouseCheckbox.Name = "mouseCheckbox";
            this.mouseCheckbox.Size = new System.Drawing.Size(145, 17);
            this.mouseCheckbox.TabIndex = 25;
            this.mouseCheckbox.Text = "Capture mouse pointer";
            this.mouseCheckbox.UseVisualStyleBackColor = false;
            this.mouseCheckbox.CheckedChanged += new System.EventHandler(this.MouseCheckboxStateChange);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.labelMouse);
            this.groupBox3.Location = new System.Drawing.Point(12, 196);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(405, 42);
            this.groupBox3.TabIndex = 26;
            this.groupBox3.TabStop = false;
            // 
            // labelMouse
            // 
            this.labelMouse.AutoSize = true;
            this.labelMouse.BackColor = System.Drawing.Color.Transparent;
            this.labelMouse.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMouse.Location = new System.Drawing.Point(6, 20);
            this.labelMouse.Name = "labelMouse";
            this.labelMouse.Size = new System.Drawing.Size(310, 13);
            this.labelMouse.TabIndex = 27;
            this.labelMouse.Text = "The system mouse pointer will be preserved in screenshots.";
            // 
            // delayCheckbox
            // 
            this.delayCheckbox.AutoSize = true;
            this.delayCheckbox.BackColor = System.Drawing.Color.Transparent;
            this.delayCheckbox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.delayCheckbox.Location = new System.Drawing.Point(19, 244);
            this.delayCheckbox.Name = "delayCheckbox";
            this.delayCheckbox.Size = new System.Drawing.Size(96, 17);
            this.delayCheckbox.TabIndex = 28;
            this.delayCheckbox.Text = "Delay capture";
            this.delayCheckbox.UseVisualStyleBackColor = false;
            this.delayCheckbox.CheckedChanged += new System.EventHandler(this.DelayCheckboxStateChange);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.labelDelay);
            this.groupBox4.Controls.Add(this.delaySeconds);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Location = new System.Drawing.Point(12, 244);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(405, 55);
            this.groupBox4.TabIndex = 29;
            this.groupBox4.TabStop = false;
            // 
            // labelDelay
            // 
            this.labelDelay.AutoSize = true;
            this.labelDelay.BackColor = System.Drawing.Color.Transparent;
            this.labelDelay.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDelay.Location = new System.Drawing.Point(6, 20);
            this.labelDelay.Name = "labelDelay";
            this.labelDelay.Size = new System.Drawing.Size(244, 13);
            this.labelDelay.TabIndex = 30;
            this.labelDelay.Text = "Adds a delay before screenshots are captured.";
            // 
            // delaySeconds
            // 
            this.delaySeconds.Location = new System.Drawing.Point(327, 25);
            this.delaySeconds.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.delaySeconds.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.delaySeconds.Name = "delaySeconds";
            this.delaySeconds.Size = new System.Drawing.Size(40, 20);
            this.delaySeconds.TabIndex = 23;
            this.delaySeconds.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(375, 27);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(24, 13);
            this.label6.TabIndex = 24;
            this.label6.Text = "sec";
            // 
            // colourDisplay
            // 
            this.colourDisplay.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.colourDisplay.Color = System.Drawing.Color.White;
            this.colourDisplay.Location = new System.Drawing.Point(327, 46);
            this.colourDisplay.Name = "colourDisplay";
            this.colourDisplay.Size = new System.Drawing.Size(72, 19);
            this.colourDisplay.TabIndex = 22;
            this.colourDisplay.Click += new System.EventHandler(this.ColourDisplayClick);
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(430, 310);
            this.Controls.Add(this.resizeCheckbox);
            this.Controls.Add(this.opaqueCheckbox);
            this.Controls.Add(this.mouseCheckbox);
            this.Controls.Add(this.delayCheckbox);
            this.Controls.Add(this.folderTextBox);
            this.Controls.Add(this.bButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.clipboardButton);
            this.Controls.Add(this.diskButton);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox4);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Settings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormClose);
            ((System.ComponentModel.ISupportInitialize)(this.windowHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.windowWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkerValue)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.delaySeconds)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion
    }
}