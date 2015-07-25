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
using System.Threading;
using System.Windows.Forms;

namespace AeroShot
{
	public class Hotkeys : Form
	{
		private const int WM_HOTKEY = 0x0312;
		private const int MOD_ALT = 0x0001;
		private const int MOD_CONTROL = 0x0002;
		private readonly int[] _windowId;
		private Thread _worker;
		private bool _busyCapturing;

		public Hotkeys()
		{
			_windowId = new[] { GetHashCode(), GetHashCode() ^ 327 };
			WindowsApi.RegisterHotKey(Handle, _windowId[0], MOD_ALT, (int)Keys.PrintScreen);
			WindowsApi.RegisterHotKey(Handle, _windowId[1], MOD_ALT | MOD_CONTROL, (int)Keys.PrintScreen);

			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormClose);
		}

		private void FormClose(object sender, FormClosingEventArgs e)
		{
			foreach (var id in _windowId)
			{
				WindowsApi.UnregisterHotKey(Handle, id);
			}            
		}

		protected override void WndProc(ref Message m)
		{
			base.WndProc(ref m);
			if (m.Msg == WM_HOTKEY)
                OnHotKey(m);
		}

        void OnHotKey(Message message)
		{
			if (_busyCapturing)
				return;

            var settings = Settings.LoadSettings();

            var info = new ScreenshotTask(
			    WindowsApi.GetForegroundWindow(),
			    settings.UseClipboard,
			    settings.FolderPath,
			    settings.ResizeDimensions.Convert(v => (Size?) v),
			    settings.OpaqueBackgroundType.GetValueOrDefault(),
			    settings.SolidBackgroundColor,
			    settings.CheckerboardBackgroundCheckerSize,
			    settings.AeroColor.Convert(v => (Color?) v),
			    settings.CaputreMouse,
			    settings.DisableClearType);

            var CtrlAlt = (message.LParam.ToInt32() & (MOD_ALT | MOD_CONTROL)) == (MOD_ALT | MOD_CONTROL);
			_busyCapturing = true;
			_worker = new Thread(() =>
			{
                if (CtrlAlt)
                    Thread.Sleep(TimeSpan.FromSeconds(3));
                else
                    settings.DelayCaptureDuration.WhenOnThen(Thread.Sleep);
				try
				{
					Screenshot.CaptureWindow(ref info);
				}
				finally
				{
					_busyCapturing = false;
				}
			})
			{
				IsBackground = true
			};
			_worker.SetApartmentState(ApartmentState.STA);
			_worker.Start();
		}
	}
}