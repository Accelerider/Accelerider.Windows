using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static void Completed()
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("=========================================================");
            Console.WriteLine("Enter any key to exit...");
            Console.WriteLine("=========================================================");
        }
    }
}
