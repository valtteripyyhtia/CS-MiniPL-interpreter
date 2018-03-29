using System;
using MiniPL.exceptions;
using MiniPL.logger;
using MiniPL.parser;
using MiniPL.parser.AST;
using MiniPL.scanner;
using MiniPL.semantics;
using MiniPL.syntax;
using MiniPL.tokens;
using Xunit;

namespace MiniPL.Tests.semantics.Tests {

  public class SemanticAnalysisTest {

    private IParser parser;

    private ISemanticAnalyzer analyzer;

    public SemanticAnalysisTest() {
      this.parser = TestHelpers.getParser(TestHelpers.sampleProgram);
      this.analyzer = new MiniPLSemanticAnalyzer();
    }

    [Fact]
    public void checkIntegerIsDeclaredWithDefaultValue() {
      this.parser = TestHelpers.getParser("var x : int;");
      Assert.True(this.parser.processAndBuildAST());
      IAST ast = this.parser.getAST();
      this.analyzer.analyze(ast);
      Assert.Equal(0, this.analyzer.getInt("x"));
    }

    [Fact]
    public void checkStringIsDeclaredWithDefaultValue() {
      this.parser = TestHelpers.getParser("var x : string;");
      Assert.True(this.parser.processAndBuildAST());
      IAST ast = this.parser.getAST();
      this.analyzer.analyze(ast);
      Assert.Equal("", this.analyzer.getString("x"));
    }

    [Fact]
    public void checkBoolIsDeclaredWithDefaultValue() {
      this.parser = TestHelpers.getParser("var trueValue : bool;");
      Assert.True(this.parser.processAndBuildAST());
      IAST ast = this.parser.getAST();
      this.analyzer.analyze(ast);
      Assert.True(this.analyzer.variableExists("trueValue"));
      Assert.False(this.analyzer.getBool("trueValue"));
    }

    [Fact]
    public void checkVariableIsDeclared() {
      this.parser = TestHelpers.getParser("var x : int;");
      Assert.True(this.parser.processAndBuildAST());
      IAST ast = this.parser.getAST();
      this.analyzer.analyze(ast);
      Assert.True(this.analyzer.variableExists("x"));
    }

    [Fact]
    public void checkVariableIsNotDeclared() {
      this.parser = TestHelpers.getParser("var x : int; var y : int;");
      Assert.True(this.parser.processAndBuildAST());
      IAST ast = this.parser.getAST();
      this.analyzer.analyze(ast);
      Assert.False(this.analyzer.variableExists("z"));
    }

    [Fact]
    public void checkThreeIntegersAreDeclaredWithDefaultValue() {
      this.parser = TestHelpers.getParser("var x : int; var y : int; var z : int;");
      Assert.True(this.parser.processAndBuildAST());
      IAST ast = this.parser.getAST();
      this.analyzer.analyze(ast);
      Assert.Equal(0, this.analyzer.getInt("x"));
      Assert.Equal(0, this.analyzer.getInt("y"));
      Assert.Equal(0, this.analyzer.getInt("z"));
    }

    [Fact]
    public void twoSameVariablesCantBeDeclared() {
      this.parser = TestHelpers.getParser("var x : int; var x : string;");
      Assert.True(this.parser.processAndBuildAST());
      IAST ast = this.parser.getAST();
      Assert.Throws<SemanticException>(() => this.analyzer.analyze(ast));
    }

    [Fact]
    public void shouldHaveThreeDifferentTypesDeclared() {
      this.parser = TestHelpers.getParser("var x : bool; var y : int; var z : string;");
      Assert.True(this.parser.processAndBuildAST());
      IAST ast = this.parser.getAST();
      this.analyzer.analyze(ast);
      Assert.Equal(false, this.analyzer.getBool("x"));
      Assert.Equal(0, this.analyzer.getInt("y"));
      Assert.Equal("", this.analyzer.getString("z"));
    }

    [Fact]
    public void integerShouldHaveValue10() {
      this.parser = TestHelpers.getParser("var x : int := 10;");
      Assert.True(this.parser.processAndBuildAST());
      IAST ast = this.parser.getAST();
      this.analyzer.analyze(ast);
      Assert.Equal(10, this.analyzer.getInt("x"));
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
      this.parser = TestHelpers.getParser(source);
      Assert.True(this.parser.processAndBuildAST());
      IAST ast = this.parser.getAST();
      this.analyzer.analyze(ast);
      Assert.Equal(value, this.analyzer.getInt("x"));
    }
        
  }
}