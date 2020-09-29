using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using SixLabors.ImageSharp.Processing;

namespace Issue1356
{
    public class ImageProcessor
    {
        public const string LowResExtension = ".low";
        public const int LowResWidth = 700;
        public const int DefaultQuality = 90;

        public MemoryStream ResizeTo(Stream input, int width)
        {
            var output = new MemoryStream();
            input.Seek(0, SeekOrigin.Begin);
            using Image image = Image.Load(input);
            image.Mutate(x => x.Resize(width: width, 0));
            image.SaveAsJpeg(output);
            output.Seek(0, SeekOrigin.Begin);
            input.Seek(0, SeekOrigin.Begin);
            return output;
        }

        public Stream ReCompress(Stream input, int quality = DefaultQuality)
        {
            var output = new MemoryStream();
            input.Seek(0, SeekOrigin.Begin);
            using Image image = Image.Load(input);
            image.Mutate(x => x.AutoOrient());
            image.SaveAsJpeg(output, new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder { Quality = quality });
            output.Seek(0, SeekOrigin.Begin);
            input.Seek(0, SeekOrigin.Begin);
            return output;
        }

        /// <summary>
        /// Rotate image so it always is displayed correctly. Workaround for client not doing this.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Stream AutoOrient(Stream input)
        {
            input.Seek(0, SeekOrigin.Begin);
            using Image image = Image.Load(input);
            IExifValue exifOrientation = image.Metadata?.ExifProfile?.GetValue(ExifTag.Orientation);
            if (exifOrientation?.GetValue() == null || exifOrientation.GetValue().ToString() == "1")
            {
                input.Seek(0, SeekOrigin.Begin);
                return input;
            }
            var output = new MemoryStream();
            image.Mutate(x => x.AutoOrient());
            image.SaveAsJpeg(output, new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder { Quality = DefaultQuality });
            output.Seek(0, SeekOrigin.Begin);
            input.Seek(0, SeekOrigin.Begin);
            return output;
        }
    }
}
