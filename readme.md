## DMO Advanced Launcher

Hello! We would like to present to you the custom Digimon Masters Online\* launcher. We invested all our effort and soul into its creation. It doesn't look like a default launcher. It is: beautiful, comfortable, functional.

[![Build status](https://ci.appveyor.com/api/projects/status/7iucftekgn7frt25?svg=true)](https://ci.appveyor.com/project/GoldRenard/dmoadvancedlauncher)
[![Release](https://img.shields.io/github/release/GoldRenard/DMOAdvancedLauncher.svg?style=flat-square)](https://github.com/GoldRenard/DMOAdvancedLauncher/releases/latest)

![preview](https://raw.githubusercontent.com/GoldRenard/DMOAdvancedLauncher/master/Shared/Assets/preview.png)

### Overview and features
**DMO Advanced Launcher** supports ADMO ([Aeria](http://www.aeriagames.com/playnow/dmus/)), GDMO ([Joymax](http://www.joymax.com/dmo/)), KDMO ([www.digimonmasters.com](www.digimonmasters.com) and [IMBC](http://dm.imbc.com/)) servers. Unfortunately, not all features are supported for all servers. For example, Digimon Rotation and Community pages aren't available for ADMO.
Functionality is distributed into different categories:

- [A] - supported for Aeria mode;
- [G] - supported for Joymax mode;
- [K] - supported for Korean mode (both www.digimonmasters.com and IMBC);
- [S] - supported for any mode.

#### Main features:
- [G|K] Rotation of top Digimons of your guild
- [G|K] Stay Up-to-Date! Getting information about any guild (ranking, reputation, list of tamers, digimons, etc)
- [G] Joymax news
- [S] Twitter statuses loaded by Twitter API 1.1 JSON user timeline
- [S] Checking and installing game updates (disabled by default)
- [S] Screenshot gallery
- [S] Let the game meets. Possibility of change some game resources like login background, dialogs, etc.
- [S] Launching game through AppLocale (fixing codepage troubles like bad cyrillic at guild chat)
- [S] Profile system. You can make many profiles for every server/account/etc.

### System Requirements
- Windows Vista SP2 / Windows 7 SP1 / Windows 8 / Windows 8.1
- Microsoft .NET Framework 4.5

### Help with Translations
We will be happy to get your help with translation the launcher into other languages. Look at the [Languages](https://github.com/GoldRenard/DMOAdvancedLauncher/tree/master/AdvancedLauncher/Languages) folder of root launcher's folder. Use [Template_for_translation.txt](https://github.com/GoldRenard/DMOAdvancedLauncher/blob/master/AdvancedLauncher/Languages/Template_for_translation.txt) as template. Also you can correct my English translation. I know that current is bad xD
Please [send an email](mailto:goldrenard@gmail.com) with your translations.

### Known issues
- Community page and DigiRotation uses information of game website. You know, not all digimons are provided at ranking or tamer's information pages (especially at ~~Failmax~~Joymax). So not all digimons provided at our launcher.
- Moreover, launcher can get only ONE digimon of ONE type from website. So if you have two Impmons then only ONE will be added to database of launcher. Usually it's a latest hatched Digimon.
- Long logon process for KDMO IMBC. Error 705 is ok, you should just wait for successful login. If you're getting error 705 10 or more times, check your account data.

## Changelog (Version 3.0)
#### Common
 - New interface based on great [MahApps.Metro](http://mahapps.com/).
 - Appearance: dark/light themes with different color schemes (thanks to MahApps.Metro)
 - Implemented proxy-server support
 - Indonesian translation (thanks to Bayu Laksana)
 - Global refactoring and clean up which means different bug fixes (and might also means new bugs o_O, but we'll fix them all!)

#### Others
 - Every third-party dependency was updated to actual version
 - Migration to Entity Framework + SQLCE database management
 - Migration to .NET Framework 4.5

You can read the full changelog [here](https://github.com/GoldRenard/DMOAdvancedLauncher/blob/master/AdvancedLauncher/Docs/changelog.md).
 
### Licence
[GNU GENERAL PUBLIC LICENSE](https://www.gnu.org/copyleft/gpl.html).

<sup>Digimon: © Akiyoshi Hongo, Toei Animation.
DIGIMON, DIGITAL MONSTER and all related logos, names and distinctive likenesses thereof are the property of Bandai/Toei Animation. DIGIMON is a registered trademark of Bandai. All other trademarks are the property of their respective owners.</sup>
