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
        public void TestInvalidString()
        {
            var scanner = new Scanner("\"bar");
            var tokens = scanner.ScanTokens();

            var xx = tokens.Count();
        }
    }
}
