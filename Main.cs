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
using System.IO;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace AeroShot {
	public sealed partial class AeroShot : Form {
		private const int GWL_STYLE = -16;
		private const int MOD_WIN = 0x0008;
		private const int WM_HOTKEY = 0x0312;
		private const long WS_CAPTION = 0x00C00000L;
		private const long WS_VISIBLE = 0x10000000L;
		private static int windowId;
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
		}

		protected override void WndProc(ref Message m) {
			base.WndProc(ref m);

			if (m.Msg == WM_HOTKEY)
				TakeScreenshot(WindowsApi.GetForegroundWindow());
		}

		private void ssButton_Click(object sender, EventArgs e) {
			TakeScreenshot(handleList[windowList.SelectedIndex]);
		}

		private void rButton_Click(object sender, EventArgs e) {
			handleList.Clear();
			windowList.Items.Clear();

			callBackPtr = new CallBackPtr(ListWindows);
			WindowsApi.EnumWindows(callBackPtr, (IntPtr) 0);
			windowList.SelectedIndex = 0;
		}

		private void AeroShot_Shown(object sender, EventArgs e) {
			rButton_Click(null, null);
		}

		private void AeroShot_Closing(object sender, FormClosingEventArgs e) {
			WindowsApi.UnregisterHotKey(Handle, windowId);
			registryKey.SetValue("LastPath", folderTextBox.Text);
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

		private void TakeScreenshot(IntPtr hWnd) {
			if (Directory.Exists(folderTextBox.Text)) {
				try {
					var length = WindowsApi.GetWindowTextLength(hWnd);
					var sb = new StringBuilder(length + 1);
					WindowsApi.GetWindowText(hWnd, sb, sb.Capacity);

					var name = sb.ToString();

					foreach (var inv in Path.GetInvalidFileNameChars())
						name = name.Replace(inv.ToString(), string.Empty);

					name = name.Trim();
					if (name == string.Empty)
						name = "AeroShot";

					var path = folderTextBox.Text + Path.DirectorySeparatorChar + name + ".png";

					if (File.Exists(path))
						for (var i = 1; i < 9999; i++) {
							path = folderTextBox.Text + Path.DirectorySeparatorChar + name + " " + i + ".png";
							if (!File.Exists(path))
								break;
						}
					var s = Screenshot.TrimTransparency(Screenshot.GetScreenshot(hWnd));
					if (s == null) {
						MessageBox.Show(
							"The screenshot taken was blank, it will not be saved.", "Warning",
							MessageBoxButtons.OK, MessageBoxIcon.Warning);
					}
					else
						s.Save(path, System.Drawing.Imaging.ImageFormat.Png);
				}
				catch (Exception) {
					MessageBox.Show(
						"An error occurred while trying to take a screenshot.\r\n\r\nPlease make sure you have selected a valid window.",
						"Error",
						MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
			else {
				MessageBox.Show("Invalid directory chosen.", "Error",
								MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void bButton_Click(object sender, EventArgs e) {
			if (folderSelection.ShowDialog() == DialogResult.OK) {
				folderTextBox.Text = folderSelection.SelectedPath;
			}
		}
	}
}