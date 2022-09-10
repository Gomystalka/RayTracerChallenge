using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Threading;
using RayTracer.Debugging;
using RayTracer.Utility;

using StackFrame = System.Diagnostics.StackFrame;

namespace RayTracer.UnitTesting
{
    /// <summary>
    /// Class used for invoking all Unit Tests within a class.
    /// </summary>
    public class TestRunner
    {
        private const string kHeaderLines = "------------------------";
        private delegate void Runnable(bool isOnSeparateThread);
        private readonly static Dictionary<Type, object> _instanceMap = new Dictionary<Type, object>();

        /// <summary>
        /// Runs all tests defined within the type <b>T</b>
        /// </summary>
        /// <param name="throwExceptionOnTestFail">Determines whether an exception should be thrown when a test fails.</param>
        public static void Run<T>(bool throwExceptionOnTestFail = true) {
            Type genericType = typeof(T);
            object context = null;

            MethodInfo[] methods = genericType.GetMethods();
            foreach (MethodInfo method in methods) {
                UnitTestAttribute attr = method.GetCustomAttribute<UnitTestAttribute>();
                if (attr == null) continue;
                string sourceName = $"{genericType.Name}{(method.IsStatic ? "" : "(Instance)")}::{method.Name}";

                if (attr.Skip) {
                    Debug.LogWarning($"[{sourceName}] -> Unit Test skipped.");
                    continue;
                }

                if (!method.IsStatic)
                {
                    if (!_instanceMap.ContainsKey(genericType))
                    {
                        context = Activator.CreateInstance<T>();
                        Debug.LogColoredMessage($"[{nameof(TestRunner)}]: Created and cached new instance for [{genericType.Name}].", ConsoleColor.Blue);
                        _instanceMap.Add(genericType, context);
                    }
                    else
                        context = _instanceMap[genericType];
                }

                if (method.ReturnType != typeof(bool))
                    throw new UnitTestException($"[{sourceName}]: The return type of a Unit Test function must be 'bool'!");

                Runnable runnable = (isOnSeparateThread) => {
                    Thread currentThread = Thread.CurrentThread;
                    Console.ForegroundColor = ConsoleColor.Cyan; //Idk why but this is required. Probably thread stuff.
                    if (isOnSeparateThread)
                        Debug.LogColoredMessage($"{System.Environment.NewLine}{kHeaderLines}[{sourceName}] on Thread #{currentThread.ManagedThreadId} START{kHeaderLines}", ConsoleColor.Cyan);
                        //Debug.LogColoredMessage($"Running [{sourceName}] on Thread #{currentThread.ManagedThreadId}", ConsoleColor.Cyan);

                    object o = method.Invoke(context, null);

                    if (o is bool result)
                        if (!result)
                        {
                            Debug.LogError($"[{sourceName}] -> Unit Test failed! Result was false.");

                            if (throwExceptionOnTestFail)
                                throw new UnitTestException($"[{sourceName}]: Unit Test failed! Result was false.");
                        }
                        else
                            Debug.LogColoredMessage($"[{sourceName}] -> Unit Test passed!", ConsoleColor.Green);

                    if (isOnSeparateThread)
                        Debug.LogColoredMessage($"{kHeaderLines}[{sourceName}] on Thread #{currentThread.ManagedThreadId} END{kHeaderLines}", ConsoleColor.Cyan);
                    //Debug.LogColoredMessage($"[{sourceName}]: Thread #{currentThread.ManagedThreadId} has been terminated.", ConsoleColor.Cyan);
                };
                if (attr.RunOnSeparateThread)
                    RunOnThread(() => {
                        runnable.Invoke(true);
                    });
                else
                    runnable.Invoke(false);
            }
        }

        private static void RunOnThread(ThreadStart runnable) {
            Thread thread = new Thread(runnable);
            thread.Start();
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class UnitTestAttribute : Attribute {
        /// <summary>
        /// Skips the test execution and notifies the user about it.
        /// </summary>
        public bool Skip { get; set; }
        /// <summary>
        /// Runs the test on a separate thread. The thread can be received with <b>Thread.CurrentThread</b>.
        /// </summary>
        public bool RunOnSeparateThread { get; set; }

        public UnitTestAttribute(bool skip = false)
            => Skip = skip;
    }

    //[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    //public class UnitTestClassAttribute : Attribute { }

    /// <summary>
    /// Generic exception
    /// </summary>
    class UnitTestException : Exception {
        public UnitTestException(string message) : base(message) { }
        public UnitTestException(string message, Exception innerException) : base(message, innerException) { }
        public UnitTestException() { }
    }
}
