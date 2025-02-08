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
                // Configura a qualidade de interpolação para alta qualidade
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

                // Desenha a imagem original no novo bitmap
                graphics.DrawImage(image, new Rectangle(0, 0, newSize.Width, newSize.Height));
                //graphics.Dispose();
                GC.Collect();
                return newImage;
            }
        }
        static public Bitmap CreateImageWithBorder(int width, int height, int borderWidth, Color borderColor, Color backgroundColor)
        {
            // Cria uma nova imagem com as dimensões especificadas
            Bitmap bitmap = new Bitmap(width, height);

            // Desenha na imagem
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                // Preenche o fundo da imagem com a cor de fundo
                graphics.Clear(backgroundColor);

                // Cria um pincel para o contorno
                using (Brush borderBrush = new SolidBrush(borderColor))
                {
                    // Desenha o contorno nas bordas
                    graphics.FillRectangle(borderBrush, 0, 0, width, borderWidth); // Topo
                    graphics.FillRectangle(borderBrush, 0, 0, borderWidth, height); // Esquerda
                    graphics.FillRectangle(borderBrush, width - borderWidth, 0, borderWidth, height); // Direita
                    graphics.FillRectangle(borderBrush, 0, height - borderWidth, width, borderWidth); // Fundo
                }
            }

            return bitmap;
        }
    }
}
