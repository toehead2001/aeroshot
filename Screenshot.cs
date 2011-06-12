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
using System.Windows.Forms;

namespace AeroShot {
	internal class Screenshot {
		private const uint SWP_NOACTIVATE = 0x0010;

		public static unsafe Bitmap GetScreenshot(IntPtr hWnd, bool opaque, int checkerSize, Color backColor) {
			if (!opaque || checkerSize > 1) backColor = Color.White;
			var backdrop = new Form {BackColor = backColor, FormBorderStyle = FormBorderStyle.None, ShowInTaskbar = false};

			// Generate a rectangle with the size of all monitors combined
			var totalSize = Rectangle.Empty;
			foreach (var s in Screen.AllScreens)
				totalSize = Rectangle.Union(totalSize, s.Bounds);

			WindowsRect rct;

			if (
				WindowsApi.DwmGetWindowAttribute(hWnd, DwmWindowAttribute.DWMWA_EXTENDED_FRAME_BOUNDS, &rct, sizeof (WindowsRect)) !=
				0)
				// DwmGetWindowAttribute() failed, usually means Aero is disabled so we fall back to GetWindowRect()
				WindowsApi.GetWindowRect(hWnd, &rct);
			else {
				// DwmGetWindowAttribute() succeeded
				// Add a 40px margin for window shadows. Excess transparency is trimmed out later
				rct.Left -= 40;
				rct.Right += 40;
				rct.Top -= 40;
				rct.Bottom += 40;
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
			var whiteShotGraphics = Graphics.FromImage(whiteShot);

			WindowsApi.ShowWindow(backdrop.Handle, 4);
			WindowsApi.SetWindowPos(backdrop.Handle, hWnd, rct.Left, rct.Top, rct.Right - rct.Left, rct.Bottom - rct.Top,
			                        SWP_NOACTIVATE);
			Application.DoEvents();

			// Capture screenshot with white background
			whiteShotGraphics.CopyFromScreen(rct.Left, rct.Top, 0, 0, new Size(rct.Right - rct.Left, rct.Bottom - rct.Top),
			                                 CopyPixelOperation.SourceCopy);
			whiteShotGraphics.Dispose();

			if (opaque && checkerSize < 2) {
				backdrop.Dispose();
				return TrimBitmap(whiteShot, backColor);
			}

			var blackShot = new Bitmap(rct.Right - rct.Left, rct.Bottom - rct.Top, PixelFormat.Format32bppArgb);
			var blackShotGraphics = Graphics.FromImage(blackShot);

			backdrop.BackColor = Color.Black;
			Application.DoEvents();

			// Capture screenshot with black background
			blackShotGraphics.CopyFromScreen(rct.Left, rct.Top, 0, 0, new Size(rct.Right - rct.Left, rct.Bottom - rct.Top),
			                                 CopyPixelOperation.SourceCopy);
			blackShotGraphics.Dispose();

			backdrop.Dispose();

			var transparentImage = TrimBitmap(DifferentiateAlpha(whiteShot, blackShot), Color.FromArgb(0, 0, 0, 0));

			whiteShot.Dispose();
			blackShot.Dispose();

			if (opaque && checkerSize > 1) {
				var final = new Bitmap(transparentImage.Width, transparentImage.Height, PixelFormat.Format32bppRgb);
				var finalGraphics = Graphics.FromImage(final);
				var brush = new TextureBrush(GenerateChecker(checkerSize));
				finalGraphics.FillRectangle(brush, finalGraphics.ClipBounds);
				finalGraphics.DrawImageUnscaled(transparentImage, 0, 0);
				finalGraphics.Dispose();
				transparentImage.Dispose();
				return final;
			}

			// Returns a bitmap with transparency, calculated by differentiating the white and black screenshots
			return transparentImage;
		}

		private static Bitmap GenerateChecker(int s) {
			var b1 = new Bitmap(s*2, s*2, PixelFormat.Format32bppRgb);
			var b = new UnsafeBitmap(b1);
			b.LockImage();
			for (int x = 0, y = 0; x < s*2 && y < s*2;) {
				if ((x >= 0 && x <= s - 1) && (y >= 0 && y <= s - 1))
					b.SetPixel(x, y, new PixelData(255));
				if ((x >= s && x <= s*2 - 1) && (y >= 0 && y <= s - 1))
					b.SetPixel(x, y, new PixelData(200));
				if ((x >= 0 && x <= s - 1) && (y >= s && y <= s*2 - 1))
					b.SetPixel(x, y, new PixelData(200));
				if ((x >= s && x <= s*2 - 1) && (y >= s && y <= s*2 - 1))
					b.SetPixel(x, y, new PixelData(255));
				if (x == s*2 - 1) {
					y++;
					x = 0;
					continue;
				}
				x++;
			}
			b.UnlockImage();
			return b1;
		}

		private static Bitmap TrimBitmap(Bitmap b1, Color trimColor) {
			if (b1 == null) return null;

			var sizeX = b1.Width;
			var sizeY = b1.Height;
			var b = new UnsafeBitmap(b1);
			b.LockImage();

			var left = -1;
			var top = -1;
			var right = -1;
			var bottom = -1;

			PixelData p;

			for (int x = 0, y = 0;;) {
				p = b.GetPixel(x, y);
				if (left == -1) {
					if ((trimColor.A == 0 && p.Alpha != 0) || (trimColor.R != p.Red & trimColor.G != p.Green & trimColor.B != p.Blue)) {
						left = x;
						x = 0;
						y = 0;
						continue;
					}
					if (y == sizeY - 1) {
						x++;
						y = 0;
					}
					else
						y++;

					continue;
				}
				if (top == -1) {
					if ((trimColor.A == 0 && p.Alpha != 0) || (trimColor.R != p.Red & trimColor.G != p.Green & trimColor.B != p.Blue)) {
						top = y;
						x = sizeX - 1;
						y = 0;
						continue;
					}
					if (x == sizeX - 1) {
						y++;
						x = 0;
					}
					else
						x++;

					continue;
				}
				if (right == -1) {
					if ((trimColor.A == 0 && p.Alpha != 0) || (trimColor.R != p.Red & trimColor.G != p.Green & trimColor.B != p.Blue)) {
						right = x + 1;
						x = 0;
						y = sizeY - 1;
						continue;
					}
					if (y == sizeY - 1) {
						x--;
						y = 0;
					}
					else
						y++;

					continue;
				}
				if (bottom == -1) {
					if ((trimColor.A == 0 && p.Alpha != 0) || (trimColor.R != p.Red & trimColor.G != p.Green & trimColor.B != p.Blue)) {
						bottom = y + 1;
						break;
					}
					if (x == sizeX - 1) {
						y--;
						x = 0;
					}
					else
						x++;

					continue;
				}
			}
			b.UnlockImage();
			if (left >= right || top >= bottom)
				return null;

			return b1.Clone(new Rectangle(left, top, right - left, bottom - top), b1.PixelFormat);
		}

		private static Bitmap DifferentiateAlpha(Bitmap whiteBitmap, Bitmap blackBitmap) {
			if (whiteBitmap == null || blackBitmap == null || whiteBitmap.Width != blackBitmap.Width ||
			    whiteBitmap.Height != blackBitmap.Height)
				return null;
			var sizeX = whiteBitmap.Width;
			var sizeY = whiteBitmap.Height;
			var a = new UnsafeBitmap(whiteBitmap);
			var b = new UnsafeBitmap(blackBitmap);
			a.LockImage();
			b.LockImage();

			var empty = true;

			PixelData pixelA;
			PixelData pixelB;

			for (int x = 0, y = 0; x < sizeX && y < sizeY;) {
				pixelA = a.GetPixel(x, y);
				pixelB = b.GetPixel(x, y);

				pixelB.Alpha =
					Convert.ToByte(255 -
					               ((Abs(pixelA.Red - pixelB.Red) + Abs(pixelA.Green - pixelB.Green) + Abs(pixelA.Blue - pixelB.Blue))/
					                3));

				pixelB.Red = ToByte(pixelB.Alpha != 0 ? pixelB.Red*255/pixelB.Alpha : 0);
				pixelB.Green = ToByte(pixelB.Alpha != 0 ? pixelB.Green*255/pixelB.Alpha : 0);
				pixelB.Blue = ToByte(pixelB.Alpha != 0 ? pixelB.Blue*255/pixelB.Alpha : 0);

				b.SetPixel(x, y, pixelB);

				if (empty && pixelB.Alpha > 0)
					empty = false;

				if (x == sizeX - 1) {
					y++;
					x = 0;
					continue;
				}
				x++;
			}

			a.UnlockImage();
			b.UnlockImage();
			return empty ? null : blackBitmap;
		}

		private static byte ToByte(int i) {
			return (byte) (i > 255 ? 255 : i);
		}

		private static int Abs(int i) {
			// This is a magnitude more faster than Math.Abs()
			return i < 0 ? -i : i;
		}
	}
}