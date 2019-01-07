using System;
using System.Collections.Generic;

namespace Launcher
{
    public class Arg<T>
    {
        private Func<string, T> _parser;

        public string Name { get; private set; }

        public static Arg<T> Create(string name, Func<string, T> parser)
        {
            return new Arg<T>
            {
                _parser = parser,
                Name = name
            };
        }

        public T GetValue(Dictionary<string, string> args)
        {
            return args.TryGetValue(Name, out var value) ? _parser(value) : default;
        }
    }

    public class Args
    {
        public const string ArgPrefix = "--";

        public static Dictionary<string, string> Parse(string[] args)
        {
            var result = new Dictionary<string, string>();
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith(ArgPrefix))
                {
                    result.Add(
                        args[i].Replace(ArgPrefix, string.Empty),
                        i + 1 < args.Length && !args[i + 1].StartsWith(ArgPrefix) 
                            ? args[++i] 
                            : null);
                }
            }

            return result;
        }
    }
}
