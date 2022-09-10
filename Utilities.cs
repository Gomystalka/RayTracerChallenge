using System;
using System.Collections.Generic;
using System.Text;

namespace RayTracer.Utility
{
    public static class Utilities
    {
        public static string Repeat(this string str, int repeatCount) {
            StringBuilder builder = new StringBuilder(str.Length * repeatCount);
            for (int i = 0; i < repeatCount; i++)
                builder.Append(str);
            return builder.ToString();
        }

        public static string Repeat(this char c, int repeatCount)
        {
            StringBuilder builder = new StringBuilder(repeatCount);
            for (int i = 0; i < repeatCount; i++)
                builder.Append(c);
            return builder.ToString();
        }

        public static bool IsApproximately(this float value, float approximate)
            => MathF.Abs((value - approximate)) <= Math.Float4.kEpsilon;

        public static int RoundToInt(this float value)
            => (int)System.Math.Round(value);
    }
}
