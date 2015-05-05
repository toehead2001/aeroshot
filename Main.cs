/*  AeroShot - Transparent screenshot utility for Windows
	Copyright (C) 2015 toe_head2001
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

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace AeroShot
{
	public sealed partial class MainForm : Form
	{
		private readonly RegistryKey _registryKey;
        Settings _settings = new Settings();

		public MainForm()
		{
			DoubleBuffered = true;
			InitializeComponent();

            folderTextBox.Text = _settings.folderTextBox;
            clipboardButton.Checked = _settings.clipboardButton;
            diskButton.Checked = _settings.diskButton;
            resizeCheckbox.Checked = _settings.resizeCheckbox;
            windowWidth.Value = _settings.windowWidth;
            windowHeight.Value = _settings.windowHeight;
            opaqueCheckbox.Checked = _settings.opaqueCheckbox;
            opaqueType.SelectedIndex = _settings.opaqueType;
            checkerValue.Value = _settings.checkerValue;
            colourHexBox.Text = _settings.colourHexBox;
            mouseCheckbox.Checked = _settings.mouseCheckbox;
            delayCheckbox.Checked = _settings.delayCheckbox;
            delaySeconds.Value = _settings.delaySeconds;

			groupBox1.Enabled = resizeCheckbox.Checked;
			groupBox2.Enabled = opaqueCheckbox.Checked;
			groupBox3.Enabled = mouseCheckbox.Checked;
			groupBox4.Enabled = delayCheckbox.Checked;

            _registryKey = Registry.CurrentUser.CreateSubKey(@"Software\AeroShot");
		}

		private void BrowseButtonClick(object sender, EventArgs e)
		{
			if (folderSelection.ShowDialog() == DialogResult.OK)
            {
                folderTextBox.Text = folderSelection.SelectedPath;
            }
		}

		private void ColourDisplayClick(object sender, EventArgs e)
		{
			if (colourDialog.ShowDialog() == DialogResult.OK)
			{
				colourDisplay.Color = colourDialog.Color;

				var hex = new StringBuilder(6);
				hex.AppendFormat("{0:X2}", colourDisplay.Color.R);
				hex.AppendFormat("{0:X2}", colourDisplay.Color.G);
				hex.AppendFormat("{0:X2}", colourDisplay.Color.B);
				colourHexBox.Text = hex.ToString();
			}
		}

		private void ResizeCheckboxStateChange(object sender, EventArgs e)
		{
			groupBox1.Enabled = resizeCheckbox.Checked;
		}

		private void OpaqueCheckboxStateChange(object sender, EventArgs e)
		{
			groupBox2.Enabled = opaqueCheckbox.Checked;
		}

		private void MouseCheckboxStateChange(object sender, EventArgs e)
		{
			groupBox3.Enabled = mouseCheckbox.Checked;
		}

		private void DelayCheckboxStateChange(object sender, EventArgs e)
		{
			groupBox4.Enabled = delayCheckbox.Checked;
		}

		private void ClipboardButtonStateChange(object sender, EventArgs e)
		{
			if (!clipboardButton.Checked)
				return;
			diskButton.Checked = false;
			folderTextBox.Enabled = false;
			bButton.Enabled = false;
		}

		private void DiskButtonStateChange(object sender, EventArgs e)
		{
			if (!diskButton.Checked)
				return;
			clipboardButton.Checked = false;
			folderTextBox.Enabled = true;
			bButton.Enabled = true;
		}

		private void OpaqueTypeItemChange(object sender, EventArgs e)
		{
			if (opaqueType.SelectedIndex == 0)
			{
				label4.Text = "Checker size:";
				checkerValue.Enabled = true;
				checkerValue.Visible = true;
				label5.Visible = true;

				colourDisplay.Enabled = false;
				colourDisplay.Visible = false;
				colourHexBox.Enabled = false;
				colourHexBox.Visible = false;
				labelHash.Visible = false;
			}
			if (opaqueType.SelectedIndex == 1)
			{
				label4.Text = "Colour:";
				colourDisplay.Enabled = true;
				colourDisplay.Visible = true;
				colourHexBox.Enabled = true;
				colourHexBox.Visible = true;
				labelHash.Visible = true;

				checkerValue.Enabled = false;
				checkerValue.Visible = false;
				label5.Visible = false;
			}
		}

		private void ColourTextboxTextChange(object sender, EventArgs e)
		{
			var c = new[] {
				'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C',
				'D', 'E', 'F', 'a', 'b', 'c', 'd', 'e', 'f'
			};
			foreach (char v in colourHexBox.Text)
			{
				bool b = false;
				foreach (char v1 in c)
				{
					if (v == v1)
					{
						b = true;
						break;
					}
				}
				if (!b)
					colourHexBox.Text = colourHexBox.Text.Replace(v.ToString(),
																  string.Empty);
			}

			if (colourHexBox.TextLength != 6)
				return;

			colourDisplay.Color = Color.FromArgb(Convert.ToInt32("FF" + colourHexBox.Text, 16));
			colourDialog.Color = Color.FromArgb(Convert.ToInt32("FF" + colourHexBox.Text, 16));
		}

        private void OkButtonClick(object sender, EventArgs e)
		{
			if (clipboardButton.Checked)
				_registryKey.SetValue("LastPath", "*" + folderTextBox.Text);
			else
				_registryKey.SetValue("LastPath", folderTextBox.Text);

			// Save resizing settings in an 8-byte long
			var b = new byte[8];
			b[0] = (byte)(resizeCheckbox.Checked ? 1 : 0);

			b[3] = (byte)((int)windowWidth.Value & 0xff);
			b[2] = (byte)(((int)windowWidth.Value >> 8) & 0xff);
			b[1] = (byte)(((int)windowWidth.Value >> 16) & 0xff);

			b[6] = (byte)((int)windowHeight.Value & 0xff);
			b[5] = (byte)(((int)windowHeight.Value >> 8) & 0xff);
			b[4] = (byte)(((int)windowHeight.Value >> 16) & 0xff);

			long data = BitConverter.ToInt64(b, 0);
			_registryKey.SetValue("WindowSize", data, RegistryValueKind.QWord);

			// Save background colour settings in an 8-byte long
			b = new byte[8];
			b[0] = (byte)(opaqueCheckbox.Checked ? 1 : 0);
			b[0] += (byte)Math.Pow(2, opaqueType.SelectedIndex + 1);

			b[1] = (byte)(checkerValue.Value - 2);

			b[2] = colourDialog.Color.R;
			b[3] = colourDialog.Color.G;
			b[4] = colourDialog.Color.B;

			data = BitConverter.ToInt64(b, 0);
			_registryKey.SetValue("Opaque", data, RegistryValueKind.QWord);

			_registryKey.SetValue("CapturePointer",
								  mouseCheckbox.Checked ? 1 : 0,
								  RegistryValueKind.DWord);

			// Save delay settings in an 8-byte long
			b = new byte[8];
			b[0] = (byte)(delayCheckbox.Checked ? 1 : 0);

			b[1] = (byte)(((int)delaySeconds.Value) & 0xff);

			data = BitConverter.ToInt64(b, 0);
			_registryKey.SetValue("Delay", data, RegistryValueKind.QWord);

            this.Close();
		}

        private void CancelButtonClick(object sender, EventArgs e)
        {
            this.Close();
        }
	}

	public class ColourDisplay : UserControl
	{
		private readonly SolidBrush _border =
			new SolidBrush(SystemColors.Window);

		private SolidBrush _brush;
		private Color _colour = Color.Black;

		public Color Color
		{
			get { return _colour; }
			set
			{
				_colour = value;
				Invalidate();
				Update();
			}
		}

		protected override void OnPaintBackground(PaintEventArgs e) { }

		protected override void OnPaint(PaintEventArgs e)
		{
			Rectangle rect = e.ClipRectangle;
			rect.X = 1;
			rect.Y = 1;
			rect.Width -= 2;
			rect.Height -= 2;

			if (Enabled)
				_brush = new SolidBrush(_colour);
			else
			{
				var grayScale =
					(byte)
					(((_colour.R * .3) + (_colour.G * .59) + (_colour.B * .11)));
				_brush =
					new SolidBrush(
						ControlPaint.Light(Color.FromArgb(grayScale, grayScale,
															grayScale)));
			}
			e.Graphics.FillRectangle(_border, e.ClipRectangle);
			e.Graphics.FillRectangle(_brush, rect);
		}
	}
}