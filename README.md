# Shibari
Management layer for AirBender & FireShock device drivers

![Disclaimer](http://nefarius.at/public/Alpha-Disclaimer.png)

## Prerequisites
 * Microsoft Windows **8.1/10** x86 or x64
 * Microsoft Visual C++ Redistributable for Visual Studio 2017 ([x64](https://go.microsoft.com/fwlink/?LinkId=746572), [x86](https://go.microsoft.com/fwlink/?LinkId=746571))
 * [.NET Framework 4.5.2](https://www.microsoft.com/en-ca/download/details.aspx?id=42642) (already included in Windows 8.1/10)
 * [Windows Management Framework 5.1](https://docs.microsoft.com/en-us/powershell/wmf/5.1/install-configure) (already included in Windows 10)

## Installation guide for the brave
If you can follow a cooking recipe the following section should be a piece of cake. If you can't cook, well, learn it, it's about time! Alright, here we go.

### Get all the files
**Important:** after you downloaded the archives, [make sure to unblock them](https://blogs.msdn.microsoft.com/delay/p/unblockingdownloadedfile/) *before* extraction!

 * Get the latest `Shibari.zip` [from here](https://buildbot.vigem.org/builds/Shibari/master/) (always pick the highest version number for most recent release)
 * Get the `devcon` command line utility [from here](https://downloads.vigem.org/other/microsoft/devcon.zip)
 * Get the latest `FireShock.zip` [from here](https://downloads.vigem.org/projects/FireShock/stable/)
   * This is required for USB connection
 * Get the latest `AirBender.zip` [from here](https://downloads.vigem.org/projects/AirBender/stable/)
   * This is required for Bluetooth connection
   * **Important:** currently suffers from connection issues on some systems, use with care
 * Unblock all the archives
 * Extract the contents to a location of your choice 
   * Note for the mentally challenged: **don't** put it in `system32`...
