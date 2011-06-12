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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;

namespace AeroShot {
	public sealed partial class MainForm : Form {
		private const int GWL_STYLE = -16;
		private const int MOD_WIN = 0x0008;
		private const int WM_HOTKEY = 0x0312;
		private const int WM_DWMCOMPOSITIONCHANGED = 0x031E;
		private const long WS_CAPTION = 0x00C00000L;
		private const long WS_VISIBLE = 0x10000000L;
		private const long WS_SIZEBOX = 0x00040000L;
		private const uint SWP_SHOWWINDOW = 0x0040;
		private static int windowId;
		private static Thread worker;
		private static readonly SolidBrush brush = new SolidBrush(Color.FromArgb(201, 199, 200));
		private static bool DwmComposited;
		private readonly List<IntPtr> handleList = new List<IntPtr>();
		private readonly RegistryKey registryKey;
		private CallBackPtr callBackPtr;

		public MainForm() {
			Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
			InitializeComponent();

			WindowsApi.DwmIsCompositionEnabled(ref DwmComposited);

			if (DwmComposited) {
				ssButton.Location = new Point(ssButton.Location.X, 248);
				var margin = new WindowsMargins(0, 0, 0, 32);
				WindowsApi.DwmExtendFrameIntoClientArea(Handle, ref margin);
			}

			windowId = GetHashCode();
			WindowsApi.RegisterHotKey(Handle, windowId, MOD_WIN, (int) Keys.PrintScreen);

			object value;
			registryKey = Registry.CurrentUser.CreateSubKey(@"Software\AeroShot");
			if ((value = registryKey.GetValue("LastPath")) != null && value.GetType() == (typeof (string)))
				folderTextBox.Text = (string) value;
			else
				folderTextBox.Text = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

			if ((value = registryKey.GetValue("WindowSize")) != null && value.GetType() == (typeof (long))) {
				var b = new byte[8];
				for (var i = 0; i < 8; i++) b[i] = (byte) (((long) value >> (i*8)) & 0xff);
				resizeCheckBox.Checked = (b[0] & 1) == 1 ? true : false;
				windowWidth.Value = b[1] << 16 | b[2] << 8 | b[3];
				windowHeight.Value = b[4] << 16 | b[5] << 8 | b[6];
			}

			if ((value = registryKey.GetValue("Opaque")) != null && value.GetType() == (typeof (long))) {
				var b = new byte[8];
				for (var i = 0; i < 8; i++) b[i] = (byte) (((long) value >> (i*8)) & 0xff);
				opaqueCheckBox.Checked = (b[0] & 1) == 1 ? true : false;
				if ((b[0] & 2) == 2)
					opaqueType.SelectedIndex = 0;
				if ((b[0] & 4) == 4)
					opaqueType.SelectedIndex = 1;

				checkerValue.Value = b[1] + 2;

				var hex = new StringBuilder(6);
				hex.AppendFormat("{0:X2}", b[2]);
				hex.AppendFormat("{0:X2}", b[3]);
				hex.AppendFormat("{0:X2}", b[4]);
				colorHexBox.Text = hex.ToString();
			}
			else opaqueType.SelectedIndex = 0;

			groupBox1.Enabled = resizeCheckBox.Checked;
			groupBox2.Enabled = opaqueCheckBox.Checked;
		}

		protected override void WndProc(ref Message m) {
			base.WndProc(ref m);

			if (m.Msg == WM_HOTKEY) {
				var f = folderTextBox.Text;
				var o = opaqueCheckBox.Checked ? true : false;
				var s = (int) (opaqueType.SelectedIndex == 0 ? checkerValue.Value : 0);
				var c = colorDialog.Color;
				worker = new Thread(() => TakeScreenshot(WindowsApi.GetForegroundWindow(), f, o, s, c))
				         	{ApartmentState = ApartmentState.STA, IsBackground = true};
				worker.Start();
			}
			if (m.Msg == WM_DWMCOMPOSITIONCHANGED) {
				WindowsApi.DwmIsCompositionEnabled(ref DwmComposited);

				if (DwmComposited) {
					ssButton.Location = new Point(ssButton.Location.X, 248);
					var margin = new WindowsMargins(0, 0, 0, 32);
					WindowsApi.DwmExtendFrameIntoClientArea(Handle, ref margin);
				}
				else ssButton.Location = new Point(ssButton.Location.X, 240);
			}
		}

		private void ssButton_Click(object sender, EventArgs e) {
			var h = handleList[windowList.SelectedIndex];
			var f = folderTextBox.Text;
			var o = opaqueCheckBox.Checked ? true : false;
			var s = (int) (opaqueType.SelectedIndex == 0 ? checkerValue.Value : 0);
			var c = colorDialog.Color;

			worker = new Thread(() => TakeScreenshot(h, f, o, s, c)) {ApartmentState = ApartmentState.STA, IsBackground = true};
			worker.Start();
		}

