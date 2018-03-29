using System;
using System.Collections.Generic;
using MiniPL.semantics.visitor;
using MiniPL.tokens;

namespace MiniPL.parser.AST {

  public class StringLiteralNode : Node<string> {

    public StringLiteralNode(Token<MiniPLTokenType> token) : base(token.getLexeme()) {}

    public override void accept(INodeVisitor visitor) {
      throw new NotImplementedException();
    }
  }

}