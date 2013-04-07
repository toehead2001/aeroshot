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

using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace AeroShot {
    [StructLayout(LayoutKind.Sequential)]
    internal struct PixelData {
        internal byte Blue;
        internal byte Green;
        internal byte Red;
        internal byte Alpha;

        internal void SetAll(byte b) {
            Red = b;
            Green = b;
            Blue = b;
            Alpha = 255;
        }
    }

    internal unsafe class UnsafeBitmap {
        private readonly Bitmap _inputBitmap;
        private BitmapData _bitmapData;
        private byte* _pBase = null;
        private int _width;

        internal UnsafeBitmap(Bitmap inputBitmap) {
            _inputBitmap = inputBitmap;
        }

        internal void LockImage() {
            var bounds = new Rectangle(Point.Empty, _inputBitmap.Size);

            _width = bounds.Width*sizeof (PixelData);
            if (_width%4 != 0)
                _width = 4*(_width/4 + 1);

            //Lock Image
            _bitmapData = _inputBitmap.LockBits(bounds, ImageLockMode.ReadWrite,
                                                PixelFormat.Format32bppArgb);
            _pBase = (byte*) _bitmapData.Scan0.ToPointer();
        }

        internal PixelData* GetPixel(int x, int y) {
            return (PixelData*) (_pBase + y*_width + x*sizeof (PixelData));
        }

        internal void UnlockImage() {
            _inputBitmap.UnlockBits(_bitmapData);
            _bitmapData = null;
            _pBase = null;
        }
    }
}