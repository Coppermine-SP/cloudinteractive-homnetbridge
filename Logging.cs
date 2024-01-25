using System;
using System.Diagnostics;
using System.Reflection;
using HomNetBridge.Services;

namespace HomNetBridge
{
    internal static class Logging
    {
        public static bool UseVerbose = false;
        public static bool UseRaw = false;

        public enum LogType { Raw = 0, Debug = 1, Info = 2, Warn = 3, Error = 4 }

        private static readonly (ConsoleColor, string)[] prefixes =
        {
            (ConsoleColor.DarkMagenta, "[RAW]"),
            (ConsoleColor.Cyan, "[i]"),
            (ConsoleColor.Blue, "[i]"),
            (ConsoleColor.DarkYellow, "[!]"),
            (ConsoleColor.DarkRed, "[!]")
        };

        public static void Print(string message, LogType type = LogType.Info)
        {
            if (type == LogType.Debug && !UseVerbose) return;
            if (type == LogType.Raw && !UseRaw) return;

            var prefix = prefixes[(int)type];

            Console.ForegroundColor = prefix.Item1;
            Console.Write(prefix.Item2);

            if (type == LogType.Raw)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
            }
            else
            {
                var stackTrace = new StackTrace();
                var frame = stackTrace.GetFrame(1);
                var method = frame?.GetMethod()?.Name ?? "NULL";
                var className = frame?.GetMethod()?.DeclaringType?.Name ?? "NULL";

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write($" [{className}.{method}]: ");
                Console.ForegroundColor = ConsoleColor.Gray;

                HAService.PushLog($"HomNetBrige::{className}.{method}", $"{prefix.Item2} {message}", type == LogType.Debug);
            }

            Console.WriteLine(message);
        }

    }
}