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
}
