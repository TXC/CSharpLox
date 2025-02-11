using System.Globalization;
using CodeCrafters.Interpreter;

namespace Interpreter.Tests;

[TestClass]
public sealed class InterpreterTest
{
    [TestInitialize]
    public void TestInitialize()
    {
        Lox.HadError = false;
        Lox.HadRuntimeError = false;
    }

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
    private static void TestControlFlow(string input, string expect)
    {
        var result = TestInputStatement(input);
        Assert.AreEqual(expect.Trim(), result);
    }

    [TestMethod]
    // This should print the string if the condition evaluates to True
    [DataRow("if (true) print \"hello\";", "hello")]
    // This should print "block body" if the condition evaluates to True
    [DataRow("if (true) {\nprint \"block body\";\n}", "block body")]
    // This program tests whether the assignment operation returns the value assigned.
    // The if condition should evaluate to true and the inner boolean expression must be printed.
    // So, in this case the if condition evaluates to true and prints the inner boolean expression
    [DataRow("var a = false;\nif (a = true) {\n  print (a == false);\n}", "false")]
    // // This program should print a different string based on the value of age
    [DataRow("var stage = \"unknown\";\nvar age = 57;\nif (age < 18) { stage = \"child\"; }\nif (age >= 18) { stage = \"adult\"; }\nprint stage;\n\nvar isAdult = age >= 18;\nif (isAdult) { print \"eligible for voting: true\"; }\nif (!isAdult) { print \"eligible for voting: false\"; }\n", "adult\neligible for voting: true")]
    public void TestControlFlowIfStatements(string input, string expect) => TestControlFlow(input, expect);

    [TestMethod]
    // This program uses a random boolean to decide which branch to execute,
    // and then prints the appropriate string
    [DataRow("if (false) print \"if branch\"; else print \"else branch\";", "else branch")]
    // This program initializes age with a random integer and then prints "adult"
    // if the age is greater than 18, otherwise it prints "child"
    [DataRow("var age = 63;\nif (age > 18) print \"adult\"; else print \"child\";", "adult")]
    // This program uses a random boolean to decide which branch to execute,
    // and then prints the appropriate string
    [DataRow("if (false) {\n  print \"if block\";\n} else print \"else statement\";\n\nif (false) print \"if statement\"; else {\n  print \"else block\";\n}", "else statement\nelse block")]
    // This program converts a random integer from Celsius to Fahrenheit
    // and prints the result. It also prints a message based on the temperature.
    [DataRow("var celsius = 13;\nvar fahrenheit = 0;\nvar isHot = false;\n\n{\n  fahrenheit = celsius * 9 / 5 + 32;\n  print celsius; print fahrenheit;\n\n  if (celsius > 30) {\n    isHot = true;\n    print \"It's a hot day. Stay hydrated!\";\n  } else {\n    print \"It's cold today. Wear a jacket!\";\n  }\n\n  if (isHot) { print \"Remember to use sunscreen!\"; }\n}", "13\n55.4\nIt's cold today. Wear a jacket!")]
    public void TestControlFlowElseStatements(string input, string expect) => TestControlFlow(input, expect);

    [TestMethod]
    // This program uses a random boolean to decide which branch to execute,
    // and then prints the appropriate string
    [DataRow("if (false) print \"if branch\"; else if (false) print \"else-if branch\";", "")]
    // This program uses a random boolean to decide which branch to execute,
    // and then prints the appropriate string
    [DataRow("if (false) {\n  print \"quz\";\n} else if (false) print \"quz\";\n\nif (false) print \"quz\"; else if (false) {\n  print \"quz\";\n}", "")]
    // This program uses multiple if statements to categorize a person
    // into different life stages based on their age
    [DataRow("var age = 13;\nvar stage = \"unknown\";\nif (age < 18) { stage = \"child\"; }\nelse if (age >= 18) { stage = \"adult\"; }\nelse if (age >= 65) { stage = \"senior\"; }\nelse if (age >= 100) { stage = \"centenarian\"; }\nprint stage;", "child")]
    // This program uses multiple if statements to determine eligibility for
    // voting, driving, and drinking based on a random integer age
    [DataRow("var age = 67;\nvar isAdult = age >= 18;\nif (isAdult) { print \"eligible for voting: true\"; }\nelse { print \"eligible for voting: false\"; }\n\nif (age < 16) { print \"eligible for driving: false\"; }\nelse if (age < 18) { print \"eligible for driving: learner's permit\"; }\nelse { print \"eligible for driving: full license\"; }\n\nif (age < 21) { print \"eligible for drinking (US): false\"; }\nelse { print \"eligible for drinking (US): true\"; }", "eligible for voting: true\neligible for driving: full license\neligible for drinking (US): true")]
    public void TestControlFlowElseIfStatements(string input, string expect) => TestControlFlow(input, expect);

