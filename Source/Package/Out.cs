using System;
using System.Collections.Generic;
using System.Linq;

namespace Package
{
    public enum OutType
    {
        Info,
        Warning,
        Error
    }

    public static class Out
    {
        private static readonly IDictionary<OutType, ConsoleColor> OutColors = new Dictionary<OutType, ConsoleColor>
        {
            [OutType.Info] = ConsoleColor.DarkGreen,
            [OutType.Warning] = ConsoleColor.DarkYellow,
            [OutType.Error] = ConsoleColor.DarkRed,
        };

        public static void Print(string message, OutType outType = OutType.Info, ConsoleColor color = ConsoleColor.Green)
        {
            var backupBackground = Console.BackgroundColor;
            var backupForeground = Console.ForegroundColor;

            Console.BackgroundColor = OutColors[outType];
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"[{outType.ToString().ToUpper()}]");
            Console.BackgroundColor = backupBackground;

            Console.ForegroundColor = OutColors[outType];
            var spaces = Enumerable.Repeat(" ", 8 - outType.ToString().Length).Aggregate((acc, item) => acc + item);
            Console.WriteLine($"{spaces}{message}");
            Console.ForegroundColor = backupForeground;
        }

        public static void EndLine()
        {
            Console.WriteLine();
            Console.WriteLine();
            Divider();
            Console.WriteLine("Enter any key to exit...");
            Divider();
        }

        public static void Divider()
        {
            Console.WriteLine("=========================================================");
        }
    }
}
