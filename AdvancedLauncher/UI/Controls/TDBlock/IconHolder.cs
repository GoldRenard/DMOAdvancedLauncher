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

using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;
using AdvancedLauncher.Management;

namespace AdvancedLauncher.UI.Controls {

    public static class IconHolder {
        private const string COMMUNITY_DIR = "Community";
        private const string PNG_FORMAT = "{0}.png";
        private const string IMAGES_REMOTE_JOYMAX = "http://img.joymax.com/property/digimon/digimon_v1/us/ranking/icon/{0}.gif";
        private const string IMAGES_REMOTE_IMBC = "http://dm.imbc.com/images/ranking/icon/{0}.gif";

        private static Dictionary<int, BitmapImage> Dictionary = new Dictionary<int, BitmapImage>();

        public static BitmapImage GetImage(int code, bool webResource = true) {
            BitmapImage image = null;
            if (Dictionary.TryGetValue(code, out image)) {
                return image;
            }

            string resource = EnvironmentManager.ResolveResource(COMMUNITY_DIR,
                string.Format(PNG_FORMAT, code),
                string.Format(EnvironmentManager.COMMUNITY_IMAGE_REMOTE_FORMAT, code));

            if (resource == null) {
                //If we don't have image yet, try to download it from joymax
                resource = EnvironmentManager.ResolveResource(COMMUNITY_DIR,
                    string.Format(PNG_FORMAT, code),
                    string.Format(IMAGES_REMOTE_JOYMAX, code));
            }

            if (resource == null) {
                //If we don't have image yet, try to download it from IMBC
                resource = EnvironmentManager.ResolveResource(COMMUNITY_DIR,
                    string.Format(PNG_FORMAT, code),
                    string.Format(IMAGES_REMOTE_IMBC, code));
            }

            if (resource != null) {
                Stream str = File.OpenRead(resource);
                if (str == null) {
                    return null;
                }
                MemoryStream img_stream = new MemoryStream();
                str.CopyTo(img_stream);
                str.Close();
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = img_stream;
                bitmap.EndInit();
                bitmap.Freeze();
                Dictionary.Add(code, bitmap);
                return bitmap;
            }
            return null;
        }
    }
}