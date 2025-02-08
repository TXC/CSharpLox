using CodeCrafters.Interpreter;

namespace Interpreter.Tests
{
    [TestClass]
    public sealed class ScanningTest
    {
        [TestMethod]
        //[DataRow("")]
        public void TestIdentifiers(string input, string expect)
        {
            var ss = new Scanner(input);
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
    }
}
