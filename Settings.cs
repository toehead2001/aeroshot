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
		public bool FirstRun;
		public int CheckerValue = 8;
		public string OpaqueColorHexBox;
		public string FolderTextBox;
		public bool OpaqueCheckbox;
		public byte OpaqueType;
		public bool AeroColorCheckbox;
		public string AeroColorHexBox;
		public bool ResizeCheckbox;
		public int WindowHeight = 640;
		public int WindowWidth = 480;
		public bool DiskButton;
		public bool ClipboardButton;
		public bool MouseCheckbox;
		public bool DelayCheckbox;
		public byte DelaySeconds = 3;
		public bool ClearTypeCheckbox;
		private readonly RegistryKey _registryKey;


		public Settings()
		{
			object value;
			_registryKey = Registry.CurrentUser.CreateSubKey(@"Software\AeroShot");

			if ((value = _registryKey.GetValue("FirstRun")) == null)
			{
				FirstRun = true;
			}


			if ((value = _registryKey.GetValue("LastPath")) != null &&
				value.GetType() == (typeof(string)))
			{
				if (((string)value).Substring(0, 1) == "*")
				{
					FolderTextBox = ((string)value).Substring(1);
					ClipboardButton = true;
				}
				else
				{
					FolderTextBox = (string)value;
					DiskButton = true;
				}
			}
			else
			{
				FolderTextBox =
					Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
			}

			if ((value = _registryKey.GetValue("WindowSize")) != null &&
				value.GetType() == (typeof(long)))
			{
				var b = new byte[8];
				for (int i = 0; i < 8; i++)
					b[i] = (byte)(((long)value >> (i * 8)) & 0xff);
				ResizeCheckbox = (b[0] & 1) == 1;
				WindowWidth = b[1] << 16 | b[2] << 8 | b[3];
				WindowHeight = b[4] << 16 | b[5] << 8 | b[6];
			}

			if ((value = _registryKey.GetValue("Opaque")) != null &&
				value.GetType() == (typeof(long)))
			{
				var b = new byte[8];
				for (int i = 0; i < 8; i++)
					b[i] = (byte)(((long)value >> (i * 8)) & 0xff);
				OpaqueCheckbox = (b[0] & 1) == 1;
				if ((b[0] & 2) == 2)
					OpaqueType = 0;
				if ((b[0] & 4) == 4)
					OpaqueType = 1;

				CheckerValue = b[1] + 2;

				var hex = new StringBuilder(6);
				hex.AppendFormat("{0:X2}", b[2]);
				hex.AppendFormat("{0:X2}", b[3]);
				hex.AppendFormat("{0:X2}", b[4]);
				OpaqueColorHexBox = hex.ToString();
			}
			else
				OpaqueType = 0;

			if ((value = _registryKey.GetValue("AeroColor")) != null &&
				value.GetType() == (typeof(long)))
			{
				var b = new byte[8];
				for (int i = 0; i < 8; i++)
					b[i] = (byte)(((long)value >> (i * 8)) & 0xff);
				AeroColorCheckbox = (b[0] & 1) == 1;

				var hex = new StringBuilder(6);
				hex.AppendFormat("{0:X2}", b[1]);
				hex.AppendFormat("{0:X2}", b[2]);
				hex.AppendFormat("{0:X2}", b[3]);
				AeroColorHexBox = hex.ToString();
			}
			else
				OpaqueType = 0;

			if ((value = _registryKey.GetValue("CapturePointer")) != null &&
				value.GetType() == (typeof(int)))
				MouseCheckbox = ((int)value & 1) == 1;

			if ((value = _registryKey.GetValue("ClearType")) != null &&
				value.GetType() == (typeof(int)))
				ClearTypeCheckbox = ((int)value & 1) == 1;

			if ((value = _registryKey.GetValue("Delay")) != null &&
				value.GetType() == (typeof(long)))
			{
				var b = new byte[8];
				for (int i = 0; i < 8; i++)
					b[i] = (byte)(((long)value >> (i * 8)) & 0xff);
				DelayCheckbox = (b[0] & 1) == 1;
				DelaySeconds = b[1];
			}
		}
	}
}