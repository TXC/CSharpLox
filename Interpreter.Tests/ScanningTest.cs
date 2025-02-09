using System.Text.RegularExpressions;
using CodeCrafters.Interpreter;
using CodeCrafters.Interpreter.Tokens;
using Microsoft.Extensions.FileSystemGlobbing.Internal;

namespace Interpreter.Tests
{
    [TestClass]
    public sealed partial class ScanningTest
    {
        [TestMethod]
        [DataRow("identifiers.lox")]
        [DataRow("keywords.lox")]
        [DataRow("numbers.lox")]
        [DataRow("punctuators.lox")]
        [DataRow("strings.lox")]
        [DataRow("whitespace.lox")]
        public void TestLoxDataFiles(string filename)
        {
            var r = LoxExpect();
            var filePath = Path.Combine("Scanning", filename);
            var content = File.ReadAllText(filePath).TrimEnd();
            var matches = r.Matches(content);
            var ss = new Scanner(content);

            var tokens = ss.ScanTokens();
            for (var i = 0; i < tokens.Count; i++)
            {
                Assert.AreEqual(matches[i].Groups[1].Value,
                                tokens[i].ToString());
            }
        }

        [TestMethod]
        [DataRow("\"bar")]
        [DataRow("\"bar\" \"unterminated")]
        public void TestInvalidString(string input)
        {
            var scanner = new Scanner(input);
            var tokens = scanner.ScanTokens();

            //var strings = tokens.Select(t => t.ToString()).ToArray();

            Assert.IsTrue(tokens.Any(x => x.Type is null));
            Assert.IsTrue(tokens.Any());
        }

        [TestMethod]
        [DataRow("14", "NUMBER 14 14.0")]
        [DataRow("123", "NUMBER 123 123.0")]
        public void TestNumber(string input, string expected)
        {
            var scanner = new Scanner(input);
            var tokens = scanner.ScanTokens();

            var value = tokens.First(x => x.Type == TokenType.NUMBER).ToString();

            Assert.AreEqual(expected, value);
        }

        [TestMethod]
        [DataRow("foo bar _hello", new object[] { "IDENTIFIER foo null", "IDENTIFIER bar null", "IDENTIFIER _hello null", "EOF  null" })]
        public void TestIdentifiers(string input, object[] expected)
        {
            var scanner = new Scanner(input);
            var tokens = scanner.ScanTokens();

            for (var i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i].ToString(), tokens[i].ToString());
            }
        }

        [GeneratedRegex(@"^// expect: (.+)$", RegexOptions.Multiline | RegexOptions.Compiled)]
        private static partial Regex LoxExpect();
    }
}
