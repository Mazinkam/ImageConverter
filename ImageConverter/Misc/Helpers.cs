using Converter.Compression;
using Converter.FileType;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Converter.Misc
{
    public static class Helpers
    {
        public static bool FileDimensionsValid(int width, int height)
        {
            return ((width % 4 == 0) && (height % 4 == 0));
        }

        /// <summary>
        /// Get minimum flags needed for a dds file
        /// </summary>
        /// <returns></returns>
        public static uint DDSMinFlags()
        {
            return DDSFileHeaderFlags.DDSCaps | DDSFileHeaderFlags.DDSHeight |
                   DDSFileHeaderFlags.DDSWidth | DDSFileHeaderFlags.DDSPixelFormat;
        }

        public static uint BC1Unorm()
        {
            return (byte)('D') |
                (byte)('X') << 8 |
                (byte)('T') << 16 |
                (byte)('1') << 24;
        }


        /// <summary>
        /// https://stackoverflow.com/questions/3722307/is-there-an-easy-way-to-blend-two-system-drawing-color-values
        /// </summary>
        public static Color565 Blend(Color565 color0, Color565 color1)
        {
            var c = BlendComponents(
                new[] { color0.R, color0.G, color0.B },
                new[] { color1.R, color1.G, color1.B });

            return FromRgb(c[0], c[1], c[2]);
        }

        private static byte[] BlendComponents(byte[] a, byte[] b)
        {
            var components = new byte[3];
            components[0] = ((a[0] + b[0]) / 2d).Round();
            components[1] = ((a[1] + b[1]) / 2d).Round();
            components[2] = ((a[2] + b[2]) / 2d).Round();
            return components;
        }

        /// <summary>
        /// https://social.msdn.microsoft.com/Forums/vstudio/en-US/5dd48231-22d4-47c4-a52c-c0c7d6fe8f52/creating-color-interpolation-by-using-a-class?forum=csharpgeneral
        /// </summary>
        public static Color565 LerpTwoThirds(Color565 a, Color565 b)
        {
            var c = LerpComponents(
                new[] { a.R, a.G, a.B },
                new[] { b.R, b.G, b.B });

            return FromRgb(c[0], c[1], c[2]);
        }

        private static byte[] LerpComponents(byte[] a, byte[] b)
        {
            var components = new byte[3];
            components[0] = ((a[0] + (2 * b[0])) / 3d).Round();
            components[1] = ((a[1] + (2 * b[1])) / 3d).Round();
            components[2] = ((a[2] + (2 * b[2])) / 3d).Round();
            return components;
        }

        /// <summary>
        /// Converts the 16-bit color into a 32-bit
        /// https://stackoverflow.com/questions/14419875/color-conversion-from-16-to-32-and-vice-versa
        /// </summary>
        public static Color To32Bit(Color565 color)
        {
            int r = color.R << 3;
            r |= (r & 0xE0) >> 5;

            int g = color.G << 2;
            g |= (g & 0xC0) >> 6;

            int b = color.B << 3;
            b |= (b & 0xE0) >> 5;

            return Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// Converts the 32-bit color into a 16-bit
        /// https://stackoverflow.com/questions/14419875/color-conversion-from-16-to-32-and-vice-versa
        /// </summary>
        public static Color565 To16Bit(Color color)
        {
            int r = color.R >> 3;
            int g = color.G >> 2;
            int b = color.B >> 3;

            return FromRgb(r, g, b);
        }

        /// <summary>
        /// Calculates the Euclidean distance between the two 32-bit colors
        /// https://stackoverflow.com/questions/3968179/compare-rgb-colors-in-c-sharp
        /// </summary>
        public static double Distance(Color color0, Color color1)
        {
            return DistanceByComponents(
                new[] { color0.R, color0.G, color0.B },
                new[] { color1.R, color1.G, color1.B });
        }

        /// <summary>
        /// Calculates the Euclidean distance between the two 16-bit colors
        /// https://stackoverflow.com/questions/3968179/compare-rgb-colors-in-c-sharp
        /// </summary>
        public static double Distance(Color565 color0, Color565 color1)
        {
            return DistanceByComponents(
                new[] { color0.R, color0.G, color0.B },
                new[] { color1.R, color1.G, color1.B });
        }

        private static double DistanceByComponents(byte[] a, byte[] b)
        {
            int expression =
                (a[0] - b[0]) * (a[0] - b[0]) +
                (a[1] - b[1]) * (a[1] - b[1]) +
                (a[2] - b[2]) * (a[2] - b[2]);

            return Math.Sqrt(expression);
        }

        /// <summary>
        /// Returns the color with the closest color
        /// https://stackoverflow.com/questions/27374550/how-to-compare-color-object-and-get-closest-color-in-an-color
        /// </summary>
        public static Color565 GetClosestColor(ICollection<Color565> colors, Color565 targetColor)
        {
            var closest = colors.FirstOrDefault();
            var closestDistance = double.MaxValue;

            foreach (var color in colors)
            {
                var distance = Distance(color, targetColor);
                if (distance < closestDistance)
                {
                    closest = color;
                    closestDistance = distance;
                }
            }

            return closest;
        }

        public static int GetIndexOfClosestColor(Color565[] colors, Color565 targetColor)
        {
            return Array.IndexOf(colors, GetClosestColor(colors, targetColor));
        }


        /// <summary>
        /// Transforms point to rwo major position
        /// https://en.wikipedia.org/wiki/Row-_and_column-major_order
        /// </summary>
        public static int ToRowMajor(Point point, int columns)
        {
            return point.X + (point.Y * columns);
        }

        /// <summary>
        /// Transform rwo major position to column major point
        /// https://en.wikipedia.org/wiki/Row-_and_column-major_order
        /// </summary>
        public static Point ToColumnMajor(int index, int columns)
        {
            int y = index / columns;
            int x = index - (y * columns);

            return new Point(x, y);
        }

        /// <summary>
        /// create color tablewith 1bitalpha colors, 
        /// </summary>
        /// <param name="color0"></param>
        /// <param name="color1"></param>
        /// <returns></returns>
        public static Color565[] CreateFor1BitAlpha(Color565 color0, Color565 color1)
        {
            var colors = new Color565[4];
            colors[0] = color0;
            colors[1] = color1;
            colors[2] = Helpers.Blend(color0, color1);
            colors[3] = Color565.Black;
            return colors;
        }

        /// <summary>
        /// Creates a 16-bit RGB565 color
        /// https://stackoverflow.com/questions/2442576/how-does-one-convert-16-bit-rgb565-to-24-bit-rgb888
        /// </summary>
        public static Color565 FromRgb(int red, int green, int blue)
        {
            int r = red > 0x1F ? 0x1F : red & 0x1F;
            int g = green > 0x3F ? 0x3F : green & 0x3F;
            int b = blue > 0x1F ? 0x1F : blue & 0x1F;

            ushort value = (ushort)((r << 11) | (g << 5) | b);

            return new Color565(value);
        }

        public static Color565 FromRgb(byte red, byte green, byte blue)
        {
            return FromRgb((int)red, (int)green, (int)blue);
        }

        /// <summary>
        /// Creates a 16-bit RGB565 color from the given 16-bit unsigned integer.
        /// </summary>
        public static Color565 FromValue(ushort value)
        {
            return new Color565(value);
        }
    }
}
