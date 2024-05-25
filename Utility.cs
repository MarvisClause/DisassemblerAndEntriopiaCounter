using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace DisEn
{
    // Utility class is used to provide useful functions across the system
    public static class Utility
    {
        public static float RandValueInRange(float lowValue, float highValue)
        {
            return lowValue + (float)new Random().NextDouble() * (highValue - lowValue);
        }

        // Maps number from one range into the other
        public static float MapNumber(float numberToMap, float currentMinRange, float currentMaxRange, float targetMinRange, float targetMaxRange)
        {
            return targetMinRange + (targetMaxRange - targetMinRange) * ((numberToMap - currentMinRange) / (currentMaxRange - currentMinRange));
        }

        public static bool NearlyEqual(double a, double b, double epsilon = 0.00001)
        {
            const double MinNormal = 2.2250738585072014E-308d;
            double absA = Math.Abs(a);
            double absB = Math.Abs(b);
            double diff = Math.Abs(a - b);

            if (a.Equals(b))
            { // shortcut, handles infinities
                return true;
            }
            else if (a == 0 || b == 0 || absA + absB < MinNormal)
            {
                // a or b is zero or both are extremely close to it
                // relative error is less meaningful here
                return diff < (epsilon * MinNormal);
            }
            else
            { // use relative error
                return diff / (absA + absB) < epsilon;
            }
        }

        // Generate distinct colors using HSL
        public static List<Color> GenerateColors(int count)
        {
            List<Color> colors = new List<Color>();

            for (int i = 0; i < count; i++)
            {
                double hue = (i * 360.0 / count) % 360; // spread hue across 360 degrees
                colors.Add(HSLToRGB(hue, 0.7, 0.7)); // use a fixed saturation and lightness
            }

            return colors;
        }

        // Convert HSL to RGB color
        public static Color HSLToRGB(double h, double s, double l)
        {
            double c = (1 - Math.Abs(2 * l - 1)) * s;
            double x = c * (1 - Math.Abs((h / 60) % 2 - 1));
            double m = l - c / 2;
            double r = 0, g = 0, b = 0;

            if (0 <= h && h < 60)
            {
                r = c; g = x; b = 0;
            }
            else if (60 <= h && h < 120)
            {
                r = x; g = c; b = 0;
            }
            else if (120 <= h && h < 180)
            {
                r = 0; g = c; b = x;
            }
            else if (180 <= h && h < 240)
            {
                r = 0; g = x; b = c;
            }
            else if (240 <= h && h < 300)
            {
                r = x; g = 0; b = c;
            }
            else if (300 <= h && h < 360)
            {
                r = c; g = 0; b = x;
            }

            r += m; g += m; b += m;

            return Color.FromRgb((byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
        }
    }
}
