/*  AeroShot - Transparent screenshot utility for Windows
	Copyright (C) 2015 toe_head2001
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
using System.Text;
using Microsoft.Win32;

namespace AeroShot
{
	public class Settings
	{
        public bool UseDisk;
        public bool UseClipboard;
        public string FolderPath;
		public bool UseOpaqueBackground;
		public byte OpaqueType;
        public string OpaqueColorHex;
        public int CheckerSize = 8;
        public bool UseAeroColor;
		public string AeroColorHex;
		public bool Resize;
		public int WindowHeight = 640;
		public int WindowWidth = 480;
		public bool CaputreMouse;
		public bool DelayCapture;
		public byte DelayCaptureSeconds = 3;
		public bool DisableClearType;
		private readonly RegistryKey _registryKey;


		public Settings()
		{
			object value;
			_registryKey = Registry.CurrentUser.CreateSubKey(@"Software\AeroShot");

			if ((value = _registryKey.GetValue("LastPath")) != null &&
				value.GetType() == (typeof(string)))
			{
				if (((string)value).Substring(0, 1) == "*")
				{
					FolderPath = ((string)value).Substring(1);
					UseClipboard = true;
				}
				else
				{
					FolderPath = (string)value;
					UseDisk = true;
				}
			}
			else
			{
				FolderPath =
					Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
			}

			if ((value = _registryKey.GetValue("WindowSize")) != null &&
				value.GetType() == (typeof(long)))
			{
				var b = new byte[8];
				for (int i = 0; i < 8; i++)
					b[i] = (byte)(((long)value >> (i * 8)) & 0xff);
				Resize = (b[0] & 1) == 1;
				WindowWidth = b[1] << 16 | b[2] << 8 | b[3];
				WindowHeight = b[4] << 16 | b[5] << 8 | b[6];
			}

			if ((value = _registryKey.GetValue("Opaque")) != null &&
				value.GetType() == (typeof(long)))
			{
				var b = new byte[8];
				for (int i = 0; i < 8; i++)
					b[i] = (byte)(((long)value >> (i * 8)) & 0xff);
				UseOpaqueBackground = (b[0] & 1) == 1;
				if ((b[0] & 2) == 2)
					OpaqueType = 0;
				if ((b[0] & 4) == 4)
					OpaqueType = 1;

				CheckerSize = b[1] + 2;

				var hex = new StringBuilder(6);
				hex.AppendFormat("{0:X2}", b[2]);
				hex.AppendFormat("{0:X2}", b[3]);
				hex.AppendFormat("{0:X2}", b[4]);
				OpaqueColorHex = hex.ToString();
			}
			else
				OpaqueType = 0;

			if ((value = _registryKey.GetValue("AeroColor")) != null &&
				value.GetType() == (typeof(long)))
			{
				var b = new byte[8];
				for (int i = 0; i < 8; i++)
					b[i] = (byte)(((long)value >> (i * 8)) & 0xff);
				UseAeroColor = (b[0] & 1) == 1;

				var hex = new StringBuilder(6);
				hex.AppendFormat("{0:X2}", b[1]);
				hex.AppendFormat("{0:X2}", b[2]);
				hex.AppendFormat("{0:X2}", b[3]);
				AeroColorHex = hex.ToString();
			}
			else
				OpaqueType = 0;

			if ((value = _registryKey.GetValue("CapturePointer")) != null &&
				value.GetType() == (typeof(int)))
				CaputreMouse = ((int)value & 1) == 1;

			if ((value = _registryKey.GetValue("ClearType")) != null &&
				value.GetType() == (typeof(int)))
				DisableClearType = ((int)value & 1) == 1;

			if ((value = _registryKey.GetValue("Delay")) != null &&
				value.GetType() == (typeof(long)))
			{
				var b = new byte[8];
				for (int i = 0; i < 8; i++)
					b[i] = (byte)(((long)value >> (i * 8)) & 0xff);
				DelayCapture = (b[0] & 1) == 1;
				DelayCaptureSeconds = b[1];
			}
		}
	}
}