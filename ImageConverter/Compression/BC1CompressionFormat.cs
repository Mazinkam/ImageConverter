using Converter.Misc;
using System.Drawing;

namespace Converter.Compression
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/windows/desktop/direct3d10/d3d10-graphics-programming-guide-resources-block-compression
    /// https://docs.microsoft.com/en-us/windows/desktop/direct2d/block-compression
    /// </summary>
    public class BC1CompressionFormat
    {

        public byte[] Compress(Color[] colors)
        {
            var colorSpace = new ColorSpace(colors);

            var colorTable = Helpers.CreateFor1BitAlpha(colorSpace.MinColor, colorSpace.MaxColor);

            var block = new BC1BlockData
            {
                Color0 = colorTable[0],
                Color1 = colorTable[1]
            };

            for (int i = 0; i < colors.Length; ++i)
            {
                var color32 = colors[i];

                // If color has alpha, use a specific index 
                // to identify the color when decompressed
                block.ColorIndexes[i] = Helpers.GetIndexOfClosestColor(colorTable, Helpers.To16Bit(color32));
            }

            return block.ToBytes();
        }

        public Color[] Decompress(byte[] blockData)
        {
            var block = BC1BlockData.FromBytes(blockData);

            var colorTable = Helpers.CreateFor1BitAlpha(block.Color0, block.Color1);
            var colors = new Color[Constants.TexelCount];

            for (int i = 0; i < colors.Length; ++i)
            {
                int index = block.ColorIndexes[i];
                int alpha = 255;

                var color16 = colorTable[index];
                var color32 = Color.FromArgb(alpha, Helpers.To32Bit(color16));

                colors[i] = color32;
            }

            return colors;
        }
    }
}