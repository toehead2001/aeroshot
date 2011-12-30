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
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AeroShot {
	internal static class Screenshot {
		private const uint SWP_NOACTIVATE = 0x0010;

		internal static unsafe Bitmap GetScreenshot(IntPtr hWnd, bool opaque, bool cursor, int checkerSize, Color backColor) {
			if (!opaque || checkerSize > 1) backColor = Color.White;
			var backdrop = new Form
			               {BackColor = backColor, FormBorderStyle = FormBorderStyle.None, ShowInTaskbar = false, Opacity = 0};

			// Generate a rectangle with the size of all monitors combined
			var totalSize = Rectangle.Empty;
			foreach (var s in Screen.AllScreens)
				totalSize = Rectangle.Union(totalSize, s.Bounds);

			var rct = new WindowsRect();

			if (
				WindowsApi.DwmGetWindowAttribute(hWnd, DwmWindowAttribute.DWMWA_EXTENDED_FRAME_BOUNDS, ref rct, sizeof (WindowsRect)) !=
				0)
				// DwmGetWindowAttribute() failed, usually means Aero is disabled so we fall back to GetWindowRect()
				WindowsApi.GetWindowRect(hWnd, ref rct);
			else {
				// DwmGetWindowAttribute() succeeded
				// Add a 100px margin for window shadows. Excess transparency is trimmed out later
				rct.Left -= 100;
				rct.Right += 100;
				rct.Top -= 100;
				rct.Bottom += 100;
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

			WindowsApi.ShowWindow(backdrop.Handle, 4);
			WindowsApi.SetWindowPos(backdrop.Handle, hWnd, rct.Left, rct.Top, rct.Right - rct.Left, rct.Bottom - rct.Top,
			                        SWP_NOACTIVATE);
			backdrop.Opacity = 1;
			Application.DoEvents();

			// Capture screenshot with white background
			var whiteShot = CaptureScreen(new Rectangle(rct.Left, rct.Top, rct.Right - rct.Left, rct.Bottom - rct.Top));

			if (opaque && checkerSize < 2) {
				backdrop.Dispose();
				if (cursor)
					DrawSystemCursor(whiteShot, new Point(rct.Left, rct.Top));
				var final = TrimBitmap(whiteShot, backColor);
				whiteShot.Dispose();
				return final;
			}

			backdrop.BackColor = Color.Black;
			Application.DoEvents();

			// Capture screenshot with black background
			var blackShot = CaptureScreen(new Rectangle(rct.Left, rct.Top, rct.Right - rct.Left, rct.Bottom - rct.Top));

			backdrop.Dispose();

			var transparentImage = DifferentiateAlpha(whiteShot, blackShot);
			if (cursor)
				DrawSystemCursor(transparentImage, new Point(rct.Left, rct.Top));
			transparentImage = TrimBitmap(transparentImage, Color.FromArgb(0, 0, 0, 0));

			whiteShot.Dispose();
			blackShot.Dispose();

			if (opaque && checkerSize > 1) {
				var final = new Bitmap(transparentImage.Width, transparentImage.Height, PixelFormat.Format24bppRgb);
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

		private static void DrawSystemCursor(Bitmap windowImage, Point offsetLocation) {
			var ci = new CursorInfoStruct();
			ci.cbSize = Marshal.SizeOf(ci);
			if (WindowsApi.GetCursorInfo(out ci)) {
				if (ci.flags == 1) {
					var hicon = WindowsApi.CopyIcon(ci.hCursor);
					IconInfoStruct icInfo;
					if (WindowsApi.GetIconInfo(hicon, out icInfo)) {
						var loc = new Point(ci.ptScreenPos.x - offsetLocation.X - icInfo.xHotspot, ci.ptScreenPos.y - offsetLocation.Y - icInfo.yHotspot);
						var ic = Icon.FromHandle(hicon);
						var bmp = ic.ToBitmap();

						var g = Graphics.FromImage(windowImage);
						g.DrawImage(bmp, new Rectangle(loc, bmp.Size));
						g.Dispose();
						WindowsApi.DestroyIcon(hicon);
						bmp.Dispose();
					}
				}
			}
		}

		private static Bitmap CaptureScreen(Rectangle crop) {
			var totalSize = Rectangle.Empty;

			foreach (var s in Screen.AllScreens) totalSize = Rectangle.Union(totalSize, s.Bounds);

			var hSrc = WindowsApi.CreateDC("DISPLAY", null, null, 0);
			var hDest = WindowsApi.CreateCompatibleDC(hSrc);
			var hBmp = WindowsApi.CreateCompatibleBitmap(hSrc, crop.Right - crop.Left, crop.Bottom - crop.Top);
			var hOldBmp = WindowsApi.SelectObject(hDest, hBmp);
			WindowsApi.BitBlt(hDest, 0, 0, crop.Right - crop.Left, crop.Bottom - crop.Top, hSrc, crop.Left, crop.Top,
			                  CopyPixelOperation.SourceCopy | CopyPixelOperation.CaptureBlt);
			var bmp = Image.FromHbitmap(hBmp);
			WindowsApi.SelectObject(hDest, hOldBmp);
			WindowsApi.DeleteObject(hBmp);
			WindowsApi.DeleteDC(hDest);
			WindowsApi.DeleteDC(hSrc);

			return bmp.Clone(new Rectangle(0, 0, bmp.Width, bmp.Height), PixelFormat.Format24bppRgb);
		}

		private static unsafe Bitmap GenerateChecker(int s) {
			var b1 = new Bitmap(s*2, s*2, PixelFormat.Format24bppRgb);
			var b = new UnsafeBitmap(b1);
			b.LockImage();
			PixelData* pixel;
			for (int x = 0, y = 0; x < s*2 && y < s*2;) {
				pixel = b.GetPixel(x, y);
				if ((x >= 0 && x <= s - 1) && (y >= 0 && y <= s - 1))
					pixel->SetAll(255);
				if ((x >= s && x <= s*2 - 1) && (y >= 0 && y <= s - 1))
					pixel->SetAll(200);
				if ((x >= 0 && x <= s - 1) && (y >= s && y <= s*2 - 1))
					pixel->SetAll(200);
				if ((x >= s && x <= s*2 - 1) && (y >= s && y <= s*2 - 1))
					pixel->SetAll(255);
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

		private static unsafe Bitmap TrimBitmap(Bitmap b1, Color trimColor) {
			if (b1 == null) return null;

			var sizeX = b1.Width;
			var sizeY = b1.Height;
			var b = new UnsafeBitmap(b1);
			b.LockImage();

			var left = -1;
			var top = -1;
			var right = -1;
			var bottom = -1;

			PixelData* pixel;

			for (int x = 0, y = 0;;) {
				pixel = b.GetPixel(x, y);
				if (left == -1) {
					if ((trimColor.A == 0 && pixel->Alpha != 0) ||
					    (trimColor.R != pixel->Red & trimColor.G != pixel->Green & trimColor.B != pixel->Blue)) {
						left = x;
						x = 0;
						y = 0;
						continue;
					}
					if (y == sizeY - 1) {
						x++;
						y = 0;
					} else
						y++;

					continue;
				}
				if (top == -1) {
					if ((trimColor.A == 0 && pixel->Alpha != 0) ||
					    (trimColor.R != pixel->Red & trimColor.G != pixel->Green & trimColor.B != pixel->Blue)) {
						top = y;
						x = sizeX - 1;
						y = 0;
						continue;
					}
					if (x == sizeX - 1) {
						y++;
						x = 0;
					} else
						x++;

					continue;
				}
				if (right == -1) {
					if ((trimColor.A == 0 && pixel->Alpha != 0) ||
					    (trimColor.R != pixel->Red & trimColor.G != pixel->Green & trimColor.B != pixel->Blue)) {
						right = x + 1;
						x = 0;
						y = sizeY - 1;
						continue;
					}
					if (y == sizeY - 1) {
						x--;
						y = 0;
					} else
						y++;

					continue;
				}
				if (bottom == -1) {
					if ((trimColor.A == 0 && pixel->Alpha != 0) ||
					    (trimColor.R != pixel->Red & trimColor.G != pixel->Green & trimColor.B != pixel->Blue)) {
						bottom = y + 1;
						break;
					}
					if (x == sizeX - 1) {
						y--;
						x = 0;
					} else
						x++;

					continue;
				}
			}
			b.UnlockImage();
			if (left >= right || top >= bottom)
				return null;

			var final = b1.Clone(new Rectangle(left, top, right - left, bottom - top), b1.PixelFormat);
			b1.Dispose();
			return final;
		}

		private static unsafe Bitmap DifferentiateAlpha(Bitmap whiteBitmap, Bitmap blackBitmap) {
			if (whiteBitmap == null || blackBitmap == null || whiteBitmap.Width != blackBitmap.Width ||
			    whiteBitmap.Height != blackBitmap.Height)
				return null;
			var sizeX = whiteBitmap.Width;
			var sizeY = whiteBitmap.Height;
			var final = new Bitmap(sizeX, sizeY, PixelFormat.Format32bppArgb);
			var a = new UnsafeBitmap(whiteBitmap);
			var b = new UnsafeBitmap(blackBitmap);
			var f = new UnsafeBitmap(final);
			a.LockImage();
			b.LockImage();
			f.LockImage();

			var empty = true;

			for (int x = 0, y = 0; x < sizeX && y < sizeY;) {
				var pixelA = a.GetPixel(x, y);
				var pixelB = b.GetPixel(x, y);
				var pixelF = f.GetPixel(x, y);

				pixelF->Alpha =
					Convert.ToByte(255 -
					               ((Abs(pixelA->Red - pixelB->Red) + Abs(pixelA->Green - pixelB->Green) +
					                 Abs(pixelA->Blue - pixelB->Blue))/3));

				pixelF->Red = ToByte(pixelF->Alpha != 0 ? pixelB->Red*255/pixelF->Alpha : 0);
				pixelF->Green = ToByte(pixelF->Alpha != 0 ? pixelB->Green*255/pixelF->Alpha : 0);
				pixelF->Blue = ToByte(pixelF->Alpha != 0 ? pixelB->Blue*255/pixelF->Alpha : 0);

				if (empty && pixelF->Alpha > 0)
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
			f.UnlockImage();
			return empty ? null : final;
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