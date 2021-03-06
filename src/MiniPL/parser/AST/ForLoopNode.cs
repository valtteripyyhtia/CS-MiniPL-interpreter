using System;
using System.Collections.Generic;
using MiniPL.semantics.visitor;
using MiniPL.syntax;

namespace MiniPL.parser.AST {

  public class ForLoopNode : Node<MiniPLSymbol> {

    public ForLoopNode() : base(MiniPLSymbol.FOR_LOOP) {}

    public override void accept(INodeVisitor visitor) {
      visitor.visitForLoop(this);
    }
  }

}