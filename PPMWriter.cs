using System;
using System.Collections.Generic;
using System.Text;
using RayTracer.Drawing;
using RayTracer.Debugging;

using SysMath = System.Math;

namespace RayTracer.IO
{
    /// <summary>
    /// A class used for writing .PPM files from pixel data.
    /// </summary>
    public class PPMWriter
    {
        /// <summary>
        /// The flavour of PPM to use.
        /// </summary>
        public const string kPPMStandard = "P3";
        /// <summary>
        /// The maximum allowed characters per line.
        /// </summary>
        public const byte kMaxCharactersPerLine = 70;
        /// <summary>
        /// The maximum color components allowed per line.
        /// </summary>
        public const byte kMaxComponentsPerLine = kMaxCharactersPerLine / (Color.kComponentCount + 1); //Assuming each component is 3 characters + new line. 

        /// <summary>
        /// Generates a valid PPM string with prettified formatiing.
        /// </summary>
        /// <param name="pixels">The pixel data to be written.</param>
        /// <returns>String containing the formatted .PPM string.</returns>
        public static string CreatePPMString(Color[,] pixels) { //TO-DO make this a ref
            StringBuilder ppmBuilder = new StringBuilder();
            int width = pixels.GetLength(0);
            int height = pixels.GetLength(1);

            ppmBuilder.Append(CreatePPMHeader(width, height));
            ppmBuilder.AppendLine();

            int totalSize = width * height;
            byte lineLength = 0;
            for (int i = 0; i < totalSize; i++) {
                int x = i % width;
                int y = (int)SysMath.Floor((double)i / width);
                Color c = pixels[x, y];

                c.EnumerateAsType = Color.EnumerationType.Byte; //Changes the enumerator value to a 0 - 255 range.

                for (int k = 0; k < Color.kComponentCount; k++) {
                    float component = c[k];
                    if (lineLength >= kMaxComponentsPerLine) {
                        lineLength = 0;
                        ppmBuilder.Append(component);
                        ppmBuilder.AppendLine();
                        continue;
                    }
                    ppmBuilder.Append($"{component}{((i == totalSize - 1 && k == Color.kComponentCount - 1) ? "" : " ")}");
                    lineLength++;
                }
            }
            //if(ppmBuilder[ppmBuilder.Length])
            ppmBuilder.AppendLine(); //End the PPM file with a newline to satisfy software requirements.

            return ppmBuilder.ToString();
        }

        /// <summary>
        /// Generates the header of the PPM file given the width and height.
        /// </summary>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        /// <returns>A string containing a correctly formatted PPM header.</returns>
        public static string CreatePPMHeader(int width, int height) {
            if (width <= 0 || height <= 0) {
                Debug.LogError($"[{nameof(PPMWriter)}] Width and Height must be greater than 0!");
                return null;
            }

            return $"{kPPMStandard}{Environment.NewLine}" +
                $"{width} {height}{Environment.NewLine}" +
                $"{byte.MaxValue}";
        }
    }
}
