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
using System.Drawing;
using Microsoft.Win32;

namespace AeroShot
{
    sealed class Settings
	{
        public bool UseDisk;
        public bool UseClipboard;
        public string FolderPath;
        public Switch<ScreenshotBackgroundType> OpaqueBackgroundType;
        public Color SolidBackgroundColor;
        public int CheckerboardBackgroundCheckerSize = 8;
        public Switch<Color> AeroColor;
        public Switch<Size> ResizeDimensions = Switch.Off(new Size(640, 480));
		public bool CaputreMouse;
        public Switch<TimeSpan> DelayCaptureDuration = Switch.Off(TimeSpan.FromSeconds(3));
		public bool DisableClearType;

        static RegistryKey CreateRegistryKey()
        {
            return Registry.CurrentUser.CreateSubKey(@"Software\AeroShot");
        }

        public static Settings LoadSettingsFromRegistry()
        {
            using (var registryKey = CreateRegistryKey())
                return LoadSettings(registryKey);
        }

        public static Settings LoadSettings(RegistryKey registryKey)
        {
            var settings = new Settings();

            object value;

			if ((value = registryKey.GetValue("LastPath")) != null &&
				value.GetType() == (typeof(string)))
			{
				if (((string)value).Substring(0, 1) == "*")
				{
				    settings.FolderPath = ((string)value).Substring(1);
				    settings.UseClipboard = true;
				}
				else
				{
				    settings.FolderPath = (string)value;
				    settings.UseDisk = true;
				}
			}
			else
			{
			    settings.FolderPath =
					Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
			}

			if ((value = registryKey.GetValue("WindowSize")) != null &&
				value.GetType() == (typeof(long)))
			{
				var b = new byte[8];
				for (int i = 0; i < 8; i++)
					b[i] = (byte)(((long)value >> (i * 8)) & 0xff);
				var use = (b[0] & 1) == 1;
				var width = b[1] << 16 | b[2] << 8 | b[3];
				var height = b[4] << 16 | b[5] << 8 | b[6];
			    settings.ResizeDimensions = Switch.Create(use, new Size(width, height));
			}

            var defaultOpaqueBackgroundType = Switch.Off(ScreenshotBackgroundType.Checkerboard);

			if ((value = registryKey.GetValue("Opaque")) != null &&
				value.GetType() == (typeof(long)))
			{
				var b = new byte[8];
				for (int i = 0; i < 8; i++)
					b[i] = (byte)(((long)value >> (i * 8)) & 0xff);
				var use = (b[0] & 1) == 1;
                var type = (b[0] & 2) == 2 ? ScreenshotBackgroundType.Checkerboard
                         : (b[0] & 4) == 4 ? ScreenshotBackgroundType.SolidColor
                         : ScreenshotBackgroundType.Transparent;
			    settings.OpaqueBackgroundType = Switch.Create(use, type);
			    settings.CheckerboardBackgroundCheckerSize = b[1] + 2;
			    settings.SolidBackgroundColor = Color.FromArgb(b[2], b[3], b[4]);
			}
			else
			    settings.OpaqueBackgroundType = defaultOpaqueBackgroundType;

			if ((value = registryKey.GetValue("AeroColor")) != null &&
				value.GetType() == (typeof(long)))
			{
				var b = new byte[8];
				for (int i = 0; i < 8; i++)
					b[i] = (byte)(((long)value >> (i * 8)) & 0xff);
				var use = (b[0] & 1) == 1;
			    settings.AeroColor = Switch.Create(use, Color.FromArgb(b[1], b[2], b[3]));
			}
			else
			    settings.OpaqueBackgroundType = defaultOpaqueBackgroundType;

			if ((value = registryKey.GetValue("CapturePointer")) != null &&
				value.GetType() == (typeof(int)))
			    settings.CaputreMouse = ((int)value & 1) == 1;

			if ((value = registryKey.GetValue("ClearType")) != null &&
				value.GetType() == (typeof(int)))
			    settings.DisableClearType = ((int)value & 1) == 1;

			if ((value = registryKey.GetValue("Delay")) != null &&
				value.GetType() == (typeof(long)))
			{
				var b = new byte[8];
				for (int i = 0; i < 8; i++)
					b[i] = (byte)(((long)value >> (i * 8)) & 0xff);
				var use = (b[0] & 1) == 1;
			    settings.DelayCaptureDuration = Switch.Create(use, TimeSpan.FromSeconds(b[1]));
			}

            return settings;
		}

        public static void SaveSettingsToRegistry(Settings settings)
        {
            using (var registryKey = CreateRegistryKey())
                SaveSettings(settings, registryKey);
        }

        public static void SaveSettings(Settings settings, RegistryKey registryKey)
        {
            registryKey.SetValue("LastPath", (settings.UseClipboard ? "*" : null) + settings.FolderPath);

            // Save resizing settings in an 8-byte long
            var b = new byte[8];
            b[0] = settings.ResizeDimensions.Convert(_ => (byte) 1);

            var x = settings.ResizeDimensions.Value.Width;
            b[3] = (byte)(x & 0xff);
            b[2] = (byte)((x >> 8) & 0xff);
            b[1] = (byte)((x >> 16) & 0xff);

            var y = settings.ResizeDimensions.Value.Height;
            b[6] = (byte)(y & 0xff);
            b[5] = (byte)((y >> 8) & 0xff);
            b[4] = (byte)((y >> 16) & 0xff);

            var data = BitConverter.ToInt64(b, 0);
            registryKey.SetValue("WindowSize", data, RegistryValueKind.QWord);

            // Save background color settings in an 8-byte long
            b = new byte[8];
            b[0] = settings.OpaqueBackgroundType.Convert(_ => (byte) 1);
            b[0] += (byte)Math.Pow(2, settings.OpaqueBackgroundType.Value == ScreenshotBackgroundType.SolidColor ? 2 : 1);

            b[1] = (byte)(settings.CheckerboardBackgroundCheckerSize - 2);

            b[2] = settings.SolidBackgroundColor.R;
            b[3] = settings.SolidBackgroundColor.G;
            b[4] = settings.SolidBackgroundColor.B;

            data = BitConverter.ToInt64(b, 0);
            registryKey.SetValue("Opaque", data, RegistryValueKind.QWord);

            // Save background color settings in an 8-byte long
            b = new byte[8];
            b[0] = settings.AeroColor.Convert(_ => (byte) 1);

            b[1] = settings.AeroColor.Value.R;
            b[2] = settings.AeroColor.Value.G;
            b[3] = settings.AeroColor.Value.B;

            data = BitConverter.ToInt64(b, 0);
            registryKey.SetValue("AeroColor", data, RegistryValueKind.QWord);

            registryKey.SetValue("CapturePointer", settings.CaputreMouse ? 1 : 0,
                                  RegistryValueKind.DWord);

            registryKey.SetValue("ClearType", settings.DisableClearType ? 1 : 0,
                      RegistryValueKind.DWord);

            // Save delay settings in an 8-byte long
            b = new byte[8];
            b[0] = settings.DelayCaptureDuration.Convert(_ => (byte) 1);

            b[1] = (byte)(((int) settings.DelayCaptureDuration.Value.TotalSeconds) & 0xff);

            data = BitConverter.ToInt64(b, 0);
            registryKey.SetValue("Delay", data, RegistryValueKind.QWord);
        }
	}

    static class HexColor
    {
        public static string Encode(Color color)
        {
            return color.IsEmpty ? null : ColorTranslator.ToHtml(color).Substring(1);
        }
    }
}