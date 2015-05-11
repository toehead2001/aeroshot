AeroShot is a free (GPLv3) and open source screenshot utility designed for capturing individual windows. It captures screenshots **with full transparency**, such as seen in the Windows Aero visual style. This allows for a clean and professional screenshot, useful for showcasing an application. The entire program is written in C#, heavily utilizing the Windows API.

AeroShot was originally developed by Caleb Joseph in 2011, and was inspired by the proprietary [Window Clippings](http://www.windowclippings.com/) program.

Please leave any feature suggestions or bug reports in the issue tracker.

Downloads are available at the bottom of the page.

## AeroShot Mini
AeroShot is dead, long live **AeroShot Mini**!

AeroShot Mini is the next generation of the original AeroShot, and is based on its Free Software code base. AeroShot Mini runs from the system tray, and doesn't have a main window GUI. A settings window is still available if need.

If you want the original AeroShot experience with the newer features, **AeroShot Classic** is available. See the comparison table below.


Feature | AeroShot Classic | AeroShot Mini
------- | ---------------- | -------------
Send screenshots to the clipboard | **Yes** | **Yes**
Save screenshots to disk | **Yes** | **Yes**
Resize windows when capturing screenshots | **Yes** | **Yes**
Transparent, Opaque, or Checkered backgrounds | **Yes** | **Yes**
Capture mouse pointer in screenshots | **Yes** | **Yes**
Delayed screenshot capture when using hotkey | **Yes** | **Yes**
Disable ClearType while capturing screenshots | **Yes** | **Yes**
Capture screenshots via GUI | **Yes** | No
System tray icon | No | **Yes**

## Usage
Press **Alt + Print Screen** on the desired window to be captured, while AeroShot is running somewhere in the background.

*If you are running* AeroShot Classic *you may also capture screenshots by selecting a window from the drop down list, and clicking the* 'Capture Screenshot' *button*.

Both of these methods will save a transparent or opaque PNG image into the chosen folder with a file name corresponding to the title of the captured window, or to the clipboard if selected. Please be aware that very few applications support pasting transparent images from the clipboard, and that they will most likely not be correctly pasted.

## Screenshots

### AeroShot Mini
![AeroShot Mini](https://aeroshot.ml/gfx/tray.png)

![AeroShot Mini - Settings](https://aeroshot.ml/gfx/settings.png)

### AeroShot Classic
![AeroShot Classic](https://aeroshot.ml/gfx/classic.png)

## Examples
These examples have all been captured with AeroShot, using the "Resize window" option. Note how they have been captured with a transparent background, checkerboards have been added to better show this effect.

![Example 1](https://aeroshot.ml/gfx/eg1.png)

![Example 2](https://aeroshot.ml/gfx/eg2.png)

![Example 3](https://aeroshot.ml/gfx/eg3.png)

## Change Log
### 1.4.0
11th May 2015
##### Both AeroShot Mini & AeroShot Classic
* New program icon. Based on icons by [Yusuke Kamiyamane](http://p.yusukekamiyamane.com/).
* Small adjustments to the GUI style.
* Added option to delay the capturing of screenshots when using the hotkey (1-10 seconds).
* Added option to temporarily disable ClearType on the target window while the screenshot is being captured.

##### AeroShot Mini
* First Release. Runs from system tray icon; screenshot capture via hotkey only.

##### AeroShot Classic
* Use a native control for the 'Capture Screenshot' button.

## Change Log from the original AeroShot

### 1.3.2
23rd October 2012

* Fixed AeroShot not working correctly on Windows XP.
* Improved the list of open windows in the user interface, it should now show all windows open on the taskbar.

### 1.3.1
28th March 2012

* **New capture hotkey**: AeroShot now uses Alt + Print Screen to capture, this resolves conflicts with Windows 8.
* AeroShot's interface now uses Segoe UI, previously Microsoft Sans Serif was used.

### 1.3
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

## Download
**AeroShot Mini** (recommended): [Download](https://aeroshot.ml/releases/?file=asm)

**AeroShot Classic**: [Download](https://aeroshot.ml/releases/?file=asc)

*Note: User of older version of Windows (XP and Vista) will need to have the [.NET Framework 3.5](http://download.microsoft.com/download/0/6/1/061F001C-8752-4600-A198-53214C69B51F/dotnetfx35setup.exe) installed.
Also be aware that I do not test on these older versions of Windows, and they are not offically supported anymore.*
