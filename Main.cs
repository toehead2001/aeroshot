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
	public sealed partial class AeroShot : Form {
		private const int GWL_STYLE = -16;
		private const int MOD_WIN = 0x0008;
		private const int WM_HOTKEY = 0x0312;
		private const long WS_CAPTION = 0x00C00000L;
		private const long WS_VISIBLE = 0x10000000L;
		private const long WS_SIZEBOX = 0x00040000L;
		private const uint SWP_SHOWWINDOW = 0x0040;
		private static int windowId;
		private static Thread worker;
		private readonly List<IntPtr> handleList = new List<IntPtr>();
		private readonly RegistryKey registryKey;
		private CallBackPtr callBackPtr;

		public AeroShot() {
			Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
			InitializeComponent();

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
				windowWidth.Value = b[0] << 16 | b[1] << 8 | b[2];
				windowHeight.Value = b[3] << 16 | b[4] << 8 | b[5];
				resizeCheckBox.Checked = b[6] != 0 ? true : false;
			}

			windowWidth.Enabled = resizeCheckBox.Checked;
			windowHeight.Enabled = resizeCheckBox.Checked;
		}

		protected override void WndProc(ref Message m) {
			base.WndProc(ref m);

			if (m.Msg == WM_HOTKEY) {
				var f = folderTextBox.Text;
				worker = new Thread(() => TakeScreenshot(WindowsApi.GetForegroundWindow(), f)) {ApartmentState = ApartmentState.STA, IsBackground = true};
				worker.Start();
			}
		}

		private void ssButton_Click(object sender, EventArgs e) {
			var f = folderTextBox.Text;
			var h = handleList[windowList.SelectedIndex];

			worker = new Thread(() => TakeScreenshot(h, f)) {ApartmentState = ApartmentState.STA, IsBackground = true};
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

		private void resizeCheckBox_CheckedChanged(object sender, EventArgs e) {
			windowWidth.Enabled = resizeCheckBox.Checked;
			windowHeight.Enabled = resizeCheckBox.Checked;
		}

		private void AeroShot_Shown(object sender, EventArgs e) {
			rButton_Click(null, null);
		}

		private void AeroShot_Closing(object sender, FormClosingEventArgs e) {
			WindowsApi.UnregisterHotKey(Handle, windowId);
			registryKey.SetValue("LastPath", folderTextBox.Text);


			var b = new byte[8];
			b[2] = (byte) ((int) windowWidth.Value & 0xff);
			b[1] = (byte) (((int) windowWidth.Value >> 8) & 0xff);
			b[0] = (byte) (((int) windowWidth.Value >> 16) & 0xff);

			b[5] = (byte) ((int) windowHeight.Value & 0xff);
			b[4] = (byte) (((int) windowHeight.Value >> 8) & 0xff);
			b[3] = (byte) (((int) windowHeight.Value >> 16) & 0xff);

			b[6] = (byte) (resizeCheckBox.Checked ? 0xff : 0x00);


			var windowData = BitConverter.ToInt64(b, 0);
			registryKey.SetValue("WindowSize", windowData, RegistryValueKind.QWord);
		}

		private bool ListWindows(IntPtr hWnd, int lParam) {
			if ((WindowsApi.GetWindowLong(hWnd, GWL_STYLE) & WS_VISIBLE) == WS_VISIBLE && (WindowsApi.GetWindowLong(hWnd, GWL_STYLE) & WS_CAPTION) == WS_CAPTION) {
				var length = WindowsApi.GetWindowTextLength(hWnd);
				var sb = new StringBuilder(length + 1);
				WindowsApi.GetWindowText(hWnd, sb, sb.Capacity);

				handleList.Add(hWnd);
				windowList.Items.Add(sb.ToString());
			}
			return true;
		}

		private void TakeScreenshot(IntPtr hWnd, string folder) {
			if (Directory.Exists(folder))
				try {
					if (WindowsApi.IsIconic(hWnd)) {
						WindowsApi.ShowWindow(hWnd, 1); // Show window if minimized
						Thread.Sleep(400); // Wait for window to be restored
					}
					WindowsApi.SetForegroundWindow(hWnd);

					var r = new WindowsRect(0);
					if (resizeCheckBox.Checked) ResizeWindow(hWnd, (int) windowWidth.Value, (int) windowHeight.Value, out r);

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
					var s = Screenshot.GetScreenshot(hWnd);
					if (s == null)
						MessageBox.Show("The screenshot taken was blank, it will not be saved.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					else {
						s.Save(path, ImageFormat.Png);
						s.Dispose();
					}

					if (resizeCheckBox.Checked)
						if ((WindowsApi.GetWindowLong(hWnd, GWL_STYLE) & WS_SIZEBOX) == WS_SIZEBOX)
							WindowsApi.SetWindowPos(hWnd, (IntPtr) 0, r.Left, r.Top, r.Right - r.Left, r.Bottom - r.Top, SWP_SHOWWINDOW);
				}
				catch (Exception) {
					MessageBox.Show("An error occurred while trying to take a screenshot.\r\n\r\nPlease make sure you have selected a valid window.", "Error", MessageBoxButtons.OK,
					                MessageBoxIcon.Error);
				}
			else
				MessageBox.Show("Invalid directory chosen.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		private static unsafe void ResizeWindow(IntPtr hWnd, int windowWidth, int windowHeight, out WindowsRect oldRect) {
			oldRect = new WindowsRect(0);
			if ((WindowsApi.GetWindowLong(hWnd, GWL_STYLE) & WS_SIZEBOX) != WS_SIZEBOX) return;

			WindowsApi.ShowWindow(hWnd, 1);

			WindowsRect r;
			WindowsApi.GetWindowRect(hWnd, &r);
			var f = Screenshot.GetScreenshot(hWnd);
			oldRect = r;

			WindowsApi.SetWindowPos(hWnd, (IntPtr) 0, r.Left, r.Top, windowWidth - (f.Width - (r.Right - r.Left)), windowHeight - (f.Height - (r.Bottom - r.Top)), SWP_SHOWWINDOW);

			f.Dispose();
		}
	}
}