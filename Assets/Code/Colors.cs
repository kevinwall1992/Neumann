
using UnityEngine;

public static class Colors
{
    public static Color Brown { get { return new Color(0.7f, 0.50f, 0.1f); } }
    public static Color Gold { get { return new Color(0.8f, 0.7f, 0.1f); } }
}

public static class ColorUtility
{
    public static Color Lerped(this Color color, Color other, float factor)
    {
        return Color.Lerp(color, other, factor);
    }

    public static Color AlphaChangedTo(this Color color, float alpha)
    {
        return new Color(color.r, color.g, color.b, alpha);
    }
}
