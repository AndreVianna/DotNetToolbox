﻿namespace DotNetToolbox.Graph.Nodes;

public class BranchingNodeBuilder(WorkflowBuilder builder, IServiceProvider services)
    : IBranchingNodeBuilder {
    private readonly Dictionary<string, INode?> _choices = [];

    public IBranchingNodeBuilder Is(string key, Action<WorkflowBuilder> setPath) {
        var branchBuilder = new WorkflowBuilder(services, builder.Id, builder.Nodes);
        setPath(branchBuilder);
        _choices[IsNotNullOrWhiteSpace(key)] = branchBuilder.Start;
        return this;
    }

    public void Otherwise(Action<WorkflowBuilder> setPath) {
        var branchBuilder = new WorkflowBuilder(services, builder.Id, builder.Nodes);
        setPath(branchBuilder);
        _choices[string.Empty] = branchBuilder.Start;
    }

    public void Configure(IBranchingNode owner) {
        owner.Choices.Clear();
        foreach (var (key, value) in _choices)
            owner.Choices[key] = value;
    }
}