    [TestMethod]
    [DataRow("if (true) if (true) print \"nested true\";", "nested true")]
    [DataRow("if (true) {\n  if (false) print \"hello\"; else print \"hello\";}", "hello")]
    //[DataRow("", "")]
    [DataRow("if (true) if (false) print \"hello\"; else print \"world\";", "world")]
    public void TestControlFlowNestedIfStatements(string input, string expect) => TestControlFlow(input, expect);

    [TestMethod]
    [DataRow("if (false or \"ok\") print \"world\";\nif (nil or \"ok\") print \"world\";\n\nif (false or false) print \"quz\";\nif (true or \"quz\") print \"quz\";\n\nif (15 or \"baz\") print \"baz\";\nif (\"baz\" or \"baz\") print \"baz\";", "world\nworld\nquz\nbaz\nbaz")]
    [DataRow("print 87 or true;\nprint false or 87;\nprint false or false or true;\n\nprint false or false;\nprint false or false or false;\nprint true or true or true or true;", "87\n87\ntrue\nfalse\nfalse\ntrue")]
    [DataRow("var a = \"world\";\nvar b = \"world\";\n(a = false) or (b = true) or (a = \"world\");\nprint a;\nprint b;", "false\ntrue")]
    [DataRow("var stage = \"unknown\";\nvar age = 74;\nif (age < 18) { stage = \"child\"; }\nif (age >= 18) { stage = \"adult\"; }\nprint stage;\n\nvar isAdult = age >= 18;\nif (isAdult) { print \"eligible for voting: true\"; }\nif (!isAdult) { print \"eligible for voting: false\"; }", "adult\neligible for voting: true")]
    public void TestControlFlowLogicalOROperator(string input, string expect) => TestControlFlow(input, expect);

    [TestMethod]
    [DataRow("if (false and \"bad\") print \"bar\";\nif (nil and \"bad\") print \"bar\";\n\nif (true and \"foo\") print \"foo\";\nif (97 and \"quz\") print \"quz\";\nif (\"quz\" and \"quz\") print \"quz\";\nif (\"\" and \"hello\") print \"hello\";", "foo\nquz\nquz\nhello")]
    [DataRow("print false and 1;\nprint true and 1;\nprint 13 and \"bar\" and false;\n\nprint 13 and true;\nprint 13 and \"bar\" and 13;", "false\n1\nfalse\ntrue\n13")]
    [DataRow("var a = \"quz\";\nvar b = \"quz\";\n(a = true) and (b = false) and (a = \"bad\");\nprint a;\nprint b;", "true\nfalse")]
    [DataRow("var stage = \"unknown\";\nvar age = 51;\nif (age < 18) { stage = \"child\"; }\nif (age >= 18) { stage = \"adult\"; }\nprint stage;\n\nvar isAdult = age >= 18;\nif (isAdult) { print \"eligible for voting: true\"; }\nif (!isAdult) { print \"eligible for voting: false\"; }", "adult\neligible for voting: true")]
    public void TestControlFlowLogicalANDOperator(string input, string expect) => TestControlFlow(input, expect);

