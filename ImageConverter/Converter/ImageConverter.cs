using Converter.Compression;
using Converter.FileType;
using Converter.Misc;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Converter.Converter
{
    public class ImageConverter
    {
        const string errorMessage = "Could not recognize image format.";
    
        /// <summary>
        /// reads a bmp file and outputs a DDS file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="compressionFormat"></param>
        public void ConvertBMPToDDS(string fileName, BC1CompressionFormat compressionFormat)
        {
            var bmpFile = ReadBMPFile(fileName);
            if(!Helpers.FileDimensionsValid((int)bmpFile.BMPFileInfoHeader.Width, (int)bmpFile.BMPFileInfoHeader.Height))
            {
                throw new ArgumentException("BMP Image dimensions not valid");
            }

            fileName = fileName.Remove(fileName.Length - 4) + ".dds";

            WriteDDSFile(fileName, bmpFile, compressionFormat);
            Console.WriteLine("Conversion complete, file created " + fileName);
        }

        /// <summary>
        /// reads a dds file and outputs a bmp file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="compressionFormat"></param>
        public void ConvertDDSToBMP(string fileName, BC1CompressionFormat compressionFormat)
        {
            var ddsFile = ReadDDSFile(fileName);
            if (!Helpers.FileDimensionsValid((int)ddsFile.DDSHeader.Width, (int)ddsFile.DDSHeader.Height))
            {
                throw new ArgumentException("DDS Image dimensions not valid");
            }
            fileName = fileName.Remove(fileName.Length - 4) + ".bmp";

            WriteBMPFile(fileName, ddsFile, compressionFormat);

            Console.WriteLine("Conversion complete, file created " + fileName);
        }

        private BMPFile ReadBMPFile(string path)
        {
            using (BinaryReader binaryReader = new BinaryReader(File.OpenRead(path)))
            {
                try
                {
                    return ReadBMPFile(binaryReader);
                }
                catch (ArgumentException e)
                {
                    if (e.Message.StartsWith(errorMessage))
                    {
                        throw new ArgumentException(errorMessage, "path", e);
                    }
                    else
                    {
                        throw e;
                    }
                }
            }
        }

        private DDSFile ReadDDSFile(string path)
        {
            using (BinaryReader binaryReader = new BinaryReader(File.OpenRead(path)))
            {
                try
                {
                    return ReadDDSFile(binaryReader);
                }
                catch (ArgumentException e)
                {
                    if (e.Message.StartsWith(errorMessage))
                    {
                        throw new ArgumentException(errorMessage, "path", e);
                    }
                    else
                    {
                        throw e;
                    }
                }
            }
        }

        private BMPFile ReadBMPFile(BinaryReader binaryReader)
        {

            //create a bmp file for us to handle bmp data
            BMPFile bmpFile = new BMPFile
            {
                BMPFileHeader = new BMPFileHeader
                {
                    MagicHeader = binaryReader.ReadBytes(2),
                    FileSize = binaryReader.ReadUInt32(),
                    Reserved1 = binaryReader.ReadInt16(),
                    Reserved2 = binaryReader.ReadInt16(),
                    OffToPixelArray = binaryReader.ReadUInt32()
                },
                BMPFileInfoHeader = new BMPFileInfoHeader
                {
                    HeaderSize = binaryReader.ReadUInt32(),
                    Width = binaryReader.ReadUInt32(),
                    Height = binaryReader.ReadUInt32(),
                    ColorPlanes = binaryReader.ReadUInt16(),
                    BitCount = binaryReader.ReadUInt16(),
                    CompressionType = binaryReader.ReadUInt32(),
                    RawDataSize = binaryReader.ReadUInt32(),
                    HorizontalPixelPerMeter = binaryReader.ReadUInt32(),
                    VerticalPixelPerMeter = binaryReader.ReadUInt32(),
                    ColorsUsed = binaryReader.ReadUInt32(),
                    ImportantColors = binaryReader.ReadUInt32()
                },

                //we at 54 now
                //all of the image data
                PixelData = binaryReader.ReadAllBytes()
            };

            return bmpFile;
        }

        private void WriteBMPFile(string fileName, DDSFile ddsFile, BC1CompressionFormat compressionFormat)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(fileName+".bmp", FileMode.Create)))
            {
                byte[] BMPMagicNumber = { 0x42, 0x4D };

                int BMPFileHeaderSize = 14;
                int BMPFileInfoHeaderSize = 40;

                int imageRawSize = (int)ddsFile.DDSHeader.Width * (int)ddsFile.DDSHeader.Height * 3;
                int fileSize =imageRawSize + BMPFileHeaderSize + BMPFileInfoHeaderSize;
            
                var bmpFile = new BMPFile();
                bmpFile.BMPFileHeader = new BMPFileHeader();
                bmpFile.BMPFileHeader.MagicHeader = BMPMagicNumber;
                bmpFile.BMPFileHeader.Reserved1 = 0;
                bmpFile.BMPFileHeader.Reserved2 = 0;
                bmpFile.BMPFileHeader.FileSize = (uint)fileSize;
                bmpFile.BMPFileHeader.OffToPixelArray = (uint)BMPFileHeaderSize + (uint)BMPFileInfoHeaderSize;

                bmpFile.BMPFileInfoHeader = new BMPFileInfoHeader();
                bmpFile.BMPFileInfoHeader.HeaderSize = (uint)BMPFileInfoHeaderSize;
                bmpFile.BMPFileInfoHeader.Width = ddsFile.DDSHeader.Width;
                bmpFile.BMPFileInfoHeader.Height = ddsFile.DDSHeader.Height;
                bmpFile.BMPFileInfoHeader.ColorPlanes = 1;
                bmpFile.BMPFileInfoHeader.BitCount = 24;
                bmpFile.BMPFileInfoHeader.CompressionType = 0;
                bmpFile.BMPFileInfoHeader.RawDataSize = (uint)imageRawSize;
                bmpFile.BMPFileInfoHeader.HorizontalPixelPerMeter = 2835;
                bmpFile.BMPFileInfoHeader.VerticalPixelPerMeter = 2835;
                bmpFile.BMPFileInfoHeader.ColorsUsed = 0;
                bmpFile.BMPFileInfoHeader.ImportantColors = 0;

                writer.Write(bmpFile.BMPFileHeader.MagicHeader);
                writer.Write(bmpFile.BMPFileHeader.FileSize);
                writer.Write(bmpFile.BMPFileHeader.Reserved1);
                writer.Write(bmpFile.BMPFileHeader.Reserved2);
                writer.Write(bmpFile.BMPFileHeader.OffToPixelArray);

            
                writer.Write(bmpFile.BMPFileInfoHeader.HeaderSize);
                writer.Write(bmpFile.BMPFileInfoHeader.Width);
                writer.Write(bmpFile.BMPFileInfoHeader.Height);
                writer.Write(bmpFile.BMPFileInfoHeader.ColorPlanes);
                writer.Write(bmpFile.BMPFileInfoHeader.BitCount);
                writer.Write(bmpFile.BMPFileInfoHeader.CompressionType);
                writer.Write(bmpFile.BMPFileInfoHeader.RawDataSize);
                writer.Write(bmpFile.BMPFileInfoHeader.HorizontalPixelPerMeter);
                writer.Write(bmpFile.BMPFileInfoHeader.VerticalPixelPerMeter);
                writer.Write(bmpFile.BMPFileInfoHeader.ColorsUsed);
                writer.Write(bmpFile.BMPFileInfoHeader.ImportantColors);

                bmpFile.PixelData = new byte[imageRawSize];

                int numberOfVerticalBlocks = (int)ddsFile.DDSHeader.Height / Constants.BlockDimension;
                int numberOfHorizontalBlocks = (int)ddsFile.DDSHeader.Width / Constants.BlockDimension;
                int numberOfBlocks = numberOfVerticalBlocks * numberOfHorizontalBlocks;

                for (int i = 0; i < numberOfBlocks; i++)
                {
                    var blockIndex = Helpers.ToColumnMajor(i, numberOfHorizontalBlocks);
                    var blockData = ddsFile.GetBlockData(blockIndex);
                    var blockColors = compressionFormat.Decompress(blockData);

                    bmpFile.SetBlockColors(blockIndex, blockColors);
                }
            
                writer.Write(bmpFile.PixelData);
            }
        }

        public DDSFile ReadDDSFile(BinaryReader binaryReader)
        {
            DDSFile ddsFile = new DDSFile();

            var magicNumber = BitConverter.ToUInt32(binaryReader.ReadBytes(Marshal.SizeOf(Constants.DDSMagicNumber)));

            if (magicNumber != Constants.DDSMagicNumber)
                throw new InvalidOperationException("Unrecognizable DDS file format.");

            // read header info
            var headerData = binaryReader.ReadBytes(Marshal.SizeOf(typeof(DDSHeader)));

            // Read the header data directly into a DDS file header structure
            GCHandle handle = GCHandle.Alloc(headerData, GCHandleType.Pinned);
            ddsFile.DDSHeader = (DDSHeader)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(DDSHeader));
            handle.Free();

            var mainImageSize = Math.Max(1, (ddsFile.DDSHeader.Width + 3) / 4) * Math.Max(1, (ddsFile.DDSHeader.Height + 3) / 4) *8;

            if(mainImageSize != ddsFile.DDSHeader.PitchOrLinearSize)
                throw new InvalidOperationException("Parsing wrong, check DDS file.");

            ddsFile.Data = binaryReader.ReadAllBytes();

            return ddsFile;
        }

        public void WriteDDSFile(string fileName, BMPFile bmpFile, BC1CompressionFormat compressionFormat)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
            {
                var ddsFile = new DDSFile();
                writer.Write(Constants.DDSMagicNumber);

                //create default header
                ddsFile.DDSHeader = CreateHeader((int)bmpFile.BMPFileInfoHeader.Width, (int)bmpFile.BMPFileInfoHeader.Height, Helpers.BC1Unorm());

                var headerSize = Marshal.SizeOf(typeof(DDSHeader));
                var headerBuffer = new byte[headerSize];

                // Populate header data directly from the bytebuffer
                GCHandle handle = GCHandle.Alloc(headerBuffer, GCHandleType.Pinned);
                Marshal.StructureToPtr(ddsFile.DDSHeader, handle.AddrOfPinnedObject(), true);
                handle.Free();

                writer.Write(headerBuffer);
            
                ddsFile.Data = new byte[ddsFile.DDSHeader.PitchOrLinearSize];

                int numberOfPixels = (int)ddsFile.DDSHeader.Width * (int)ddsFile.DDSHeader.Height;
                int numberOfRequiredBlocks = numberOfPixels / Constants.TexelCount;
                int bufferSize = numberOfRequiredBlocks * Constants.BC1ByteSize;
                ddsFile.Data = new byte[bufferSize];

                int numberOfVerticalBlocks = (int)bmpFile.BMPFileInfoHeader.Height / Constants.BlockDimension;
                int numberOfHorizontalBlocks = (int)bmpFile.BMPFileInfoHeader.Width / Constants.BlockDimension;
                int numberOfBlocks = numberOfVerticalBlocks * numberOfHorizontalBlocks;

                for (int i = 0; i < numberOfBlocks; i++)
                {
                    var blockIndex = Helpers.ToColumnMajor(i, numberOfVerticalBlocks);
                    var blockColors = bmpFile.GetBlockColors(blockIndex);
                    var blockData = compressionFormat.Compress(blockColors);

                    ddsFile.SetBlockData(blockIndex, blockData);
                }

                writer.Write(ddsFile.Data);

            }
        }

        public DDSHeader CreateHeader(int pixelWidth, int pixelHeight, uint fourCC)
        {
            if (pixelHeight < 0 || pixelWidth < 0)
                throw new InvalidOperationException("Image width or height is negative.");

            var pixelFormat = new DDSPixelFormat
            {
                Size = 32,
                Flags = 0x4,
                FourCC = fourCC
            };

            var header = new DDSHeader
            {
                Size = 124, // has to be 124bytes ¯\_(ツ)_/¯
                Flags = Helpers.DDSMinFlags(),
                Height = (uint)pixelHeight,
                Width = (uint)pixelWidth,
                PixelFormat = pixelFormat,
                MipMapCount = 0,
                Caps = 0x1000 // texture, we don't handle harder things right now,

            };

            header.PitchOrLinearSize = Math.Max(1, (header.Width + 3) / 4) * Math.Max(1, (header.Height + 3) / 4) * 8;

            return header;
        }
    }
}


