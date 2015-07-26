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

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace AdvancedLauncher.UI.Extension {

    internal class JpegEncoder {

        public Image ResizeScreenShot(string source, string destination) {
            Image originalImage, resizedImage;

            System.IO.FileStream fs = new System.IO.FileStream(source, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            originalImage = System.Drawing.Image.FromStream(fs);
            fs.Close();

            Size size = new Size();
            size.Height = 100;
            size.Width = -1;
            resizedImage = resizeImage(originalImage, size);

            if (resizedImage != null) {
                saveJpeg(destination, (Bitmap)resizedImage, 100L);
                return resizedImage;
            }
            return null;
        }

        private void saveJpeg(string path, Bitmap image, long quality) {
            EncoderParameter qualityParam = new EncoderParameter(Encoder.Quality, quality);
            ImageCodecInfo jpegCodec = getEncoderInfo("image/jpeg");

            if (jpegCodec == null)
                return;

            EncoderParameters encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = qualityParam;

            image.Save(path, jpegCodec, encoderParams);
        }

        private static ImageCodecInfo getEncoderInfo(string mimeType) {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            for (int i = 0; i < codecs.Length; i++)
                if (codecs[i].MimeType == mimeType)
                    return codecs[i];
            return null;
        }

        private static Image resizeImage(Image imgToResize, Size size) {
            int sourceWidth = imgToResize.Width;
            int sourceHeight = imgToResize.Height;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            if (size.Width == -1)
                nPercent = ((float)size.Height / (float)sourceHeight);
            else if (size.Height == -1)
                nPercent = ((float)size.Width / (float)sourceWidth);
            else {
                nPercentW = ((float)size.Width / (float)sourceWidth);
                nPercentH = ((float)size.Height / (float)sourceHeight);

                if (nPercentH < nPercentW)
                    nPercent = nPercentH;
                else
                    nPercent = nPercentW;
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage((Image)b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();

            return (Image)b;
        }
    }
}