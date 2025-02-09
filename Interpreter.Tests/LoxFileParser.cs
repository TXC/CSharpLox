using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace Interpreter.Tests
{
    internal partial class LoxFileParser
    {
        private readonly ILogger _logger;
        private readonly string _source;

        public LoxFileParser(string source)
        {
            using var factory = LoggerFactory.Create(builder => builder.AddConsole());
            _logger = factory.CreateLogger("LoxFileParser");
            _source = source;
        }

        public List<string> Parse()
        {
            var r = LoxExpect();
            var matches = r.Matches(_source);
            var res = new List<string>();

            for (var i = 0; i < matches.Count; i++)
            {
                res.Add(matches[i].Groups[1].Value);
            }

            return res;
        }

        [GeneratedRegex(@"^// expect: (.+)$", RegexOptions.Multiline | RegexOptions.Compiled)]
        private static partial Regex LoxExpect();
    }
}
