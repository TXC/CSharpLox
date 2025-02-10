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

    #region Evaluating Expressions
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
    #endregion Evaluating Expressions

    #region Statements & State
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
    [DataRow("// This program declares a variable and prints its value\n// It also uses a previously declared variable's value to initialize a new variable\nvar world = \"after\";\nvar world = \"before\";\nvar world = world;\nprint world;\n", "before")]
    [DataRow("// This program declares and initializes multiple variables and prints their values\nvar hello = 2;\nprint hello;\nvar hello = 3;\nprint hello;\nvar world = 5;\nprint world;\nvar hello = world;\nprint hello;", "2\n3\n5\n5")]
    [DataRow("// This program declares a variable and initializes it with the value of another variable\n// But as the variable is not declared before it is used it leads to a runtime error\nvar bar = baz;", "")]
    public void TestStatementsAndStateVariablesRedeclareVariables(string input, string expect)
    {
        var result = TestInputStatement(input);
        //Assert.IsTrue(Lox.HadRuntimeError);
        //Assert.IsFalse(Lox.HadError);
        Assert.AreEqual(expect, result);
    }

    [TestMethod]
    [DataRow("// This program tests that the assignment operator returns the value that was assigned\n// And prints the value of the variable after each assignment\nvar world;\nworld = 1;\nprint world;\nprint world = 2;\nprint world;", "1\n2\n2")]
    [DataRow("// This program tests that the assignment operator works on any declared variable\nvar baz = 32;\nvar bar = 32;\nbar = baz;\nbaz = bar;\nprint baz + bar;", "64")]
    [DataRow("// This program tests that the assignment operator returns the value that was assigned\n// So, it can be used multiple times in a single statement\nvar bar;\nvar baz;\n    \nbar = baz = 47 + 28 * 17;\nprint bar;\nprint baz;", "523\n523")]
    [DataRow("// This program tests that the assignment operator returns the value that was assigned\n// So, it can be used multiple times in a single statement\nvar bar = 47;\nvar quz;\nvar hello;\n    \nbar = quz = hello = bar * 2;\nprint bar;\nprint quz;\nprint quz;\n", "94\n94\n94")]
    public void TestStatementsAndStateAssignmentOperation(string input, string expect)
    {
        var result = TestInputStatement(input);
        Assert.AreEqual(expect, result);
    }

    [TestMethod]
    [DataRow("// This program tests that curly braces can be used to group multiple statements into blocks\n{\n    var foo = \"quz\";\n    print foo;\n}", "quz")]
    [DataRow("// This program tests that blocks can be used to group statements and variables\n// creating local scopes\n{\n    var bar = \"before\";\n    print bar;\n}\n{\n    var bar = \"after\";\n    print bar;\n}", "before\nafter")]
    [DataRow("// This program tests that scopes can be nested\n{\n    var hello = 14;\n    {\n        var bar = 14;\n        print bar;\n    }\n    print hello;\n}", "14\n14")]
    [DataRow("// This program tests that scopes are properly closed\n// Here the inner scope is missing a closing curly brace\n// Which leads to a compilation error\n{\n    var hello = 17;\n    var foo = 17;\n    {\n        print hello + foo;\n}", "")]
    public void TestStatementsAndStateBlockSyntax(string input, string expect)
    {
        var result = TestInputStatement(input);
        Assert.AreEqual(expect, result);
    }

    [TestMethod]
    [DataRow("// This program tests that nested scopes can be created\n// and that local variables can be accessed\nvar hello = (61 * 25) - 26;\n{\n    var quz = \"world\" + \"83\";\n    print quz;\n}\nprint hello;", "world83\n1499")]
    [DataRow("// This program tests variable shadowing across nested scopes\n{\n    var baz = \"before\";\n    {\n        var baz = \"after\";\n        print baz;\n    }\n    print baz;\n}", "after\nbefore")]
    [DataRow("// This program creates nested scopes and tests local scopes and variable shadowing\nvar baz = \"global baz\";\nvar world = \"global world\";\nvar hello = \"global hello\";\n{\n  var baz = \"outer baz\";\n  var world = \"outer world\";\n  {\n    var baz = \"inner baz\";\n    print baz;\n    print world;\n    print hello;\n  }\n  print baz;\n  print world;\n  print hello;\n}\nprint baz;\nprint world;\nprint hello;\n\n", "inner baz\nouter world\nglobal hello\nouter baz\nouter world\nglobal hello\nglobal baz\nglobal world\nglobal hello")]
    [DataRow("// This program tests that when a variable is declared in an outer scope,\n// it can be used in an inner scope\n// But when a variable is declared in an inner scope,\n// it can't be used in an outer scope\n{\n  var foo = \"outer foo\";\n  var hello = \"outer hello\";\n  {\n    foo = \"modified foo\";\n    var hello = \"inner hello\";\n    print foo;\n    print hello;\n  }\n  print foo;\n  print hello;\n}\nprint hello;", "modified foo\ninner hello\nmodified foo\nouter hello")]
    public void TestStatementsAndStateScopes(string input, string expect)
    {
        var result = TestInputStatement(input);
        Assert.AreEqual(expect, result);
    }
    #endregion Statements & State

    #region Control Flow
    #endregion Control Flow

    #region Functions
    #endregion Functions
}
