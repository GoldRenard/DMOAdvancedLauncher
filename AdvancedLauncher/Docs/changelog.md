# DMO Advanced Launcher Changelog:

## Version 3.1.120 (hotfix)
 - Fixed application crash on closing
 - GameKing migration for Aeria and Joymax (ranking not available yet)
 - Locale Emulator 2.1.0.0 support (it is compatible with GameGuard)

## Version 3.1
#### Common
 - NT Locale Emulator Advance support (AppLocale not working on Windows 10)
 - Improved UI performance
 - Support for cancellation of logging in
 - "Check for updates" option
 - Joymax guild info grabber improvements
 - Italian translation (thanks to Andrea Milano)

#### Plugin system
 - In order to support plugin system, new modular architecture was implemented using IoC paradigm and Ninject library.
 - For more information about plugin system and SDK please visit [wiki page](https://github.com/GoldRenard/DMOAdvancedLauncher/wiki/Plugin-system).

#### Others
 - BugTrap intergration (automatic error and exception logging, tracking, and reporting)

## Version 3.0
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

## Version 2.3a
##### Common
 - Fixed Aeria authentication
 - Added pt-BR translation. Sorry, I've forgot the author's name :( Anyway, thanks!
 - Workaround for in-game keyboard layout changing ("Fix keyboard layout changing" settings option)
 - Some UI changes and improvements
 - Application console & logging for easy debug and troubleshooting
 - Global application code refactoring and minor bug fixes

##### DigiRotation and Community:
 - Fixed Joymax (nope, FailMax :D) GuildInfo grabbing stability
 - Partial support for future Digimons
 - Updated database

## Version 2.2a
##### Common
 - New "All in one" profile system
 - New Settings/About interface
 - Improvements in almost any module

##### DigiRotation
 - Added feature to disable DigiRotation just by typing empty guild name
 - Added feature to show Digimons of specific Tamer

##### Account authentication
 - Account data can be stored in profile (optional, password will be encrypted)
 - Fixed Aeria authentication

##### Update engine
 - New algorithm. It must be a more stable.
 - Fixed critical bug in filemap structure. If you played Aeria with enabled update engine, please install a fresh copy of Aeria client.

## Version 2.1c
 - Fixed crash in customization caused by unknown file path (resource file with only known ID)

## Version 2.1b
##### Common
 - Fixed: "Last session" was available for Aeria
 - Updated pack of icons (new Tamers & Digimons)

##### Twitter
 - Fixed Twitter user timeline (now use Twitter API v1.1).
 - Is no longer supported easy reading timelines of any user (Twitter API 1.1 requires OAuth authentification)
 - Setting "Twitter username" was replaced with URL to the generated Twitter's JSON (statuses/user_timeline).

## Version 2.1a
 - Fixed SQLite initialization error
 - Fixed DPI of some icons (DigiRotation)
 - Some improvements of update engine
 - Twitter: #HashTags and @UserNames are clickable now
 - Polish translation added (thanks Czip)
 - Minor fixes and improvements

## Version 2.1
 - Added ADMO (Aeria) and KDMO support
 - Added profile system
 - Various bug fixes