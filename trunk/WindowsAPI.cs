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
using System.Runtime.InteropServices;
using System.Text;

namespace AeroShot {
	public delegate bool CallBackPtr(IntPtr hwnd, int lParam);

	public struct WindowsRect {
		public int Left;
		public int Top;
		public int Right;
		public int Bottom;

		public WindowsRect(int l, int t, int r, int b) {
			Left = l;
			Top = t;
			Right = r;
			Bottom = b;
		}

		public WindowsRect(int x) {
			Left = x;
			Top = x;
			Right = x;
			Bottom = x;
		}
	}

	public struct WindowsMargins {
		public int LeftWidth;
		public int RightWidth;
		public int TopHeight;
		public int BottomHeight;

		public WindowsMargins(int left, int right, int top, int bottom) {
			LeftWidth = left;
			RightWidth = right;
			TopHeight = top;
			BottomHeight = bottom;
		}

		public WindowsMargins(int x) {
			LeftWidth = x;
			RightWidth = x;
			TopHeight = x;
			BottomHeight = x;
		}
	}

	public enum DwmWindowAttribute {
		DWMWA_NCRENDERING_ENABLED = 1,
		DWMWA_NCRENDERING_POLICY,
		DWMWA_TRANSITIONS_FORCEDISABLED,
		DWMWA_ALLOW_NCPAINT,
		DWMWA_CAPTION_BUTTON_BOUNDS,
		DWMWA_NONCLIENT_RTL_LAYOUT,
		DWMWA_FORCE_ICONIC_REPRESENTATION,
		DWMWA_FLIP3D_POLICY,
		DWMWA_EXTENDED_FRAME_BOUNDS,
		DWMWA_HAS_ICONIC_BITMAP,
		DWMWA_DISALLOW_PEEK,
		DWMWA_EXCLUDED_FROM_PEEK,
		DWMWA_LAST
	}

	internal unsafe class WindowsApi {
		[DllImport("user32.dll")]
		public static extern bool SetForegroundWindow(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		[DllImport("user32.dll")]
		public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int width, int height,
		                                       uint uFlags);

		[DllImport("user32.dll")]
		public static extern bool GetWindowRect(IntPtr hWnd, WindowsRect* rect);

		[DllImport("user32.dll")]
		public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);

		[DllImport("user32.dll")]
		public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

		[DllImport("user32.dll")]
		public static extern bool EnumWindows(CallBackPtr lpEnumFunc, IntPtr lParam);

		[DllImport("user32.dll")]
		public static extern bool IsIconic(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern long GetWindowLong(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll")]
		public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

		[DllImport("user32.dll")]
		public static extern int GetWindowTextLength(IntPtr hWnd);

		[DllImport("dwmapi.dll")]
		public static extern int DwmGetWindowAttribute(IntPtr hWnd, DwmWindowAttribute dwAttribute, WindowsRect* pvAttribute,
		                                               int cbAttribute);

		[DllImport("dwmapi.dll")]
		public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, WindowsMargins* pMarInset);

		[DllImport("dwmapi.dll")]
		public static extern int DwmIsCompositionEnabled(bool* pfEnabled);

		[DllImport("user32.dll")]
		public static extern IntPtr GetForegroundWindow();
	}
}