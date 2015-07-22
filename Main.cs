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
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace AeroShot
{
	public sealed partial class MainForm : Form
	{
		private const uint SPI_GETFONTSMOOTHING = 0x004A;
		private const uint SPI_GETFONTSMOOTHINGTYPE = 0x200A;

		Settings _settings = Settings.LoadSettingsFromRegistry();

		public MainForm()
		{
			DoubleBuffered = true;
			Icon = new Icon(typeof(MainForm), "icon.ico");
			InitializeComponent();

			folderTextBox.Text = _settings.FolderPath;
			clipboardButton.Checked = _settings.UseClipboard;
			diskButton.Checked = _settings.UseDisk;
			resizeCheckbox.Checked = _settings.ResizeDimensions.On;
            windowWidth.Value = _settings.ResizeDimensions.Value.Width;
			windowHeight.Value = _settings.ResizeDimensions.Value.Height;
            opaqueCheckbox.Checked = _settings.OpaqueBackgroundType.On;
            opaqueType.SelectedIndex = _settings.OpaqueBackgroundType.Value == ScreenshotBackgroundType.Checkerboard ? 0
                                     : _settings.OpaqueBackgroundType.Value == ScreenshotBackgroundType.SolidColor ? 1
                                     : -1;
			checkerValue.Value = _settings.CheckerboardBackgroundCheckerSize;
            opaqueColorHexBox.Text = HexColor.Encode(_settings.SolidBackgroundColor);
			aeroColorCheckbox.Checked = _settings.AeroColor.On;
            aeroColorHexBox.Text = HexColor.Encode(_settings.AeroColor.Value);
			mouseCheckbox.Checked = _settings.CaputreMouse;
			delayCheckbox.Checked = _settings.DelayCaptureDuration.On;
			delaySeconds.Value = (decimal) _settings.DelayCaptureDuration.Value.TotalSeconds;
			clearTypeCheckbox.Checked = _settings.DisableClearType;

			if (!GlassAvailable())
			{
				aeroColorCheckbox.Checked = false;
				aeroColorCheckbox.Enabled = false;
			}

			if (!ClearTypeEnabled())
			{
				clearTypeCheckbox.Checked = false;
				clearTypeCheckbox.Enabled = false;
			}

			resizeGroupBox.Enabled = resizeCheckbox.Checked;
			opaqueGroupBox.Enabled = opaqueCheckbox.Checked;
			mouseGroupBox.Enabled = mouseCheckbox.Checked;
			delayGroupBox.Enabled = delayCheckbox.Checked;
			clearTypeGroupBox.Enabled = clearTypeCheckbox.Checked;
			aeroColorGroupBox.Enabled = aeroColorCheckbox.Checked;
		}

		private static bool GlassAvailable()
		{
			if (Environment.OSVersion.Version.Major >= 6 && Environment.OSVersion.Version.Minor > 1) return false;

			bool aeroEnabled;
			WindowsApi.DwmIsCompositionEnabled(out aeroEnabled);
			return aeroEnabled;
		}

		private static bool ClearTypeEnabled()
		{
			int sv = 0;
			/* Call to systemparametersinfo to get the font smoothing value. */
			WindowsApi.SystemParametersInfo(SPI_GETFONTSMOOTHING, 0, ref sv, 0);

			int stv = 0;
			/* Call to systemparametersinfo to get the font smoothing Type value. */
			WindowsApi.SystemParametersInfo(SPI_GETFONTSMOOTHINGTYPE, 0, ref stv, 0);

			if (sv > 0 && stv == 2) //if smoothing is on, and is set to cleartype
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		private void BrowseButtonClick(object sender, EventArgs e)
		{
			if (folderSelectionDialog.ShowDialog() == DialogResult.OK)
			{
				folderTextBox.Text = folderSelectionDialog.SelectedPath;
			}
		}

		private void opaqueColorDisplayClick(object sender, EventArgs e)
		{
			if (opaqueColorDialog.ShowDialog() == DialogResult.OK)
			{
				opaqueColorDisplay.Color = opaqueColorDialog.Color;

				var hex = new StringBuilder(6);
				hex.AppendFormat("{0:X2}", opaqueColorDisplay.Color.R);
				hex.AppendFormat("{0:X2}", opaqueColorDisplay.Color.G);
				hex.AppendFormat("{0:X2}", opaqueColorDisplay.Color.B);
				opaqueColorHexBox.Text = hex.ToString();
			}
		}

		private void AeroColorDisplayClick(object sender, EventArgs e)
		{
			if (aeroColorDialog.ShowDialog() == DialogResult.OK)
			{
				aeroColorDisplay.Color = aeroColorDialog.Color;

				var hex = new StringBuilder(6);
				hex.AppendFormat("{0:X2}", aeroColorDisplay.Color.R);
				hex.AppendFormat("{0:X2}", aeroColorDisplay.Color.G);
				hex.AppendFormat("{0:X2}", aeroColorDisplay.Color.B);
				aeroColorHexBox.Text = hex.ToString();
			}
		}

		private void ResizeCheckboxStateChange(object sender, EventArgs e)
		{
			resizeGroupBox.Enabled = resizeCheckbox.Checked;
		}

		private void OpaqueCheckboxStateChange(object sender, EventArgs e)
		{
			opaqueGroupBox.Enabled = opaqueCheckbox.Checked;
		}

		private void AeroColorCheckboxStateChange(object sender, EventArgs e)
		{
			aeroColorGroupBox.Enabled = aeroColorCheckbox.Checked;
		}

		private void MouseCheckboxStateChange(object sender, EventArgs e)
		{
			mouseGroupBox.Enabled = mouseCheckbox.Checked;
		}

		private void DelayCheckboxStateChange(object sender, EventArgs e)
		{
			delayGroupBox.Enabled = delayCheckbox.Checked;
		}

		private void ClearTypeCheckboxStateChange(object sender, EventArgs e)
		{
			clearTypeGroupBox.Enabled = clearTypeCheckbox.Checked;
		}

		private void ClipboardButtonStateChange(object sender, EventArgs e)
		{
			if (!clipboardButton.Checked)
				return;
			diskButton.Checked = false;
			folderTextBox.Enabled = false;
			browseButton.Enabled = false;
		}

		private void DiskButtonStateChange(object sender, EventArgs e)
		{
			if (!diskButton.Checked)
				return;
			clipboardButton.Checked = false;
			folderTextBox.Enabled = true;
			browseButton.Enabled = true;
		}

		private void OpaqueTypeItemChange(object sender, EventArgs e)
		{
			if (opaqueType.SelectedIndex == 0)
			{
				opaqueVarLabel.Text = "Checker size:";
				checkerValue.Enabled = true;
				checkerValue.Visible = true;
				pxLabel.Visible = true;

				opaqueColorDisplay.Enabled = false;
				opaqueColorDisplay.Visible = false;
				opaqueColorHexBox.Enabled = false;
				opaqueColorHexBox.Visible = false;
				opaqueHashLabel.Visible = false;
			}
			if (opaqueType.SelectedIndex == 1)
			{
				opaqueVarLabel.Text = "Color:";
				opaqueColorDisplay.Enabled = true;
				opaqueColorDisplay.Visible = true;
				opaqueColorHexBox.Enabled = true;
				opaqueColorHexBox.Visible = true;
				opaqueHashLabel.Visible = true;

				checkerValue.Enabled = false;
				checkerValue.Visible = false;
				pxLabel.Visible = false;
			}
		}

		private void opaqueColorHexBoxTextChange(object sender, EventArgs e)
		{
			var c = new[] {
				'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C',
				'D', 'E', 'F', 'a', 'b', 'c', 'd', 'e', 'f'
			};
			foreach (char v in opaqueColorHexBox.Text)
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
					opaqueColorHexBox.Text = opaqueColorHexBox.Text.Replace(v.ToString(),
																  string.Empty);
			}

			if (opaqueColorHexBox.TextLength != 6)
				return;

			opaqueColorDisplay.Color = Color.FromArgb(Convert.ToInt32("FF" + opaqueColorHexBox.Text, 16));
			opaqueColorDialog.Color = Color.FromArgb(Convert.ToInt32("FF" + opaqueColorHexBox.Text, 16));
		}

		private void AeroColorTextboxTextChange(object sender, EventArgs e)
		{
			var c = new[] {
				'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C',
				'D', 'E', 'F', 'a', 'b', 'c', 'd', 'e', 'f'
			};
			foreach (char v in aeroColorHexBox.Text)
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
					aeroColorHexBox.Text = aeroColorHexBox.Text.Replace(v.ToString(),
																  string.Empty);
			}

			if (opaqueColorHexBox.TextLength != 6)
				return;

			aeroColorDisplay.Color = Color.FromArgb(Convert.ToInt32("FF" + aeroColorHexBox.Text, 16));
			aeroColorDialog.Color = Color.FromArgb(Convert.ToInt32("FF" + aeroColorHexBox.Text, 16));
		}

		private void OkButtonClick(object sender, EventArgs e)
		{
            Settings.SaveSettingsToRegistry(new Settings
            {
                UseClipboard         = clipboardButton.Checked,
                FolderPath           = folderTextBox.Text,
                ResizeDimensions     = Switch.Create(resizeCheckbox.Checked,
                                                     new Size((int) windowWidth.Value,
                                                              (int) windowHeight.Value)),
                OpaqueBackgroundType = Switch.Create(opaqueCheckbox.Checked,
                                                     opaqueType.SelectedIndex == 1
                                                     ? ScreenshotBackgroundType.SolidColor
                                                     : ScreenshotBackgroundType.Checkerboard),
                SolidBackgroundColor = opaqueColorDialog.Color,
                AeroColor            = Switch.Create(aeroColorCheckbox.Checked, aeroColorDialog.Color),
                CaputreMouse         = mouseCheckbox.Checked,
                DisableClearType     = clearTypeCheckbox.Checked,
                DelayCaptureDuration = Switch.Create(delayCheckbox.Checked, TimeSpan.FromSeconds((int) delaySeconds.Value)),
            });

			this.Close();
		}

		private void CancelButtonClick(object sender, EventArgs e)
		{
			this.Close();
		}
	}

	public class ColorDisplay : UserControl
	{
		private readonly SolidBrush _border =
			new SolidBrush(SystemColors.Window);

		private SolidBrush _brush;
		private Color _color = Color.Black;

		public Color Color
		{
			get { return _color; }
			set
			{
				_color = value;
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
				_brush = new SolidBrush(_color);
			else
			{
				var grayScale =
					(byte)
					(((_color.R * .3) + (_color.G * .59) + (_color.B * .11)));
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