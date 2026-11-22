using System;

namespace RocketAnalyzer
{
    public static class GuiUtil
    {
        public static string FormatMassKg(double kg)
        {
            return (kg / 1000.0).ToString("N2") + " t";
        }

        public static string FormatThrustN(double n)
        {
            return (n / 1000.0).ToString("N1") + " kN";
        }

        public static string FormatDeltaV(double mps)
        {
            return mps.ToString("N0") + " m/s";
        }

        public static string FormatSeconds(double s)
        {
            if (double.IsNaN(s) || double.IsInfinity(s) || s < 0)
                return "-";
            TimeSpan t = TimeSpan.FromSeconds(s);
            return t.Hours > 0 ? t.ToString(@"h\:mm\:ss") : t.ToString(@"m\:ss");
        }

        public static string FormatDistance(double m)
        {
            return (m / 1000.0).ToString("N0") + " km";
        }
    }
}
