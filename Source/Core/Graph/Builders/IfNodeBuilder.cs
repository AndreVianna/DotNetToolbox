﻿namespace DotNetToolbox.Graph.Builders;

public class IfNodeBuilder(IServiceProvider services, IIfNode parent, string nodeSequenceKey, Dictionary<string, INode> tagMap)
    : IIfNodeBuilder,
      IElseNodeBuilder {
    private INode? _trueNode;
    private INode? _falseNode;
    private readonly IIfNode _parent = IsNotNull(parent);

    public IElseNodeBuilder IsTrue(Action<IWorkflowBuilder> setPath) {
        var trueBuilder = new WorkflowBuilder(services, nodeSequenceKey, tagMap);
        setPath(trueBuilder);
        _trueNode = trueBuilder.BuildBranch();
        return this;
    }

    public INodeBuilder<IIfNode> IsFalse(Action<IWorkflowBuilder> setPath) {
        var falseBuilder = new WorkflowBuilder(services, nodeSequenceKey, tagMap);
        setPath(falseBuilder);
        _falseNode = falseBuilder.BuildBranch();
        return this;
    }

    public IIfNode Build() {
        if (_trueNode == null)
            throw new InvalidOperationException("Missing true condition branch.");
        _parent.IsTrue = _trueNode;
        _parent.IsFalse = _falseNode;
        return _parent;
    }
}
