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
    }
}
