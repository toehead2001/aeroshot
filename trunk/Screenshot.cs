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
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace AeroShot {
	internal struct ScreenshotTask {
		public enum BackgroundType {
			Transparent,
			Checkerboard,
			SolidColour
		}

		public BackgroundType Background;
		public Color BackgroundColour;
		public bool CaptureMouse;
		public int CheckerboardSize;
		public string DiskSaveDirectory;
		public bool DoResize;
		public int ResizeX;
		public int ResizeY;
		public bool ClipboardNotDisk;
		public IntPtr WindowHandle;

		public ScreenshotTask(IntPtr window, bool clipboard, string file, bool resize, int resizeX, int resizeY,
		                      BackgroundType backType, Color backColour, int checkerSize, bool mouse) {
			WindowHandle = window;
			ClipboardNotDisk = clipboard;
			DiskSaveDirectory = file;
			DoResize = resize;
			ResizeX = resizeX;
			ResizeY = resizeY;
			Background = backType;
			BackgroundColour = backColour;
			CheckerboardSize = checkerSize;
			CaptureMouse = mouse;
		}
	}

	internal static class Screenshot {
		private const uint SWP_NOACTIVATE = 0x0010;
		private const int GWL_STYLE = -16;
		private const long WS_SIZEBOX = 0x00040000L;
		private const uint SWP_SHOWWINDOW = 0x0040;

		internal static void CaptureWindow(ref ScreenshotTask data) {
			var start = WindowsApi.FindWindow("Button", "Start");
			var taskbar = WindowsApi.FindWindow("Shell_TrayWnd", null);
			if (data.ClipboardNotDisk || Directory.Exists(data.DiskSaveDirectory))
				try {
					// Hide the taskbar, just incase it gets in the way
					if (data.WindowHandle != start && data.WindowHandle != taskbar) {
						WindowsApi.ShowWindow(start, 0);
						WindowsApi.ShowWindow(taskbar, 0);
						Application.DoEvents();
					}
					if (WindowsApi.IsIconic(data.WindowHandle)) {
						WindowsApi.ShowWindow(data.WindowHandle, 1);
						Thread.Sleep(300); // Wait for window to be restored
					} else {
						WindowsApi.ShowWindow(data.WindowHandle, 5);
						Thread.Sleep(100);
					}
					WindowsApi.SetForegroundWindow(data.WindowHandle);

					var r = new WindowsRect(0);
					if (data.DoResize) {
						SmartResizeWindow(ref data, out r);
						Thread.Sleep(100);
					}

					var length = WindowsApi.GetWindowTextLength(data.WindowHandle);
					var sb = new StringBuilder(length + 1);
					WindowsApi.GetWindowText(data.WindowHandle, sb, sb.Capacity);

					var name = sb.ToString();

					foreach (var inv in Path.GetInvalidFileNameChars())
						name = name.Replace(inv.ToString(), string.Empty);

					var s = CaptureCompositeScreenshot(ref data);

					// Show the taskbar again
					if (data.WindowHandle != start && data.WindowHandle != taskbar) {
						WindowsApi.ShowWindow(start, 1);
						WindowsApi.ShowWindow(taskbar, 1);
					}

					if (s == null) {
						MessageBox.Show("The screenshot taken was blank, it will not be saved.", "Warning", MessageBoxButtons.OK,
						                MessageBoxIcon.Warning);
					} else {
						if (data.ClipboardNotDisk && data.Background != ScreenshotTask.BackgroundType.Transparent) {
							// Screenshot is already opaque, don't need to modify it
							Clipboard.SetImage(s);
						} else if (data.ClipboardNotDisk) {
							var whiteS = new Bitmap(s.Width, s.Height, PixelFormat.Format24bppRgb);
							using (var graphics = Graphics.FromImage(whiteS)) {
								graphics.Clear(Color.White);
								graphics.DrawImage(s, 0, 0, s.Width, s.Height);
							}
							using (var stream = new MemoryStream()) {
								// Save screenshot in clipboard as PNG which some applications support (eg. Microsoft Office)
								s.Save(stream, ImageFormat.Png);
								var pngClipboardData = new DataObject("PNG", stream);

								// Add fallback for applications that don't support PNG from clipboard (eg. Photoshop or Paint)
								pngClipboardData.SetData(DataFormats.Bitmap, whiteS);
								Clipboard.Clear();
								Clipboard.SetDataObject(pngClipboardData, true);
							}
							whiteS.Dispose();
						} else {
							name = name.Trim();
							if (name == string.Empty)
								name = "AeroShot";
							var path = Path.Combine(data.DiskSaveDirectory, name + ".png");

							if (File.Exists(path))
								for (var i = 1; i < 9999; i++) {
									path = Path.Combine(data.DiskSaveDirectory, name + " " + i + ".png");
									if (!File.Exists(path))
										break;
								}
							s.Save(path, ImageFormat.Png);
						}
						s.Dispose();
					}

					if (data.DoResize)
						if ((WindowsApi.GetWindowLong(data.WindowHandle, GWL_STYLE) & WS_SIZEBOX) == WS_SIZEBOX)
							WindowsApi.SetWindowPos(data.WindowHandle, (IntPtr)0, r.Left, r.Top, r.Right - r.Left, r.Bottom - r.Top, SWP_SHOWWINDOW);
				} catch (Exception) {
					if (data.WindowHandle != start && data.WindowHandle != taskbar) {
						WindowsApi.ShowWindow(start, 1);
						WindowsApi.ShowWindow(taskbar, 1);
					}
					MessageBox.Show(
						"An error occurred while trying to take a screenshot.\r\n\r\nPlease make sure you have selected a valid window.",
						"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				} else
				MessageBox.Show("Invalid directory chosen.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		private static void SmartResizeWindow(ref ScreenshotTask data, out WindowsRect oldWindowSize) {
			oldWindowSize = new WindowsRect(0);
			if ((WindowsApi.GetWindowLong(data.WindowHandle, GWL_STYLE) & WS_SIZEBOX) != WS_SIZEBOX) return;

			var r = new WindowsRect();
			WindowsApi.GetWindowRect(data.WindowHandle, ref r);
			oldWindowSize = r;

			var f = CaptureCompositeScreenshot(ref data);
			if (f != null) {
				WindowsApi.SetWindowPos(data.WindowHandle, (IntPtr)0, r.Left, r.Top, data.ResizeX - (f.Width - (r.Right - r.Left)),
										data.ResizeY - (f.Height - (r.Bottom - r.Top)), SWP_SHOWWINDOW);
				f.Dispose();
			} else WindowsApi.SetWindowPos(data.WindowHandle, (IntPtr)0, r.Left, r.Top, data.ResizeX, data.ResizeY, SWP_SHOWWINDOW);
		}

		private static unsafe Bitmap CaptureCompositeScreenshot(ref ScreenshotTask data) {
			var tmpColour = data.BackgroundColour;
			if (data.Background == ScreenshotTask.BackgroundType.Transparent || data.Background == ScreenshotTask.BackgroundType.Checkerboard)
				tmpColour = Color.White;
			var backdrop = new Form { BackColor = tmpColour, FormBorderStyle = FormBorderStyle.None, ShowInTaskbar = false, Opacity = 0 };

			// Generate a rectangle with the size of all monitors combined
			var totalSize = Rectangle.Empty;
			foreach (var s in Screen.AllScreens)
				totalSize = Rectangle.Union(totalSize, s.Bounds);

			var rct = new WindowsRect();

			if (
				WindowsApi.DwmGetWindowAttribute(data.WindowHandle, DwmWindowAttribute.DWMWA_EXTENDED_FRAME_BOUNDS, ref rct, sizeof(WindowsRect)) !=
				0)
				// DwmGetWindowAttribute() failed, usually means Aero is disabled so we fall back to GetWindowRect()
				WindowsApi.GetWindowRect(data.WindowHandle, ref rct);
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
			WindowsApi.SetWindowPos(backdrop.Handle, data.WindowHandle, rct.Left, rct.Top, rct.Right - rct.Left, rct.Bottom - rct.Top,
			                        SWP_NOACTIVATE);
			backdrop.Opacity = 1;
			Application.DoEvents();

			// Capture screenshot with white background
			var whiteShot = CaptureScreenRegion(new Rectangle(rct.Left, rct.Top, rct.Right - rct.Left, rct.Bottom - rct.Top));

			if (data.Background == ScreenshotTask.BackgroundType.SolidColour) {
				backdrop.Dispose();
				if (data.CaptureMouse)
					DrawCursorToBitmap(whiteShot, new Point(rct.Left, rct.Top));
				var final = CropEmptyEdges(whiteShot, tmpColour);
				whiteShot.Dispose();
				return final;
			}

			backdrop.BackColor = Color.Black;
			Application.DoEvents();

			// Capture screenshot with black background
			var blackShot = CaptureScreenRegion(new Rectangle(rct.Left, rct.Top, rct.Right - rct.Left, rct.Bottom - rct.Top));

			backdrop.Dispose();

			var transparentImage = DifferentiateAlpha(whiteShot, blackShot);
			if (data.CaptureMouse)
				DrawCursorToBitmap(transparentImage, new Point(rct.Left, rct.Top));
			transparentImage = CropEmptyEdges(transparentImage, Color.FromArgb(0, 0, 0, 0));

			whiteShot.Dispose();
			blackShot.Dispose();

			if (data.Background == ScreenshotTask.BackgroundType.Checkerboard) {
				var final = new Bitmap(transparentImage.Width, transparentImage.Height, PixelFormat.Format24bppRgb);
				var finalGraphics = Graphics.FromImage(final);
				var brush = new TextureBrush(GenerateChecker(data.CheckerboardSize));
				finalGraphics.FillRectangle(brush, finalGraphics.ClipBounds);
				finalGraphics.DrawImageUnscaled(transparentImage, 0, 0);
				finalGraphics.Dispose();
				transparentImage.Dispose();
				return final;
			}
			// Returns a bitmap with transparency, calculated by differentiating the white and black screenshots
			return transparentImage;
		}

		private static void DrawCursorToBitmap(Bitmap windowImage, Point offsetLocation) {
			var ci = new CursorInfoStruct();
			ci.cbSize = Marshal.SizeOf(ci);
			if (WindowsApi.GetCursorInfo(out ci))
				if (ci.flags == 1) {
					var hicon = WindowsApi.CopyIcon(ci.hCursor);
					IconInfoStruct icInfo;
					if (WindowsApi.GetIconInfo(hicon, out icInfo)) {
						var loc = new Point(ci.ptScreenPos.x - offsetLocation.X - icInfo.xHotspot,
						                    ci.ptScreenPos.y - offsetLocation.Y - icInfo.yHotspot);
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

		private static Bitmap CaptureScreenRegion(Rectangle crop) {
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

		private static unsafe Bitmap CropEmptyEdges(Bitmap b1, Color trimColour) {
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
					if ((trimColour.A == 0 && pixel->Alpha != 0) ||
					    (trimColour.R != pixel->Red & trimColour.G != pixel->Green & trimColour.B != pixel->Blue)) {
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
					if ((trimColour.A == 0 && pixel->Alpha != 0) ||
					    (trimColour.R != pixel->Red & trimColour.G != pixel->Green & trimColour.B != pixel->Blue)) {
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
					if ((trimColour.A == 0 && pixel->Alpha != 0) ||
					    (trimColour.R != pixel->Red & trimColour.G != pixel->Green & trimColour.B != pixel->Blue)) {
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
					if ((trimColour.A == 0 && pixel->Alpha != 0) ||
					    (trimColour.R != pixel->Red & trimColour.G != pixel->Green & trimColour.B != pixel->Blue)) {
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