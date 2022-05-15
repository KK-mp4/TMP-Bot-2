namespace TMP_Bot_2
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.Drawing.Text;
    using System.IO;
    using Newtonsoft.Json;

    /// <summary>
    /// Class that generates thumbnail for the episode.
    /// </summary>
    internal class Thumbnail
    {
        private class Divisor
        {
            public int a { get; set; }

            public int b { get; set; }

            public Divisor(int a, int b)
            {
                this.a = a;
                this.b = b;
            }
        }

        private static List<Divisor> divisorList = new List<Divisor>();

        public static Bitmap Generate(string episodeNumber)
        {
            int resX = 1920;
            int resY = 1080;

            string path = @"..\..\..\TMP_List.json";
            if (!File.Exists(path))
            {
                return null;
            }

            List<Video> vidList = JsonConvert.DeserializeObject<List<Video>>(File.ReadAllText(path));

            if (vidList == null)
            {
                return null;
            }

            var dimentions = GetDimentions(vidList.Count);

            Bitmap thumbnail = new Bitmap(resX, resY);

            int smallResX = Convert.ToInt32((double)resX / dimentions.Item1);
            int smallResY = Convert.ToInt32((double)resY / dimentions.Item2);

            // builds mosaic from TMP videos thumbnails
            using (Graphics g = Graphics.FromImage(thumbnail))
            {
                g.Clear(Color.Black);

                for (int j = 0; j < dimentions.Item2; j++)
                {
                     for (int i = 0; i < dimentions.Item1; i++)
                     {
                        int index = (j * dimentions.Item1) + i;
                        if (index > vidList.Count - 1)
                        {
                            break;
                        }

                        string filename = vidList[index].Url.Replace("https://b23.tv/", string.Empty);
                        filename = filename.Replace("https://youtu.be/", string.Empty);

                        Bitmap imgSmall = null;
                        try
                        {
                            imgSmall = new Bitmap(@$"..\..\..\Thumbnails\{ filename }.jpg");
                        }
                        catch
                        {
                            imgSmall = new Bitmap(188, 100);
                        }

                        g.DrawImage(imgSmall, new Rectangle(smallResX * i, smallResY * j, smallResX + 1, smallResY + 1));

                        imgSmall.Dispose();
                    }
                }
            }

            // blurs mosaic
            thumbnail = Blur(thumbnail, new Rectangle(0, 0, thumbnail.Width, thumbnail.Height), 3);

            // darkens
            using (Graphics g = Graphics.FromImage(thumbnail))
            {
                using (Brush cloud_brush = new SolidBrush(Color.FromArgb(100, 0, 0, 0)))
                {
                    Rectangle r = new Rectangle(0, 0, thumbnail.Width, thumbnail.Height);
                    g.FillRectangle(cloud_brush, r);
                }
            }

            //draws text and shadow
            using (Graphics g = Graphics.FromImage(thumbnail))
            {
                int shadowOffset = 16;

                RectangleF rectf1 = new RectangleF(0, 410, thumbnail.Width, 300);
                RectangleF rectf2 = new RectangleF(0 + shadowOffset, 410 + shadowOffset, thumbnail.Width, 300);

                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                // set string format so in centers text
                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Center;

                // load Minecrafter font
                var foo = new PrivateFontCollection();
                foo.AddFontFile(@"..\..\..\Minecrafter.Reg.ttf");
                var myCustomFont = new Font((FontFamily)foo.Families[0], 77f);

                // draw shadow
                var shadowBrush = new SolidBrush(Color.FromArgb(200, 0, 0, 0));
                g.DrawString($"Technical Minecraft Podcast\nEpisode {episodeNumber}", myCustomFont, shadowBrush, rectf2, sf);

                // draw main text
                g.DrawString($"Technical Minecraft Podcast\nEpisode {episodeNumber}", myCustomFont, Brushes.White, rectf1, sf);
            }

            return thumbnail;
        }

        private static Tuple<int, int> GetDimentions(int vidCount)
        {
            divisorList.Clear();

            if (IsPrime(vidCount))
            {
                vidCount++;
            }

            divisorList.Add(new Divisor(1, vidCount));

            for (int i = 2; i <= vidCount; i++)
            {
                if (vidCount % i == 0)
                {
                    divisorList.Add(new Divisor(i, Convert.ToInt32((double)vidCount / i)));
                }
            }

            double ratio = 16D / 9;
            double min = Math.Abs(((double)divisorList[0].a / divisorList[0].b) - ratio);
            int besta = divisorList[0].a;
            int bestb = divisorList[0].b;

            for (int i = 1; i < divisorList.Count; i++)
            {
                double newRatio = (double)divisorList[i].a / divisorList[i].b;
                if (Math.Abs(newRatio - ratio) < min)
                {
                    min = Math.Abs(newRatio - ratio);
                    besta = divisorList[i].a;
                    bestb = divisorList[i].b;
                }
            }

            return Tuple.Create(besta, bestb);
        }

        private static bool IsPrime(int number)
        {
            if (number <= 1)
            {
                return false;
            }

            if (number == 2)
            {
                return true;
            }

            if (number % 2 == 0)
            {
                return false;
            }

            var boundary = (int)Math.Floor(Math.Sqrt(number));

            for (int i = 3; i <= boundary; i += 2)
            {
                if (number % i == 0)
                {
                    return false;
                }
            }

            return true;
        }

        private static unsafe Bitmap Blur(Bitmap image, Rectangle rectangle, int blurSize)
        {
            Bitmap blurred = new Bitmap(image.Width, image.Height);

            // make an exact copy of the bitmap provided
            using (Graphics graphics = Graphics.FromImage(blurred))
            {
                graphics.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height), new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
            }

            // Lock the bitmap's bits
            BitmapData blurredData = blurred.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, blurred.PixelFormat);

            // Get bits per pixel for current PixelFormat
            int bitsPerPixel = Image.GetPixelFormatSize(blurred.PixelFormat);

            // Get pointer to first line
            byte* scan0 = (byte*)blurredData.Scan0.ToPointer();

            // look at every pixel in the blur rectangle
            for (int xx = rectangle.X; xx < rectangle.X + rectangle.Width; xx++)
            {
                for (int yy = rectangle.Y; yy < rectangle.Y + rectangle.Height; yy++)
                {
                    int avgR = 0, avgG = 0, avgB = 0;
                    int blurPixelCount = 0;

                    // average the color of the red, green and blue for each pixel in the
                    // blur size while making sure you don't go outside the image bounds
                    for (int x = xx; x < xx + blurSize && x < image.Width; x++)
                    {
                        for (int y = yy; y < yy + blurSize && y < image.Height; y++)
                        {
                            // Get pointer to RGB
                            byte* data = scan0 + (y * blurredData.Stride) + (x * bitsPerPixel / 8);

                            avgB += data[0];
                            avgG += data[1];
                            avgR += data[2];

                            blurPixelCount++;
                        }
                    }

                    avgR /= blurPixelCount;
                    avgG /= blurPixelCount;
                    avgB /= blurPixelCount;

                    // now that we know the average for the blur size, set each pixel to that color
                    for (int x = xx; x < xx + blurSize && x < image.Width && x < rectangle.Width; x++)
                    {
                        for (int y = yy; y < yy + blurSize && y < image.Height && y < rectangle.Height; y++)
                        {
                            // Get pointer to RGB
                            byte* data = scan0 + (y * blurredData.Stride) + (x * bitsPerPixel / 8);

                            // Change values
                            data[0] = (byte)avgB;
                            data[1] = (byte)avgG;
                            data[2] = (byte)avgR;
                        }
                    }
                }
            }

            // Unlock the bits
            blurred.UnlockBits(blurredData);

            return blurred;
        }
    }
}
