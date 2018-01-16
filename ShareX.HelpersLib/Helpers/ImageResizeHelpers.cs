using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace ShareX.HelpersLib.Helpers
{
    public static class ImageResizeHelpers
    {
        private const InterpolationMode DefaultInterpolationMode = InterpolationMode.HighQualityBicubic;

        public static Image ResizeImage(Image img, int width, int height, InterpolationMode interpolationMode = DefaultInterpolationMode)
        {
            if (width < 1 || height < 1 || (img.Width == width && img.Height == height))
            {
                return img;
            }

            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            bmp.SetResolution(img.HorizontalResolution, img.VerticalResolution);

            using (img)
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.InterpolationMode = interpolationMode;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.CompositingMode = CompositingMode.SourceOver;

                using (ImageAttributes ia = new ImageAttributes())
                {
                    ia.SetWrapMode(WrapMode.TileFlipXY);
                    g.DrawImage(img, new Rectangle(0, 0, width, height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, ia);
                }
            }

            return bmp;
        }

        public static Image ResizeImage(Image img, Size size, InterpolationMode interpolationMode = DefaultInterpolationMode)
        {
            return ResizeImage(img, size.Width, size.Height, interpolationMode);
        }

        public static Image ResizeImageByPercentage(Image img, float percentageWidth, float percentageHeight, InterpolationMode interpolationMode = DefaultInterpolationMode)
        {
            int width = (int)Math.Round(percentageWidth / 100 * img.Width);
            int height = (int)Math.Round(percentageHeight / 100 * img.Height);
            return ResizeImage(img, width, height, interpolationMode);
        }

        public static Image ResizeImageByPercentage(Image img, float percentage, InterpolationMode interpolationMode = DefaultInterpolationMode)
        {
            return ResizeImageByPercentage(img, percentage, percentage, interpolationMode);
        }

        public static Image ResizeImage(Image img, Size size, bool allowEnlarge, bool centerImage = true)
        {
            return ResizeImage(img, size.Width, size.Height, allowEnlarge, centerImage);
        }

        public static Image ResizeImage(Image img, int width, int height, bool allowEnlarge, bool centerImage = true)
        {
            return ResizeImage(img, width, height, allowEnlarge, centerImage, Color.Transparent);
        }

        public static Image ResizeImage(Image img, int width, int height, bool allowEnlarge, bool centerImage, Color backColor)
        {
            double ratio;
            int newWidth, newHeight;

            if (!allowEnlarge && img.Width <= width && img.Height <= height)
            {
                ratio = 1.0;
                newWidth = img.Width;
                newHeight = img.Height;
            }
            else
            {
                double ratioX = (double)width / img.Width;
                double ratioY = (double)height / img.Height;
                ratio = ratioX < ratioY ? ratioX : ratioY;
                newWidth = (int)(img.Width * ratio);
                newHeight = (int)(img.Height * ratio);
            }

            int newX = 0;
            int newY = 0;

            if (centerImage)
            {
                newX += (int)((width - (img.Width * ratio)) / 2);
                newY += (int)((height - (img.Height * ratio)) / 2);
            }

            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            bmp.SetResolution(img.HorizontalResolution, img.VerticalResolution);

            using (Graphics g = Graphics.FromImage(bmp))
            using (img)
            {
                g.Clear(backColor);
                g.SetHighQuality();
                g.DrawImage(img, newX, newY, newWidth, newHeight);
            }

            return bmp;
        }

        public static Image ResizeImageLimit(Image img, Size size)
        {
            return ResizeImageLimit(img, size.Width, size.Height);
        }

        /// <summary>If image size bigger than "size" then resize it and keep aspect ratio else return image.</summary>
        public static Image ResizeImageLimit(Image img, int width, int height)
        {
            if (img.Width <= width && img.Height <= height)
            {
                return img;
            }

            int newWidth, newHeight;
            double ratioX = (double)width / img.Width;
            double ratioY = (double)height / img.Height;

            if (ratioX < ratioY)
            {
                newWidth = width;
                newHeight = (int)(img.Height * ratioX);
            }
            else
            {
                newWidth = (int)(img.Width * ratioY);
                newHeight = height;
            }

            return ResizeImage(img, newWidth, newHeight);
        }

        public static Image ResizeImageLimit(Image img, int maxSize)
        {
            double ratio = (double)img.Width / img.Height;
            double x = Math.Sqrt(maxSize / ratio);

            int width, height;
            if (ratio > 1)
            {
                width = (int)(ratio * x);
                height = (int)(width / ratio);
            }
            else
            {
                height = (int)(ratio * x);
                width = (int)(height / ratio);
            }

            return ResizeImage(img, width, height);
        }
    }
}
