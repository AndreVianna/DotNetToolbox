﻿using Microsoft.Extensions.DependencyInjection;

namespace DotNetToolbox.Graph.Nodes;

public sealed class ActionNode
    : ActionNode<ActionNode> {
    private readonly Action<Context> _execute;
    private const string _defaultLabel = "action";

    public ActionNode(uint id, string label, Action<Context> execute, IServiceProvider services)
        : base(id, label, services) {
        _execute = execute;
    }

    public ActionNode(uint id, Action<Context> execute, IServiceProvider services)
        : base(id, _defaultLabel, services) {
        _execute = execute;
    }

    protected override void Execute(Context context)
        => IsNotNull(_execute)(context);

    internal static ActionNode Create(uint id,
                                      string? label,
                                      Action<Context> execute,
                                      IServiceProvider services)
        => new(id, label ?? _defaultLabel, execute, services);

    public static TNode Create<TNode>(uint id,
                                      string label,
                                      IServiceProvider services)
        where TNode : ActionNode<TNode>
        => InstanceFactory.Create<TNode>(id, label, services);

    public static TNode Create<TNode>(uint id,
                                      IServiceProvider services)
        where TNode : ActionNode<TNode>
        => InstanceFactory.Create<TNode>(id, services);
}

public abstract class ActionNode<TAction>
    : Node<TAction>,
      IActionNode
    where TAction : ActionNode<TAction> {
    private readonly IPolicy _policy;

    protected ActionNode(uint id, string label, IServiceProvider services)
        : base(id, label, services) {
        _policy = Services.GetService<IPolicy>() ?? Policy.Default;
    }

    protected ActionNode(uint id, IServiceProvider services)
        : base(id, services) {
        _policy = Services.GetService<IPolicy>() ?? Policy.Default;
    }

    protected abstract void Execute(Context context);

    protected sealed override INode? GetNext(Context context)
        => Next;

    protected sealed override void UpdateState(Context context)
        => _policy.Execute(() => Execute(context));
}
