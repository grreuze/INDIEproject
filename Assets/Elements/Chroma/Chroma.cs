using UnityEngine;

[System.Serializable]
public struct Chroma {

    const int MAX = 3;
    const int HDREmission = 2;
    public int r, g, b;

    public Color color {
        get {
            int maxValue = Mathf.Max(r, g, b);
            return new Color(r / maxValue, g / maxValue, b / maxValue) * HDREmission;
        }
    }

    public Chroma(int r, int g, int b) {
        this.r = r;
        this.g = g;
        this.b = b;
    }

    public static void ReBalance(Chroma c) {

        while (c.r > MAX || c.g > MAX || c.b > MAX) {
            c.r--;
            c.g--;
            c.b--;
        }
        if (c.r < 0) c.r = 0;
        if (c.g < 0) c.g = 0;
        if (c.b < 0) c.b = 0;

        if (c.r == c.g && c.g == c.b)
            c.r = c.g = c.b = 1;

        if (c.r == c.r + c.g + c.b) c.r = 1;
        if (c.g == c.r + c.g + c.b) c.g = 1;
        if (c.b == c.r + c.g + c.b) c.b = 1;
    }

    #region Static Colors

    public static Chroma red {
        get { return new Chroma(1, 0, 0); }
    }
    public static Chroma green {
        get { return new Chroma(0, 1, 0); }
    }
    public static Chroma blue {
        get { return new Chroma(0, 0, 1); }
    }
    public static Chroma white {
        get { return new Chroma(1, 1, 1); }
    }

    #endregion

    #region Operators

    public static Chroma operator + (Chroma a, Chroma b) {
        a.r += b.r;
        a.g += b.g;
        a.b += b.b;
        ReBalance(a);
        return a;
    }

    public static Chroma operator - (Chroma a, Chroma b) {
        a.r -= b.r;
        a.g -= b.g;
        a.b -= b.b;
        ReBalance(a);
        return a;
    }

    public static Chroma operator + (Chroma a, int b) {
        a.r += b;
        a.g += b;
        a.b += b;
        ReBalance(a);
        return a;
    }

    public static Chroma operator - (Chroma a, int b) {
        a.r -= b;
        a.g -= b;
        a.b -= b;
        ReBalance(a);
        return a;
    }

    public static Chroma operator * (Chroma a, int b) {
        a.r *= b;
        a.g *= b;
        a.b *= b;
        ReBalance(a);
        return a;
    }

    #endregion

}
