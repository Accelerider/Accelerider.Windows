using System;
using System.Collections.Generic;
using System.Linq;
// ReSharper disable LocalizableElement

namespace Package
{
    public static class Print
    {
        private enum OutType
        {
            Info,
            Warning,
            Error
        }

        private static readonly IDictionary<OutType, ConsoleColor> OutColors = new Dictionary<OutType, ConsoleColor>
        {
            [OutType.Info] = ConsoleColor.DarkGreen,
            [OutType.Warning] = ConsoleColor.DarkYellow,
            [OutType.Error] = ConsoleColor.DarkRed,
        };

        public static void Info(string message) => WriteToConsole(message);

        public static void Warning(string message) => WriteToConsole(message, OutType.Warning);

        public static void Error(string message) => WriteToConsole(message, OutType.Error);

        private static void WriteToConsole(string message, OutType outType = OutType.Info)
        {
            var backupBackground = Console.BackgroundColor;
            var backupForeground = Console.ForegroundColor;

            Console.BackgroundColor = OutColors[outType];
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write($"[{outType.ToString().ToUpper()}]");
            Console.BackgroundColor = backupBackground;

            Console.ForegroundColor = OutColors[outType];
            var spaces = Enumerable.Repeat(" ", 8 - outType.ToString().Length).Aggregate((acc, item) => acc + item);
            Console.WriteLine($"{spaces}{message}");
            Console.ForegroundColor = backupForeground;
        }

        public static void Divider()
        {
            Console.WriteLine("=========================================================");
        }

        public static void EndLine()
        {
            Console.WriteLine();
            Console.WriteLine();
            Divider();
            Console.WriteLine("Enter any key to exit...");
            Divider();
        }
    }
}