		private void rButton_Click(object sender, EventArgs e) {
			handleList.Clear();
			windowList.Items.Clear();

			callBackPtr = new CallBackPtr(ListWindows);
			WindowsApi.EnumWindows(callBackPtr, (IntPtr) 0);
			windowList.SelectedIndex = 0;
		}

		private void bButton_Click(object sender, EventArgs e) {
			if (folderSelection.ShowDialog() == DialogResult.OK) folderTextBox.Text = folderSelection.SelectedPath;
		}

		private void colorDisplay_Click(object sender, EventArgs e) {
			if (colorDialog.ShowDialog() == DialogResult.OK) {
				colorDisplay.Color = colorDialog.Color;

				var hex = new StringBuilder(6);
				hex.AppendFormat("{0:X2}", colorDisplay.Color.R);
				hex.AppendFormat("{0:X2}", colorDisplay.Color.G);
				hex.AppendFormat("{0:X2}", colorDisplay.Color.B);
				colorHexBox.Text = hex.ToString();
			}
		}

		private void resizeCheckBox_CheckedChanged(object sender, EventArgs e) {
			groupBox1.Enabled = resizeCheckBox.Checked;
		}

		private void opaqueCheckBox_CheckedChanged(object sender, EventArgs e) {
			groupBox2.Enabled = opaqueCheckBox.Checked;
		}

		private void opaqueType_SelectedIndexChanged(object sender, EventArgs e) {
			if (opaqueType.SelectedIndex == 0) {
				label6.Text = "Checker size:";
				checkerValue.Enabled = true;
				checkerValue.Visible = true;
				label7.Visible = true;

				colorDisplay.Enabled = false;
				colorDisplay.Visible = false;
				colorHexBox.Enabled = false;
				colorHexBox.Visible = false;
				label8.Visible = false;
			}
			if (opaqueType.SelectedIndex == 1) {
				label6.Text = "Color:";
				colorDisplay.Enabled = true;
				colorDisplay.Visible = true;
				colorHexBox.Enabled = true;
				colorHexBox.Visible = true;
				label8.Visible = true;

				checkerValue.Enabled = false;
				checkerValue.Visible = false;
				label7.Visible = false;
			}
		}

		private void colorHexBox_TextChanged(object sender, EventArgs e) {
			var c = new[]
			        	{
			        		'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'a', 'b', 'c', 'd', 'e',
			        		'f'
			        	};
			foreach (var v in colorHexBox.Text) {
				var b = false;
				foreach (var v1 in c)
					if (v == v1) {
						b = true;
						break;
					}
				if (!b)
					colorHexBox.Text = colorHexBox.Text.Replace(v.ToString(), string.Empty);
			}

			if (colorHexBox.TextLength != 6) return;

			colorDisplay.Color = Color.FromArgb(Convert.ToInt32("FF" + colorHexBox.Text, 16));
			colorDialog.Color = Color.FromArgb(Convert.ToInt32("FF" + colorHexBox.Text, 16));
		}

		private void AeroShot_Shown(object sender, EventArgs e) {
			rButton_Click(null, null);
		}

		private void AeroShot_Closing(object sender, FormClosingEventArgs e) {
			WindowsApi.UnregisterHotKey(Handle, windowId);
			registryKey.SetValue("LastPath", folderTextBox.Text);

			// Save resizing settings in an 8-byte long
			var b = new byte[8];
			b[0] = (byte) (resizeCheckBox.Checked ? 1 : 0);

			b[3] = (byte) ((int) windowWidth.Value & 0xff);
			b[2] = (byte) (((int) windowWidth.Value >> 8) & 0xff);
			b[1] = (byte) (((int) windowWidth.Value >> 16) & 0xff);

			b[6] = (byte) ((int) windowHeight.Value & 0xff);
			b[5] = (byte) (((int) windowHeight.Value >> 8) & 0xff);
			b[4] = (byte) (((int) windowHeight.Value >> 16) & 0xff);

			var data = BitConverter.ToInt64(b, 0);
			registryKey.SetValue("WindowSize", data, RegistryValueKind.QWord);

			// Save background color settings in an 8-byte long
			b = new byte[8];
			b[0] = (byte) (opaqueCheckBox.Checked ? 1 : 0);
			b[0] += (byte) Math.Pow(2, opaqueType.SelectedIndex + 1);

			b[1] = (byte) (checkerValue.Value - 2);

			b[2] = colorDialog.Color.R;
			b[3] = colorDialog.Color.G;
			b[4] = colorDialog.Color.B;

			data = BitConverter.ToInt64(b, 0);
			registryKey.SetValue("Opaque", data, RegistryValueKind.QWord);
		}

		protected override void OnPaintBackground(PaintEventArgs e) {
			var r = e.ClipRectangle;
			e.Graphics.FillRectangle(new SolidBrush(BackColor), r);

			if (!DwmComposited) return;

			r.Y = r.Bottom - 32;
			r.Height = 32;
			e.Graphics.FillRectangle(brush, r);
		}

