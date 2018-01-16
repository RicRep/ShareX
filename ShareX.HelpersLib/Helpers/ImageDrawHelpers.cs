using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace ShareX.HelpersLib.Helpers
{
    public static class ImageDrawHelpers
    {
        private const InterpolationMode DefaultInterpolationMode = InterpolationMode.HighQualityBicubic;

        public static Image DrawBorder(Image img, Color borderColor, int borderSize, BorderType borderType)
        {
            using (Pen borderPen = new Pen(borderColor, borderSize) { Alignment = PenAlignment.Inset })
            {
                return DrawBorder(img, borderPen, borderType);
            }
        }

        public static Image DrawBorder(Image img, Color fromBorderColor, Color toBorderColor, LinearGradientMode gradientType, int borderSize, BorderType borderType)
        {
            int width = img.Width;
            int height = img.Height;

            if (borderType == BorderType.Outside)
            {
                width += borderSize * 2;
                height += borderSize * 2;
            }

            using (LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, width, height), fromBorderColor, toBorderColor, gradientType))
            using (Pen borderPen = new Pen(brush, borderSize) { Alignment = PenAlignment.Inset })
            {
                return DrawBorder(img, borderPen, borderType);
            }
        }

        public static Image DrawBorder(Image img, Pen borderPen, BorderType borderType)
        {
            Bitmap bmp;

            if (borderType == BorderType.Inside)
            {
                bmp = (Bitmap)img;

                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.DrawRectangleProper(borderPen, 0, 0, img.Width, img.Height);
                }
            }
            else
            {
                int borderSize = (int)borderPen.Width;
                bmp = img.CreateEmptyBitmap(borderSize * 2, borderSize * 2);

                using (Graphics g = Graphics.FromImage(bmp))
                using (img)
                {
                    g.DrawRectangleProper(borderPen, 0, 0, bmp.Width, bmp.Height);
                    g.SetHighQuality();
                    g.DrawImage(img, borderSize, borderSize, img.Width, img.Height);
                }
            }

            return bmp;
        }

        public static Image DrawCheckers(Image img)
        {
            return DrawCheckers(img, 10, Color.FromArgb(230, 230, 230), Color.White);
        }

        public static Image DrawCheckers(Image img, int size, Color color1, Color color2)
        {
            Bitmap bmp = img.CreateEmptyBitmap();

            using (Graphics g = Graphics.FromImage(bmp))
            using (Image checker = ImageHelpers.CreateCheckerPattern(size, size, color1, color2))
            using (Brush checkerBrush = new TextureBrush(checker, WrapMode.Tile))
            using (img)
            {
                g.FillRectangle(checkerBrush, new Rectangle(0, 0, bmp.Width, bmp.Height));
                g.SetHighQuality();
                g.DrawImage(img, 0, 0, img.Width, img.Height);
            }

            return bmp;
        }

        public static Image DrawCheckers(int width, int height)
        {
            Bitmap bmp = new Bitmap(width, height);

            using (Graphics g = Graphics.FromImage(bmp))
            using (Image checker = ImageHelpers.CreateCheckerPattern())
            using (Brush checkerBrush = new TextureBrush(checker, WrapMode.Tile))
            {
                g.FillRectangle(checkerBrush, new Rectangle(0, 0, bmp.Width, bmp.Height));
            }

            return bmp;
        }

        public static void DrawColorPickerIcon(Graphics g, Color color, Rectangle rect, int holeSize = 0)
        {
            if (color.A < 255)
            {
                using (Image checker = ImageHelpers.CreateCheckerPattern(rect.Width / 2, rect.Height / 2))
                {
                    g.DrawImage(checker, rect);
                }
            }

            using (SolidBrush brush = new SolidBrush(color))
            {
                g.FillRectangle(brush, rect);
            }

            g.DrawRectangleProper(Pens.Black, rect);

            if (holeSize > 0)
            {
                g.CompositingMode = CompositingMode.SourceCopy;

                Rectangle holeRect = new Rectangle(rect.Width / 2 - holeSize / 2, rect.Height / 2 - holeSize / 2, holeSize, holeSize);

                g.FillRectangle(Brushes.Transparent, holeRect);
                g.DrawRectangleProper(Pens.Black, holeRect);
            }
        }

        public static Image DrawReflection(Image img, int percentage, int maxAlpha, int minAlpha, int offset, bool skew, int skewSize)
        {
            Bitmap reflection = ImageHelpers.AddReflection(img, percentage, maxAlpha, minAlpha);

            if (skew)
            {
                reflection = ImageHelpers.AddSkew(reflection, skewSize, 0);
            }

            Bitmap result = new Bitmap(reflection.Width, img.Height + reflection.Height + offset);

            using (Graphics g = Graphics.FromImage(result))
            using (img)
            using (reflection)
            {
                g.SetHighQuality();
                g.DrawImage(img, 0, 0, img.Width, img.Height);
                g.DrawImage(reflection, 0, img.Height + offset, reflection.Width, reflection.Height);
            }

            return result;
        }
    }

}
