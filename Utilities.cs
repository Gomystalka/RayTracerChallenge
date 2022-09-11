using System;
using System.Collections.Generic;
using System.Text;

namespace RayTracer.Utility
{
    public static class Utilities
    {
        /// <summary>
        /// Constructs a string which contains the specified <b>str</b> string, repeated <b>repeatCount</b> time(s).
        /// </summary>
        /// <param name="str">The string to repeat.</param>
        /// <param name="repeatCount">How many times the string will be repeated.</param>
        /// <returns>A new string containing the repeated string.</returns>
        public static string Repeat(this string str, int repeatCount) {
            StringBuilder builder = new StringBuilder(str.Length * repeatCount);
            for (int i = 0; i < repeatCount; i++)
                builder.Append(str);
            return builder.ToString();
        }

        /// <summary>
        /// Constructs a string which contains the specified <b>c</b> character, repeated <b>repeatCount</b> time(s).
        /// </summary>
        /// <param name="c">The character to repeat.</param>
        /// <param name="repeatCount">How many times the character will be repeated.</param>
        /// <returns>A new string containing the repeated character.</returns>
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

        /// <summary>
        /// Resizes the specified two dimensional array <b>array</b> to the specified width and height.
        /// </summary>
        /// <typeparam name="T">The type of the array elements.</typeparam>
        /// <param name="array">The array to resize.</param>
        /// <param name="newWidth">The new width of the array.</param>
        /// <param name="newHeight">The new height of the array.</param>
        public static void ResizeTwoDimensionalArray<T>(ref T[,] array, int newWidth, int newHeight) {
            T[,] arr = new T[newWidth, newHeight];

            for (int x = 0; x < Math.Min(array.GetLength(0), newWidth); ++x) {
                for (int y = 0; y < Math.Min(array.GetLength(1), newHeight); ++y)
                    arr[x, y] = array[x, y];
            }
                //Array.Copy(array, i * array.GetLength(0), arr, i * newWidth, Math.Min(array.GetLength(0), newWidth));
            array = arr;
        }
    }
}
