using Converter.Misc;
using System;
using System.Drawing;

namespace Converter.FileType
{ 
    /// <summary>
    /// https://en.wikipedia.org/wiki/BMP_file_format#Example_1
    /// </summary>
    public class BMPFile
    {
        public BMPFileHeader BMPFileHeader;
        public BMPFileInfoHeader BMPFileInfoHeader;
        public byte[] PixelData;


        /// <summary>
        /// https://docs.microsoft.com/en-us/windows/desktop/direct3d10/d3d10-graphics-programming-guide-resources-block-compression
        /// </summary>
        /// <param name="blockIndex"></param>
        /// <returns></returns>
        public Color[] GetBlockColors(Point blockIndex)
        {
            var firstPixel = new Point(
                blockIndex.X * Constants.BlockDimension,
                blockIndex.Y * Constants.BlockDimension);

            var lastPixel = new Point(
                firstPixel.X + Constants.BlockDimension,
                firstPixel.Y + Constants.BlockDimension);

            var colors = new Color[Constants.TexelCount];
            int colorIndex = 0;

            for (int y = firstPixel.Y; y < lastPixel.Y; ++y)
            {
                for (int x = firstPixel.X; x < lastPixel.X ; ++x)
                {
                    var pixel = new Point(x, y);
                    int pixelIndex = Helpers.ToRowMajor(pixel, (int)BMPFileInfoHeader.Width) * 3;

                    byte a = 0;

                    // Bytes stored in reverse
                    byte r = PixelData[pixelIndex + 2];
                    byte g = PixelData[pixelIndex + 1];
                    byte b = PixelData[pixelIndex];

                    var values = new byte[] { a, r, g, b };

                    if (BitConverter.IsLittleEndian)
                        Array.Reverse(values);

                    int argb = BitConverter.ToInt32(values, 0);

                    colors[colorIndex++] = Color.FromArgb(argb);

                }
            }

            return colors;
        }

        /// <summary>
        /// https://docs.microsoft.com/en-us/windows/desktop/direct3d10/d3d10-graphics-programming-guide-resources-block-compression
        /// </summary>
        /// <param name="blockIndex"></param>
        /// <param name="colors"></param>
        public void SetBlockColors(Point blockIndex, Color[] colors)
        {
            var firstPixel = new Point(
                blockIndex.X * Constants.BlockDimension,
                blockIndex.Y * Constants.BlockDimension);

            var lastPixel = new Point(
                firstPixel.X + Constants.BlockDimension,
                firstPixel.Y + Constants.BlockDimension);

            int colorIndex = 0;

            for (int y = firstPixel.Y; y < lastPixel.Y; ++y)
            {
                for (int x = firstPixel.X; x < lastPixel.X; ++x)
                {
                    var pixel = new Point(x, y);
                    int pixelIndex = (Helpers.ToRowMajor(pixel, (int)BMPFileInfoHeader.Width) * 3);

                    var color = colors[colorIndex++];

                    // Bytes are stored in inverted (BGRA) order
                    PixelData[pixelIndex + 2] = color.R;
                    PixelData[pixelIndex + 1] = color.G;
                    PixelData[pixelIndex] = color.B;
                }
            }
        }
    }

    public struct BMPFileHeader
    {
        //the two magic numberd 0x42 and 0x4D
        public byte[] MagicHeader;
        public uint FileSize;
        public short Reserved1;
        public short Reserved2;
        public uint OffToPixelArray;
    }

    public struct BMPFileInfoHeader
    {
        public uint HeaderSize;
        public uint Width;
        public uint Height;
        public ushort ColorPlanes;
        public ushort BitCount;
        public uint CompressionType;
        public uint RawDataSize;
        public uint HorizontalPixelPerMeter;
        public uint VerticalPixelPerMeter;
        public uint ColorsUsed;
        public uint ImportantColors;
    }
}