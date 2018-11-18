using System;
using System.Drawing;

/// <summary>
/// https://docs.microsoft.com/en-us/windows/desktop/direct3ddds/dx-graphics-dds-pguide#dds-file-layout
/// </summary>
public class DDSFile
{
    public DDSHeader DDSHeader;
    public byte[] Data;
    public byte[] Data2;

    public void SetBlockData(Point block, byte[] data)
    {
        var blockSize = data.Length;
        var numberOfHorizontalBlocks = (int)DDSHeader.Width / Constants.BlockDimension;

        var blockIndex = Helpers.ToRowMajor(block, numberOfHorizontalBlocks);
        var bufferIndex = blockIndex * blockSize;

        Array.Copy(data, 0, Data, bufferIndex, blockSize);
    }

    public byte[] GetBlockData(Point block)
    {
        var numberOfHorizontalBlocks = (int)DDSHeader.Width / Constants.BlockDimension;

        var blockIndex = Helpers.ToRowMajor(block, numberOfHorizontalBlocks);
        var bufferIndex = blockIndex * Constants.BC1ByteSize;

        var blockData = Data.SubArray(bufferIndex, Constants.BC1ByteSize);

        return blockData;
    }
}

public static class DDSFileHeaderFlags
{
    public const uint DDSCaps = 0x1;
    public const uint DDSHeight = 0x2;
    public const uint DDSWidth = 0x4;
    public const uint DDSPitch = 0x8;
    public const uint DDSPixelFormat = 0x1000;
    public const uint DDSMipMapCount = 0x20000;
    public const uint DDSLinearSize = 0x80000;
    public const uint DDSDepth = 0x800000;
}

public struct DDSPixelFormat
{

    public uint Size;
    public uint Flags;
    public uint FourCC;
    public uint RGBBitCount;
    public uint RBitMask;
    public uint GBitMask;
    public uint BBitMask;
    public uint ABitMask;
}

/// <summary>
/// https://docs.microsoft.com/en-gb/windows/desktop/direct3ddds/dds-header
/// </summary>
public unsafe struct DDSHeader
{
    public uint Size;
    public uint Flags;
    public uint Height;
    public uint Width;
    public uint PitchOrLinearSize;
    public uint Depth;
    public uint MipMapCount;
    public fixed uint Reserved1[11];
    public DDSPixelFormat PixelFormat;
    public uint Caps;
    public uint Caps2;
    public uint Caps3;
    public uint Caps4;
    public uint Reserved2;
}

