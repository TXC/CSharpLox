using System.Linq.Expressions;
using CodeCrafters.Interpreter;
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
        var scanner = new Scanner($"print {input};");
        var tokens = scanner.ScanTokens();

        var parser = new Parser(tokens);
        var expression = parser.Parse();
        var interpreter = new CodeCrafters.Interpreter.Interpreter();

        if (Lox.HadError)
        {
            throw new LoxException(64, "Has Error");
        }

        var res = interpreter.Run(expression);

        if (Lox.HadRuntimeError)
        {
            throw new LoxException(70, "Has Error");
        }

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

    //[TestMethod]
    //[DataRow("", "")]
    //[DataRow("", "")]
    //[DataRow("", "")]
    //[DataRow("", "")]
    //public void TestEvaluatingExpressionsArithmeticOperators(string input, string expect)
    //{
    //    var result = TestInput(input);
    //    Assert.AreEqual(expect, result);
    //}

    //[TestMethod]
    //[DataRow("", "")]
    //[DataRow("", "")]
    //[DataRow("", "")]
    //[DataRow("", "")]
    //public void TestEvaluatingExpressions(string input, string expect)
    //{
    //    var result = TestInput(input);
    //    Assert.AreEqual(expect, result);
    //}

    //[TestMethod]
    //[DataRow("", "")]
    //[DataRow("", "")]
    //[DataRow("", "")]
    //[DataRow("", "")]
    //public void TestEvaluatingExpressions(string input, string expect)
    //{
    //    var result = TestInput(input);
    //    Assert.AreEqual(expect, result);
    //}

    //[TestMethod]
    //[DataRow("", "")]
    //[DataRow("", "")]
    //[DataRow("", "")]
    //[DataRow("", "")]
    //public void TestEvaluatingExpressions(string input, string expect)
    //{
    //    var result = TestInput(input);
    //    Assert.AreEqual(expect, result);
    //}

    //[TestMethod]
    //[DataRow("", "")]
    //[DataRow("", "")]
    //[DataRow("", "")]
    //[DataRow("", "")]
    //public void TestEvaluatingExpressions(string input, string expect)
    //{
    //    var result = TestInput(input);
    //    Assert.AreEqual(expect, result);
    //}

    [TestMethod]
    [DataRow("\"bar\" < true")]
    //[DataRow("")]
    //[DataRow("")]
    //[DataRow("")]
    [ExpectedException(typeof(LoxException))]
    public void TestEvaluatingExpressions(string input)
    {
        TestInput(input);
    }
}
