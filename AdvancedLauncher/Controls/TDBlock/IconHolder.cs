// ======================================================================
// DIGIMON MASTERS ONLINE ADVANCED LAUNCHER
// Copyright (C) 2014 Ilya Egorov (goldrenard@gmail.com)

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
using AdvancedLauncher.Environment;

namespace AdvancedLauncher.Controls {

    public static class IconHolder {
        private const string IMAGES_3RDPARTY_DIR = "{0}\\Community\\3rd";
        private const string IMAGES_FILE = "{0}\\Community\\{1}.png";
        private const string IMAGES_3RDPARTY_FILE = "{0}\\{1}.png";
        private const string IMAGES_REMOTE_OWN = "{0}Community/{1}.png";
        private const string IMAGES_REMOTE_JOYMAX = "http://img.joymax.com/property/digimon/digimon_v1/us/ranking/icon/{0}.gif";
        private const string IMAGES_REMOTE_IMBC = "http://dm.imbc.com/images/ranking/icon/{0}.gif";

        private struct DigiImage {
            public int Id;
            public BitmapImage Image;
        }

        private static List<DigiImage> ImagesCollection = new List<DigiImage>();

        public static BitmapImage GetImage(int digimonId) {
            DigiImage Image = ImagesCollection.Find(i => i.Id == digimonId);
            if (Image.Image != null) {
                return Image.Image;
            }

            string Image3rdDirectory = string.Format(IMAGES_3RDPARTY_DIR, LauncherEnv.GetResourcesPath());
            string ImageFile = string.Format(IMAGES_FILE, LauncherEnv.GetResourcesPath(), digimonId);
            string ImageFile3rd = string.Format(IMAGES_3RDPARTY_FILE, Image3rdDirectory, digimonId);

            //If we don't have image, try to download it from author's resource
            if (!File.Exists(ImageFile)) {
                try {
                    LauncherEnv.WebClient.DownloadFile(string.Format(IMAGES_REMOTE_OWN, LauncherEnv.REMOTE_PATH, digimonId), ImageFile);
                } catch {
                }
            }

            //If we don't have image yet, try to download it from joymsx
            if (!File.Exists(ImageFile) && !File.Exists(ImageFile3rd)) {
                try {
                    if (!Directory.Exists(Image3rdDirectory)) {
                        Directory.CreateDirectory(Image3rdDirectory);
                    }
                    LauncherEnv.WebClient.DownloadFile(string.Format(IMAGES_REMOTE_JOYMAX, digimonId), ImageFile3rd);
                } catch {
                }
            }

            //If we don't have image yet, try to download it from IMBC
            if (!File.Exists(ImageFile) && !File.Exists(ImageFile3rd)) {
                try {
                    LauncherEnv.WebClient.DownloadFile(string.Format(IMAGES_REMOTE_IMBC, digimonId), ImageFile3rd);
                } catch {
                }
            }

            //If we don't have our own image but downloaded 3rd one, use it
            if (!File.Exists(ImageFile) && File.Exists(ImageFile3rd)) {
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
                ImagesCollection.Add(new DigiImage() {
                    Image = bitmap,
                    Id = digimonId
                });
                return bitmap;
            }
            return null;
        }
    }
}