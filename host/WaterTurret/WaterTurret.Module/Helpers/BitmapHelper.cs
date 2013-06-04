using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
using System.IO;
using AForge.Imaging;
using AForge.Imaging.Filters;

namespace WaterTurret.Module.Helpers
{
    public static class BitmapHelper
    {
        public static BitmapImage ToBitmapImage(this Bitmap bitmap)
        {
            if (bitmap == null)
            {
                return null;
            }


            var ms = new MemoryStream();
            lock (bitmap)
            {
                bitmap.Save(ms, ImageFormat.Bmp);
            }
            ms.Seek(0, SeekOrigin.Begin);

            var bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = ms;
            bi.EndInit();

            return bi;

        }

        public static double Saturation(this Bitmap bitmap)
        {
            var s = new ImageStatisticsHSL(bitmap);
            return s.Saturation.Mean;
        }

        public static double Luminance(this Bitmap bitmap)
        {
            var s = new ImageStatisticsHSL(bitmap);
            return s.Luminance.Mean;
        }

        public static Rectangle GetRectangle(this Bitmap bitmap)
        {
            return new Rectangle(0, 0, bitmap.Width, bitmap.Height);
        }

        public static Bitmap CloneBitmap(this Bitmap bitmap)
        {
            return bitmap.Clone(GetRectangle(bitmap), PixelFormat.DontCare);
        }
    }
}
