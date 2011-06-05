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
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows.Forms;

namespace AeroShot
{
	internal class Screenshot
	{
		public static unsafe Bitmap GetScreenshot(IntPtr hWnd)
		{
			WindowsApi.ShowWindow(hWnd, 1); // Show selected window, if minimized
			WindowsApi.SetForegroundWindow(hWnd);
			Thread.Sleep(100); // Wait for window to be fully visible

			// Generate a rectangle with the size of all monitors combined
			Rectangle totalSize = Rectangle.Empty;
			foreach (Screen s in Screen.AllScreens)
				totalSize = Rectangle.Union(totalSize, s.Bounds);

			WindowsApi.Rect rct;

			if (WindowsApi.DwmGetWindowAttribute(hWnd, WindowsApi.DwmWindowAttribute.DWMWA_EXTENDED_FRAME_BOUNDS, &rct, sizeof(WindowsApi.Rect)) != 0)
			{
				// DwmGetWindowAttribute() failed, usually means Aero is disabled so we fall back to GetWindowRect()
				WindowsApi.GetWindowRect(hWnd, &rct);
			}
			else // DwmGetWindowAttribute() succeeded
			{
				// Add a 30px margin for window shadows
				rct.Left -= 30;
				rct.Right += 30;
				rct.Top -= 30;
				rct.Bottom += 30;
			}

			// These next 4 checks handle if the window is outside of the visible screen
			if (rct.Left < totalSize.Left)
				rct.Left = totalSize.Left;
			if (rct.Right > totalSize.Right)
				rct.Right = totalSize.Right;
			if (rct.Top < totalSize.Top)
				rct.Top = totalSize.Top;
			if (rct.Bottom > totalSize.Bottom)
				rct.Bottom = totalSize.Bottom;

			var whiteShot = new Bitmap(rct.Right - rct.Left, rct.Bottom - rct.Top, PixelFormat.Format32bppArgb);
			Graphics whiteShotGraphics = Graphics.FromImage(whiteShot);
			var blackShot = new Bitmap(rct.Right - rct.Left, rct.Bottom - rct.Top, PixelFormat.Format32bppArgb);
			Graphics blackShotGraphics = Graphics.FromImage(blackShot);

			var backdrop = new Backdrop();
			backdrop.BackColor = Color.FromArgb(255, 255, 255);
			backdrop.Width = rct.Right - rct.Left;
			backdrop.Height = rct.Bottom - rct.Top;
			backdrop.Show();
			backdrop.Location = new Point(rct.Left, rct.Top);

			WindowsApi.SetForegroundWindow(backdrop.Handle);
			Thread.Sleep(100);
			WindowsApi.SetForegroundWindow(hWnd);
			Thread.Sleep(100);

			// Capture screenshot with white background
			whiteShotGraphics.CopyFromScreen(rct.Left, rct.Top, 0, 0, new Size(rct.Right - rct.Left, rct.Bottom - rct.Top),
			                                 CopyPixelOperation.SourceCopy);
			whiteShotGraphics.Dispose();

			backdrop.BackColor = Color.FromArgb(0, 0, 0);
			WindowsApi.SetForegroundWindow(backdrop.Handle);
			Thread.Sleep(100);
			WindowsApi.SetForegroundWindow(hWnd);
			Thread.Sleep(100);

			// Capture screenshot with black background
			blackShotGraphics.CopyFromScreen(rct.Left, rct.Top, 0, 0, new Size(rct.Right - rct.Left, rct.Bottom - rct.Top),
			                                 CopyPixelOperation.SourceCopy);
			blackShotGraphics.Dispose();

			backdrop.Dispose();

			// Returns a bitmap with transparency, calculated by differentiating the white and black screenshots
			return DifferentiateAlpha(whiteShot, blackShot);
		}

		private static Bitmap DifferentiateAlpha(Bitmap a1, Bitmap b1)
		{
			if (a1.Width != b1.Width || a1.Height != b1.Height)
			{
				return null;
			}
			int sizeX = a1.Width;
			int sizeY = a1.Height;
			var f1 = new Bitmap(sizeX, sizeY, PixelFormat.Format32bppArgb);
			var a = new UnsafeBitmap(a1);
			var b = new UnsafeBitmap(b1);
			var f = new UnsafeBitmap(f1);
			a.LockImage();
			b.LockImage();
			f.LockImage();

			byte[] pixelA;
			byte[] pixelB;

			for (int x = 0, y = 0; x < sizeX && y < sizeY; )
			{
				pixelA = a.GetPixel(x, y);
				pixelB = b.GetPixel(x, y);

				pixelB[3] = Convert.ToByte(255 - ((Abs(pixelA[0] - pixelB[0]) + Abs(pixelA[1] - pixelB[1]) + Abs(pixelA[2] - pixelB[2])) / 3));

				pixelB[0] = (byte) (pixelB[3] != 0 ? pixelB[0] * 255 / pixelB[3] : 0);
				pixelB[1] = (byte) (pixelB[3] != 0 ? pixelB[1] * 255 / pixelB[3] : 0);
				pixelB[2] = (byte) (pixelB[3] != 0 ? pixelB[2] * 255 / pixelB[3] : 0);

				f.SetPixel(x, y, pixelB);

				if (x == sizeX - 1)
				{
					y++;
					x = 0;
					continue;
				}
				x++;
			}

			a.UnlockImage();
			b.UnlockImage();
			f.UnlockImage();
			return f1;
		}

		private static int Abs(int i)
		{
			// This is a magnitude more faster than Math.Abs()
			return i < 0 ? -i : i;
		}
	}
}