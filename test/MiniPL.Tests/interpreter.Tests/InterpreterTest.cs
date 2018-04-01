using System;
using MiniPL.interpreter;
using MiniPL.logger;
using MiniPL.parser;
using MiniPL.parser.AST;
using MiniPL.semantics;
using Xunit;

namespace MiniPL.Tests.semantics.Tests {

  public class InterpreterTest {

    private IInterpreter interpreter;

    private ISymbolTable symbolTable;

    public InterpreterTest() {
      this.symbolTable = new SymbolTable();
      this.interpreter = getInterpreter(TestHelpers.sampleProgram);
    }

    private IInterpreter getInterpreter(string source) {
      return new MiniPLInterpreter(source, this.symbolTable, new ConsoleLogger());
    }

    [Fact]
    public void interpretSimpleProgram() {
      //this.interpreter.interpret();
    }

    [Fact]
    public void integerShouldHaveValue10() {
      this.interpreter = getInterpreter("var x : int := 10;");
      this.interpreter.interpret();
      Assert.Equal(10, this.symbolTable.getInt("x"));
    }

    [Fact]
    public void integerShouldHaveValue3() {
      this.interpreter = getInterpreter("var x : int := 1 + 2;");
      this.interpreter.interpret();
      Assert.Equal(3, this.symbolTable.getInt("x"));
    }

    [Fact]
    public void stringShouldHaveValueHelloWorld() {
      this.interpreter = getInterpreter("var x : string := \"Hello World\";");
      this.interpreter.interpret();
      Assert.Equal("Hello World", this.symbolTable.getString("x"));
    }

    [Theory]
    [InlineData("var x : int := 10;", 10)]
    [InlineData("var x : int := 1 + 3;", 4)]
    [InlineData("var x : int := 4 + 3;", 7)]
    [InlineData("var x : int := (4 + 3) + (10);", 17)]
    [InlineData("var x : int := (4 + 3) + (10 + 4);", 21)]
    [InlineData("var x : int := ((4 + 3) + 12) + (10 + 4);", 33)]
    [InlineData("var x : int := 1 - 3;", -2)]
    [InlineData("var x : int := (4 + 3) - (10);", -3)]
    [InlineData("var x : int := (4 + 3) + (10 - 4);", 13)]
    public void integerShouldHaveValueThatIsCalculatedFromExpression(string source, int value) {
      this.interpreter = getInterpreter(source);
      this.interpreter.interpret();
      Assert.Equal(value, this.symbolTable.getInt("x"));
    }

    [Theory]
    [InlineData("var x : int := 10; x := x * x;", 100)]
    [InlineData("var x : int := 10; var z : int; z := x + 2; x := (z + x) - 10;", 12)]
    public void varAssignmentShouldUpdateTheIntegerValue(string source, int value) {
      this.interpreter = getInterpreter(source);
      this.interpreter.interpret();
      Assert.Equal(value, this.symbolTable.getInt("x"));
    }

    [Theory]
    [InlineData("var x : string := \"Hello\"; x := x + \" World!\";", "Hello World!")]
    public void varAssignmentShouldUpdateTheStringValue(string source, string value) {
      this.interpreter = getInterpreter(source);
      this.interpreter.interpret();
      Assert.Equal(value, this.symbolTable.getString("x"));
    }

    [Theory]
    [InlineData("var x : bool; x := (10 < 100);", true)]
    [InlineData("var x : bool; x := (100 < 100);", false)]
    public void varAssignmentShouldUpdateTheBoolValue(string source, bool value) {
      this.interpreter = getInterpreter(source);
      this.interpreter.interpret();
      Assert.Equal(value, this.symbolTable.getBool("x"));
      Assert.True(this.symbolTable.hasBool("x"));
    }

    [Theory]
    [InlineData("var x : string := \"Hello \" + \"World\";", "Hello World")]
    [InlineData("var x : string := (\"Hello \") + \"World\";", "Hello World")]
    [InlineData("var x : string := (\"Hello \") + (\"World\" + \"!\");", "Hello World!")]
    public void stringShouldHaveValueThatIsCalculatedFromExpression(string source, string value) {
      this.interpreter = getInterpreter(source);
      this.interpreter.interpret();
      Assert.Equal(value, this.symbolTable.getString("x"));
    }

