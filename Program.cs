using System;
using RayTracer.UnitTesting;
using RayTracer.Maths;
using System.Threading;

using RayTracer.Sandbox;
using RayTracer.Debugging;
using BenchmarkDotNet.Running;

namespace RayTracer.Core
{//Finished off at Cross Product
    public class Program
    {
        public static void Main(string[] args)
        {
            TestRunner.Run<UnitTesting.Tests.Tests>(false);
            //BenchmarkRunner.Run<UnitTesting.Tests.Tests>();
        }
    }
}
