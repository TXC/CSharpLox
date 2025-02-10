using System.Linq.Expressions;
using CodeCrafters.Interpreter;
using CodeCrafters.Interpreter.Ast;
using CodeCrafters.Interpreter.Exceptions;
using static CodeCrafters.Interpreter.Stmt;

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

    private static string TestInputEvaluation(string source)
    {
        var input = Lox.AddPrintInput(source);

        var scanner = new Scanner(input);
        var tokens = scanner.ScanTokens();

        var parser = new Parser(tokens);
        var statements = parser.Parse();
        var interpreter = new CodeCrafters.Interpreter.Interpreter();

        var res = interpreter.Run(statements);
        //if (Lox.HadError)
        //{
        //    throw new LoxException(64, "Has Error");
        //}
        //if (Lox.HadRuntimeError)
        //{
        //    throw new LoxException(70, "Has Runtime Error");
        //}

        return res.Trim();
    }

    private static string TestInputStatement(string source)
    {
        var input = source;

        var scanner = new Scanner(input);
        var tokens = scanner.ScanTokens();

        var parser = new Parser(tokens);
        var statements = parser.Parse();
        var interpreter = new CodeCrafters.Interpreter.Interpreter();

        var res = interpreter.Run(statements);
        return res.Trim();
    }

    [TestMethod]
    [DataRow("true", "true")]
    [DataRow("false", "false")]
    [DataRow("nil", "nil")]
    public void TestEvaluatingExpressionsLiteralsBooleansAndNil(string input, string expect)
    {
        var result = TestInputEvaluation(input);
        Assert.AreEqual(expect.Trim(), result);
    }

    [TestMethod]
    [DataRow("79", "79")]
    [DataRow("42.96", "42.96")]
    [DataRow("\"baz bar\"", "baz bar")]
    [DataRow("\"25\"", "25")]
    public void TestEvaluatingExpressionsLiteralsStringsAndNumbers(string input, string expect)
    {
        var result = TestInputEvaluation(input);
        Assert.AreEqual(expect.Trim(), result);
    }

    [TestMethod]
    [DataRow("(true)", "true")]
    [DataRow("(64)", "64")]
    [DataRow("(\"hello foo\")", "hello foo")]
    [DataRow("((false))", "false")]
    public void TestEvaluatingExpressionsLiteralsParentheses(string input, string expect)
    {
        var result = TestInputEvaluation(input);
        Assert.AreEqual(expect.Trim(), result);
    }

    [TestMethod]
    [DataRow("-84", "-84")]
    [DataRow("!false", "true")]
    [DataRow("!nil", "true")]
    [DataRow("(!!75)", "true")]
    public void TestEvaluatingExpressionsUnaryOperatorsNegationAndNot(string input, string expect)
    {
        var result = TestInputEvaluation(input);
        Assert.AreEqual(expect.Trim(), result);
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
        var result = TestInputEvaluation(input);
        Assert.AreEqual(expect.Trim(), result);
    }

    [TestMethod]
    [DataRow("\"quz\" + \"quz\"", "quzquz")]
    [DataRow("\"world\" + \"34\"", "world34")]
    [DataRow("\"foo\" + \"foo\" + \"hello\"", "foofoohello")]
    [DataRow("(\"foo\" + \"foo\") + (\"bar\" + \"quz\")", "foofoobarquz")]
    public void TestEvaluatingExpressionsStringConcatenation(string input, string expect)
    {
        var result = TestInputEvaluation(input);
        Assert.AreEqual(expect.Trim(), result);
    }

    [TestMethod]
    [DataRow("98 > -151", "true")]
    [DataRow("98 <= 241", "true")]
    [DataRow("90 >= 90", "true")]
    [DataRow("(58 - 99) >= -(196 / 98 + 45)", "true")]
    public void TestEvaluatingExpressionsRelationalOperators(string input, string expect)
    {
        var result = TestInputEvaluation(input);
        Assert.AreEqual(expect.Trim(), result);
    }

    [TestMethod]
    [DataRow("\"hello\" != \"quz\"", "true")]
    [DataRow("\"world\" == \"world\"", "true")]
    [DataRow("28 == \"28\"", "false")]
    [DataRow("166 == (92 + 74)", "true")]
    public void TestEvaluatingExpressionsEqualityOperators(string input, string expect)
    {
        var result = TestInputEvaluation(input);
        Assert.AreEqual(expect.Trim(), result);
    }

    [TestMethod]
    [DataRow("-\"foo\"")]
    [DataRow("-true")]
    [DataRow("-false")]
    [DataRow("-(\"baz\" + \"baz\")")]
    public void TestEvaluatingExpressionsRuntimeErrorsUnaryOperators(string input)
    {
        var result = TestInputEvaluation(input);
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
        var result = TestInputEvaluation(input);
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
        var result = TestInputEvaluation(input);
        Assert.IsTrue(Lox.HadRuntimeError);
        Assert.IsFalse(Lox.HadError);
        Assert.AreEqual("", result);
    }

    [TestMethod]
    [DataRow("// This program prints a boolean\nprint false;", "false")]
    [DataRow("// This program prints the result of the concatenation of three strings\nprint \"quz\" + \"bar\" + \"foo\";", "quzbarfoo")]
    [DataRow("// This program prints the result of an arithmetic expression\nprint (54 * 2 + 38 * 2) / (2);", "92")]
    [DataRow("// Print statements expect an expression\n// So, this program leads to a compilation error\nprint;", "")]
    public void TestStatementsAndStatePrintGenerateOutput(string input, string expect)
    {
        var result = TestInputStatement(input);
        Assert.AreEqual(expect.Trim(), result);
    }

    [TestMethod]
    [DataRow("// This program prints the result of arithmetic operations involving strings and integers\nprint \"baz\" + \"bar\" + \"world\";\nprint 70 - 91;\nprint \"world\" == \"hello\";", "bazbarworld\n-21\nfalse\n")]
    [DataRow("// This program tests the syntax of multiple statements in a single line\nprint \"quz\"; print true;\nprint true;\nprint \"foo\"; print 28;","quz\ntrue\ntrue\nfoo\n28\n")]
    [DataRow("print 89;\n   print 89 + 19;\n       print 89 + 19 + 44;\n","89\n108\n152\n")]
    [DataRow("// This program prints the result of a comparison operation\n"+
             "// It also tests multi-line strings and non-ASCII characters\n" +
             "print false != false;\n\n" +
             "print \"87\n67\n30\n\";\n\n" +
             "print \"There should be an empty line above this.\";\n\n" +
             "print \"(\" + \"\" + \")\";\n\n" +
             "print \"non-ascii: ॐ\";\n",
              "false\n87\n67\n30\n\nThere should be an empty line above this.\n()\nnon-ascii: ॐ\n")]
    public void TestStatementsAndStatePrintMultipleStatements(string input, string expect)
    {
        var result = TestInputStatement(input);

        Assert.AreEqual(expect.Trim(), result);
    }

    [TestMethod]
    [DataRow("// This program tests that statements are executed even if they don't have any side effects\n// It also tests complex arithmetic expressions and string concatenation\n(25 + 55 - 77) > (69 - 25) * 2;\nprint !false;\n\"foo\" + \"hello\" + \"baz\" + \"quz\" == \"foohellobazquz\";\nprint !false;", "true\ntrue")]
    [DataRow("// This program tests statements that don't have any side effects\n41 - 80 >= -15 * 2 / 15 + 97;\nfalse == false;\n(\"bar\" == \"baz\") == (\"quz\" != \"hello\");\nprint false;\n", "false")]
    [DataRow("// This program tests that the + operator is only supported when both operands are numbers or both are strings\nprint \"the expression below is invalid\";\n59 + \"hello\";\nprint \"this should not be printed\";\n", "the expression below is invalid")]
    [DataRow("// This program tests that the * operator is only supported when both operands are numbers\nprint \"97\" + \"hello\";\nprint true * (51 + 20);\n", "97hello")]
    public void TestStatementsAndStateExpressionStatements(string input, string expect)
    {
        var result = TestInputStatement(input);
        Assert.AreEqual(expect, result);
    }

    [TestMethod]
    [DataRow("// This program tests that variables are initialized to the correct value\nvar bar = 10;\nprint bar;", "10")]
    [DataRow("// This program declares multiple variables and<|SPACE|>// prints the result of arithmetic operations on them\nvar foo = 89;\nvar hello = 89;\nprint foo + hello;\nvar baz = 89;\nprint foo + hello + baz;\n", "178\n267")]
    [DataRow("// This program assigns the result of an arithmetic expression to a variable\n// Then it prints the value of the variable\nvar bar = (8 * (56 + 56)) / 4 + 56;\nprint bar;\n", "280")]
    [DataRow("// This program declares variables and performs operations on them\n// Finally it prints the result of the operations\nvar baz = 47;\nvar bar = baz;\nprint bar + baz;\n", "94")]
    public void TestStatementsAndStateVariablesDeclareVariables(string input, string expect)
    {
        var result = TestInputStatement(input);
        //Assert.IsTrue(Lox.HadRuntimeError);
        //Assert.IsFalse(Lox.HadError);
        Assert.AreEqual(expect, result);
    }

    [TestMethod]
    [DataRow("// This program tries to access a variable before it is declared\n// It leads to a runtime error\nprint 96;\nprint x;\n", "96")]
    [DataRow("// This program tries to access a variable before it is declared\n// It leads to a runtime error\nvar bar = 29;\nprint hello;\n", "")]
    [DataRow("// This program tries to access a variable before it is declared\n// It leads to a runtime error\nvar foo = 33;\nvar result = (foo + quz) / world;\nprint result;\n", "")]
    [DataRow("// This program tries to access a variable befdecore it is lared\n// It leads to a runtime error\nvar quz = 34;\nvar baz = 21;\nvar bar = 27;\nprint quz + baz + bar + world; print 33;\n", "")]
    public void TestStatementsAndStateVariablesRuntimeErrors(string input, string expect)
    {
        var result = TestInputStatement(input);
        Assert.IsTrue(Lox.HadRuntimeError);
        Assert.IsFalse(Lox.HadError);
        Assert.AreEqual(expect, result);
    }


    [TestMethod]
    [DataRow("// This program declares a variable but doesn't initialize it\n// It prints the variable's value, which should be nil\nvar baz;\nprint baz;\n", "nil")]
    [DataRow("// This program declares a variable but doesn't initialize it\n// It prints the variable's value\nvar foo = \"baz\";\nvar bar;\nprint bar;", "nil")]
    [DataRow("// This program declares a variable but doesn't initialize it\n// It prints the variable's value\nvar bar = 45;\nvar baz;\nvar quz;\nprint baz;\n", "nil")]
    [DataRow("// This program declares a variable but doesn't initialize it\n// It prints the variable's value\nvar quz = 70 + 15 * 62;\nprint quz;\nvar hello = 15 * 62;\nprint quz + hello;\nvar baz;\nprint baz;", "1000\n1930\nnil")]
    public void TestStatementsAndStateVariablesInitializeVariables(string input, string expect)
    {
        var result = TestInputStatement(input);
        Assert.AreEqual(expect, result);
    }

    [TestMethod]
    [DataRow("// This program declares a variable and prints its value\n// It then redeclares the variable and prints its value again\nvar baz = \"before\";\nprint baz;\nvar baz = \"after\";\nprint baz;\n", "before\nafter")]
    //[DataRow("", "")]
    //[DataRow("", "")]
    //[DataRow("", "")]
    public void TestStatementsAndStateVariablesRedeclareVariables(string input, string expect)
    {
        var result = TestInputStatement(input);
        Assert.AreEqual(expect, result);
    }




    //[TestMethod]
    //[DataRow("", "")]
    //[DataRow("", "")]
    //[DataRow("", "")]
    //[DataRow("", "")]
    //public void TestStatementsAndState(string input, string expect)
    //{
    //    var result = TestInputStatement(input);
    //    Assert.AreEqual(expect, result);
    //}

    //[TestMethod]
    //[DataRow("", "")]
    //[DataRow("", "")]
    //[DataRow("", "")]
    //[DataRow("", "")]
    //public void TestStatementsAndState(string input, string expect)
    //{
    //    var result = TestInputStatement(input);
    //    Assert.AreEqual(expect, result);
    //}

    //[TestMethod]
    //[DataRow("", "")]
    //[DataRow("", "")]
    //[DataRow("", "")]
    //[DataRow("", "")]
    //public void TestStatementsAndState(string input, string expect)
    //{
    //    var result = TestInputStatement(input);
    //    Assert.AreEqual(expect, result);
    //}
}
