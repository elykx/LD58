using UnityEngine;

public static class ColorUtils
{
    public static Color HexToColor(string hex)
    {
        hex = hex.Replace("#", "");

        if (hex.Length == 6)
            hex += "FF";

        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        byte a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);

        return new Color32(r, g, b, a);
    }

    public static string ColorToHex(Color color, bool includeAlpha = false)
    {
        Color32 c = color;
        string hex = $"#{c.r:X2}{c.g:X2}{c.b:X2}";
        if (includeAlpha)
            hex += $"{c.a:X2}";
        return hex;
    }

    public static Color Darken(Color color, float amount)
    {
        amount = Mathf.Clamp01(amount);
        return new Color(
            color.r * (1f - amount),
            color.g * (1f - amount),
            color.b * (1f - amount),
            color.a
        );
    }

    public static Color Lighten(Color color, float amount)
    {
        amount = Mathf.Clamp01(amount);
        return new Color(
            color.r + (1f - color.r) * amount,
            color.g + (1f - color.g) * amount,
            color.b + (1f - color.b) * amount,
            color.a
        );
    }

    public static Color Random(bool randomAlpha = false)
    {
        return new Color(
            UnityEngine.Random.Range(0f, 1f),
            UnityEngine.Random.Range(0f, 1f),
            UnityEngine.Random.Range(0f, 1f),
            randomAlpha ? UnityEngine.Random.Range(0f, 1f) : 1f
        );
    }

    public static Color Invert(Color color)
    {
        return new Color(1f - color.r, 1f - color.g, 1f - color.b, color.a);
    }
}