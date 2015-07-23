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

namespace AeroShot
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;
    using Microsoft.Win32;
    using Gini;

    sealed class Settings
	{
        public bool UseDisk;
        public bool UseClipboard;
        public string FolderPath;
        public Switch<ScreenshotBackgroundType> OpaqueBackgroundType = Switch.Off(ScreenshotBackgroundType.Checkerboard);
        public Color SolidBackgroundColor;
        public int CheckerboardBackgroundCheckerSize = 8;
        public Switch<Color> AeroColor = Switch.Off(Color.White);
        public Switch<Size> ResizeDimensions = Switch.Off(new Size(640, 480));
		public bool CaputreMouse;
        public Switch<TimeSpan> DelayCaptureDuration = Switch.Off(TimeSpan.FromSeconds(3));
		public bool DisableClearType;

        static string _iniFilePath;

        static string IniFilePath
        {
            get { return _iniFilePath ?? (_iniFilePath = Path.Combine(Application.StartupPath, "AeroShot.ini")); }
        }

        public static Settings LoadSettings()
        {
            return File.Exists(IniFilePath)
                 ? LoadSettingsFromIniFile(IniFilePath)
                 : LoadSettingsFromRegistry();
        }

        static Settings LoadSettingsFromIniFile(string path)
        {
            return LoadSettingsFromIni(File.ReadAllText(path));
        }

        static Settings LoadSettingsFromIni(string source)
        {
            var settings = new Settings();

            var ini = Ini.Parse(source)
                         .Where(s => string.IsNullOrEmpty(s.Key))
                         .Select(s => s.ToDictionary(e => e.Key, e => e.Value, StringComparer.OrdinalIgnoreCase))
                         .FirstOrDefault();

            if (ini != null)
            {
                settings.UseDisk          = ReadSetting(ini, "save-device", v => "file".Equals(v, StringComparison.OrdinalIgnoreCase));
                settings.UseClipboard     = !settings.UseDisk;
                settings.FolderPath       = ReadSetting(ini, "save-file-path", Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), s => s);
                settings.OpaqueBackgroundType
                                          = Switch.Create(ReadSetting(ini, "use-opaque-background", Truthy.Parse),
                                                          ReadSetting(ini, "opaque-background-type", v => "solid".Equals(v, StringComparison.OrdinalIgnoreCase)
                                                                                                          ? ScreenshotBackgroundType.SolidColor
                                                                                                          : ScreenshotBackgroundType.Checkerboard));
                settings.SolidBackgroundColor
                                          = ReadSetting(ini, "solid-background-color", settings.SolidBackgroundColor, ColorTranslator.FromHtml);
                settings.CheckerboardBackgroundCheckerSize
                                          = ReadSetting(ini, "checkerboard-background-checker-size", settings.CheckerboardBackgroundCheckerSize, ParseInt);
                settings.AeroColor        = Switch.Create(ReadSetting(ini, "use-aero-color", Truthy.Parse),
                                                          ReadSetting(ini, "aero-color", settings.AeroColor.Value, ColorTranslator.FromHtml));
                settings.ResizeDimensions = Switch.Create(ReadSetting(ini, "use-window-resize-dimensions", Truthy.Parse),
                                                          new Size(ReadSetting(ini, "resize-window-width", settings.ResizeDimensions.Value.Width, ParseInt),
                                                                   ReadSetting(ini, "resize-window-height", settings.ResizeDimensions.Value.Height, ParseInt)));
                settings.DelayCaptureDuration
                                          = Switch.Create(ReadSetting(ini, "use-delay-capture", Truthy.Parse),
                                                          ReadSetting(ini, "delay-capture-seconds", settings.DelayCaptureDuration.Value, v => TimeSpan.FromSeconds(ParseInt(v))));
                settings.CaputreMouse     = ReadSetting(ini, "capture-pointer", Truthy.Parse);
                settings.DisableClearType = ReadSetting(ini, "disable-clear-type", Truthy.Parse);
            }

            return settings;
        }

        static T ReadSetting<T>(IDictionary<string, string> section, string key, Func<string, T> selector)
        {
            return ReadSetting(section, key, default(T), selector);
        }

        static T ReadSetting<T>(IDictionary<string, string> section, string key, T defaultValue, Func<string, T> selector)
        {
            string value;
            if (section.TryGetValue(key, out value))
                value = value.Trim();
            return !string.IsNullOrEmpty(value) ? selector(value) : defaultValue;
        }

        static int ParseInt(string s) { return int.Parse(s, NumberStyles.None, CultureInfo.InvariantCulture); }

        static class Truthy
        {
            static readonly Regex Regex = new Regex(@"^\s*(true|yes|on|1)\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            public static bool Parse(string s) { return Regex.IsMatch(s); }
        }

        public static void SaveSettings(Settings settings)
        {
            File.WriteAllText(IniFilePath, SettingsIni(settings));
            // TODO Handle UnauthorizedAccessException
        }

        static string SettingsIni(Settings settings)
        {
            var section = new[]
            {
                Setting("app-version"                         , Application.ProductVersion),
                Setting("save-device"                         , settings.UseClipboard ? "clipboard" : "file"),
                Setting("save-file-path"                      , settings.FolderPath),
                Setting("use-opaque-background"               , settings.OpaqueBackgroundType.Convert(_ => true)),
                Setting("opaque-background-type"              , settings.OpaqueBackgroundType.Value == ScreenshotBackgroundType.SolidColor ? "solid" : "checkerboard"),
                Setting("solid-background-color"              , ColorTranslator.ToHtml(settings.SolidBackgroundColor)),
                Setting("checkerboard-background-checker-size", settings.CheckerboardBackgroundCheckerSize),
                Setting("use-aero-color"                      , settings.AeroColor.Convert(_ => true)),
                Setting("aero-color"                          , ColorTranslator.ToHtml(settings.AeroColor.Value)),
                Setting("use-window-resize-dimensions"        , settings.ResizeDimensions.Convert(_ => true)),
                Setting("resize-window-width"                 , settings.ResizeDimensions.Value.Width),
                Setting("resize-window-height"                , settings.ResizeDimensions.Value.Height),
                Setting("capture-pointer"                     , settings.CaputreMouse),
                Setting("use-delay-capture"                   , settings.DelayCaptureDuration.Convert(_ => true)),
                Setting("delay-capture-seconds"               , settings.DelayCaptureDuration.Value.TotalSeconds),
                Setting("disable-clear-type"                  , settings.DisableClearType),
            };

            return string.Join(Environment.NewLine,
                               Ini.Format(new[] { section }, e => null, e => e, e => e)
                                  .ToArray());
        }

        static KeyValuePair<string, string> Setting(string key, string value)
        {
            return KeyValuePair.Create(key, value ?? string.Empty);
        }

        static KeyValuePair<string, string> Setting(string key, bool value)
        {
            return KeyValuePair.Create(key, value ? "true" : "false");
        }

        static KeyValuePair<string, string> Setting(string key, object value)
        {
            return Setting(key, value == null
                              ? string.Empty
                              : Convert.ToString(value, CultureInfo.InvariantCulture));
        }

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
	}

    static class KeyValuePair
    {
        public static KeyValuePair<TKey, TValue> Create<TKey, TValue>(TKey key, TValue value)
        {
            return new KeyValuePair<TKey, TValue>(key, value);
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

#region The MIT License (MIT)
//
// Copyright (c) 2015 Atif Aziz. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
#endregion

namespace Gini
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    partial class Ini
    {
        public static IEnumerable<string> Format<TSection, TEntry>(
            IEnumerable<TSection> sections,
            Func<TSection, string> sectionNameSelector,
            Func<TSection, IEnumerable<TEntry>> entriesSelector,
            Func<TEntry, KeyValuePair<string, string>> entrySelector)
        {
            if (sections == null) throw new ArgumentNullException("sections");
            if (sectionNameSelector == null) throw new ArgumentNullException("sectionNameSelector");
            if (entriesSelector == null) throw new ArgumentNullException("entriesSelector");
            if (entrySelector == null) throw new ArgumentNullException("entrySelector");

            return
                from section in sections
                select new
                {
                    Name = sectionNameSelector(section),
                    Entries = entriesSelector(section),
                }
                into section
                from lines in new[]
                {
                    new[] { !string.IsNullOrEmpty(section.Name) ? "[" + section.Name + "]" : null },
                    from entry in section.Entries
                    select entrySelector(entry) into entry
                    select entry.Key + "=" + entry.Value,
                    new[] { string.Empty },
                }
                from line in lines
                where line != null
                select line;
        }
    }
}