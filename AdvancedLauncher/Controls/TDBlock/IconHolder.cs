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
using System.Net;
using System.Windows.Media.Imaging;
using AdvancedLauncher.Environment;
using DMOLibrary;

namespace AdvancedLauncher.Controls {

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

            string ImageFile = Path.Combine(LauncherEnv.InitFolder(LauncherEnv.GetResourcesPath(), COMMUNITY_DIR), string.Format(PNG_FORMAT, code));
            string ImageFile3rd = Path.Combine(LauncherEnv.InitFolder(LauncherEnv.Get3rdResourcesPath(), COMMUNITY_DIR), string.Format(PNG_FORMAT, code));

            if (!File.Exists(ImageFile)) {
                if (!File.Exists(ImageFile3rd)) {
                    using (WebClient webClient = new WebClientEx()) {
                        try {
                            webClient.DownloadFile(string.Format(LauncherEnv.COMMUNITY_IMAGE_REMOTE_FORMAT, code), ImageFile3rd);
                        } catch {
                            // fall down and try to download from other source
                        }

                        if (webResource) {
                            //If we don't have image yet, try to download it from joymsx
                            if (!File.Exists(ImageFile3rd)) {
                                try {
                                    webClient.DownloadFile(string.Format(IMAGES_REMOTE_JOYMAX, code), ImageFile3rd);
                                } catch {
                                    // fall down and try to download from other source
                                }
                            }

                            //If we don't have image yet, try to download it from IMBC
                            if (!File.Exists(ImageFile3rd)) {
                                try {
                                    webClient.DownloadFile(string.Format(IMAGES_REMOTE_IMBC, code), ImageFile3rd);
                                } catch {
                                    // fall down
                                }
                            }
                        }
                    }
                }
                ImageFile = ImageFile3rd;
            }

            if (File.Exists(ImageFile)) {
                Stream str = File.OpenRead(ImageFile);
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