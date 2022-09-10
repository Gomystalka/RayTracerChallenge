using System.Threading;
using RayTracer.Math;
using RayTracer.Sandbox;
using RayTracer.Debugging;
using RayTracer.Drawing;
using RayTracer.IO;
using System;

using SysMath = System.Math;
using RayTracer.Utility;

namespace RayTracer.UnitTesting.Tests
{

    public class Tests
    {
        [UnitTest]
        public static bool TestFloat4Comparison()
        {
            Float4 x = (1f, 1f, 1f);
            Float4 dx = (1f, 2f, 1f);

            return x != dx;
        }

        [UnitTest]
        public static bool TestFloat4Multiplication()
        {
            Float4 x = (1f, 1f, 1f);
            Float4 expectedResult = (3f, 3f, 3f);

            return x * 3f == expectedResult;
        }

        [UnitTest]
        public static bool TestFloat4Division()
        {
            Float4 x = (1f, 1f, 1f);
            Float4 expectedResult = (.5f, .5f, .5f);

            return x / 2f == expectedResult;
        }

        [UnitTest]
        public static bool TestFloat4Addition()
        {
            Float4 x = (1f, 1f, 1f);
            Float4 expectedResult = (2f, 2f, 2f);

            return x + x == expectedResult;
        }

        [UnitTest]
        public static bool TestFloat4Subtraction()
        {
            Float4 p1 = (3, 2, 1);
            Float4 p2 = (5, 6, 7);
            Float4 expectedResult = (-2, -4, -6);

            return p1 - p2 == expectedResult;
        }

        [UnitTest]
        public static bool TestFloat4Magnitude()
        {
            Float4 p = (1, 0, 0);

            return p.Magnitude == 1f;
        }

        [UnitTest]
        public static bool TestFloat4Normalisation()
        {
            Float4 p = (4, 0, 0);
            return p.Normalised == (1, 0, 0);
        }

        [UnitTest]
        public static bool TestFloat4DotProduct()
        {
            Float4 p1 = (1, 2, 3);
            Float4 p2 = (2, 3, 4);

            return Float4.Dot(p1, p2) == 20f;
        }

        [UnitTest]
        public static bool TestFloat4CrossProduct()
        {
            Float4 p1 = (1, 2, 3);
            Float4 p2 = (2, 3, 4);

            return Float4.Cross(p1, p2) == (-1f, 2f, -1f) &&
                Float4.Cross(p2, p1) == (1, -2, 1);
        }

        [UnitTest]
        public static bool TestColorTuple() {
            Color c = new Color(-0.5f, 0.4f, 1.7f);

            return c.red == -0.5f && c.green == 0.4f && c.blue == 1.7f;
        }

        [UnitTest]
        public static bool TestColorAddition() {
            Color c1 = new Color(0.9f, 0.6f, 0.75f);
            Color c2 = new Color(0.7f, 0.1f, 0.25f);

            return c1 + c2 == new Color(1.6f, 0.7f, 1.0f);
        }

        [UnitTest]
        public static bool TestColorSubtraction()
        {
            Color c1 = new Color(0.9f, 0.6f, 0.75f);
            Color c2 = new Color(0.7f, 0.1f, 0.25f);

            return c1 - c2 == new Color(0.2f, 0.5f, 0.5f);
        }

        [UnitTest]
        public static bool TestColorScalarMultiplication()
        {
            Color c1 = new Color(0.2f, 0.3f, 0.4f);

            return c1 * 2f == new Color(0.4f, 0.6f, 0.8f);
        }

        [UnitTest]
        public static bool TestColorByColorMultiplication()
        {
            Color c1 = new Color(1.0f, 0.2f, 0.4f);
            Color c2 = new Color(0.9f, 1f, 0.1f);

            return c1 * c2 == new Color(0.9f, 0.2f, 0.04f);
        }

        [UnitTest]
        public static bool TestCanvas() {
            Canvas canvas = new Canvas(10, 20);
            canvas.FillPixel(2, 3, Color.Red);

            return canvas.GetPixel(2, 3) == Color.Red;
        }

        [UnitTest]
        public static bool TestPPMHeaderCreation() {
            string correctHeader = $"P3{Environment.NewLine}10 20{Environment.NewLine}255";

            return PPMWriter.CreatePPMHeader(10, 20) == correctHeader;
        }

        [UnitTest()]
        public static bool TestPPMCreation() {
            Canvas c = new Canvas(10, 2);
            c.FillAllPixels(Color.Red);
            string ppm = PPMWriter.CreatePPMString(c.Pixels);
            System.IO.File.WriteAllText("Test.ppm", ppm);
            string nl = Environment.NewLine;

            string expected = $"{PPMWriter.CreatePPMHeader(c.Width, c.Height)}{nl}" +
                              $"255 0 0 255 0 0 255 0 0 255 0 0 255 0 0 255 0 0{nl}" +
                              $"255 0 0 255 0 0 255 0 0 255 0 0 255 0 0 255 0 0{nl}" +
                              $"255 0 0 255 0 0 255 0 0 255 0 0 255 0 0 255 0 0{nl}" + 
                              $"255 0 0 255 0 0{nl}";
            return ppm == expected && ppm.EndsWith(nl);
        }

        [UnitTest]
        public static bool TestProjectileDrawing() {
            Canvas c = new Canvas(800, 600);
            Projectile proj = new Projectile()
            {
                position = Float4.Point(0f, 0.1f, 0f),
                velocity = Float4.Vector(0.62f, 1.8f, 0f).Normalised * 10f
            };
            World world = new World()
            {
                gravity = Float4.Vector(0f, -0.1f, 0f),
                windVelocity = Float4.Vector(0.01f, 0f, 0f)
            };

            int thickness = 6;
            float maxHeight = 300f;

            while (proj.position.y > 0)
            {
                proj.Update(world);
                int canvasRelativeY = (int)(c.Height - proj.position.y) + 4;

                for (int dx = -thickness; dx < thickness; dx++)
                {
                    for (int dy = -thickness; dy < thickness; dy++)
                    {
                        Color clr = Color.FromHSV((proj.position.y / maxHeight) * 360f, 1f, 1f);
                        c.FillPixel(proj.position.x.RoundToInt() + dx, canvasRelativeY + dy, clr);
                    }
                }
                //Thread.Sleep(1);
            }
            Debug.Log("Projectile Landed");
            System.IO.File.WriteAllText("ProjectileTest.ppm", PPMWriter.CreatePPMString(c.Pixels));

            return true;
        }

        [UnitTest]
        public static bool TestHSVToRGBConversion() {
            float h = 20f;
            float s = 1f;
            float v = 1f;

            Color c = Color.FromHSV(h, s, v);
            return c == new Color(255, 85, 0);
        }

        [UnitTest(RunOnSeparateThread = true, Skip = true)]
        public static bool TestProjectile()
        {
            Projectile proj = new Projectile()
            {
                position = Float4.Point(0f, 1f, 0f),
                velocity = Float4.Vector(1f, 1f, 0f).Normalised
            };
            World world = new World()
            {
                gravity = Float4.Vector(0f, -0.1f, 0f),
                windVelocity = Float4.Vector(-0.01f, 0f, 0f)
            };

            while (proj.position.y > 0)
            {
                proj.Update(world);
                Thread.Sleep(1);
            }
            Debug.Log("Projectile Landed");
            return true;
        }
    }
}