    [Theory]
    [InlineData("var x : int := 10;", 10)]
    [InlineData("var x : int := 1 + 3;", 4)]
    [InlineData("var x : int := 2 * 3;", 6)]
    [InlineData("var x : int := 3 / 2;", 1)]
    [InlineData("var x : int := ((2 + 1) * ((4 / 2) + 1)) - 1;", 8)]
    [InlineData("var x : int := ((2 * 3) + (16 / 4));", 10)]
    public void integerShouldHaveValueThatIsCalculatedFromDifferentExpressions(string source, int value) {
      this.interpreter = getInterpreter(source);
      this.interpreter.interpret();
      Assert.Equal(value, this.symbolTable.getInt("x"));
    }

    [Theory]
    [InlineData("var x : bool := 1 = 2;", false)]
    [InlineData("var x : bool := 2 = 2;", true)]
    [InlineData("var x : bool := (1 + 2) = (4 - 1);", true)]
    [InlineData("var x : bool := (1 + 2) = (4 * 1);", false)]
    [InlineData("var x : bool := 1 < 2;", true)]
    [InlineData("var x : bool := 1 < 0;", false)]
    [InlineData("var x : bool := 1 < 1;", false)]
    [InlineData("var x : bool := \"abba\" = \"dagga\";", false)]
    [InlineData("var x : bool := \"abba\" = \"abba\";", true)]
    [InlineData("var x : bool := (\"abba\" = \"abba\") = (2 < 100);", true)]
    [InlineData("var x : bool := \"abba\" < \"dagga\";", true)]
    [InlineData("var x : bool := \"gagga\" < \"dagga\";", false)]
    [InlineData("var x : bool := \"abba\" < \"abba\";", false)]
    [InlineData("var x : bool := (2 < 1) < (1 < 2);", true)]
    [InlineData("var x : bool := (1 < 2) < (1 < 2);", false)]
    [InlineData("var x : bool := (1 < 2) < (2 < 1);", false)]
    [InlineData("var x : bool := (2 < 1) < (2 < 1);", false)]
    [InlineData("var x : bool := (\"abba\" < \"abba\") < (1 < 2);", true)]
    [InlineData("var x : bool := (2 < 1) < (\"abba\" < \"abba\");", false)]
    public void checkBoolExpressions(string source, bool value) {
      this.interpreter = getInterpreter(source);
      this.interpreter.interpret();
      Assert.Equal(value, this.symbolTable.getBool("x"));
      Assert.True(this.symbolTable.hasBool("x"));
    }

    [Fact]
    public void shouldHaveIntValue100WhenReadFromVariable() {
      this.interpreter = getInterpreter("var x : int := 30;\nvar z : int := x + 70;");
      this.interpreter.interpret();
      Assert.Equal(100, this.symbolTable.getInt("z"));
    }

    [Theory]
    [InlineData("var x : string := \"Hello\"; var y : string := \"World\"; var z : string := (x + \" \") + y;", "Hello World")]
    [InlineData("var x : string := \"Hello\"; var y : string := \"World\"; var z : string := (x + \" \") + (y + \"!\");", "Hello World!")]
    public void shouldHaveCorrectValueFromStringVariable(string source, string value) {
      this.interpreter = getInterpreter(source);
      this.interpreter.interpret();
      Assert.Equal(value, this.symbolTable.getString("z"));
    }

    [Theory]
    [InlineData("var x : bool := (2 < 1) < (2 < 1); var y : string := \"Hello\" + \" World\"; var z : int := (3 * 3) + 1;", 10, "Hello World", false)]
    public void checkMultipleVarDeclarations(string source, int intValue, string strValue, bool boolValue) {
      this.interpreter = getInterpreter(source);
      this.interpreter.interpret();
      Assert.Equal(intValue, this.symbolTable.getInt("z"));
      Assert.Equal(strValue, this.symbolTable.getString("y"));
      Assert.Equal(boolValue, this.symbolTable.getBool("x"));
    }

    [Theory]
    [InlineData("var x : int := 3 + 4; var y : int := 3 + (x - 2); var z : int := x * y;", 7, 8, 56)]
    public void checkMultipleIntegerDeclarations(string source, int int1, int int2, int int3) {
      this.interpreter = getInterpreter(source);
      this.interpreter.interpret();
      Assert.Equal(int1, this.symbolTable.getInt("x"));
      Assert.Equal(int2, this.symbolTable.getInt("y"));
      Assert.Equal(int3, this.symbolTable.getInt("z"));
    }
  }
}