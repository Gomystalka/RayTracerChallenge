using System;
using System.Collections.Generic;
using System.Text;
using RayTracer.Drawing;
using RayTracer.Debugging;

using SysMath = System.Math;

namespace RayTracer.IO
{
    //TO-DO PPM Writer
    public class PPMWriter
    {
        public const string kPPMStandard = "P3";
        public const byte kMaxCharactersPerLine = 70;
        public const byte kMaxComponentsPerLine = kMaxCharactersPerLine / (Color.kComponentCount + 1); //Assuming each component is 3 characters + new line. 

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
