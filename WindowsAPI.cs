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

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace AeroShot
{
	internal delegate bool CallBackPtr(IntPtr hwnd, int lParam);

	[StructLayout(LayoutKind.Sequential)]
	internal struct WindowsRect
	{
		internal int Left;
		internal int Top;
		internal int Right;
		internal int Bottom;

		internal WindowsRect(int x)
		{
			Left = x;
			Top = x;
			Right = x;
			Bottom = x;
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct WindowsMargins
	{
		internal int LeftWidth;
		internal int RightWidth;
		internal int TopHeight;
		internal int BottomHeight;

		internal WindowsMargins(int left, int right, int top, int bottom)
		{
			LeftWidth = left;
			RightWidth = right;
			TopHeight = top;
			BottomHeight = bottom;
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct BitmapInfo
	{
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
	internal struct IconInfoStruct
	{
		internal bool fIcon;
		internal Int32 xHotspot;
		internal Int32 yHotspot;
		internal IntPtr hbmMask;
		internal IntPtr hbmColor;
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct CursorInfoStruct
	{
		internal Int32 cbSize;
		internal Int32 flags;
		internal IntPtr hCursor;
		internal PointStruct ptScreenPos;
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct PointStruct
	{
		internal int X;
		internal int Y;
	}

	internal enum DwmWindowAttribute
	{
		NonClientRenderingEnabled = 1,
		NonClientRenderingPolicy,
		TransitionsForceDisabled,
		AllowNonClientPaint,
		CaptionButtonBounds,
		NonClientRtlLayout,
		ForceIconicRepresentation,
		Flip3DPolicy,
		ExtendedFrameBounds,
		HasIconicBitmap,
		DisallowPeek,
		ExcludedFromPeek,
		Last
	}

	internal static class WindowsApi
	{
		// Safe method of calling dwmapi.dll, for versions of Windows lower than Vista
		internal static int DwmGetWindowAttribute(IntPtr hWnd,
												  DwmWindowAttribute dwAttribute,
												  ref WindowsRect pvAttribute,
												  int cbAttribute)
		{
			IntPtr dwmDll = LoadLibrary("dwmapi.dll");
			if (dwmDll == IntPtr.Zero)
				return Marshal.GetLastWin32Error();
			IntPtr dwmFunction = GetProcAddress(dwmDll, "DwmGetWindowAttribute");
			if (dwmFunction == IntPtr.Zero)
				return Marshal.GetLastWin32Error();
			var call =
				(DwmGetWindowAttributeDelegate)
				Marshal.GetDelegateForFunctionPointer(dwmFunction,
													  typeof(
														  DwmGetWindowAttributeDelegate
														  ));
			int result = call(hWnd, dwAttribute, ref pvAttribute, cbAttribute);
			FreeLibrary(dwmDll);
			return result;
		}

		internal static int DwmExtendFrameIntoClientArea(IntPtr hWnd,
														 ref WindowsMargins
															 pMarInset)
		{
			IntPtr dwmDll = LoadLibrary("dwmapi.dll");
			if (dwmDll == IntPtr.Zero)
				return Marshal.GetLastWin32Error();
			IntPtr dwmFunction = GetProcAddress(dwmDll,
												"DwmExtendFrameIntoClientArea");
			if (dwmFunction == IntPtr.Zero)
				return Marshal.GetLastWin32Error();
			var call =
				(DwmExtendFrameIntoClientAreaDelegate)
				Marshal.GetDelegateForFunctionPointer(dwmFunction,
													  typeof(
														  DwmExtendFrameIntoClientAreaDelegate
														  ));
			int result = call(hWnd, ref pMarInset);
			FreeLibrary(dwmDll);
			return result;
		}

		internal static int DwmIsCompositionEnabled(ref bool pfEnabled)
		{
			IntPtr dwmDll = LoadLibrary("dwmapi.dll");
			if (dwmDll == IntPtr.Zero)
				return Marshal.GetLastWin32Error();
			IntPtr dwmFunction = GetProcAddress(dwmDll,
												"DwmIsCompositionEnabled");
			if (dwmFunction == IntPtr.Zero)
				return Marshal.GetLastWin32Error();
			var call =
				(DwmIsCompositionEnabledDelegate)
				Marshal.GetDelegateForFunctionPointer(dwmFunction,
													  typeof(
														  DwmIsCompositionEnabledDelegate
														  ));
			int result = call(ref pfEnabled);
			FreeLibrary(dwmDll);
			return result;
		}

		[DllImport("user32.dll")]
		internal static extern IntPtr FindWindow(string lpClassName,
												 string lpWindowName);

		[DllImport("user32.dll")]
		internal static extern bool SetForegroundWindow(IntPtr hWnd);

		[DllImport("user32.dll")]
		internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		[DllImport("user32.dll")]
		internal static extern bool SetWindowPos(IntPtr hWnd,
												 IntPtr hWndInsertAfter, int x,
												 int y, int width, int height,
												 uint uFlags);

		[DllImport("user32.dll")]
		internal static extern bool GetWindowRect(IntPtr hWnd,
												  ref WindowsRect rect);

		[DllImport("user32.dll")]
		internal static extern bool EnumWindows(CallBackPtr lpEnumFunc,
												IntPtr lParam);

		[DllImport("user32.dll")]
		internal static extern bool IsIconic(IntPtr hWnd);

		[DllImport("user32.dll")]
		internal static extern bool IsWindowVisible(IntPtr hWnd);

		[DllImport("user32.dll")]
		internal static extern long GetWindowLong(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll")]
		internal static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

		[DllImport("user32.dll")]
		internal static extern int GetWindowText(IntPtr hWnd,
												 StringBuilder lpString,
												 int nMaxCount);

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
		public static extern bool GetIconInfo(IntPtr hIcon,
											  out IconInfoStruct piconinfo);

		[DllImport("user32.dll")]
		internal static extern bool RegisterHotKey(IntPtr hWnd, int id,
												   int fsModifiers, int vlc);

		[DllImport("user32.dll")]
		internal static extern bool UnregisterHotKey(IntPtr hWnd, int id);

		[DllImport("gdi32.dll")]
		internal static extern bool BitBlt(IntPtr hdcDest, int xDest, int yDest,
										   int wDest, int hDest,
										   IntPtr hdcSource, int xSrc, int ySrc,
										   CopyPixelOperation rop);

		[DllImport("gdi32.dll")]
		internal static extern IntPtr CreateDC(string lpszDriver,
											   string lpszDevice,
											   string lpszOutput, int lpInitData);

		[DllImport("gdi32.dll")]
		internal static extern IntPtr DeleteDC(IntPtr hDc);

		[DllImport("gdi32.dll")]
		internal static extern int SaveDC(IntPtr hdc);

		[DllImport("gdi32.dll")]
		internal static extern IntPtr CreateDIBSection(IntPtr hdc,
													   [In] ref BitmapInfo pbmi,
													   uint pila,
													   out IntPtr ppvBits,
													   IntPtr hSection,
													   uint dwOffset);

		[DllImport("gdi32.dll")]
		internal static extern IntPtr DeleteObject(IntPtr hDc);

		[DllImport("gdi32.dll")]
		internal static extern IntPtr CreateCompatibleBitmap(IntPtr hdc,
															 int nWidth,
															 int nHeight);

		[DllImport("gdi32.dll")]
		internal static extern IntPtr CreateCompatibleDC(IntPtr hdc);

		[DllImport("gdi32.dll")]
		internal static extern IntPtr SelectObject(IntPtr hdc, IntPtr bmp);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool FreeLibrary(IntPtr hModule);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern IntPtr LoadLibrary(string dllToLoad);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern IntPtr GetProcAddress(IntPtr hModule,
													string procedureName);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate int DwmExtendFrameIntoClientAreaDelegate(
			IntPtr hWnd, ref WindowsMargins pMarInset);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate int DwmGetWindowAttributeDelegate(
			IntPtr hWnd, DwmWindowAttribute dwAttribute,
			ref WindowsRect pvAttribute, int cbAttribute);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate int DwmIsCompositionEnabledDelegate(ref bool pfEnabled);
	}
}