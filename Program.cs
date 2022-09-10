using System;
using RayTracer.UnitTesting;
using RayTracer.Math;
using System.Threading;

using RayTracer.Sandbox;
using RayTracer.Debugging;

namespace RayTracer.Core
{//Finished off at Cross Product
    public class Program
    {
        public static void Main(string[] args)
        {
            TestRunner.Run<UnitTesting.Tests.Tests>(false);
        }
    }
}
