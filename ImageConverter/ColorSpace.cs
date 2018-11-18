using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

public class ColorSpace : IReadOnlyList<Color>
{
    private readonly List<Color> _colors;

    public Color565 MinColor { get; private set; }
    public Color565 MaxColor { get; private set; }

    public byte MinAlpha { get; private set; }

    public byte MaxAlpha { get; private set; }

    public int Count => _colors.Count;

    public Color this[int index] => _colors[index];

    public ColorSpace(IEnumerable<Color> colors)
    {
        _colors = colors.ToList();
        CalculateColorSpace();
    }

    private void CalculateColorSpace()
    {
        var referenceColor = Color.FromArgb(0, 0, 0);
        var lowColor = Color.FromArgb(255, 255, 255);
        var highColor = Color.FromArgb(0, 0, 0);
        var lowDistance = Helpers.Distance(lowColor, referenceColor);
        var highDistance = Helpers.Distance(highColor, referenceColor);

        MinAlpha = 255;
        MaxAlpha = 0;

        foreach (var color in _colors)
        {
            var distance = Helpers.Distance(color, referenceColor);
            if (distance < lowDistance)
            {
                lowColor = color;
                lowDistance = distance;
            }
            if (distance > highDistance)
            {
                highColor = color;
                highDistance = distance;
            }

            if (color.A < MinAlpha)
                MinAlpha = color.A;
            if (color.A > MaxAlpha)
                MaxAlpha = color.A;
        }

        // Calculate min and max colors from 16-bit colors because their 
        // order might change if the comparison was made with 32-bit colors.

        var lowColor16 = Helpers.To16Bit(lowColor);
        var highColor16 = Helpers.To16Bit(highColor);

        if (lowColor16.Value < highColor16.Value)
        {
            MinColor = lowColor16;
            MaxColor = highColor16;
        }
        else
        {
            MinColor = highColor16;
            MaxColor = lowColor16;
        }
    }

    public IEnumerator<Color> GetEnumerator()
    {
        return _colors.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

