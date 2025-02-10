using System.Linq.Expressions;
using CodeCrafters.Interpreter;
using CodeCrafters.Interpreter.Ast;
using CodeCrafters.Interpreter.Exceptions;

namespace Interpreter.Tests;

[TestClass]
public sealed class InterpreterTest
{
    [TestMethod]
    [DataRow("evaluate.lox")]
    [DataRow("parse.lox")]
    public void TestLoxDataFiles(string filename)
    {
        var filePath = Path.Combine("LoxFiles", "Expressions", filename);
        //var content = File.ReadAllText(filePath).TrimEnd();
        //var matches = new LoxFileParser(content).Parse();
        //var interpreter = new CodeCrafters.Interpreter.Interpreter();
        //interpreter.Interpret();

        //for (var i = 0; i < tokens.Count; i++)
        //{
        //    Assert.AreEqual(matches[i], tokens[i].ToString());
        //}
        Assert.IsNotNull(filePath);
    }

    private static string TestInput(string input)
    {
        input = Lox.AddPrintInput(input);

        var scanner = new Scanner(input);
        var tokens = scanner.ScanTokens();

        var parser = new Parser(tokens);
        var expression = parser.Parse();
        var interpreter = new CodeCrafters.Interpreter.Interpreter();

        var res = interpreter.Run(expression);
        //if (Lox.HadError)
        //{
        //    throw new LoxException(64, "Has Error");
        //}
        //if (Lox.HadRuntimeError)
        //{
        //    throw new LoxException(70, "Has Runtime Error");
        //}

        return res;
    }

    [TestMethod]
    [DataRow("true", "true")]
    [DataRow("false", "false")]
    [DataRow("nil", "nil")]
    public void TestEvaluatingExpressionsLiteralsBooleansAndNil(string input, string expect)
    {
        var result = TestInput(input);
        Assert.AreEqual(expect, result);
    }

    [TestMethod]
    [DataRow("79", "79")]
    [DataRow("42.96", "42.96")]
    [DataRow("\"baz bar\"", "baz bar")]
    [DataRow("\"25\"", "25")]
    public void TestEvaluatingExpressionsLiteralsStringsAndNumbers(string input, string expect)
    {
        var result = TestInput(input);
        Assert.AreEqual(expect, result);
    }

    [TestMethod]
    [DataRow("(true)", "true")]
    [DataRow("(64)", "64")]
    [DataRow("(\"hello foo\")", "hello foo")]
    [DataRow("((false))", "false")]
    public void TestEvaluatingExpressionsLiteralsParentheses(string input, string expect)
    {
        var result = TestInput(input);
        Assert.AreEqual(expect, result);
    }

    [TestMethod]
    [DataRow("-84", "-84")]
    [DataRow("!false", "true")]
    [DataRow("!nil", "true")]
    [DataRow("(!!75)", "true")]
    public void TestEvaluatingExpressionsUnaryOperatorsNegationAndNot(string input, string expect)
    {
        var result = TestInput(input);
        Assert.AreEqual(expect, result);
    }

    [TestMethod]
    [DataRow("73 - 69", "4")]
    [DataRow("42 + 69 - 66", "45")]
    [DataRow("33 + 82 - (-(34 - 49))", "100")]
    [DataRow("(-93 + 93) * (32 * 63) / (1 + 4)", "0")]
    [DataRow("45 * 87", "3915")]
    [DataRow("57 / 5", "11.4")]
    [DataRow("7 * 3 / 7 / 1", "3")]
    [DataRow("(18 * 3 / (3 * 6))", "3")]
    public void TestEvaluatingExpressionsArithmeticOperators(string input, string expect)
    {
        var result = TestInput(input);
        Assert.AreEqual(expect, result);
    }

    [TestMethod]
    [DataRow("\"quz\" + \"quz\"", "quzquz")]
    [DataRow("\"world\" + \"34\"", "world34")]
    [DataRow("\"foo\" + \"foo\" + \"hello\"", "foofoohello")]
    [DataRow("(\"foo\" + \"foo\") + (\"bar\" + \"quz\")", "foofoobarquz")]
    public void TestEvaluatingExpressionsStringConcatenation(string input, string expect)
    {
        var result = TestInput(input);
        Assert.AreEqual(expect, result);
    }

    [TestMethod]
    [DataRow("98 > -151", "true")]
    [DataRow("98 <= 241", "true")]
    [DataRow("90 >= 90", "true")]
    [DataRow("(58 - 99) >= -(196 / 98 + 45)", "true")]
    public void TestEvaluatingExpressionsRelationalOperators(string input, string expect)
    {
        var result = TestInput(input);
        Assert.AreEqual(expect, result);
    }

    [TestMethod]
    [DataRow("\"hello\" != \"quz\"", "true")]
    [DataRow("\"world\" == \"world\"", "true")]
    [DataRow("28 == \"28\"", "false")]
    [DataRow("166 == (92 + 74)", "true")]
    public void TestEvaluatingExpressionsEqualityOperators(string input, string expect)
    {
        var result = TestInput(input);
        Assert.AreEqual(expect, result);
    }

    [TestMethod]
    [DataRow("-\"foo\"")]
    [DataRow("-true")]
    [DataRow("-false")]
    [DataRow("-(\"baz\" + \"baz\")")]
    public void TestEvaluatingExpressionsRuntimeErrorsUnaryOperators(string input)
    {
        var result = TestInput(input);
        Assert.IsTrue(Lox.HadRuntimeError);
        Assert.IsFalse(Lox.HadError);
        Assert.AreEqual("", result);
    }

    [TestMethod]
    [DataRow("\"bar\" + false")]
    [DataRow("65 + \"quz\" + 12")]
    [DataRow("79 - false")]
    [DataRow("false - (\"hello\" + \"world\")")]
    [DataRow("66 * \"foo\"")]
    [DataRow("\"hello\" / 84")]
    [DataRow("true / false")]
    [DataRow("(\"world\" + \"hello\") * (\"baz\" + \"hello\")")]
    public void TestEvaluatingExpressionsRuntimeErrorsBinaryOperators(string input)
    {
        var result = TestInput(input);
        Assert.IsTrue(Lox.HadRuntimeError);
        Assert.IsFalse(Lox.HadError);
        Assert.AreEqual("", result);
    }

    [TestMethod]
    [DataRow("\"quz\" < true")]
    [DataRow("true <= (38 + 29)")]
    [DataRow("94 > (\"quz\" + \"hello\")")]
    [DataRow("false >= true")]
    public void TestEvaluatingExpressionsRuntimeErrorsRelationalOperators(string input)
    {
        var result = TestInput(input);
        Assert.IsTrue(Lox.HadRuntimeError);
        Assert.IsFalse(Lox.HadError);
        Assert.AreEqual("", result);
    }

    [TestMethod]
    [DataRow("// This program prints a boolean\nprint false;", "false")]
    [DataRow("// This program prints the result of the concatenation of three strings\nprint \"quz\" + \"bar\" + \"foo\";", "quzbarfoo")]
    [DataRow("// This program prints the result of an arithmetic expression\nprint (54 * 2 + 38 * 2) / (2);", "92")]
    [DataRow("// Print statements expect an expression\n// So, this program leads to a compilation error\nprint;", "Error at ';': Expect expression.")]
    public void TestStatementsAndStatePrintGenerateOutput(string input, string expect)
    {
        var result = TestInput(input);
        Assert.AreEqual(expect, result);
    }
}
