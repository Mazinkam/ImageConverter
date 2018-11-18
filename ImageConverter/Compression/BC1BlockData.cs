using Converter.Misc;

namespace Converter.Compression
{ 
    /// <summary>
    /// Show layout for 8byte BC1 Color block data.
    /// Essentially compresses pixel data into 4×4 blocks of texels. Instead of storing 16 colors, the algorithm saves 2 reference colors (color_0 and color_1) and 16 2-bit color indices
    /// https://docs.microsoft.com/en-us/windows/desktop/direct3d10/d3d10-graphics-programming-guide-resources-block-compression
    /// http://www.reedbeta.com/blog/understanding-bcn-texture-compression-formats/
    /// https://docs.microsoft.com/en-us/windows/desktop/direct2d/block-compression
    /// </summary>
    /// <remarks>
    /// <para>
    /// The block stores two 16-bit reference colors and a 32-bit index table
    /// for mapping a 2-bit color table index to each texel in the block.
    /// </para>
    public class BC1BlockData
    {

        public BC1BlockData()
        { }


        public Color565 Color0 { get; set; }
        public Color565 Color1 { get; set; }


        public int[] ColorIndexes { get; } = new int[Constants.TexelCount];

        /// <summary>
        /// Convert the block data into a 8-byte BC1 format byte array.
        /// </summary>
        public byte[] ToBytes()
        {
            var color0Low = (byte)((Color0.Value & 0x00FF) >> 0);
            var color0High = (byte)((Color0.Value & 0xFF00) >> 8);
            var color1Low = (byte)((Color1.Value & 0x00FF) >> 0);
            var Color1High = (byte)((Color1.Value & 0xFF00) >> 8);
            var indexes = new byte[4];

            for (int p = 0, row = 0; p < Constants.TexelCount; p += Constants.BlockDimension, ++row)
            {
                var a = p;
                var b = p + 1;
                var c = p + 2;
                var d = p + 3;

                indexes[row] = (byte)((ColorIndexes[a] & 0x03) |
                                      ((ColorIndexes[b] & 0x03) << 2) |
                                      ((ColorIndexes[c] & 0x03) << 4) |
                                      ((ColorIndexes[d] & 0x03) << 6));
            }

            var bytes = new byte[8];

            bytes[0] = color0Low;
            bytes[1] = color0High;
            bytes[2] = color1Low;
            bytes[3] = Color1High;
            bytes[4] = indexes[0];
            bytes[5] = indexes[1];
            bytes[6] = indexes[2];
            bytes[7] = indexes[3];

            return bytes;
        }

        /// <summary>
        /// Instantiates a <see cref="BC1BlockData"/> from compressed BC1 block data.
        /// </summary>
        public static BC1BlockData FromBytes(byte[] bytes)
        {
            var color0Low = bytes[0];
            var color0High = bytes[1];
            var color1Low = bytes[2];
            var color1Hight = bytes[3];
            var indexes = new byte[4];
            indexes[0] = bytes[4];
            indexes[1] = bytes[5];
            indexes[2] = bytes[6];
            indexes[3] = bytes[7];

            var block = new BC1BlockData
            {
                Color0 = Helpers.FromValue((ushort)((color0High << 8) | color0Low)),
                Color1 = Helpers.FromValue((ushort)((color1Hight << 8) | color1Low))
            };

            for (int p = 0, row = 0; p < Constants.TexelCount; p += Constants.BlockDimension, ++row)
            {
                block.ColorIndexes[p] = indexes[row] & 0x03;
                block.ColorIndexes[p + 1] = (indexes[row] >> 2) & 0x03;
                block.ColorIndexes[p + 2] = (indexes[row] >> 4) & 0x03;
                block.ColorIndexes[p + 3] = (indexes[row] >> 6) & 0x03;
            }

            return block;
        }
    }
}