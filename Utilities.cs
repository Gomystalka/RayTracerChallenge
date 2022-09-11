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
            => MathF.Abs((value - approximate)) <= Maths.Float4.kEpsilon;

        public static int RoundToInt(this float value)
            => (int)System.Math.Round(value);

        public static void ResizeTwoDimensionalArray<T>(ref T[,] array, int newWidth, int newHeight) {
            T[,] arr = new T[newWidth, newHeight];

            for (int x = 0; x < Math.Min(array.GetLength(0), newWidth); ++x) {
                for (int y = 0; y < Math.Min(array.GetLength(1), newHeight); ++y) {
                    arr[x, y] = array[x, y];
                }
            }
                //Array.Copy(array, i * array.GetLength(0), arr, i * newWidth, Math.Min(array.GetLength(0), newWidth));
            array = arr;
        }
    }
}