		private bool ListWindows(IntPtr hWnd, int lParam) {
			if ((WindowsApi.GetWindowLong(hWnd, GWL_STYLE) & WS_VISIBLE) == WS_VISIBLE &&
			    (WindowsApi.GetWindowLong(hWnd, GWL_STYLE) & WS_CAPTION) == WS_CAPTION) {
				var length = WindowsApi.GetWindowTextLength(hWnd);
				var sb = new StringBuilder(length + 1);
				WindowsApi.GetWindowText(hWnd, sb, sb.Capacity);

				handleList.Add(hWnd);
				windowList.Items.Add(sb.ToString());
			}
			return true;
		}

		private void TakeScreenshot(IntPtr hWnd, string folder, bool opaque, int checkerSize, Color color) {
			if (Directory.Exists(folder))
				try {
					if (WindowsApi.IsIconic(hWnd)) {
						WindowsApi.ShowWindow(hWnd, 1); // Show window if minimized
						Thread.Sleep(300); // Wait for window to be restored
					}
					WindowsApi.SetForegroundWindow(hWnd);

					var r = new WindowsRect(0);
					if (resizeCheckBox.Checked) {
						ResizeWindow(hWnd, (int) windowWidth.Value, (int) windowHeight.Value, out r);
						Thread.Sleep(100);
					}

					var length = WindowsApi.GetWindowTextLength(hWnd);
					var sb = new StringBuilder(length + 1);
					WindowsApi.GetWindowText(hWnd, sb, sb.Capacity);

					var name = sb.ToString();

					foreach (var inv in Path.GetInvalidFileNameChars())
						name = name.Replace(inv.ToString(), string.Empty);

					name = name.Trim();
					if (name == string.Empty)
						name = "AeroShot";

					var path = folder + Path.DirectorySeparatorChar + name + ".png";

					if (File.Exists(path))
						for (var i = 1; i < 9999; i++) {
							path = folder + Path.DirectorySeparatorChar + name + " " + i + ".png";
							if (!File.Exists(path))
								break;
						}
					var s = Screenshot.GetScreenshot(hWnd, opaque, checkerSize, color);
					if (s == null)
						MessageBox.Show("The screenshot taken was blank, it will not be saved.", "Warning", MessageBoxButtons.OK,
						                MessageBoxIcon.Warning);
					else {
						s.Save(path, ImageFormat.Png);
						s.Dispose();
					}

					if (resizeCheckBox.Checked)
						if ((WindowsApi.GetWindowLong(hWnd, GWL_STYLE) & WS_SIZEBOX) == WS_SIZEBOX)
							WindowsApi.SetWindowPos(hWnd, (IntPtr) 0, r.Left, r.Top, r.Right - r.Left, r.Bottom - r.Top, SWP_SHOWWINDOW);
				}
				catch (Exception) {
					MessageBox.Show(
						"An error occurred while trying to take a screenshot.\r\n\r\nPlease make sure you have selected a valid window.",
						"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			else
				MessageBox.Show("Invalid directory chosen.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		private static void ResizeWindow(IntPtr hWnd, int windowWidth, int windowHeight, out WindowsRect oldRect) {
			oldRect = new WindowsRect(0);
			if ((WindowsApi.GetWindowLong(hWnd, GWL_STYLE) & WS_SIZEBOX) != WS_SIZEBOX) return;

			WindowsApi.ShowWindow(hWnd, 1);

			var r = new WindowsRect();
			WindowsApi.GetWindowRect(hWnd, ref r);
			var f = Screenshot.GetScreenshot(hWnd, false, 0, Color.Black);
			oldRect = r;

			WindowsApi.SetWindowPos(hWnd, (IntPtr) 0, r.Left, r.Top, windowWidth - (f.Width - (r.Right - r.Left)),
			                        windowHeight - (f.Height - (r.Bottom - r.Top)), SWP_SHOWWINDOW);

			f.Dispose();
		}
	}

	public class ColorDisplay : UserControl {
		private readonly SolidBrush _border = new SolidBrush(SystemColors.ControlDark);
		private SolidBrush _brush;
		private Color _color = Color.Black;

		public Color Color {
			get { return _color; }
			set {
				if (value.R == 201 && value.G == 199 && value.B == 200)
					_color = Color.FromArgb(200, 200, 200); // Hacky fix for the main form's transparency key
				else
					_color = value;
				Refresh();
			}
		}

		protected override void OnPaintBackground(PaintEventArgs e) {}

		protected override void OnPaint(PaintEventArgs e) {
			var rect = e.ClipRectangle;
			rect.X = 2;
			rect.Y = 2;
			rect.Width -= 4;
			rect.Height -= 4;

			if (Enabled) _brush = new SolidBrush(_color);
			else {
				var grayScale = (byte) (((_color.R*.3) + (_color.G*.59) + (_color.B*.11)));
				_brush = new SolidBrush(ControlPaint.Light(Color.FromArgb(grayScale, grayScale, grayScale)));
			}
			e.Graphics.FillRectangle(_border, e.ClipRectangle);
			e.Graphics.FillRectangle(_brush, rect);
		}
	}
}