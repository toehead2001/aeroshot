using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace AeroShot
{
    public static class Dibv5
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct BITMAPV5HEADER
        {
            public uint bV5Size;
            public int bV5Width;
            public int bV5Height;
            public ushort bV5Planes;
            public ushort bV5BitCount;
            public uint bV5Compression;
            public uint bV5SizeImage;
            public int bV5XPelsPerMeter;
            public int bV5YPelsPerMeter;
            public uint bV5ClrUsed;
            public uint bV5ClrImportant;
            public uint bV5RedMask;
            public uint bV5GreenMask;
            public uint bV5BlueMask;
            public uint bV5AlphaMask;
            public uint bV5CSType;
            // CIEXYZTRIPLE bV5Endpoints
            // bV5Endpoints.ciexyzRed
            public int ciexyzRedX;
            public int ciexyzRedY;
            public int ciexyzRedZ;
            // bV5Endpoints.ciexyzGreen
            public int ciexyzGreenX;
            public int ciexyzGreenY;
            public int ciexyzGreenZ;
            // bV5Endpoints.ciexyzBlue
            public int ciexyzBlueX;
            public int ciexyzBlueY;
            public int ciexyzBlueZ;
            public uint bV5GammaRed;
            public uint bV5GammaGreen;
            public uint bV5GammaBlue;
            public uint bV5Intent;
            public uint bV5ProfileData;
            public uint bV5ProfileSize;
            public uint bV5Reserved;
        }

        [DllImport("user32.dll")]
        private static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("user32.dll")]
        private static extern bool CloseClipboard();

        [DllImport("user32.dll")]
        private static extern bool EmptyClipboard();

        [DllImport("user32.dll")]
        private static extern IntPtr SetClipboardData(uint uFormat, IntPtr hMemory);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern uint RegisterClipboardFormat(string lpszFormat);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GlobalAlloc(uint uFlags, UIntPtr cbBytes);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GlobalLock(IntPtr hMemory);

        [DllImport("kernel32.dll")]
        private static extern bool GlobalUnlock(IntPtr hMemory);

        private const uint GMEM_MOVEABLE = 0x0002;

        private const uint CF_DIBV5 = 17;

        private const uint BI_RGB = 0;

        private const uint LCS_GM_GRAPHICS = 0x00000002;

        private const uint LCS_WINDOWS_COLOR_SPACE = 0x57696E20; // 'Win ' in ASCII (or ' niW')

        private const uint LCS_sRGB = 0x73524742; // 'sRGB' in ASCII (or 'BGRs')

        private const int SIZEOF_BITMAPV5HEADER = 124;

        public static void SetImageAsDibv5(Image i)
        {
            if (i == null)
            {
                throw new ArgumentNullException(nameof(i), "Image cannot be null.");
            }

            int w, h, stride, siz;
            byte[] pixbuff, pngbuff;

            // save as raw png before doing anything...
            using (var ms = new MemoryStream())
            {
                i.Save(ms, ImageFormat.Png);
                pngbuff = ms.ToArray();
            }

            // use 32bit Premultiplied ARGB format since this is what GDI is using internally for bitmaps w/ alpha
            using (var bm = new Bitmap(i.Width, i.Height, PixelFormat.Format32bppPArgb))
            {
                using (var g = Graphics.FromImage(bm))
                {
                    // draw
                    g.DrawImage(i, 0, 0);
                }

                // flip Y because BITMAPV5 expects a different pixel order
                bm.RotateFlip(RotateFlipType.RotateNoneFlipY);

                // lock with our current pixel format (already prepared)
                {
                    var dat = bm.LockBits(new Rectangle(0, 0, bm.Width, bm.Height), ImageLockMode.ReadOnly, bm.PixelFormat);
                    // copy data
                    w = dat.Width;
                    h = dat.Height;
                    stride = dat.Stride;
                    siz = stride * h; // magic!
                    pixbuff = new byte[siz];
                    Marshal.Copy(dat.Scan0, pixbuff, 0, siz);
                    bm.UnlockBits(dat);
                }
                // unlock
            }

            // copy raw pixels into a byte array with 124 extra bytes for DIBV5 info
            byte[] final = new byte[SIZEOF_BITMAPV5HEADER + pixbuff.Length];
            Array.Copy(pixbuff, 0, final, SIZEOF_BITMAPV5HEADER, pixbuff.Length);

            // prepare DIBV5
            var v5hdr = new BITMAPV5HEADER();
            v5hdr.bV5Size = SIZEOF_BITMAPV5HEADER; // sizeof(BITMAPV5HEADER); in Visual C++
            v5hdr.bV5Width = w;
            v5hdr.bV5Height = h; // already flipped for us, no need to negate, FIXES DISCORD/CHROME
            v5hdr.bV5Planes = 1; // must be set to 1
            v5hdr.bV5BitCount = 32; // argb bit count
            v5hdr.bV5Compression = BI_RGB;
            v5hdr.bV5SizeImage = (uint)siz; // in bytes
            v5hdr.bV5XPelsPerMeter = 1; // ????
            v5hdr.bV5YPelsPerMeter = 1; // ????
            v5hdr.bV5ClrUsed = 0;
            v5hdr.bV5ClrImportant = 0;
            v5hdr.bV5RedMask = 0;
            v5hdr.bV5GreenMask = 0;
            v5hdr.bV5BlueMask = 0;
            v5hdr.bV5AlphaMask = 0xff000000; // important!!
            v5hdr.bV5CSType = LCS_WINDOWS_COLOR_SPACE; // default color space.
            v5hdr.ciexyzRedX = 0;
            v5hdr.ciexyzRedY = 0;
            v5hdr.ciexyzRedZ = 0;
            v5hdr.ciexyzGreenX = 0;
            v5hdr.ciexyzGreenY = 0;
            v5hdr.ciexyzGreenZ = 0;
            v5hdr.ciexyzBlueX = 0;
            v5hdr.ciexyzBlueY = 0;
            v5hdr.ciexyzBlueZ = 0;
            v5hdr.bV5GammaRed = 0;
            v5hdr.bV5GammaGreen = 0;
            v5hdr.bV5GammaBlue = 0;
            v5hdr.bV5Intent = LCS_GM_GRAPHICS; // GRAPHICS intent, raw pixels please
            v5hdr.bV5ProfileData = 0;
            v5hdr.bV5ProfileSize = 0;
            v5hdr.bV5Reserved = 0;

            // -- UNSAFE
            {
                IntPtr raw = Marshal.AllocHGlobal((int)v5hdr.bV5Size);
                Marshal.StructureToPtr(v5hdr, raw, false);
                Marshal.Copy(raw, final, 0, (int)v5hdr.bV5Size);
                Marshal.FreeHGlobal(raw);
            }
            // -- UNSAFE END

            if (OpenClipboard(IntPtr.Zero))
            {
                if (EmptyClipboard())
                {
                    // -- UNSAFE
                    IntPtr hPng = GlobalAlloc(GMEM_MOVEABLE, new UIntPtr((uint)pngbuff.Length));
                    {
                        IntPtr pPng = GlobalLock(hPng);
                        Marshal.Copy(pngbuff, 0, pPng, pngbuff.Length);
                        GlobalUnlock(pPng);
                    }
                    IntPtr hFinal = GlobalAlloc(GMEM_MOVEABLE, new UIntPtr((uint)final.Length));
                    {
                        IntPtr pFinal = GlobalLock(hFinal);
                        Marshal.Copy(final, 0, pFinal, final.Length);
                        GlobalUnlock(pFinal);
                    }
                    SetClipboardData(RegisterClipboardFormat("PNG"), hPng);
                    SetClipboardData(CF_DIBV5, hFinal);
                    // handles are moved to Windows's ownership.
                    // -- UNSAFE END
                }
                CloseClipboard();
            }
        }
    }
}