    [TestMethod]
    [DataRow("var baz = 0;\nwhile (baz < 3) print baz = baz + 1;", "1\n2\n3")]
    [DataRow("var foo = 0;\nwhile (foo < 3) {\n print foo;\n foo = foo + 1;\n}", "0\n1\n2")]
    [DataRow("while (false) { print \"should not print \"; }\n\nvar product = 1;\nvar i = 1;\n\nwhile (i <= 5) {\n  product = product * i;\n  i = i + 1;\n}\n\nprint \"Factorial of 5: \"; print product;\n", "Factorial of 5: \n120")]
    [DataRow("var n = 10;\nvar fm = 0;\nvar fn = 1;\nvar index = 0;\n\nwhile (index < n) {\n   print fm;\n   var temp = fm;\n   fm = fn;\n   fn = temp + fn;\n   index = index + 1;\n}", "0\n1\n1\n2\n3\n5\n8\n13\n21\n34")]
    public void TestControlFlowWhileStatements(string input, string expect) => TestControlFlow(input, expect);


    [TestMethod]
    [DataRow("for (var quz = 0; quz < 3;) print quz = quz + 1;", "1\n2\n3")]
    [DataRow("for (var bar = 0; bar < 3; bar = bar + 1) {\n  print bar;\n}", "0\n1\n2")]
    [DataRow("var quz = 0;\nfor (; quz < 2; quz = quz + 1) print quz;\n\nfor (var foo = 0; foo < 2;) {\n print foo;\n foo = foo + 1;\n}", "0\n1\n0\n1")]
    //[DataRow("", "")]
    public void TestControlFlowForStatements(string input, string expect) => TestControlFlow(input, expect);

    [TestMethod]
    [DataRow("for (;;) var foo;")]
    [DataRow("for (var a = 1; {}; a = a + 1) {}")]
    [DataRow("for (var a = 1; a < 2; {}) {}")]
    [DataRow("for ({}; a < 2; a = a + 1) {}")]
    public void TestControlFlowSyntacticErrors(string input)
    {
        var result = TestInputEvaluation(input);
        //Assert.IsFalse(Lox.HadRuntimeError);
        Assert.IsTrue(Lox.HadError);
        Assert.AreEqual("", result);
    }
    #endregion Control Flow

    #region Functions

    private static void TestFunctions(string input, string expect)
    {
        var result = TestInputStatement(input);
        Assert.AreEqual(expect.Trim(), result);
    }

