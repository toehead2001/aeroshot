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
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace AeroShot {
	internal delegate bool CallBackPtr(IntPtr hwnd, int lParam);

	internal delegate IntPtr HookProc(int code, IntPtr wParam, IntPtr lParam);

	[StructLayout(LayoutKind.Sequential)]
	internal struct WindowsRect {
		internal int Left;
		internal int Top;
		internal int Right;
		internal int Bottom;

		internal WindowsRect(int x) {
			Left = x;
			Top = x;
			Right = x;
			Bottom = x;
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct WindowsMargins {
		internal int LeftWidth;
		internal int RightWidth;
		internal int TopHeight;
		internal int BottomHeight;

		internal WindowsMargins(int left, int right, int top, int bottom) {
			LeftWidth = left;
			RightWidth = right;
			TopHeight = top;
			BottomHeight = bottom;
		}
	}

	public enum HookType {
		WH_JOURNALRECORD = 0,
		WH_JOURNALPLAYBACK = 1,
		WH_KEYBOARD = 2,
		WH_GETMESSAGE = 3,
		WH_CALLWNDPROC = 4,
		WH_CBT = 5,
		WH_SYSMSGFILTER = 6,
		WH_MOUSE = 7,
		WH_HARDWARE = 8,
		WH_DEBUG = 9,
		WH_SHELL = 10,
		WH_FOREGROUNDIDLE = 11,
		WH_CALLWNDPROCRET = 12,
		WH_KEYBOARD_LL = 13,
		WH_MOUSE_LL = 14
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct HookKeyStruct {
		public Keys vkCode;
		public uint scanCode;
		public uint flags;
		public uint time;
		public IntPtr dwExtraInfo;
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct BitmapInfo {
		public Int32 biSize;
		public Int32 biWidth;
		public Int32 biHeight;
		public Int16 biPlanes;
		public Int16 biBitCount;
		public Int32 biCompression;
		public Int32 biSizeImage;
		public Int32 biXPelsPerMeter;
		public Int32 biYPelsPerMeter;
		public Int32 biClrUsed;
		public Int32 biClrImportant;
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct IconInfoStruct {
		internal bool fIcon;
		internal Int32 xHotspot;
		internal Int32 yHotspot;
		internal IntPtr hbmMask;
		internal IntPtr hbmColor;
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct CursorInfoStruct {
		internal Int32 cbSize;
		internal Int32 flags;
		internal IntPtr hCursor;
		internal PointStruct ptScreenPos;
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct PointStruct {
		internal int x;
		internal int y;
	}

	internal enum DwmWindowAttribute {
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

	internal static class WindowsApi {
		// Safe method of calling dwmapi.dll, for versions of Windows lower than Vista
		internal static int DwmGetWindowAttribute(IntPtr hWnd, DwmWindowAttribute dwAttribute, ref WindowsRect pvAttribute,
		                                          int cbAttribute) {
			var dwmDll = LoadLibrary("dwmapi.dll");
			if (dwmDll == IntPtr.Zero) return Marshal.GetLastWin32Error();
			var dwmFunction = GetProcAddress(dwmDll, "DwmGetWindowAttribute");
			if (dwmFunction == IntPtr.Zero) return Marshal.GetLastWin32Error();
			var call =
				(DwmGetWindowAttributeDelegate)
				Marshal.GetDelegateForFunctionPointer(dwmFunction, typeof (DwmGetWindowAttributeDelegate));
			var result = call(hWnd, dwAttribute, ref pvAttribute, cbAttribute);
			FreeLibrary(dwmDll);
			return result;
		}

		internal static int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref WindowsMargins pMarInset) {
			var dwmDll = LoadLibrary("dwmapi.dll");
			if (dwmDll == IntPtr.Zero) return Marshal.GetLastWin32Error();
			var dwmFunction = GetProcAddress(dwmDll, "DwmExtendFrameIntoClientArea");
			if (dwmFunction == IntPtr.Zero) return Marshal.GetLastWin32Error();
			var call =
				(DwmExtendFrameIntoClientAreaDelegate)
				Marshal.GetDelegateForFunctionPointer(dwmFunction, typeof (DwmExtendFrameIntoClientAreaDelegate));
			var result = call(hWnd, ref pMarInset);
			FreeLibrary(dwmDll);
			return result;
		}

		internal static int DwmIsCompositionEnabled(ref bool pfEnabled) {
			var dwmDll = LoadLibrary("dwmapi.dll");
			if (dwmDll == IntPtr.Zero) return Marshal.GetLastWin32Error();
			var dwmFunction = GetProcAddress(dwmDll, "DwmIsCompositionEnabled");
			if (dwmFunction == IntPtr.Zero) return Marshal.GetLastWin32Error();
			var call =
				(DwmIsCompositionEnabledDelegate)
				Marshal.GetDelegateForFunctionPointer(dwmFunction, typeof (DwmIsCompositionEnabledDelegate));
			var result = call(ref pfEnabled);
			FreeLibrary(dwmDll);
			return result;
		}

		[DllImport("user32.dll")]
		internal static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

		[DllImport("user32.dll")]
		internal static extern bool SetForegroundWindow(IntPtr hWnd);

		[DllImport("user32.dll")]
		internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		[DllImport("user32.dll")]
		internal static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int width, int height,
		                                         uint uFlags);

		[DllImport("user32.dll")]
		internal static extern bool GetWindowRect(IntPtr hWnd, ref WindowsRect rect);

		[DllImport("user32.dll")]
		internal static extern IntPtr SetWindowsHookEx(HookType hookType, HookProc lpfn, IntPtr hMod, uint dwThreadId);

		[DllImport("user32.dll")]
		internal static extern bool UnhookWindowsHookEx(IntPtr hhk);

		[DllImport("user32.dll")]
		internal static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll")]
		internal static extern bool EnumWindows(CallBackPtr lpEnumFunc, IntPtr lParam);

		[DllImport("user32.dll")]
		internal static extern bool IsIconic(IntPtr hWnd);

		[DllImport("user32.dll")]
		internal static extern long GetWindowLong(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll")]
		internal static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

		[DllImport("user32.dll")]
		internal static extern int GetWindowTextLength(IntPtr hWnd);

		[DllImport("user32.dll")]
		internal static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll")]
		public static extern bool GetCursorInfo(out CursorInfoStruct pci);

		[DllImport("user32.dll")]
		public static extern IntPtr DestroyIcon(IntPtr hIcon);

		[DllImport("user32.dll")]
		public static extern IntPtr CopyIcon(IntPtr hIcon);

		[DllImport("user32.dll")]
		public static extern bool GetIconInfo(IntPtr hIcon, out IconInfoStruct piconinfo);

		[DllImport("user32.dll")]
		public static extern short GetAsyncKeyState(Keys key);

		[DllImport("gdi32.dll")]
		internal static extern bool BitBlt(IntPtr hdcDest, int xDest, int yDest, int wDest, int hDest, IntPtr hdcSource,
		                                   int xSrc, int ySrc, CopyPixelOperation rop);

		[DllImport("gdi32.dll")]
		internal static extern IntPtr CreateDC(string lpszDriver, string lpszDevice, string lpszOutput, int lpInitData);

		[DllImport("gdi32.dll")]
		internal static extern IntPtr DeleteDC(IntPtr hDc);

		[DllImport("gdi32.dll")]
		internal static extern int SaveDC(IntPtr hdc);

		[DllImport("gdi32.dll")]
		internal static extern IntPtr CreateDIBSection(IntPtr hdc, [In] ref BitmapInfo pbmi, uint pila, out IntPtr ppvBits,
		                                               IntPtr hSection, uint dwOffset);

		[DllImport("gdi32.dll")]
		internal static extern IntPtr DeleteObject(IntPtr hDc);

		[DllImport("gdi32.dll")]
		internal static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

		[DllImport("gdi32.dll")]
		internal static extern IntPtr CreateCompatibleDC(IntPtr hdc);

		[DllImport("gdi32.dll")]
		internal static extern IntPtr SelectObject(IntPtr hdc, IntPtr bmp);

		[DllImport("kernel32.dll")]
		internal static extern IntPtr GetModuleHandle(string name);

		[DllImport("kernel32.dll")]
		private static extern bool FreeLibrary(IntPtr hModule);

		[DllImport("kernel32.dll")]
		private static extern IntPtr LoadLibrary(string dllToLoad);

		[DllImport("kernel32.dll")]
		private static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate int DwmExtendFrameIntoClientAreaDelegate(IntPtr hWnd, ref WindowsMargins pMarInset);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate int DwmGetWindowAttributeDelegate(
			IntPtr hWnd, DwmWindowAttribute dwAttribute, ref WindowsRect pvAttribute, int cbAttribute);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate int DwmIsCompositionEnabledDelegate(ref bool pfEnabled);
	}
}