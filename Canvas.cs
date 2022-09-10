using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using RayTracer.Math;
using RayTracer.Utility;
using SysMath = System.Math;

namespace RayTracer.Drawing
{
    public class Canvas
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public Color[,] Pixels
        {
            get => _pixels;
            private set => _pixels = value;
        }

        private Color[,] _pixels;

        public Canvas(int width, int height) {
            Width = width;
            Height = height;
            Pixels = new Color[width, height];
            FillAllPixels(Color.Black);
        }

        public void FillAllPixels(Color fill) {
            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {
                    Pixels[x, y] = fill;
                }
            }
        }

        public void FillPixel(int x, int y, Color fill) {
            if (x < Width && y < Height && x >= 0 && y >= 0)
                Pixels[x, y] = fill;
        }

        public Color GetPixel(int x, int y) {
            if (x < Width && y < Height)
                return Pixels[x, y];
            return Color.Black;
        }
    }

    public struct Color : IEnumerable<float> {
        public const byte kComponentCount = 3;
        public static Color Black => new Color(0f, 0f, 0f);
        public static Color White => new Color(1f, 1f, 1f);
        public static Color Red => new Color(1f, 0f, 0f);
        public static Color Green => new Color(0f, 1f, 0f);
        public static Color Blue => new Color(0f, 0f, 1f);
        public static Color Cyan => new Color(0f, 1f, 1f);
        public static Color Yellow => new Color(1f, 1f, 0f);
        public static Color Magenta => new Color(1f, 0f, 1f);

        public float red;
        public float green;
        public float blue;

        public int RedByte => (red * byte.MaxValue).RoundToInt();
        public int GreenByte => (green * byte.MaxValue).RoundToInt();
        public int BlueByte => (blue * byte.MaxValue).RoundToInt();
        public int RedByteClamped => SysMath.Clamp(RedByte, 0, byte.MaxValue);
        public int GreenByteClamped => SysMath.Clamp(GreenByte, 0, byte.MaxValue);
        public int BlueByteClamped => SysMath.Clamp(BlueByte, 0, byte.MaxValue);

        public EnumerationType EnumerateAsType { get; set; }
        //float a?

        public float this[int index] {
            get {
                switch (index) {
                    case 0:
                        return EnumerateAsType == EnumerationType.Float ? red : RedByteClamped;
                    case 1:
                        return EnumerateAsType == EnumerationType.Float ? green : GreenByteClamped;
                    case 2:
                        return EnumerateAsType == EnumerationType.Float ? blue : BlueByteClamped;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }

        public Color(float r, float g, float b, EnumerationType enumerationType = EnumerationType.Float) {
            red = r;
            green = g;
            blue = b;
            EnumerateAsType = enumerationType;
        }

        public Color(byte r, byte g, byte b, EnumerationType enumerationType = EnumerationType.Float) {
            red = (float)r / byte.MaxValue;
            green = (float)g / byte.MaxValue;
            blue = (float)b / byte.MaxValue;
            EnumerateAsType = enumerationType;
        }

        public static implicit operator Color(Float4 color)
            => new Color(color.x, color.y, color.z);

        public static implicit operator Float4(Color color)
            => new Float4(color.red, color.green, color.blue, 0);

        public static bool operator ==(Color color, Color comp)
        {
            return (SysMath.Abs(color.red - comp.red)) <= Float4.kEpsilon &&
                           (SysMath.Abs(color.green - comp.green)) <= Float4.kEpsilon &&
                           (SysMath.Abs(color.blue - comp.blue)) <= Float4.kEpsilon;
        }

        public static bool operator !=(Color color, Color comp)
            => !(color == comp);

        public static Color operator +(Color color, Color other)
        {
            color.red += other.red;
            color.green += other.green;
            color.blue += other.blue;
            return color;
        }

        public static Color operator -(Color color, Color other)
        {
            color.red -= other.red;
            color.green -= other.green;
            color.blue -= other.blue;
            return color;
        }

        public static Color operator /(Color color, Color other)
        {
            color.red /= other.red;
            color.green /= other.green;
            color.blue /= other.blue;
            return color;
        }

        public static Color operator *(Color color, Color other)
        {
            color.red *= other.red;
            color.green *= other.green;
            color.blue *= other.blue;
            return color;
        }

        public static Color operator *(Color color, float scalar)
        {
            color.red *= scalar;
            color.green *= scalar;
            color.blue *= scalar;
            return color;
        }

        public static Color operator /(Color color, float divider)
        {
            color.red /= divider;
            color.green /= divider;
            color.blue /= divider;
            return color;
        }

        public static Color operator -(Color color)
        {
            color.red = -color.red;
            color.green = -color.green;
            color.blue = -color.blue;
            return color;
        }

        public override bool Equals(object obj)
        {
            if (obj is Color comp)
                return (Color)obj == comp;

            return false;
        }

        public static Color Blend(Color color1, Color color2)
            => color1 * color2;

        public string ToByteString()
           => $"[R:{RedByte}, G:{GreenByte}, B:{BlueByte}]";

        public override int GetHashCode()
            => red.GetHashCode() ^ green.GetHashCode() ^ blue.GetHashCode();

        public override string ToString()
            => $"[R:{red}, G:{green}, B:{blue}]";

        public IEnumerator<float> GetEnumerator()
        {
            yield return EnumerateAsType == EnumerationType.Float ? red : RedByteClamped;
            yield return EnumerateAsType == EnumerationType.Float ? green : GreenByteClamped;
            yield return EnumerateAsType == EnumerationType.Float ? blue : BlueByteClamped;
        }

        /// <summary>
        /// Converts a HSV value to RGB.
        /// </summary>
        /// <param name="h">The <b>Hue</b> value. [Range 0 - 360]</param>
        /// <param name="s">The <b>Saturation</b> value. [Range 0 - 1]</param>
        /// <param name="v">The <b>Value</b>. [Range 0 - 1]</param>
        /// <returns></returns>
        public static Color FromHSV(float h, float s, float v)
        {
            float r = 0, g = 0, b = 0;

            float hc = h / 60f;
            float c = v * s;
            float x = c * (1f - MathF.Abs((hc % 2f) - 1f));

            if (hc >= 0f && hc < 1f) {
                r = c;
                g = x;
            }
            else if(hc >= 1f && hc < 2f)
            {
                r = x;
                g = c;
            }
            else if(hc >= 2f && hc < 3f) {
                g = c;
                b = x;
            }
            else if(hc >= 3f && hc < 4f) {
                g = x;
                b = c;
            }
            else if(hc >= 4f && hc < 5f) {
                r = x;
                b = c;
            }
            else  {
                r = c;
                b = x;
            }
            float m = v - c;

            return new Color(r + m, g + m, b + m);
        }

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public enum EnumerationType { 
            Float,
            Byte
        }
    }
}
