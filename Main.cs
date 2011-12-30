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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
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
		private static bool DwmComposited;
		private readonly List<IntPtr> handleList = new List<IntPtr>();
		private readonly RegistryKey registryKey;
		private CallBackPtr callBackPtr;
		private Image ssButtonImage;

		public MainForm() {
			DoubleBuffered = true;
			Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
			InitializeComponent();

			if (WindowsApi.DwmIsCompositionEnabled(ref DwmComposited) == 0)
				if (DwmComposited) {
					ssButton.Location = new Point(ssButton.Location.X, 310);
					var margin = new WindowsMargins(0, 0, 0, 35);
					WindowsApi.DwmExtendFrameIntoClientArea(Handle, ref margin);
				}

			windowId = GetHashCode();
			WindowsApi.RegisterHotKey(Handle, windowId, MOD_WIN, (int) Keys.PrintScreen);

			object value;
			registryKey = Registry.CurrentUser.CreateSubKey(@"Software\AeroShot");
			if ((value = registryKey.GetValue("LastPath")) != null && value.GetType() == (typeof(string))) {
				if (((string)value).Substring(0, 1) == "*") {
					folderTextBox.Text = ((string) value).Substring(1);
					clipboardButton.Checked = true;
				} else {
					folderTextBox.Text = (string)value;
					diskButton.Checked = true;
				}
			} else
				folderTextBox.Text = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

			if ((value = registryKey.GetValue("WindowSize")) != null && value.GetType() == (typeof (long))) {
				var b = new byte[8];
				for (var i = 0; i < 8; i++) b[i] = (byte) (((long) value >> (i*8)) & 0xff);
				resizeCheckbox.Checked = (b[0] & 1) == 1;
				windowWidth.Value = b[1] << 16 | b[2] << 8 | b[3];
				windowHeight.Value = b[4] << 16 | b[5] << 8 | b[6];
			}

			if ((value = registryKey.GetValue("Opaque")) != null && value.GetType() == (typeof (long))) {
				var b = new byte[8];
				for (var i = 0; i < 8; i++) b[i] = (byte) (((long) value >> (i*8)) & 0xff);
				opaqueCheckbox.Checked = (b[0] & 1) == 1;
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
			} else opaqueType.SelectedIndex = 0;
			if ((value = registryKey.GetValue("CapturePointer")) != null && value.GetType() == (typeof(int))) {
				mouseCheckbox.Checked = ((int)value & 1) == 1;
			}

			groupBox1.Enabled = resizeCheckbox.Checked;
			groupBox2.Enabled = opaqueCheckbox.Checked;
			groupBox3.Enabled = mouseCheckbox.Checked;

			ssButtonImage = Resources.capture;
		}

		private void ScreenshotButtonPlaceholderMouseEnter(object sender, EventArgs e) {
			ssButtonImage = Resources.capture_hover;
			Invalidate();
			Update();
		}

		private void ScreenshotButtonPlaceholderMouseDown(object sender, MouseEventArgs e) {
			ssButtonImage = Resources.capture_press;
			Invalidate();
			Update();
		}

		private void ScreenshotButtonPlaceholderMouseLeave(object sender, EventArgs e) {
			ssButtonImage = Resources.capture;
			Invalidate();
			Update();
		}

		private void ScreenshotButtonPlaceholderMouseUp(object sender, MouseEventArgs e) {
			if (e != null && (e.X < 0 || e.Y < 0 || e.X > ssButton.Size.Width || e.Y > ssButton.Size.Height)) {
				ssButtonImage = Resources.capture;
				Invalidate();
				Update();
				return;
			}
			ssButtonImage = Resources.capture_hover;
			Invalidate();
			Update();

			var h = handleList[windowList.SelectedIndex];
			var f = folderTextBox.Text;
			var b = clipboardButton.Checked;
			var o = opaqueCheckbox.Checked;
			var s = (int)(opaqueType.SelectedIndex == 0 ? checkerValue.Value : 0);
			var c = colorDialog.Color;

			worker = new Thread(() => TakeScreenshot(h, f, b, false, o, s, c)) { IsBackground = true };
			worker.SetApartmentState(ApartmentState.STA);
			worker.Start();
		}

		private void ScreenshotButtonPlaceholderKeyDown(object sender, KeyEventArgs e) {
			if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Enter)
				ScreenshotButtonPlaceholderMouseDown(null, null);
		}

		private void ScreenshotButtonPlaceholderKeyUp(object sender, KeyEventArgs e) {
			if(e.KeyCode == Keys.Space || e.KeyCode == Keys.Enter)
				ScreenshotButtonPlaceholderMouseUp(null, null);
		}

		private void ScreenshotButtonPlaceholderEnter(object sender, EventArgs e) {
			ScreenshotButtonPlaceholderMouseEnter(null, null);
		}

		private void ScreenshotButtonPlaceholderLeave(object sender, EventArgs e) {
			ScreenshotButtonPlaceholderMouseLeave(null, null);
		}

		private void RefreshButtonClick(object sender, EventArgs e) {
			handleList.Clear();
			windowList.Items.Clear();

			callBackPtr = ListWindows;
			WindowsApi.EnumWindows(callBackPtr, (IntPtr) 0);
			windowList.SelectedIndex = 0;
		}

		private void BrowseButtonClick(object sender, EventArgs e) {
			if (folderSelection.ShowDialog() == DialogResult.OK) folderTextBox.Text = folderSelection.SelectedPath;
		}

		private void ColorDisplayClick(object sender, EventArgs e) {
			if (colorDialog.ShowDialog() == DialogResult.OK) {
				colorDisplay.Color = colorDialog.Color;

				var hex = new StringBuilder(6);
				hex.AppendFormat("{0:X2}", colorDisplay.Color.R);
				hex.AppendFormat("{0:X2}", colorDisplay.Color.G);
				hex.AppendFormat("{0:X2}", colorDisplay.Color.B);
				colorHexBox.Text = hex.ToString();
			}
		}

		private void ResizeCheckboxStateChange(object sender, EventArgs e) {
			groupBox1.Enabled = resizeCheckbox.Checked;
		}

		private void OpaqueCheckboxStateChange(object sender, EventArgs e) {
			groupBox2.Enabled = opaqueCheckbox.Checked;
		}
		private void MouseCheckboxStateChange(object sender, EventArgs e) {
			groupBox3.Enabled = mouseCheckbox.Checked;
		}
		private void ClipboardButtonStateChange(object sender, EventArgs e) {
			if (!clipboardButton.Checked) return;
			diskButton.Checked = false;
			folderTextBox.Enabled = false;
			bButton.Enabled = false;
		}

		private void DiskButtonStateChange(object sender, EventArgs e) {
			if (!diskButton.Checked) return;
			clipboardButton.Checked = false;
			folderTextBox.Enabled = true;
			bButton.Enabled = true;
		}
		private void OpaqueTypeItemChange(object sender, EventArgs e) {
			if (opaqueType.SelectedIndex == 0) {
				label4.Text = "Checker size:";
				checkerValue.Enabled = true;
				checkerValue.Visible = true;
				label5.Visible = true;

				colorDisplay.Enabled = false;
				colorDisplay.Visible = false;
				colorHexBox.Enabled = false;
				colorHexBox.Visible = false;
				labelHash.Visible = false;
			}
			if (opaqueType.SelectedIndex == 1) {
				label4.Text = "Color:";
				colorDisplay.Enabled = true;
				colorDisplay.Visible = true;
				colorHexBox.Enabled = true;
				colorHexBox.Visible = true;
				labelHash.Visible = true;

				checkerValue.Enabled = false;
				checkerValue.Visible = false;
				label5.Visible = false;
			}
		}

		private void ColorTextboxTextChange(object sender, EventArgs e) {
			var c = new[] {
			              	'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'a', 'b', 'c', 'd', 'e', 'f'
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

		private void FormShown(object sender, EventArgs e) {
			RefreshButtonClick(null, null);
		}

		private void FormClose(object sender, FormClosingEventArgs e) {
			WindowsApi.UnregisterHotKey(Handle, windowId);
			if(clipboardButton.Checked)
				registryKey.SetValue("LastPath", "*" + folderTextBox.Text);	
			else
				registryKey.SetValue("LastPath",folderTextBox.Text);

			// Save resizing settings in an 8-byte long
			var b = new byte[8];
			b[0] = (byte) (resizeCheckbox.Checked ? 1 : 0);

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
			b[0] = (byte) (opaqueCheckbox.Checked ? 1 : 0);
			b[0] += (byte) Math.Pow(2, opaqueType.SelectedIndex + 1);

			b[1] = (byte) (checkerValue.Value - 2);

			b[2] = colorDialog.Color.R;
			b[3] = colorDialog.Color.G;
			b[4] = colorDialog.Color.B;

			data = BitConverter.ToInt64(b, 0);
			registryKey.SetValue("Opaque", data, RegistryValueKind.QWord);

			registryKey.SetValue("CapturePointer", mouseCheckbox.Checked ? 1 : 0, RegistryValueKind.DWord);
		}
		private void FormSizeChange(object sender, EventArgs e) {
			if (WindowState == FormWindowState.Normal) {
				Invalidate();
				Update();
			}
		}

		private void OnPaint(object sender, PaintEventArgs e) {
			if (DwmComposited) {
				var rc = new Rectangle(0, ClientSize.Height - 35, ClientSize.Width, 35);
				var destdc = e.Graphics.GetHdc();
				var memdc = WindowsApi.CreateCompatibleDC(destdc);
				var bitmapOld = IntPtr.Zero;
				var dib = new BitmapInfo {
				                         	biHeight = -(rc.Bottom - rc.Top),
				                         	biWidth = rc.Right - rc.Left,
				                         	biPlanes = 1,
				                         	biSize = Marshal.SizeOf(typeof (BitmapInfo)),
				                         	biBitCount = 32,
				                         	biCompression = 0
				                         };
				if (WindowsApi.SaveDC(memdc) != 0) {
					IntPtr tmp;
					var bitmap = WindowsApi.CreateDIBSection(memdc, ref dib, 0, out tmp, IntPtr.Zero, 0);
					if (!(bitmap == IntPtr.Zero)) {
						bitmapOld = WindowsApi.SelectObject(memdc, bitmap);
						WindowsApi.BitBlt(destdc, rc.Left, rc.Top, rc.Right - rc.Left, rc.Bottom - rc.Top, memdc, 0, 0,
						                  CopyPixelOperation.SourceCopy);
					}
					WindowsApi.SelectObject(memdc, bitmapOld);
					WindowsApi.DeleteObject(bitmap);
					WindowsApi.DeleteDC(memdc);
				}
				e.Graphics.ReleaseHdc(destdc);
			}
			e.Graphics.DrawImage(ssButtonImage, new Rectangle(ssButton.Location, ssButton.Size));
		}

		protected override void WndProc(ref Message m) {
			base.WndProc(ref m);

			if (m.Msg == WM_HOTKEY) {
				var f = folderTextBox.Text;
				var b = clipboardButton.Checked;
				var p = mouseCheckbox.Checked;
				var o = opaqueCheckbox.Checked;
				var s = (int)(opaqueType.SelectedIndex == 0 ? checkerValue.Value : 0);
				var c = colorDialog.Color;
				worker = new Thread(() => TakeScreenshot(WindowsApi.GetForegroundWindow(), f, b, p, o, s, c)) { IsBackground = true };
				worker.SetApartmentState(ApartmentState.STA);
				worker.Start();
			}
			if (m.Msg == WM_DWMCOMPOSITIONCHANGED) {
				WindowsApi.DwmIsCompositionEnabled(ref DwmComposited);

				if (DwmComposited) {
					ssButton.Location = new Point(ssButton.Location.X, 310);
					var margin = new WindowsMargins(0, 0, 0, 35);
					WindowsApi.DwmExtendFrameIntoClientArea(Handle, ref margin);
				} else ssButton.Location = new Point(ssButton.Location.X, 305);
			}
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

		private void TakeScreenshot(IntPtr hWnd, string folder, bool clipboard, bool cursor, bool opaque, int checkerSize, Color color) {
			var start = WindowsApi.FindWindow("Button", "Start");
			var taskbar = WindowsApi.FindWindow("Shell_TrayWnd", null);
			if (Directory.Exists(folder))
				try {
					// Hide the taskbar, just incase it gets in the way
					if (hWnd != start && hWnd != taskbar) {
						WindowsApi.ShowWindow(start, 0);
						WindowsApi.ShowWindow(taskbar, 0);
						Application.DoEvents();
					}
					if (WindowsApi.IsIconic(hWnd)) {
						WindowsApi.ShowWindow(hWnd, 1);
						Thread.Sleep(300); // Wait for window to be restored
					} else {
						WindowsApi.ShowWindow(hWnd, 5);
						Thread.Sleep(100);
					}
					WindowsApi.SetForegroundWindow(hWnd);

					var r = new WindowsRect(0);
					if (resizeCheckbox.Checked) {
						ResizeWindow(hWnd, (int) windowWidth.Value, (int) windowHeight.Value, opaque && checkerSize < 2, color, out r);
						Thread.Sleep(100);
					}

					var length = WindowsApi.GetWindowTextLength(hWnd);
					var sb = new StringBuilder(length + 1);
					WindowsApi.GetWindowText(hWnd, sb, sb.Capacity);

					var name = sb.ToString();

					foreach (var inv in Path.GetInvalidFileNameChars())
						name = name.Replace(inv.ToString(), string.Empty);

					var s = Screenshot.GetScreenshot(hWnd, opaque, cursor, checkerSize, color);

					// Show the taskbar again
					if (hWnd != start && hWnd != taskbar) {
						WindowsApi.ShowWindow(start, 1);
						WindowsApi.ShowWindow(taskbar, 1);
					}

					if (s == null)
						MessageBox.Show("The screenshot taken was blank, it will not be saved.", "Warning", MessageBoxButtons.OK,
						                MessageBoxIcon.Warning);
					else {
						if(clipboard && opaque) {
							// Screenshot is already opaque, don't need to modify it
							Clipboard.SetImage(s);
						}
						else if(clipboard) {
							var whiteS = new Bitmap(s.Width, s.Height, PixelFormat.Format24bppRgb);
							using (var graphics = Graphics.FromImage(whiteS)) {
								graphics.Clear(Color.White);
								graphics.DrawImage(s, 0, 0, s.Width, s.Height);
							}
							using (var stream = new MemoryStream()) {
								// Save screenshot in clipboard as PNG which some applications support (eg. Microsoft Office)
								s.Save(stream, ImageFormat.Png);
								var data = new DataObject("PNG", stream);

								// Add fallback for applications that don't support PNG from clipboard (eg. Photoshop or Paint)
								data.SetData(DataFormats.Bitmap, whiteS);
								Clipboard.Clear();
								Clipboard.SetDataObject(data, true);
							}
							whiteS.Dispose();
						} else {
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
							s.Save(path, ImageFormat.Png);
						}
						s.Dispose();
					}

					if (resizeCheckbox.Checked)
						if ((WindowsApi.GetWindowLong(hWnd, GWL_STYLE) & WS_SIZEBOX) == WS_SIZEBOX)
							WindowsApi.SetWindowPos(hWnd, (IntPtr) 0, r.Left, r.Top, r.Right - r.Left, r.Bottom - r.Top, SWP_SHOWWINDOW);
				} catch (Exception) {
					if (hWnd != start && hWnd != taskbar) {
						WindowsApi.ShowWindow(start, 1);
						WindowsApi.ShowWindow(taskbar, 1);
					}
					MessageBox.Show(
						"An error occurred while trying to take a screenshot.\r\n\r\nPlease make sure you have selected a valid window.",
						"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			else
				MessageBox.Show("Invalid directory chosen.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		private static void ResizeWindow(IntPtr hWnd, int windowWidth, int windowHeight, bool opaque, Color color,
		                                 out WindowsRect oldRect) {
			oldRect = new WindowsRect(0);
			if ((WindowsApi.GetWindowLong(hWnd, GWL_STYLE) & WS_SIZEBOX) != WS_SIZEBOX) return;

			var r = new WindowsRect();
			WindowsApi.GetWindowRect(hWnd, ref r);
			oldRect = r;

			var f = Screenshot.GetScreenshot(hWnd, opaque, false, 0, color);
			if (f != null) {
				WindowsApi.SetWindowPos(hWnd, (IntPtr) 0, r.Left, r.Top, windowWidth - (f.Width - (r.Right - r.Left)),
				                        windowHeight - (f.Height - (r.Bottom - r.Top)), SWP_SHOWWINDOW);
				f.Dispose();
			} else WindowsApi.SetWindowPos(hWnd, (IntPtr) 0, r.Left, r.Top, windowWidth, windowHeight, SWP_SHOWWINDOW);
		}
	}

	internal class ColorDisplay : UserControl {
		private readonly SolidBrush _border = new SolidBrush(SystemColors.ControlDark);
		private SolidBrush _brush;
		private Color _color = Color.Black;

		public Color Color {
			get { return _color; }
			set {
				_color = value;
				Invalidate();
				Update();
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

	internal class Placeholder : Control {
		internal Placeholder() {
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
		}
	}
}