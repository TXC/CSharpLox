using CodeCrafters.Interpreter;
using CodeCrafters.Interpreter.Commands;

namespace Interpreter.Tests
{
    [TestClass]
    public sealed class ParserTest
    {
        [TestMethod]
        [DataRow("evaluate.lox")]
        [DataRow("parse.lox")]
        public void TestLoxDataFiles(string filename)
        {
            var filePath = Path.Combine("LoxFiles", "Expressions", filename);
            var content = File.ReadAllText(filePath).TrimEnd();
            var matches = new LoxFileParser(content).Parse();

            //var content = ParseFile(args);
            var scanner = new Scanner(content);
            var tokens = scanner.ScanTokens();

            var parser = new Parser(tokens);
            var expressions = parser.Parse();

            for (var i = 0; i < tokens.Count; i++)
            {
                Assert.AreEqual(matches[i], tokens[i].ToString());
            }
        }

        [TestMethod]
        public void TestBasicExpression()
        {
            var expected = "(+ 2.0 3.0)";
            var content = "2 + 3";

            var scanner = new Scanner(content);
            var tokens = scanner.ScanTokens();

            var parser = new Parser(tokens);
            var expressions = parser.Parse() ?? throw new NullReferenceException();

            var ast = new AstPrinter();
            var result = ast.Print(expressions);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        //[DataRow(null, "nil")]
        [DataRow("true", "true")]
        [DataRow("false", "false")]
        public void TestBooleanExpression(string input, string expect)
        {
            var scanner = new Scanner(input);
            var tokens = scanner.ScanTokens();

            var parser = new Parser(tokens);
            var expressions = parser.Parse() ?? throw new NullReferenceException();

            var ast = new AstPrinter();
            var result = ast.Print(expressions);

            Assert.AreEqual(expect, result);
        }

        [TestMethod]
        [DataRow("while", "")]
        public void TestKeywordExpression(string input, string expect)
        {
            var scanner = new Scanner(input);
            var tokens = scanner.ScanTokens();

            var parser = new Parser(tokens);
            var expressions = parser.Parse();

            var ast = new AstPrinter();
            var result = ast.Print(expressions);

            Assert.AreEqual(expect, result);
        }
    }
}
