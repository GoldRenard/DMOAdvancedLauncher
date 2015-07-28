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

using System;
using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.UI.Extension;
using Ninject;

namespace AdvancedLauncher.Tools {

    public static class URLUtils {
        public const string REMOTE_VERSION_FILE = "https://raw.githubusercontent.com/GoldRenard/DMOAdvancedLauncher/master/version.xml";
        public const string COMMUNITY_IMAGE_REMOTE_FORMAT = "https://raw.githubusercontent.com/GoldRenard/DMOAdvancedLauncher/master/AdvancedLauncher/Resources/Community/{0}.png";
        public const string DIGIROTATION_IMAGE_REMOTE_FORMAT = "https://raw.githubusercontent.com/GoldRenard/DMOAdvancedLauncher/master/AdvancedLauncher/Resources/DigiRotation/{0}.png";
        public const string DEFAULT_TWITTER_SOURCE = "http://renamon.ru/launcher/dmor_timeline.php";

        /// <summary> Opens URL with default browser </summary>
        /// <param name="url">URL to web</param>
        public static void OpenSite(string url) {
            ILanguageManager LanguageManager = App.Kernel.Get<ILanguageManager>();
            try {
                System.Diagnostics.Process.Start(System.Web.HttpUtility.UrlDecode(url));
            } catch (Exception ex) {
                DialogsHelper.ShowErrorDialog(LanguageManager.Model.CantOpenLink + ex.Message);
            }
        }

        /// <summary> Opens URL with default browser (without URL decode) </summary>
        /// <param name="url">URL to web</param>
        public static void OpenSiteNoDecode(string url) {
            ILanguageManager LanguageManager = App.Kernel.Get<ILanguageManager>();
            try {
                System.Diagnostics.Process.Start(url);
            } catch (Exception ex) {
                DialogsHelper.ShowErrorDialog(LanguageManager.Model.CantOpenLink + ex.Message);
            }
        }
    }
}