    [TestMethod]
    [DataRow("print clock() + 69;")]
    //[DataRow("", "")]
    //[DataRow("", "")]
    //[DataRow("", "")]
    public void TestFunctionsNativeFunctionsPart1(string input)
    {
        var time = (double)((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();
        var expect = (time + 69).ToString(CultureInfo.InvariantCulture);
        var result = TestInputStatement(input);
        Assert.AreEqual(expect.Trim(), result);
    }

    [TestMethod]
    //[DataRow("", "")]
    [DataRow("var startTime = clock();\nvar lastCheck = startTime;\nvar running = true;\nprint \"Starting timer for 0.2 seconds\";\nvar startTime = clock();\nwhile (running) {\n  if (clock() > startTime + 0.2) {\n    print \"Timer ended\";\n    running = false;\n  }\n}", "Starting timer for 0.2 seconds\nTimer ended")]
    //[DataRow("", "")]
    //[DataRow("", "")]
    public void TestFunctionsNativeFunctionsPart2(string input, string expect) => TestFunctions(input, expect);

    [TestMethod]
    [DataRow("fun foo() {}\nprint foo;", "<fn foo>")]
    [DataRow("fun f() {}\nf();", "")]
    [DataRow("fun baz() { print 34; }\nbaz();", "34")]
    //[DataRow("", "")]
    public void TestFunctionsFunctionsWithoutArguments(string input, string expect) => TestFunctions(input, expect);

    [TestMethod]
    [DataRow(
        "fun f8(a, b, c, d, e, f, g, h) { print a - b + c * d + e - f + g - h; }\nf8(59, 59, 59, 59, 59, 59, 59, 59);",
        "3481")]
    [DataRow("fun f3(a, b, c) { print a + b + c; }\nf3(27, 27, 27);", "81")]
    [DataRow("fun f1(a) { print a; }\nf1(19);", "19")]
    //[DataRow("", "")]
    public void TestFunctionsFunctionsWithArguments(string input, string expect) => TestFunctions(input, expect);


    [TestMethod]
    [DataRow("fun foo(a, b c, d, e, f) {}\nfoo();", "")]
    [DataRow("fun f() 55;\nprint f();", "")]
    [DataRow("print clock)));", "")]
    [DataRow("print clock(;", "")]
    public void TestFunctionsSyntaxErrors(string input, string expect) => TestFunctions(input, expect);

    [TestMethod]
    [DataRow("fun f() {\nreturn;\nprint \"bad\";\n}\nprint f();", "nil")]
    [DataRow("fun f() {\n  while (!true) return \"ok\";\n}\n\nprint f();", "nil")]
    [DataRow("fun f() {\n  if (true) return \"no\"; else return \"ok\";\n}\n\nprint f();", "no")]
    [DataRow("fun fib(n) {\n  if (n < 2) return n;\n  return fib(n - 2) + fib(n - 1);\n}\n\nvar start = clock();\nprint fib(10) == 55;\nprint (clock() - start) < 5; // 5 seconds", "true\ntrue")]
    public void TestFunctionsReturnStatements(string input, string expect) => TestFunctions(input, expect);

    [TestMethod]
    [DataRow("var globalGreeting = \"Hello\";\n\nfun makeGreeter() {\n  fun greet(name) {\n    print globalGreeting + \" \" + name;\n  }\n  return greet;\n}\n\nvar sayHello = makeGreeter();\n\nsayHello(\"Bob\");\nsayHello(\"Alice\");\nsayHello(\"Eve\");", "Hello Bob\nHello Alice\nHello Eve")]
    [DataRow("fun returnArg(arg) {\n  return arg;\n}\n\nfun returnFunCallWithArg(func, arg) {\n  return returnArg(func)(arg);\n}\n\nfun printArg(arg) {\n  print arg;\n}\n\nreturnFunCallWithArg(printArg, \"world\");", "world")]
    //[DataRow("", "")]
    //[DataRow("", "")]
    public void TestFunctionsHigherOrderFunctions(string input, string expect) => TestFunctions(input, expect);

    [TestMethod]
    [DataRow("(true == true)();", "")]
    [DataRow("fun f(a, b) {}\nf(1);", "")]
    [DataRow("fun f(a, b) {\n  print a;\n  print b;\n}\n\nf(1, 2, 3, 4);", "")]
    [DataRow("51();", "")]
    public void TestFunctionsRuntimeErrors(string input, string expect)
    {
        TestFunctions(input, expect);
        Assert.IsTrue(Lox.HadRuntimeError);
        Assert.IsFalse(Lox.HadError);
    }

    [TestMethod]
    [DataRow("var counter = 68;\n\nfun incrementCounter(amount) {\n  counter = counter + amount;\n  print counter;\n}\n\n{\n  counter = 14;\n  incrementCounter(4);\n  print counter;\n}\nprint counter;\n", "18\n18\n18")]
    [DataRow("var a = 91;\n\nfun printAndModify() {\n  print a;\n  var a = 69;\n  print a;\n}\n\nprint a;\nvar a = 93;\nprintAndModify();", "91\n93\n69")]
    //[DataRow("", "")]
    //[DataRow("", "")]
    public void TestFunctionsFunctionScope(string input, string expect) => TestFunctions(input, expect);

    [TestMethod]
    [DataRow("{\n  var threshold = 50;\n\n  fun isEven(n) {\n    if (n == 0) return true;\n    if (n > threshold) return false;\n    return isOdd(n - 1);\n  }\n\n  fun isOdd(n) {\n    if (n == 0) return false;\n    if (n > threshold) return false;\n    return isEven(n - 1);\n  }\n\n  print isEven(94);\n}", "false")]
    [DataRow("fun makeCounter() {\n  var i = 0;\n  fun count() {\n    i = i + 6;\n    print i;\n  }\n\n  return count;\n}\n\nvar counter = makeCounter();\ncounter();\ncounter();\n", "6\n12")]
    //[DataRow("", "")]
    //[DataRow("", "")]
    public void TestFunctionsClosures(string input, string expect) => TestFunctions(input, expect);
    #endregion Functions
}
