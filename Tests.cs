using System.Threading;
using RayTracer.Maths;
using RayTracer.Sandbox;
using RayTracer.Debugging;
using RayTracer.Drawing;
using RayTracer.IO;
using RayTracer.Core;
using System;
using BenchmarkDotNet;

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

        [UnitTest(Skip = true)]
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

        [UnitTest]
        public static bool TestMatrixCreation() {
            Matrix matrix = new Matrix() {
                { 1f, 2f, 3f, 4f},
                { 5.5f, 6.5f, 7.5f, 8.5f},
                { 9f, 10f, 11f, 12f},
                { 13.5f, 14.5f, 15.5f, 16.5f}
            };

            return matrix[0,0] == 1f &&
                matrix[0, 3] == 4f &&
                matrix[1, 0] == 5.5f &&
                matrix[1, 2] == 7.5f &&
                matrix[2, 2] == 11f &&
                matrix[3, 0] == 13.5f &&
                matrix[3, 2] == 15.5f;
        }

        [UnitTest]
        public static bool TestCustomMatrixCreation()
        {
            Matrix matrix2x2 = new Matrix() {
                { -3f, 5f},
                { 1f, -2f},
            };
            //Debug.Log(matrix2x2);

            Matrix matrix3x3 = new Matrix() {
                { -3f, 5f, 0f},
                { 1f, -2f, -7f},
                {0f, 1f, 1f}
            };
            //Debug.Log(matrix3x3);

            return matrix2x2[0, 0] == -3f &&
                matrix2x2[0, 1] == 5f &&
                matrix2x2[1, 0] == 1f &&
                matrix2x2[1, 1] == -2f &&
                matrix3x3[0, 0] == -3f &&
                matrix3x3[1, 1] == -2f &&
                matrix3x3[2, 2] == 1f;
        }

        [UnitTest]
        public static bool TestMatrixComparisonEqual() {
            Matrix m = new Matrix() {
                { 1, 2, 3, 4},
                { 5, 6, 7, 8},
                { 9, 10, 11, 12},
                { 13, 14, 15, 16}
            };

            Matrix m2 = new Matrix() {
                { 1, 2, 3, 4},
                { 5, 6, 7, 8},
                { 9, 10, 11, 12},
                { 13, 14, 15, 16}
            };

            return m == m2;
        }

        [UnitTest]
        public static bool TestMatrixComparisonNotEqual()
        {
            Matrix m = new Matrix() {
                { 1, 2, 3, 4},
                { 5, 6, 7, 8},
                { 9, 8, 7, 6},
                { 5, 4, 3, 2}
            };

            Matrix m2 = new Matrix() {
                { 2, 3, 4, 5},
                { 6, 7, 8, 9},
                { 8, 7, 6, 5},
                { 4, 3, 2, 1}
            };

            return m != m2;
        }

        [UnitTest]
        public static bool TestMatrixXMatrixMultiplication() {
            Matrix m = new Matrix() {
                { 1, 2, 3, 4},
                { 5, 6, 7, 8},
                { 9, 8, 7, 6},
                { 5, 4, 3, 2}
            };

            Matrix mult = new Matrix() {
                { -2, 1, 2, 3},
                { 3, 2, 1, -1},
                { 4, 3, 6, 5},
                { 1, 2, 7, 8}
            };

            Matrix multResult = new Matrix() {
                { 20, 22, 50, 48},
                { 44, 54, 114, 108},
                { 40, 58, 110, 102},
                { 16, 26, 46, 42}
            };

            return m * mult == multResult;
        }

        [UnitTest]
        public static bool TestMatrixXFloat4Multiplication()
        {
            Matrix m = new Matrix() {
                { 1, 2, 3, 4},
                { 2, 4, 4, 2},
                { 8, 6, 4, 1},
                { 0, 0, 0, 1}
            };
            Float4 f = new Float4(1, 2, 3, 1);
            return f * m == new Float4(18, 24, 33, 1);
        }

        [UnitTest]
        public static bool TestIdentityMatrixMultiplication()
        {
            Matrix m = new Matrix() {
                { 1, 2, 3, 4},
                { 2, 4, 4, 2},
                { 8, 6, 4, 1},
                { 0, 0, 0, 1}
            };
            return m * Matrix.identity == m;
        }

        [UnitTest]
        public static bool TestMatrixTranspose() {
            Matrix m = new Matrix() {
                { 0, 9, 3, 0},
                { 9, 8, 0, 8},
                { 1, 8, 5, 3},
                { 0, 0, 5, 8}
            };

            Matrix expected = new Matrix() {
                { 0, 9, 1, 0},
                { 9, 8, 8, 0},
                { 3, 0, 5, 5},
                { 0, 8, 3, 8}
            };

            return m.Transpose() == expected;
        }

        [UnitTest]
        public static bool TestIdentityMatrixTranspose()
        {

            return Matrix.identity.Transpose() == Matrix.identity;
        }

        [UnitTest]
        public static bool TestMatrix2x2Determinant() {
            Matrix m = new Matrix(2, 2);
            m[0, 0] = 1;
            m[1, 0] = -3;
            m[0, 1] = 5;
            m[1, 1] = 2;

            return m.CalculateDeterminant() == 17;
        }

        [UnitTest]
        public static bool TestSubMatrix()
        {
            Matrix m3x3 = new Matrix() {
                { 1, 5, 0},
                { -3, 2, 7},
                { 0, 6, -3}
            };
            Matrix m4x4 = new Matrix() {
                { -6, 1, 1, 6},
                { -8, 5, 8, 6},
                { -1, 0, 8, 2},
                { -7, 1, -1, 1}
            };

            Matrix m3x3sub = new Matrix()
            {
                { -3, 2},
                { 0, 6}
            };

            Matrix m4x4sub = new Matrix()
            {
                { -6, 1, 6},
                { -8, 8, 6},
                { -7, -1, 1}
            };

            return m3x3.GetSubMatrix(0, 2) == m3x3sub && m4x4.GetSubMatrix(2, 1) == m4x4sub;
            //return m3x3;
        }

        [UnitTest]
        public static bool TestMatrix3x3Minor() {
            Matrix m = new Matrix()
            {
                { 3, 5, 0},
                { 2, -1, 7},
                { 6, -1, 5}
            };
            return m.CalculateMinor(1, 0) == 25;
        }

        [UnitTest]
        public static bool TestMatrix3x3Cofactor() {
            Matrix m = new Matrix()
            {
                { 3, 5, 0},
                { 2, -1, -7},
                { 6, -1, 5}
            };
            return m.CalculateMinor(0, 0) == -12 &&
                m.CalculateCofactor(0, 0) == -12 &&
                m.CalculateMinor(1, 0) == 25 &&
                m.CalculateCofactor(1, 0) == -25;
        }

        [UnitTest]
        public static bool TestMatrix3x3Determinant()
        {
            Matrix m3x3 = new Matrix() {
                { 1, 2, 6},
                { -5, 8, -4},
                { 2, 6, 4},
            };

            return m3x3.CalculateCofactor(0, 0) == 56 &&
                m3x3.CalculateCofactor(0, 1) == 12 &&
                m3x3.CalculateCofactor(0, 2) == -46 &&
                m3x3.CalculateDeterminant() == -196;
        }

        [UnitTest]
        public static bool TestMatrix4x4Determinant()
        {
            Matrix m4x4 = new Matrix() {
                { -2, -8, 3, 5},
                { -3, 1, 7, 3},
                { 1, 2, -9, 6},
                { -6, 7, 7, -9}
            };

            return m4x4.CalculateCofactor(0, 0) == 690 &&
                m4x4.CalculateCofactor(0, 1) == 447 &&
                m4x4.CalculateCofactor(0, 2) == 210 &&
                m4x4.CalculateCofactor(0, 3) == 51 &&
                m4x4.CalculateDeterminant() == -4071;
        }

        [UnitTest]
        public static bool TestMatrixInvertibility() {
            Matrix m = new Matrix() {
                { 6, 4, 4, 4},
                { 5, 5, 7, 6},
                { 4, -9, 3, -7},
                { 9, 1, 7, -6}
            };

            Matrix mx = new Matrix() {
                { -4, 2, -2, -3},
                { 9, 6, 2, 6},
                { 0, -5, 1, -5},
                { 0, 0, 0, 0}
            };

            return m.Invertible && !mx.Invertible;
        }

        [UnitTest]
        public static bool TestMatrix4x4Inverse() {
            Matrix m = new Matrix() {
                { -5, 2, 6, -8},
                { 1, -5, 1, 8},
                { 7, 7, -6, -7},
                { 1, -3, 7, 4}
            };

            Matrix expectedInverse = new Matrix() {
                { 0.21805f, 0.45113f, 0.24060f, -0.04511f},
                { -0.80827f, -1.45677f, -0.44361f, 0.52068f},
                { -0.07895f, -0.22368f, -0.05263f, 0.19737f},
                { -0.52256f, -0.81391f, -0.30075f, 0.30639f}
            };

            return m.Inverse == expectedInverse;
        }

        [UnitTest]
        public static bool TestMatrix4x4InverseApplication()
        {
            Matrix m = new Matrix() {
                { 3, -9, 7, 3},
                { 3, -8, 2, -9},
                { -4, 4, 4, 1},
                { -6, 5, -1, 1}
            };

            Matrix mx = new Matrix() {
                { 8, 2, 2, 2},
                { 3, -1, 7, 0},
                { 7, 0, 5, 4},
                { 6, -2, 0, 5}
            };

            Matrix mm = m * mx;

            return mm * mx.Inverse == m;
        }

        [UnitTest]
        public static bool TestTranslationMatrix() {
            Matrix translationMatrix = Matrix.Translation(5, -3, 2);
            Float4 p = Float4.Point(-3, 4, 5);

            return translationMatrix * p == Float4.Point(2, 1, 7);
        }

        [UnitTest]
        public static bool TestInverseTranslationMatrix()
        {
            Matrix translationMatrix = Matrix.Translation(5, -3, 2);
            translationMatrix = translationMatrix.Inverse;

            Float4 p = Float4.Point(-3, 4, 5);

            return translationMatrix * p == Float4.Point(-8, 7, 3);
        }

        [UnitTest]
        public static bool TestTranslationMatrixOnVector()
        {
            Matrix translationMatrix = Matrix.Translation(5, -3, 2);
            Float4 v = Float4.Vector(-3, 4, 5);

            return translationMatrix * v == v;
        }

        [UnitTest]
        public static bool TestScaleMatrix() {
            Matrix scaleMatrix = Matrix.Scaled(2, 3, 4);
            Float4 p = Float4.Point(-4, 6, 8);
            return scaleMatrix * p == Float4.Point(-8, 18, 32);
        }

        [UnitTest]
        public static bool TestScaleMatrixOnVector()
        {
            Matrix scaleMatrix = Matrix.Scaled(2, 3, 4);
            Float4 v = Float4.Vector(-4, 6, 8);
            return scaleMatrix * v == Float4.Vector(-8, 18, 32);
        }

        [UnitTest]
        public static bool TestInverseScaleMatrix()
        {
            Matrix scaleMatrix = Matrix.Scaled(2, 3, 4);
            scaleMatrix = scaleMatrix.Inverse;
            Float4 v = Float4.Vector(-4, 6, 8);
            return scaleMatrix * v == Float4.Vector(-2, 2, 2);
        }

        [UnitTest]
        public static bool TestScaleMatrixReflection() {
            Matrix scaleMatrix = Matrix.Scaled(-1, 1, 1);
            Float4 p = Float4.Point(2, 3, 4);
            return scaleMatrix * p == Float4.Point(-2, 3, 4);
        }

        [UnitTest]
        public static bool TestRotationMatrixX() {
            Float4 p = Float4.Point(0, 1, 0);
            Matrix hq = Matrix.RotationX(MathF.PI / 4f);
            Matrix fq = Matrix.RotationX(MathF.PI / 2f);

            return hq * p == Float4.Point(0f, MathF.Sqrt(2f) / 2f, MathF.Sqrt(2f) / 2f) &&
                fq * p == Float4.Point(0f, 0f, 1f);
        }

        [UnitTest]
        public static bool TestRotationMatrixXInverse()
        {
            Float4 p = Float4.Point(0f, 1f, 0f);
            Matrix hq = Matrix.RotationX(MathF.PI / 4f);
            hq = hq.Inverse;

            return hq * p == Float4.Point(0, MathF.Sqrt(2f) / 2f, -MathF.Sqrt(2f) / 2f);
        }

        [UnitTest]
        public static bool TestRotationMatrixY()
        {
            Float4 p = Float4.Point(0f, 0f, 1f);
            Matrix hq = Matrix.RotationY(MathF.PI / 4f);
            Matrix fq = Matrix.RotationY(MathF.PI / 2f);

            return hq * p == Float4.Point(MathF.Sqrt(2f) / 2f, 0f, MathF.Sqrt(2f) / 2f) &&
                fq * p == Float4.Point(1f, 0f, 0f);
        }

        [UnitTest]
        public static bool TestRotationMatrixZ()
        {
            Float4 p = Float4.Point(0f, 1f, 0f);
            Matrix hq = Matrix.RotationZ(MathF.PI / 4f);
            Matrix fq = Matrix.RotationZ(MathF.PI / 2f);

            return hq * p == Float4.Point(-MathF.Sqrt(2f) / 2f, MathF.Sqrt(2f) / 2f, 0f) &&
                fq * p == Float4.Point(-1f, 0f, 0f);
        }

        [UnitTest]
        public static bool TestMatrixShearXY() {
            Matrix shear = Matrix.Sheared(1, 0, 0, 0, 0, 0);
            Float4 p = Float4.Point(2, 3, 4);

            return shear * p == Float4.Point(5, 3, 4);
        }

        [UnitTest]
        public static bool TestMatrixShearXZ()
        {
            Matrix shear = Matrix.Sheared(0, 1, 0, 0, 0, 0);
            Float4 p = Float4.Point(2, 3, 4);

            return shear * p == Float4.Point(6, 3, 4);
        }

        [UnitTest]
        public static bool TestMatrixShearYX()
        {
            Matrix shear = Matrix.Sheared(0, 0, 1, 0, 0, 0);
            Float4 p = Float4.Point(2, 3, 4);

            return shear * p == Float4.Point(2, 5, 4);
        }

        [UnitTest]
        public static bool TestMatrixShearYZ()
        {
            Matrix shear = Matrix.Sheared(0, 0, 0, 1, 0, 0);
            Float4 p = Float4.Point(2, 3, 4);

            return shear * p == Float4.Point(2, 7, 4);
        }

        [UnitTest]
        public static bool TestMatrixShearZX()
        {
            Matrix shear = Matrix.Sheared(0, 0, 0, 0, 1, 0);
            Float4 p = Float4.Point(2, 3, 4);

            return shear * p == Float4.Point(2, 3, 6);
        }

        [UnitTest]
        public static bool TestMatrixShearZY()
        {
            Matrix shear = Matrix.Sheared(0, 0, 0, 0, 0, 1);
            Float4 p = Float4.Point(2, 3, 4);

            return shear * p == Float4.Point(2, 3, 7);
        }

        [UnitTest]
        public static bool TestMatrixTransformationChainInOrder() {
            Float4 p = Float4.Point(1, 0, 1);
            Matrix rx = Matrix.RotationX(MathF.PI / 2f);
            Matrix s = Matrix.Scaled(5f, 5f, 5f);
            Matrix t = Matrix.Translation(10, 5, 7);

            p *= rx;
            if (p != Float4.Point(1f, -1f, 0f))
                return false;

            p *= s;
            if (p != Float4.Point(5f, -5f, 0f))
                return false;
            p *= t;
            if (p != Float4.Point(15f, 0f, 7f))
                return false;

            return true;
        }

        [UnitTest]
        public static bool TestMatrixTransformationChainReverseOrder() {
            Float4 p = Float4.Point(1, 0, 1);
            Matrix rx = Matrix.RotationX(MathF.PI / 2f);
            Matrix s = Matrix.Scaled(5f, 5f, 5f);
            Matrix t = Matrix.Translation(10, 5, 7);

            Matrix T = t * s * rx; //Order of operation must be reversed to achieve correct results.
            Matrix Tordered = rx * s * t;
            return T * p == Float4.Point(15f, 0f, 7f) 
                && Tordered * p != Float4.Point(15f, 0f, 7f);
        }

        [UnitTest]
        public static bool TestMatrixFluentTransformations() {
            Float4 p = Float4.Point(1, 0, 1);
            
            Matrix m = Matrix.identity
                .RotateX(MathF.PI / 2f)
                .Scale(5f, 5f, 5f)
                .Translate(10f, 5f, 7f);

            return m * p == Float4.Point(15f, 0f, 7f);
        }

        [UnitTest]
        public static bool TestClockFaceMatrixCreation() {
            float radius = 40f;

            Float4 p = Float4.Point(0, 0, 1f);
            Canvas c = new Canvas(99, 99);
            Matrix s = Matrix.Scaled(radius, radius, radius);
            Matrix t = Matrix.Translation(49, 49, 49);

            Float4 lastPoint = new Float4();
            int pointThickness = 2;

            for (int i = 0; i < 13; i++)
            {
                Matrix r = Matrix.RotationY(i * ((MathF.PI / 6f)));
                p = Float4.Point(0f, 0f, 1f);
                p *= t * r * s;


                if (i != 0)
                {
                    Float4 pDir = (lastPoint - p);
                    float pDist = pDir.Magnitude;
                    for (int d = 0; d < Math.Abs(pDist); d++)
                    {
                        Float4 pp = p + (pDir.Normalised * d);
                        if(c.GetPixel(((int)pp.x), ((int)pp.z)) == Color.Black)
                            c.FillPixel(pp.x, pp.z, Color.Yellow);
                    }
                }

                for (int dx = -pointThickness; dx < pointThickness; dx++)
                {
                    for (int dy = -pointThickness; dy < pointThickness; dy++)
                    {
                            c.FillPixel(p.x + dx, p.z + dy, Color.FromHSV((360f / 12f) * i, 1f, 1f));
                    }
                }

                lastPoint = Float4.Point(p.x, p.y, p.z);
            }

            c.FillPixel(49f, 49f, Color.Red);

            string ppm = PPMWriter.CreatePPMString(c.Pixels);
            System.IO.File.WriteAllText("ClockFace.ppm", ppm);
            return true;
        }

        [UnitTest]
        public static bool TestRayCreation()
        {
            Float4 origin = Float4.Point(1, 2, 3);
            Float4 dir = Float4.Vector(4, 5, 6);

            Ray ray = new Ray(origin, dir);

            return ray.origin == origin && ray.direction == dir;
        }        
        
        [UnitTest]
        public static bool TestRayPositionRetrieval()
        {
            Float4 origin = Float4.Point(2, 3, 4);
            Float4 dir = Float4.Vector(1, 0, 0);

            Ray ray = new Ray(origin, dir);

            return ray.GetPositionAtTime(0) == origin &&
                ray.GetPositionAtTime(1) == Float4.Point(3, 3, 4) &&
                ray.GetPositionAtTime(-1) == Float4.Point(1, 3, 4) &&
                ray.GetPositionAtTime(2.5f) == Float4.Point(4.5f, 3, 4);
        }

        [UnitTest]
        public static bool TestRaySphereIntersection() {
            Float4 origin = Float4.Point(0, 0, -5);
            Float4 dir = Float4.Vector(0, 0, 1);

            Ray ray = new Ray(origin, dir);
        }
    }
}
