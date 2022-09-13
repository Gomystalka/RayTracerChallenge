using System;
using RayTracer.UnitTesting;
using RayTracer.Maths;
using System.Threading;

using RayTracer.Sandbox;
using RayTracer.Debugging;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Attributes;

namespace RayTracer.Core
{//Finished off at Cross Product]

    [MemoryDiagnoser]
    public class Program
    {
        public static void Main(string[] args)
        {
            TestRunner.Run<UnitTesting.Tests.Tests>(false);
            //BenchmarkRunner.Run<Program>();
            //unsafe
            //{
            //    Debug.Log($"Size of TRS: {Matrix.identity.SizeOf + (sizeof(Float4) * 3f)}b");
            //}

            //long before = GC.GetTotalMemory(true);
            //Matrix m = Matrix.Translation(100, 100, 100);
            //Debug.Log($"Full Alloc Size: {GC.GetTotalMemory(true) - before}b");
            Debug.Log($"Calculated Matrix Size: {Matrix.identity.SizeOf}b");
        }

        [Benchmark]
        public void CalculateTRSMatrix() {
            //Matrix m = Matrix.TRS(Float4.Point(10, 2, 3), Float4.Vector(0, 2, 0), Float4.Vector(10, 10, 10));
            //Matrix m = new Matrix(4, 4);
        }

        [Benchmark]
        public bool TestMatrixFluentTransformations()
        {
            Float4 p = Float4.Point(1, 0, 1);

            Matrix m = Matrix.identity
                .RotateX(MathF.PI / 2f)
                .Scale(5f, 5f, 5f)
                .Translate(10f, 5f, 7f);

            return m * p == Float4.Point(15f, 0f, 7f);
        }
    }
}
