
AeroShot was orginal developed by Caleb Joseph during 2011-2013 as a free and open source screenshot utility designed for capturing individual windows. It captures screenshots **with full transparency**, such as seen in the Windows Aero visual style. This allows for a clean and professional screenshot, useful for showcasing an application. The entire program is written in C#, heavily utilizing the Windows API. AeroShot was inspired from the proprietary [Window Clippings](http://www.windowclippings.com/).


## 'AeroShot Mini' and 'AeroShot Classic'

Building on the Free Software code base of the original AeroShot, **AeroShot Mini** and **AeroShot Classic** are it's unoffical continuation. See Changelog below.

**AeroShot Classic** is the AeroShot you know and love, but with added functionality and UI enhancements.

**AeroShot Mini** runs from the system tray, and doesn't have a main window UI. A settings window can still be opened when needed. Development will be focused here.

Feature | AeroShot Classic | AeroShot Mini
------- | ---------------- | -------------
Send screenshots to clipboard | **Yes**  | **Yes**
Save screenshots to disk | **Yes**  | **Yes**
Automaticaly resize windows before capturing screenshots | **Yes**  | **Yes**
Transparent/Colored/Checkerd background on screenshots | **Yes**  | **Yes**
Capture mouse pointer in screenshots | **Yes**  | **Yes**
Delayed screenshot capture | **Yes**  | **Yes**
Screenshot capture via hotkey | **Yes**  | **Yes**
Screenshot capture via GUI | **Yes**  | No
System Tray icon | No  | **Yes**

Users of oldler versions of Windows (Windows XP and Vista) will need to have have the [.NET Framework 3.5](http://download.microsoft.com/download/0/6/1/061F001C-8752-4600-A198-53214C69B51F/dotnetfx35setup.exe) installed.

Please leave any feedback, feature suggestions or bug reports in the issue tracker and star the project if you find AeroShot useful!

The latest version is 1.3.2, a change log can be found [here](http://code.google.com/p/aeroshot/#Change_Log).
### [Download AeroShot](http://file/path/AeroShot.exe) ###

## Usage
Press **Alt + Print Screen** on the desired window to be captured, while AeroShot is running somewhere in the background.

*If you are running* AeroShot Classic *you can also capture a screenshot by selecting a window from the drop down list, and clicking the* 'Capture Screenshot' *button.*

Both of these methods will save a transparent or opaque PNG image into the chosen folder with a file name corresponding to the title of the captured window, or to the clipboard if selected. Please be aware that very few applications support pasting transparent images from the clipboard, and that they will most likely not be correctly pasted.

## Screenshots

### AeroShot Classic

![AeroShot Classic](https://aeroshot.ml/gfx/classic.png)

### AeroShot Mini

![AeroShot Mini](https://aeroshot.ml/gfx/tray.png)

![AeroShot Mini - Settings](https://aeroshot.ml/gfx/settings.png)

## Examples
These examples have all been captured with AeroShot, using the "Resize window" option. Note how they have been captured with a transparent background, checkerboards have been added to better show this effect.

![http://i43.tinypic.com/4h5tl1.png](http://i43.tinypic.com/4h5tl1.png)

![http://i43.tinypic.com/nyiaes.png](http://i43.tinypic.com/nyiaes.png)

![http://i41.tinypic.com/2jj3k.png](http://i41.tinypic.com/2jj3k.png)

## Change Log
### 1.4.0
6th October 2012

  * Fixed AeroShot not working correctly on Windows XP.
  * Improved the list of open windows in the user interface, it should now show all windows open on the taskbar.

### 1.3.2
23rd October 2012

  * Fixed AeroShot not working correctly on Windows XP.
  * Improved the list of open windows in the user interface, it should now show all windows open on the taskbar.


### 1.3.1
28th March 2012

  * **New capture hotkey**: AeroShot now uses Alt + Print Screen to capture, this resolves conflicts with Windows 8.
  * AeroShot's interface now uses Segoe UI, previously Microsoft Sans Serif was used.


### 1.3.0
31st December 2011

  * **Capture to Clipboard**: Screenshots may now be copied to the clipboard instead of being saved to the disk.
  * **Show Mouse Pointer**: The mouse pointer may now be visible in screenshots when using Windows Key + Print Screen.
**Changes**:
  * Improved how windows are focused on capture.
  * Improved how the taskbar is hidden and restored on capture.
  * "Take Screenshot" button replaced with a custom button.


### 1.2.1
8th November 2011

**Changes**:
  * Slightly faster image processing


### 1.2
10th August 2011

**Changes**:
  * Taskbar is hidden during capture, this fixes issues when taking screenshots of maximized windows.
  * Increased the margin of the capture area from 40 pixels to 100 pixels. This improves capturing of custom themes with large window drop shadows.


### 1.1
13th June 2011

  * **Image Cropping**: Transparent areas of screenshots are now automatically cropped out.
  * **Window Resizing**: You may now specify a resolution you wish your screenshot to be captured in. The selected window will be automatically resized to fit the resolution chosen. This is useful for web design where you need screenshots to be of an exact size.
  * **Opaque Backgrounds**: Lets you save screenshots with a solid background, rather than a transparent one.
  * **Interface Improvements**: The main GUI has been cleaned up.
  * **Improved Capturing**: Screenshots are now taken much more faster and reliably.
  * **Windows XP support**: AeroShot will now function flawlessly on Windows XP.


### 1.0
5th June 2011

  * **Initial release**
