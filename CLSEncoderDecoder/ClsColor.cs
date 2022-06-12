using System;

namespace CLSEncoderDecoder;

public struct ClsColor : IEquatable<ClsColor>
{
    public ClsColor(byte red, byte green, byte blue, byte alpha)
    {
        Red = red;
        Green = green;
        Blue = blue;
        Alpha = alpha;
    }

    public byte Red { get; set; }
    public byte Green { get; set; }
    public byte Blue { get; set; }
    public byte Alpha { get; set; }

    public override string ToString()
    {
        return $"RGBA: {Red} {Green} {Blue} {Alpha}";
    }

    public override bool Equals(object obj)
    {
        return obj is ClsColor color && color.Red == Red && color.Green == Green && color.Blue == Blue && color.Alpha == Alpha;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = Red.GetHashCode();
            hashCode = (hashCode * 397) ^ Green.GetHashCode();
            hashCode = (hashCode * 397) ^ Blue.GetHashCode();
            hashCode = (hashCode * 397) ^ Alpha.GetHashCode();
            return hashCode;
        }
    }

    public bool Equals(ClsColor other)
    {
        return Red == other.Red && Green == other.Green && Blue == other.Blue && Alpha == other.Alpha;
    }

    public static bool operator ==(ClsColor left, ClsColor right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ClsColor left, ClsColor right)
    {
        return !left.Equals(right);
    }
}