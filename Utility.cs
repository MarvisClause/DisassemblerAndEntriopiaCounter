using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
