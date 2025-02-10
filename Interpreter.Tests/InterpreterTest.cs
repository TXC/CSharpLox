using System.Linq.Expressions;
using CodeCrafters.Interpreter;

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
        var content = File.ReadAllText(filePath).TrimEnd();
        var matches = new LoxFileParser(content).Parse();
        var interpreter = new CodeCrafters.Interpreter.Interpreter();
        //interpreter.Interpret();

        //for (var i = 0; i < tokens.Count; i++)
        //{
        //    Assert.AreEqual(matches[i], tokens[i].ToString());
        //}
    }

    private string TestInput(string input)
    {
        var scanner = new Scanner(input);
        var tokens = scanner.ScanTokens();

        var parser = new Parser(tokens);
        var expression = parser.Parse();

        if (expression is null)
        {
            return "";
        }

        var interpreter = new CodeCrafters.Interpreter.Interpreter();

        if (Lox.CheckError())
        {
            return "";
        }

        return interpreter.Run(expression);
    }

    [TestMethod]
    [DataRow("true", "true")]
    [DataRow("false", "false")]
    [DataRow("nil", "nil")]
    [DataRow("", "")]
    public void TestEvaluatingExpressionsLiteralsBooleansAndNil(string input, string expect)
    {
        var result = TestInput(input);
        Assert.AreEqual(expect, result);
    }

}
