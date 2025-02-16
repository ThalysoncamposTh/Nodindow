using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Nodindow.myPackages
{
    static public class imageManager
    {
        static public Image resizeImage(Image image, Size newSize)
        {
            Bitmap newImage = new Bitmap(newSize.Width, newSize.Height);
            using (Graphics graphics = Graphics.FromImage(newImage))
            {
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(image, new Rectangle(0, 0, newSize.Width, newSize.Height));
                GC.Collect();
                return newImage;
            }
        }
        static public Bitmap CreateImageWithBorder(int width, int height, int borderWidth, Color borderColor, Color backgroundColor)
        {
            Bitmap bitmap = new Bitmap(width, height);

            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                // Fills the image background with the background color
                graphics.Clear(backgroundColor);

                using (Brush borderBrush = new SolidBrush(borderColor))
                {
                    // Draw the contour on the edges
                    graphics.FillRectangle(borderBrush, 0, 0, width, borderWidth); // Top
                    graphics.FillRectangle(borderBrush, 0, 0, borderWidth, height); // left
                    graphics.FillRectangle(borderBrush, width - borderWidth, 0, borderWidth, height); // right
                    graphics.FillRectangle(borderBrush, 0, height - borderWidth, width, borderWidth); // bottom
                }
            }

            return bitmap;
        }
    }
}
