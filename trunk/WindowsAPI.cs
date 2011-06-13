﻿/*  AeroShot - Transparent screenshot utility for Windows
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
using System.Drawing;
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

	internal class WindowsApi {
		// Safe method of calling dwmapi.dll, for versions of windows lower than Vista
		public static int DwmGetWindowAttribute(IntPtr hWnd, DwmWindowAttribute dwAttribute, ref WindowsRect pvAttribute,
		                                 int cbAttribute) {
			var dwmDll = LoadLibrary("dwmapi.dll");
			if (dwmDll == IntPtr.Zero) return Marshal.GetLastWin32Error();

			var dwmFunction = GetProcAddress(dwmDll, "DwmGetWindowAttribute");
			if (dwmDll == IntPtr.Zero) return Marshal.GetLastWin32Error();

			var call =
				(_dwmGetWindowAttribute) Marshal.GetDelegateForFunctionPointer(dwmFunction, typeof (_dwmGetWindowAttribute));

			var result = call(hWnd, dwAttribute, ref pvAttribute, cbAttribute);

			FreeLibrary(dwmDll);

			return result;
		}

		public static int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref WindowsMargins pMarInset) {
			var dwmDll = LoadLibrary("dwmapi.dll");
			if (dwmDll == IntPtr.Zero) return Marshal.GetLastWin32Error();
			var dwmFunction = GetProcAddress(dwmDll, "DwmExtendFrameIntoClientArea");
			if (dwmDll == IntPtr.Zero) return Marshal.GetLastWin32Error();
			var call =
				(_dwmExtendFrameIntoClientArea)Marshal.GetDelegateForFunctionPointer(dwmFunction, typeof(_dwmExtendFrameIntoClientArea));
			var result = call(hWnd, ref pMarInset);
			FreeLibrary(dwmDll);
			return result;
		}

		public static int DwmIsCompositionEnabled(ref bool pfEnabled) {
			var dwmDll = LoadLibrary("dwmapi.dll");
			if (dwmDll == IntPtr.Zero) return Marshal.GetLastWin32Error();
			var dwmFunction = GetProcAddress(dwmDll, "DwmIsCompositionEnabled");
			if (dwmDll == IntPtr.Zero) return Marshal.GetLastWin32Error();
			var call =
				(_dwmIsCompositionEnabled)Marshal.GetDelegateForFunctionPointer(dwmFunction, typeof(_dwmIsCompositionEnabled));
			var result = call(ref pfEnabled);
			FreeLibrary(dwmDll);
			return result;
		}

		[DllImport("kernel32.dll")]
		public static extern bool FreeLibrary(IntPtr hModule);

		[DllImport("kernel32.dll")]
		public static extern IntPtr LoadLibrary(string dllToLoad);

		[DllImport("kernel32.dll")]
		public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);



		[DllImport("user32.dll")]
		public static extern bool SetForegroundWindow(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		[DllImport("user32.dll")]
		public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int width, int height,
		                                       uint uFlags);

		[DllImport("user32.dll")]
		public static extern bool GetWindowRect(IntPtr hWnd, ref WindowsRect rect);

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

		[DllImport("user32.dll")]
		public static extern IntPtr GetForegroundWindow();



		[DllImport("gdi32.dll")]
		public static extern bool BitBlt(IntPtr hdcDest, int xDest, int yDest, int wDest, int hDest, IntPtr hdcSource,
		                                 int xSrc, int ySrc, CopyPixelOperation rop);

		[DllImport("gdi32.dll")]
		public static extern IntPtr CreateDC(string lpszDriver, string lpszDevice, string lpszOutput, int lpInitData);

		[DllImport("gdi32.dll")]
		public static extern IntPtr DeleteDC(IntPtr hDc);

		[DllImport("gdi32.dll")]
		public static extern IntPtr DeleteObject(IntPtr hDc);

		[DllImport("gdi32.dll")]
		public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

		[DllImport("gdi32.dll")]
		public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

		[DllImport("gdi32.dll")]
		public static extern IntPtr SelectObject(IntPtr hdc, IntPtr bmp);



		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate int _dwmExtendFrameIntoClientArea(IntPtr hWnd, ref WindowsMargins pMarInset);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate int _dwmGetWindowAttribute(
			IntPtr hWnd, DwmWindowAttribute dwAttribute, ref WindowsRect pvAttribute, int cbAttribute);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate int _dwmIsCompositionEnabled(ref bool pfEnabled);
	}
}