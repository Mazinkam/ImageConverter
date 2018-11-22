using System;
using System.IO;

namespace Converter.Misc
{
    public static class HelperExtensions
    {
        /// <summary>
        /// flips array horizontally
        /// </summary>
        /// <param name="source"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static byte[] FlipArrayHorizontal(this byte[] source, int height)
        {
            byte[] tempArray = new byte[source.Length];

            var bytesPerRow = source.Length / height;

            for (int i = 0; i < height; i++)
            {
                int sourceIndex = i * (int)bytesPerRow;
                int destIndex = ((int)(height - i) * (int)bytesPerRow) - (int)bytesPerRow;

                Array.Copy(source, sourceIndex, tempArray, destIndex, bytesPerRow);
            }

            return tempArray;
        }

        /// <summary>
        /// https://stackoverflow.com/questions/943635/getting-a-sub-array-from-an-existing-array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="sourceIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        /// <summary>
        /// Reads all remaining bytes in a binaryStream
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static byte[] ReadAllBytes(this BinaryReader reader)
        {
            const int bufferSize = 4096;
            using (var ms = new MemoryStream())
            {
                byte[] buffer = new byte[bufferSize];
                int count;
                while ((count = reader.Read(buffer, 0, buffer.Length)) != 0)
                    ms.Write(buffer, 0, count);
                return ms.ToArray();
            }

        }

        public static byte Round(this double value)
        {
            var rounded = Math.Round(value, MidpointRounding.AwayFromZero);
            return (byte)(rounded > 255 ? 255 : rounded);
        }

    }
}
