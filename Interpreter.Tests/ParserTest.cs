using CodeCrafters.Interpreter;
using CodeCrafters.Interpreter.Commands;

namespace Interpreter.Tests
{
    [TestClass]
    public sealed class ParserTest
    {
        /*
        [TestMethod]
        //[DataRow("evaluate.lox")]
        //[DataRow("parse.lox")]
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
        */

        private string TestInput(string input)
        {
            var scanner = new Scanner(input);
            var tokens = scanner.ScanTokens();

            var parser = new Parser(tokens);
            var expressions = parser.Parse();

            var ast = new AstPrinter();
            return ast.Print(expressions);
        }

        [TestMethod]
        [DataRow("\"bar", "")]
        //[DataRow("", "")]
        //[DataRow("", "")]
        //[DataRow("", "")]
        [ExpectedException(typeof(NullReferenceException))]
        public void TestParsingExpressionsSyntacticErrors(string input, string expect)
        {
            var result = TestInput(input);
            Assert.AreEqual(expect, result);
        }

        [TestMethod]
        [DataRow("\"hello\"!=\"foo\"", "(!= hello foo)")]
        [DataRow("\"foo\" == \"foo\"", "(== foo foo)")]
        [DataRow("76 == 69", "(== 76.0 69.0)")]
        [DataRow("(61 != 62) == ((-20 + 44) >= (95 * 15))", "(== (group (!= 61.0 62.0)) (group (>= (group (+ (- 20.0) 44.0)) (group (* 95.0 15.0)))))")]
        public void TestParsingExpressionsEqualityOperators(string input, string expect)
        {
            var result = TestInput(input);
            Assert.AreEqual(expect, result);
        }

        [TestMethod]
        [DataRow("81 > 24", "(> 81.0 24.0)")]
        [DataRow("57 <= 138", "(<= 57.0 138.0)")]
        [DataRow("81 < 138 < 195", "(< (< 81.0 138.0) 195.0)")]
        [DataRow("(30 - 60) >= -(25 / 54 + 59)", "(>= (group (- 30.0 60.0)) (- (group (+ (/ 25.0 54.0) 59.0))))")]
        public void TestParsingExpressionsComparisonOperators(string input, string expect)
        {
            var result = TestInput(input);
            Assert.AreEqual(expect, result);
        }

        [TestMethod]
        [DataRow("78 * 74 / 64", "(/ (* 78.0 74.0) 64.0)")]
        [DataRow("37 / 58 / 38", "(/ (/ 37.0 58.0) 38.0)")]
        [DataRow("52 * 45 * 46 / 60", "(/ (* (* 52.0 45.0) 46.0) 60.0)")]
        [DataRow("(67 * -48 / (78 * 17))", "(group (/ (* 67.0 (- 48.0)) (group (* 78.0 17.0))))")]
        [DataRow("\"hello\" + \"world\"", "(+ hello world)")]
        [DataRow("72 - 40 * 90 - 82", "(- (- 72.0 (* 40.0 90.0)) 82.0)")]
        [DataRow("37 + 50 - 99 / 37", "(- (+ 37.0 50.0) (/ 99.0 37.0))")]
        [DataRow("(-97 + 71) * (86 * 31) / (89 + 63)", "(/ (* (group (+ (- 97.0) 71.0)) (group (* 86.0 31.0))) (group (+ 89.0 63.0)))")]
        public void TestParsingExpressionsArithmeticOperators(string input, string expect)
        {
            var result = TestInput(input);
            Assert.AreEqual(expect, result);
        }

        [TestMethod]
        [DataRow("!false", "(! false)")]
        [DataRow("-88", "(- 88.0)")]
        [DataRow("!!false", "(! (! false))")]
        [DataRow("(!!(false))", "(group (! (! (group false))))")]
        public void TestParsingExpressionsUnaryOperators(string input, string expect)
        {
            var result = TestInput(input);
            Assert.AreEqual(expect, result);
        }

        [TestMethod]
        [DataRow("(\"foo\")", "(group foo)")]
        [DataRow("((true))", "(group (group true))")]
        [DataRow("(nil)", "(group nil)")]
        [DataRow("(74.22)", "(group 74.22)")]
        public void TestParsingExpressionsParentheses(string input, string expect)
        {
            var result = TestInput(input);
            Assert.AreEqual(expect, result);
        }

        [TestMethod]
        [DataRow("\"bar hello\"", "bar hello")]
        [DataRow("\"'world'\"", "'world'")]
        [DataRow("\"// baz\"", "// baz")]
        [DataRow("\"97\"", "97")]
        public void TestParsingExpressionsStringLiterals(string input, string expect)
        {
            var result = TestInput(input);
            Assert.AreEqual(expect, result);
        }

        [TestMethod]
        [DataRow("81", "81.0")]
        [DataRow("0.0", "0.0")]
        [DataRow("58.17", "58.17")]
        public void TestParsingExpressionsNumberLiterals(string input, string expect)
        {
            var result = TestInput(input);
            Assert.AreEqual(expect, result);
        }

        [TestMethod]
        [DataRow("true", "true")]
        [DataRow("false", "false")]
        [DataRow("nil", "nil")]
        public void TestParsingExpressionsBooleansAndNil(string input, string expect)
        {
            var result = TestInput(input);
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

            //Assert.AreEqual(expect, result);
        }
    }
}
