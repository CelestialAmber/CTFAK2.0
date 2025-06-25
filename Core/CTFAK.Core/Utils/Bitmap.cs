using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace CTFAK.Core.Utils
{
    //TODO: add support for different formats
    public class Bitmap
    {
        Image<Rgba32> image;
        public int width
        {
            get {
                return image.Width;
            }
        }
        public int height
        {
            get {
                return image.Height;
            }
        }

        public Bitmap(int width, int height)
        {
            image = new Image<Rgba32>(width, height);
        }

        public Bitmap(Image<Rgba32> image)
        {
            this.image = image;
        }

        public Bitmap(Stream stream)
        {
            image = Image.Load<Rgba32>(stream);
        }

        public void CopyColorDataFromArray(byte[] array)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int colorIndex = (y * width + x) * 4;
                    byte r = array[colorIndex];
                    byte g = array[colorIndex + 1];
                    byte b = array[colorIndex + 2];
                    byte a = array[colorIndex + 3];
                    Rgba32 col = new Rgba32(r, g, b, a);

                    image[x, y] = col;
                }
            }
        }

        public Rgba32 GetPixel(int x, int y)
        {
            return image[x, y];
        }

        public void SetPixel(int x, int y, Rgba32 color)
        {
            image[x, y] = color;
        }

        //Creates a resized copy of this image.
        public Bitmap ResizeImage(int width, int height)
        {
            Image<Rgba32> newImage = image.Clone();
            newImage.Mutate(x => x.Resize(width, height));
            return new Bitmap(newImage);
        }

        public Bitmap Clone()
        {
            return new Bitmap(image.Clone());
        }

        public void SaveToPng(string path)
        {
            image.SaveAsPng(path);
        }

        public static Bitmap FromStream(Stream stream)
        {
            return new Bitmap(Image.Load<Rgba32>(stream));
        }
    }
}
