using System;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        var ImageConverter = new ImageConverter();
        var compressionFormat = new BC1CompressionFormat();

        Console.WriteLine("Welcome to Image Converter. Currently supporting DDS -> BMP and BMP -> DDS");
        Console.WriteLine("Image Converter support 24bit bmp files and DXT1 DDS files");
        Console.WriteLine("Type file name.");
         var fileName = Console.ReadLine();

         if (fileName.ToLower().EndsWith(".dds"))
         {
             ImageConverter.ConvertDDSToBMP(fileName, compressionFormat);
         }
         else if(fileName.ToLower().EndsWith(".bmp"))
         {
             ImageConverter.ConvertBMPToDDS(fileName, compressionFormat);
         }
         else
         {
             Console.WriteLine("File format not supported.");
         }

        Console.ReadKey();
    }
}
