using System;
/// <summary>
/// 16bit
/// </summary>
public class Color565
{
    public ushort Value;
    public static Color565 Black => new Color565(0);

    public byte R => (byte)((Value & 0xF800) >> 11);
    public byte G => (byte)((Value & 0x7E0) >> 5);
    public byte B => (byte)(Value & 0x1F);

    public Color565(ushort value)
    {
        this.Value = value;
    }
}
