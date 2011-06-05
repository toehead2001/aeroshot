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

namespace AeroShot
{
	public unsafe class UnsafeBitmap
	{
		private readonly Bitmap _inputBitmap;
		private BitmapData _bitmapData;
		private Byte* _pBase = null;
		private PixelData* _pixelData = null;
		private int _width;

		private struct PixelData
		{
			public byte Red;
			public byte Green;
			public byte Blue;
			public byte Alpha;
		}

		public UnsafeBitmap(Bitmap inputBitmap)
		{
			_inputBitmap = inputBitmap;
		}

		public void LockImage()
		{
			var bounds = new Rectangle(Point.Empty, _inputBitmap.Size);

			_width = bounds.Width*sizeof (PixelData);
			if (_width%4 != 0) _width = 4*(_width/4 + 1);

			//Lock Image
			_bitmapData = _inputBitmap.LockBits(bounds, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
			_pBase = (Byte*) _bitmapData.Scan0.ToPointer();
		}

		public byte[] GetPixel(int x, int y)
		{
			_pixelData = (PixelData*) (_pBase + y*_width + x*sizeof (PixelData));
			return new[] {_pixelData->Red, _pixelData->Green, _pixelData->Blue, _pixelData->Alpha};
		}

		public void SetPixel(int x, int y, byte[] p)
		{
			var data = (PixelData*) (_pBase + y*_width + x*sizeof (PixelData));
			data->Red = p[0];
			data->Green = p[1];
			data->Blue = p[2];
			data->Alpha = p[3];
		}

		public void UnlockImage()
		{
			_inputBitmap.UnlockBits(_bitmapData);
			_bitmapData = null;
			_pBase = null;
		}
	}
}