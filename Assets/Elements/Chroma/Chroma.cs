using UnityEngine;

[System.Serializable]
public struct Chroma {

    public const int MAX = 3;
    const int HDREmission = 1;
    public int r, g, b;

    public Color color {
        get {
            int maxValue = Mathf.Max(r, g, b);
            return new Color((float)r / maxValue, (float)g / maxValue, (float)b / maxValue) * HDREmission;
        }
    }

    public Chroma(int r, int g, int b) {
        this.r = r;
        this.g = g;
        this.b = b;
    }

    public bool isPure {
        get { return r + g + b == 1; }
    }

    public void ReBalance() {
        while (r > MAX || g > MAX || b > MAX) {
             r--;
             g--;
             b--;
        }
        if (r < 0) r = 0;
        if (g < 0) g = 0;
        if (b < 0) b = 0;

        if (r == g && g == b)
            r = g = b = 1;

        if (r == r + g + b) r = 1;
        if (g == r + g + b) g = 1;
        if (b == r + g + b) b = 1;
    }

    #region Static Colors
    
    public static Chroma zero {
        get { return new Chroma(0, 0, 0); }
    }
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
        return a;
    }

    public static Chroma operator - (Chroma a, Chroma b) {
        a.r -= b.r;
        a.g -= b.g;
        a.b -= b.b;
        return a;
    }

    public static Chroma operator * (Chroma a, Chroma b) {
        a.r *= b.r;
        a.g *= b.g;
        a.b *= b.b;
        return a;
    }

    public static Chroma operator + (Chroma a, int b) {
        a.r += b;
        a.g += b;
        a.b += b;
        return a;
    }

    public static Chroma operator - (Chroma a, int b) {
        a.r -= b;
        a.g -= b;
        a.b -= b;
        return a;
    }

    public static Chroma operator * (Chroma a, int b) {
        a.r *= b;
        a.g *= b;
        a.b *= b;
        return a;
    }

    #endregion

}
