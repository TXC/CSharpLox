using CodeCrafters.Interpreter;

namespace Interpreter.Tests
{
    [TestClass]
    public sealed class ScanningTest
    {
        [TestMethod]
        [DataRow("identifiers.lox")]
        [DataRow("keywords.lox")]
        [DataRow("numbers.lox")]
        [DataRow("punctuators.lox")]
        [DataRow("strings.lox")]
        [DataRow("whitespace.lox")]
        public void TestLoxDataFiles(string filename)
        {
            var filePath = Path.Combine("LoxFiles", "Scanning", filename);
            var content = File.ReadAllText(filePath).TrimEnd();
            var matches = new LoxFileParser(content).Parse();
            var ss = new Scanner(content);

            var tokens = ss.ScanTokens();
            for (var i = 0; i < tokens.Count; i++)
            {
                Assert.AreEqual(matches[i], tokens[i].ToString());
            }
        }

        private static void TestInput(string input, object[] expected)
        {
            var scanner = new Scanner(input);
            var tokens = scanner.ScanTokens();
            for (var i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i].ToString(), tokens[i].ToString());
            }
        }

        [TestMethod]
        [DataRow("\"bar")]
        [DataRow("\"bar\" \"unterminated")]
        public void TestInvalidString(string input)
        {
            var scanner = new Scanner(input);
            var tokens = scanner.ScanTokens();

            //var strings = tokens.Select(t => t.ToString()).ToArray();

            Assert.IsTrue(tokens.Any(x => x.Type is null));
            Assert.IsTrue(tokens.Count != 0);
        }

        [TestMethod]
        [DataRow("14", "NUMBER 14 14.0")]
        [DataRow("123", "NUMBER 123 123.0")]
        public void TestNumber(string input, string expected)
        {
            var scanner = new Scanner(input);
            var tokens = scanner.ScanTokens();

            var value = tokens.First(x => x.Type == TokenType.NUMBER).ToString();

            Assert.AreEqual(expected, value);
        }

        [TestMethod]
        [DataRow("foo bar _hello", new object[] { "IDENTIFIER foo null", "IDENTIFIER bar null", "IDENTIFIER _hello null", "EOF  null" })]
        public void TestIdentifiers(string input, object[] expected)
        {
            var scanner = new Scanner(input);
            var tokens = scanner.ScanTokens();

            for (var i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i].ToString(), tokens[i].ToString());
            }
        }

        [TestMethod]
        [DataRow("and", "AND and null")]
        [DataRow("class", "CLASS class null")]
        [DataRow("else", "ELSE else null")]
        [DataRow("false", "FALSE false null")]
        [DataRow("for", "FOR for null")]
        [DataRow("fun", "FUN fun null")]
        [DataRow("if", "IF if null")]
        [DataRow("nil", "NIL nil null")]
        [DataRow("or", "OR or null")]
        [DataRow("print", "PRINT print null")]
        [DataRow("return", "RETURN return null")]
        [DataRow("super", "SUPER super null")]
        [DataRow("this", "THIS this null")]
        [DataRow("true", "TRUE true null")]
        [DataRow("var", "VAR var null")]
        [DataRow("while", "WHILE while null")]
        public void TestKeywords(string input, string expected)
        {
            var scanner = new Scanner(input);
            var tokens = scanner.ScanTokens();

            Assert.AreEqual(expected, tokens[0].ToString());
        }

        [TestMethod]
        [DataRow("", new object[] { "EOF  null" })]
        public void TestScanningEmptyFile(string input, object[] expected) => TestInput(input, expected);

        [TestMethod]
        [DataRow("(", new object[] { "LEFT_PAREN ( null", "EOF  null" })]
        [DataRow("))", new object[] { "RIGHT_PAREN ) null", "RIGHT_PAREN ) null", "EOF  null" })]
        [DataRow(")))((", new object[] { "RIGHT_PAREN ) null", "RIGHT_PAREN ) null", "RIGHT_PAREN ) null", "LEFT_PAREN ( null", "LEFT_PAREN ( null", "EOF  null" })]
        [DataRow(")))((()", new object[] { "RIGHT_PAREN ) null", "RIGHT_PAREN ) null", "RIGHT_PAREN ) null", "LEFT_PAREN ( null", "LEFT_PAREN ( null", "LEFT_PAREN ( null", "RIGHT_PAREN ) null", "EOF  null" })]
        public void TestScanningParentheses(string input, object[] expected) => TestInput(input, expected);

        [TestMethod]
        [DataRow("}", new object[] { "RIGHT_BRACE } null", "EOF  null" })]
        [DataRow("{{}}", new object[] { "LEFT_BRACE { null", "LEFT_BRACE { null", "RIGHT_BRACE } null", "RIGHT_BRACE } null", "EOF  null" })]
        [DataRow("}}{{}", new object[] { "RIGHT_BRACE } null", "RIGHT_BRACE } null", "LEFT_BRACE { null", "LEFT_BRACE { null", "RIGHT_BRACE } null", "EOF  null" })]
        [DataRow("({)(})}", new object[] { "LEFT_PAREN ( null", "LEFT_BRACE { null", "RIGHT_PAREN ) null", "LEFT_PAREN ( null", "RIGHT_BRACE } null", "RIGHT_PAREN ) null", "RIGHT_BRACE } null", "EOF  null" })]
        public void TestScanningBraces(string input, object[] expected) => TestInput(input, expected);

        [TestMethod]
        [DataRow("+-", new object[] { "PLUS + null", "MINUS - null", "EOF  null" })]
        [DataRow("++--**..,,;;", new object[] { "PLUS + null", "PLUS + null", "MINUS - null", "MINUS - null", "STAR * null", "STAR * null", "DOT . null", "DOT . null", "COMMA , null", "COMMA , null", "SEMICOLON ; null", "SEMICOLON ; null", "EOF  null" })]
        [DataRow(";-*+.;,", new object[] { "SEMICOLON ; null", "MINUS - null", "STAR * null", "PLUS + null", "DOT . null", "SEMICOLON ; null", "COMMA , null", "EOF  null" })]
        [DataRow("({*;.,-})", new object[] { "LEFT_PAREN ( null", "LEFT_BRACE { null", "STAR * null", "SEMICOLON ; null", "DOT . null", "COMMA , null", "MINUS - null", "RIGHT_BRACE } null", "RIGHT_PAREN ) null", "EOF  null" })]
        public void TestScanningOtherSingleCharacterTokens(string input, object[] expected) => TestInput(input, expected);

        [TestMethod]
        [DataRow("@", new object[] { "", "EOF  null" })]
        [DataRow(",.$(#", new object[] { "COMMA , null", "DOT . null", "", "LEFT_PAREN ( null", "", "EOF  null" })]
        [DataRow("@%#@$", new object[] { "", "", "", "", "", "EOF  null" })]
        [DataRow("{(%+-*.#;)}", new object[] { "LEFT_BRACE { null", "LEFT_PAREN ( null", "", "PLUS + null", "MINUS - null", "STAR * null", "DOT . null", "", "SEMICOLON ; null", "RIGHT_PAREN ) null", "RIGHT_BRACE } null", "EOF  null" })]
        public void TestScanningLexicalErrors(string input, object[] expected) => TestInput(input, expected);

        [TestMethod]
        [DataRow("=", new object[] { "EQUAL = null", "EOF  null" })]
        [DataRow("==", new object[] { "EQUAL_EQUAL == null", "EOF  null" })]
        [DataRow("({=}){==}", new object[] { "LEFT_PAREN ( null", "LEFT_BRACE { null", "EQUAL = null", "RIGHT_BRACE } null", "RIGHT_PAREN ) null", "LEFT_BRACE { null", "EQUAL_EQUAL == null", "RIGHT_BRACE } null", "EOF  null" })]
        [DataRow("(($=#==%))", new object[] { "LEFT_PAREN ( null", "LEFT_PAREN ( null", "", "EQUAL = null", "", "EQUAL_EQUAL == null", "", "RIGHT_PAREN ) null", "RIGHT_PAREN ) null", "EOF  null" })]
        public void TestScanningAssignmentAndEqualityOperators(string input, object[] expected) => TestInput(input, expected);

        [TestMethod]
        [DataRow("!=", new object[] { "BANG_EQUAL != null", "EOF  null" })]
        [DataRow("!!===", new object[] { "BANG ! null", "BANG_EQUAL != null", "EQUAL_EQUAL == null", "EOF  null" })]
        [DataRow("!{!}(!===)=", new object[] { "BANG ! null", "LEFT_BRACE { null", "BANG ! null", "RIGHT_BRACE } null", "LEFT_PAREN ( null", "BANG_EQUAL != null", "EQUAL_EQUAL == null", "RIGHT_PAREN ) null", "EQUAL = null", "EOF  null" })]
        [DataRow("{(!@=#$)}", new object[] { "LEFT_BRACE { null", "LEFT_PAREN ( null", "BANG ! null", "", "EQUAL = null", "", "", "RIGHT_PAREN ) null", "RIGHT_BRACE } null", "EOF  null" })]
        public void TestScanningNegationAndInequalityOperators(string input, object[] expected) => TestInput(input, expected);

        [TestMethod]
        [DataRow(">=", new object[] { "GREATER_EQUAL >= null", "EOF  null" })]
        [DataRow("<<<=>>>=", new object[] { "LESS < null", "LESS < null", "LESS_EQUAL <= null", "GREATER > null", "GREATER > null", "GREATER_EQUAL >= null", "EOF  null" })]
        [DataRow(">=><=<>=", new object[] { "GREATER_EQUAL >= null", "GREATER > null", "LESS_EQUAL <= null", "LESS < null", "GREATER_EQUAL >= null", "EOF  null" })]
        [DataRow("(){!=<===}", new object[] { "LEFT_PAREN ( null", "RIGHT_PAREN ) null", "LEFT_BRACE { null", "BANG_EQUAL != null", "LESS_EQUAL <= null", "EQUAL_EQUAL == null", "RIGHT_BRACE } null", "EOF  null" })]
        public void TestScanningRelationalOperators(string input, object[] expected) => TestInput(input, expected);

        [TestMethod]
        [DataRow("//Comment", new object[] { "EOF  null" })]
        [DataRow("(///Unicode:\u00a3\u00a7᯽\u263a\u2663)", new object[] { "LEFT_PAREN ( null", "EOF  null" })]
        [DataRow("/", new object[] { "SLASH / null", "EOF  null" })]
        [DataRow("({(<=!,)})//Comment", new object[] { "LEFT_PAREN ( null", "LEFT_BRACE { null", "LEFT_PAREN ( null", "LESS_EQUAL <= null", "BANG ! null", "COMMA , null", "RIGHT_PAREN ) null", "RIGHT_BRACE } null", "RIGHT_PAREN ) null", "EOF  null" })]
        public void TestScanningDivisionOperatorAndComments(string input, object[] expected) => TestInput(input, expected);

        [TestMethod]
        [DataRow(" ", new object[] { "EOF  null" })]
        [DataRow("\t\n ", new object[] { "EOF  null" })]
        [DataRow("{ \t}\n((-*+, ))", new object[] { "LEFT_BRACE { null", "RIGHT_BRACE } null", "LEFT_PAREN ( null", "LEFT_PAREN ( null", "MINUS - null", "STAR * null", "PLUS + null", "COMMA , null", "RIGHT_PAREN ) null", "RIGHT_PAREN ) null", "EOF  null" })]
        [DataRow("{\t\t\r\n  }\r\n((;\t <=))", new object[] { "LEFT_BRACE { null", "RIGHT_BRACE } null", "LEFT_PAREN ( null", "LEFT_PAREN ( null", "SEMICOLON ; null", "LESS_EQUAL <= null", "RIGHT_PAREN ) null", "RIGHT_PAREN ) null", "EOF  null" })]
        public void TestScanningWhitespace(string input, object[] expected) => TestInput(input, expected);

        [TestMethod]
        [DataRow("() \t@", new object[] { "LEFT_PAREN ( null", "RIGHT_PAREN ) null", "", "EOF  null" })]
        [DataRow("@# ", new object[] { "", "", "EOF  null" })]
        [DataRow("()  #\t{}\r\n@\r\n$\r\n+++\r\n// Let's Go!\r\n+++\r\n#\r\n", new object[] { "LEFT_PAREN ( null", "RIGHT_PAREN ) null", "", "LEFT_BRACE { null", "RIGHT_BRACE } null", "", "", "PLUS + null", "PLUS + null", "PLUS + null", "PLUS + null", "PLUS + null", "PLUS + null", "", "EOF  null" })]
        [DataRow("({,\r\n$})\r\n", new object[] { "LEFT_PAREN ( null", "LEFT_BRACE { null", "COMMA , null", "", "RIGHT_BRACE } null", "RIGHT_PAREN ) null", "EOF  null" })]
        public void TestScanningMultiLineErrors(string input, object[] expected) => TestInput(input, expected);

        [TestMethod]
        [DataRow("\"hello\"", new object[] { "STRING \"hello\" hello", "EOF  null" })]
        [DataRow("\"hello\" \"unterminated", new object[] { "STRING \"hello\" hello", "", "EOF  null" })]
        [DataRow("\"foo \tbar 123 // hello world!\"", new object[] { "STRING \"foo \tbar 123 // hello world!\" foo \tbar 123 // hello world!", "EOF  null" })]
        [DataRow("(\"foo\"+\"baz\") != \"other_string\"", new object[] { "LEFT_PAREN ( null", "STRING \"foo\" foo", "PLUS + null", "STRING \"baz\" baz", "RIGHT_PAREN ) null", "BANG_EQUAL != null", "STRING \"other_string\" other_string", "EOF  null" })]
        public void TestScanningStringLiterals(string input, object[] expected) => TestInput(input, expected);

        [TestMethod]
        [DataRow("86", new object[] { "NUMBER 86 86.0", "EOF  null" })]
        [DataRow("84.35", new object[] { "NUMBER 84.35 84.35", "EOF  null" })]
        [DataRow("57.0000", new object[] { "NUMBER 57.0000 57.0", "EOF  null" })]
        [DataRow("(79+91) > 20 != (\"Success\" != \"Failure\") != (76 >= 24)", new object[] { "LEFT_PAREN ( null", "NUMBER 79 79.0", "PLUS + null", "NUMBER 91 91.0", "RIGHT_PAREN ) null", "GREATER > null", "NUMBER 20 20.0", "BANG_EQUAL != null", "LEFT_PAREN ( null", "STRING \"Success\" Success", "BANG_EQUAL != null", "STRING \"Failure\" Failure", "RIGHT_PAREN ) null", "BANG_EQUAL != null", "LEFT_PAREN ( null", "NUMBER 76 76.0", "GREATER_EQUAL >= null", "NUMBER 24 24.0", "RIGHT_PAREN ) null", "EOF  null" })]
        public void TestScanningNumberLiterals(string input, object[] expected) => TestInput(input, expected);

        [TestMethod]
        [DataRow("bar baz", new object[] { "IDENTIFIER bar null", "IDENTIFIER baz null", "EOF  null" })]
        [DataRow("_1236ar 6az _hello foo bar", new object[] { "IDENTIFIER _1236ar null", "NUMBER 6 6.0", "IDENTIFIER az null", "IDENTIFIER _hello null", "IDENTIFIER foo null", "IDENTIFIER bar null", "EOF  null" })]
        [DataRow("message = \"Hello, World!\"\nnumber = 123", new object[] { "IDENTIFIER message null", "EQUAL = null", "STRING \"Hello, World!\" Hello, World!", "IDENTIFIER number null", "EQUAL = null", "NUMBER 123 123.0", "EOF  null" })]
        [DataRow("{\r\n// This is a complex test case\r\nstr1 = \"Test\"\r\nstr2 = \"Case\"\r\nnum1 = 100\r\nnum2 = 200.00\r\nresult = (str1 == str2) != ((num1 + num2) >= 300)\r\n}\r\n", new object[] { "LEFT_BRACE { null", "IDENTIFIER str1 null", "EQUAL = null", "STRING \"Test\" Test", "IDENTIFIER str2 null", "EQUAL = null", "STRING \"Case\" Case", "IDENTIFIER num1 null", "EQUAL = null", "NUMBER 100 100.0", "IDENTIFIER num2 null", "EQUAL = null", "NUMBER 200.00 200.0", "IDENTIFIER result null", "EQUAL = null", "LEFT_PAREN ( null", "IDENTIFIER str1 null", "EQUAL_EQUAL == null", "IDENTIFIER str2 null", "RIGHT_PAREN ) null", "BANG_EQUAL != null", "LEFT_PAREN ( null", "LEFT_PAREN ( null", "IDENTIFIER num1 null", "PLUS + null", "IDENTIFIER num2 null", "RIGHT_PAREN ) null", "GREATER_EQUAL >= null", "NUMBER 300 300.0", "RIGHT_PAREN ) null", "RIGHT_BRACE } null", "EOF  null" })]
        public void TestScanningIdentifiers(string input, object[] expected) => TestInput(input, expected);

        [TestMethod]
        [DataRow("else", new object[] { "ELSE else null", "EOF  null" })]
        [DataRow("class or FALSE super return " +
                 "VAR else false for PRINT " +
                 "TRUE RETURN WHILE fun " +
                 "IF var SUPER FOR and " +
                 "NIL while ELSE FUN " +
                 "nil AND print if OR " +
                 "this CLASS true THIS",
            new object[]
            {
                "CLASS class null", "OR or null", "IDENTIFIER FALSE null", "SUPER super null", "RETURN return null",
                "IDENTIFIER VAR null", "ELSE else null", "FALSE false null", "FOR for null", "IDENTIFIER PRINT null",
                "IDENTIFIER TRUE null", "IDENTIFIER RETURN null", "IDENTIFIER WHILE null", "FUN fun null",
                "IDENTIFIER IF null", "VAR var null", "IDENTIFIER SUPER null", "IDENTIFIER FOR null", "AND and null",
                "IDENTIFIER NIL null", "WHILE while null", "IDENTIFIER ELSE null", "IDENTIFIER FUN null",
                "NIL nil null", "IDENTIFIER AND null", "PRINT print null", "IF if null", "IDENTIFIER OR null",
                "THIS this null", "IDENTIFIER CLASS null", "TRUE true null", "IDENTIFIER THIS null", "EOF  null"
            })]
        [DataRow("var greeting = \"Hello\"\nif (greeting == \"Hello\") {\n    return true\n} else {\n    return false\n}\n", new object[]
        {
            "VAR var null",
            "IDENTIFIER greeting null",
            "EQUAL = null",
            "STRING \"Hello\" Hello",
            "IF if null",
            "LEFT_PAREN ( null",
            "IDENTIFIER greeting null",
            "EQUAL_EQUAL == null",
            "STRING \"Hello\" Hello",
            "RIGHT_PAREN ) null",
            "LEFT_BRACE { null",
            "RETURN return null",
            "TRUE true null",
            "RIGHT_BRACE } null",
            "ELSE else null",
            "LEFT_BRACE { null",
            "RETURN return null",
            "FALSE false null",
            "RIGHT_BRACE } null",
            "EOF  null",
        })]
        [DataRow("var result = (a + b) > 7 or \"Success\" != \"Failure\" or x >= 5\r\n" +
                 "while (result) {\r\n" +
                 "    var counter = 0\r\n" +
                 "    counter = counter + 1\r\n" +
                 "    if (counter == 10) {\r\n" +
                 "        return nil\r\n" +
                 "    }\r\n" +
                 "}\r\n",
            new object[]
        {
            "VAR var null", "IDENTIFIER result null", "EQUAL = null", "LEFT_PAREN ( null",
            "IDENTIFIER a null", "PLUS + null", "IDENTIFIER b null", "RIGHT_PAREN ) null",
            "GREATER > null", "NUMBER 7 7.0", "OR or null", "STRING \"Success\" Success",
            "BANG_EQUAL != null", "STRING \"Failure\" Failure", "OR or null", "IDENTIFIER x null",
            "GREATER_EQUAL >= null", "NUMBER 5 5.0",

            "WHILE while null", "LEFT_PAREN ( null",
            "IDENTIFIER result null", "RIGHT_PAREN ) null", "LEFT_BRACE { null", "VAR var null",
            "IDENTIFIER counter null", "EQUAL = null", "NUMBER 0 0.0", "IDENTIFIER counter null",
            "EQUAL = null", "IDENTIFIER counter null", "PLUS + null", "NUMBER 1 1.0",
            "IF if null", "LEFT_PAREN ( null", "IDENTIFIER counter null", "EQUAL_EQUAL == null",
            "NUMBER 10 10.0", "RIGHT_PAREN ) null", "LEFT_BRACE { null", "RETURN return null",
            "NIL nil null", "RIGHT_BRACE } null", "RIGHT_BRACE } null", "EOF  null"
        })]
        public void TestScanningReservedWords(string input, object[] expected) => TestInput(input, expected);
    }
}
