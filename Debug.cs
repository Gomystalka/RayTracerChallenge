using System;
using System.Collections.Generic;
using System.Text;

namespace RayTracer.Debugging
{
    public static class Debug
    {
        public static void Log(string message)
            => Console.WriteLine(message);
        public static void Log(object obj)
            => Console.WriteLine(obj ?? "null");
        public static void LogWarning(object warning)
            => LogColoredMessage(warning ?? "null", ConsoleColor.DarkYellow);
        public static void LogError(object error)
            => LogColoredMessage(error ?? "null", ConsoleColor.Red);
        public static void LogAssert(bool assertionCondition, object message) {
            if (!assertionCondition)
                LogWarning(message ?? "null");
        }
        public static void LogColoredMessage(object obj, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(obj ?? "null");
            ResetConsole();
        }

        public static ConsoleKeyInfo WaitForKey(bool intercept = false)
            => Console.ReadKey(intercept);

        private static void ResetConsole() {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
        }

        public static void Clear()
            => Console.Clear();
    }
}
