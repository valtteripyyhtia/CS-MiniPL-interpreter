using System;
using System.Collections.Generic;
using MiniPL.semantics.visitor;
using MiniPL.tokens;

namespace MiniPL.parser.AST {

  public class LogicalNotOperationNode : Node<MiniPLTokenType> {

    public LogicalNotOperationNode() : base(MiniPLTokenType.LOGICAL_NOT) {}

    public override void accept(INodeVisitor visitor) {
      visitor.visitLogicalNotOperator(this);
    }
  }

}
