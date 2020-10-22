using System;
using System.Collections.Generic;

namespace Launcher
{
    public class ArgParser<TArgs> where TArgs : new()
    {
        public const string ArgNamePrefix = "--";
        public const string StringTrue = "true";

        //private static readonly Regex ArgRegex = new Regex("^--([\\w\\.-]+?)(?:([\"\\w\\.-])+?)?$", RegexOptions.Compiled);

        private readonly Dictionary<string, Action<TArgs, string>> _parsers = new Dictionary<string, Action<TArgs, string>>();

        public ArgParser<TArgs> Define<T>(string name, Action<TArgs, T> parser)
        {
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));

            return Define(name, (o, value) =>
            {
                var typeChangedValue = (T)Convert.ChangeType(value, typeof(T));
                parser(o, typeChangedValue);
            });
        }

        public ArgParser<TArgs> Define(string name, Action<TArgs, string> parser)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            if (parser == null)
                throw new ArgumentNullException(nameof(parser));

            _parsers.Add(name, parser);

            return this;
        }

        public TArgs Parse(string[] args)
        {
            var result = new TArgs();

            var argDictionary = new Dictionary<string, string>();
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith(ArgNamePrefix))
                {
                    argDictionary.Add(
                        args[i].Replace(ArgNamePrefix, string.Empty),
                        i + 1 < args.Length && !args[i + 1].StartsWith(ArgNamePrefix)
                            ? args[++i]
                            : StringTrue);
                }
            }

            foreach (var arg in argDictionary)
            {
                if (_parsers.TryGetValue(arg.Key, out var parser))
                {
                    parser(result, arg.Value);
                }
            }

            return result;
        }
    }
}
