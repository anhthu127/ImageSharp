using System;
using System.IO;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Issue1356
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var imageProcessor = new ImageProcessor();

            string outputFileName = "/app/docker_output/small.jpg";
            string inputFile = @"93362884-d5684100-f846-11ea-852a-b3735e5503a7_small.png";
            await ResizeTestAsync(imageProcessor, outputFileName, inputFile);

            outputFileName = "/app/docker_output/garden.jpg";
            inputFile = @"94578895-84aa0c80-0278-11eb-8306-c80f391eb0c3.jpg";
            await ResizeTestAsync(imageProcessor, outputFileName, inputFile);

            Console.WriteLine("done");
            Console.ReadLine();
        }

        private static async Task ResizeTestAsync(ImageProcessor imageProcessor, string outputFileName, string inputFile)
        {
            using Stream inputStream = new FileStream(inputFile, FileMode.Open, FileAccess.Read);
            using (MemoryStream lowResStream = imageProcessor.ResizeTo(inputStream, ImageProcessor.LowResWidth))
            {
                using (Stream outputStream = new FileStream(outputFileName, FileMode.Create, FileAccess.Write))
                {
                    await lowResStream.CopyToAsync(outputStream);
                }
            }
        }
    }
}
