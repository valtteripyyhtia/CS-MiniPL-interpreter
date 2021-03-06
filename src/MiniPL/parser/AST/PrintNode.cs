using System;
using System.Collections.Generic;
using MiniPL.semantics.visitor;
using MiniPL.syntax;

namespace MiniPL.parser.AST {

  public class PrintNode : Node<MiniPLSymbol> {

    public PrintNode() : base(MiniPLSymbol.PRINT_PROCEDURE) {}

    public override void accept(INodeVisitor visitor) {
      visitor.visitPrint(this);
    }
  }

}