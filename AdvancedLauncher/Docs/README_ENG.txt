// ======================================================================
// DIGIMON MASTERS ONLINE ADVANCED LAUNCHER
// Copyright (C) 2015 Ilya Egorov (goldrenard@gmail.com)

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
// ======================================================================

1. Overview and features:

DMO Advanced Launcher supports ADMO (Aeria), GDMO (Joymax), KDMO (www.digimonmasters.com and IMBC) servers. But not all features are supported for all servers. For example, Digimon Rotation and Community page not available for ADMO.
Let's make "marks" to define support features for a each server:
[A] - supported for Aeria mode;
[G] - supported for Joymax mode;
[K] - supported for Korean mode (both www.digimonmasters.com and IMBC);
[S] - supported for any mode.

Main features:
● [G|K] Rotation of top Digimons of your guild
● [G|K] Stay Up-to-Date! Getting information about any guild (ranking, reputation, list of tamers, digimons, etc)
● [A|G] Joymax news
● [S] Twitter statuses loaded by Twitter API 1.1 JSON user timeline
● [S] Checking and installing game updates (beta)*
● [S] Screenshot gallery
● [S] Let the game meets. Possibility of change some game resources like login background, dialogs, etc.
● [S] Launching game through AppLocale (fixing codepage troubles like bad cyrillic at guild chat)
● [S] Profile system. You can make many profiles for every server/account/etc.
* Update feature on the BETA stage and it is DISABLED by default. I'm still not sure that I did everything absolutely correctly. Use that at your own risk.
With disabled option checking updates still works, but will be used default launcher for updating.

2. System requiremets:
● Windows® XP (SP3) / Vista (SP1+) / 7 / 8
● Microsoft .NET Framework 4.0

3. Installing:
1. Download and install the latest version of launcher: https://sourceforge.net/projects/gdmolauncher/
2. If you have Windows XP and want use AppLocale, you MUST install East Asian Language support (http://www.voom.net/install-files-for-east-asian-languages-windows-xp), then turn on the AppLocale in options of launcher.

4. Help with Translations:
We'll glad to getting help in the translation of the launcher into other languages. Look at the folder "Languages" of root launcher's folder. Use "Template_for_translation.txt" as template. Also you can correct my English translation. I know that current is bad xD
Please send an email with new translations.
All translations I will add to Translations page and include into installer in future.

5. Known issues:
Community page and DigiRotation uses information of game website. Not all digimons are provided at ranking or tamer's information pages (especially at Joymax). So not all digimons provided at our launcher.
In additional, launcher can read only ONE digimon of ONE type from website. So if you have two Impmons then only ONE will be added to database of launcher.

6. Source code:
It's a Open source project under GNU GPL 3.0, so all paranoids may check whole code before using the launcher ;)
You can download source code from Git: https://sourceforge.net/p/gdmolauncher/code/