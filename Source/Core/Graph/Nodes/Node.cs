﻿namespace DotNetToolbox.Graph.Nodes;

public abstract class Node
    : INode {
    protected Node(string? id = null, IGuidProvider? guid = null) {
        guid ??= new GuidProvider();
        Id = id ?? guid.AsSortable.Create().ToString();
    }

    #region Factory

    private static readonly INodeFactory _factory = new NodeFactory();

    public static IIfNode If(Func<Context, bool> predicate, INode trueNode, INode? falseNode = null, IGuidProvider? guid = null)
        => _factory.If(predicate, trueNode, falseNode, guid);

    public static IIfNode If(string id, Func<Context, bool> predicate, INode truePath, INode? falsePath = null)
        => _factory.If(id, predicate, truePath, falsePath);

    public static IMapNode<TKey> Map<TKey>(Func<Context, TKey> select, IReadOnlyDictionary<TKey, INode?> nodeOptions, IGuidProvider? guid = null)
        where TKey : notnull
        => _factory.Select(select, nodeOptions, guid);

    public static IMapNode<TKey> Map<TKey>(string id, Func<Context, TKey> select, IReadOnlyDictionary<TKey, INode?> paths)
        where TKey : notnull
        => _factory.Select(id, select, paths);

    public static IMapNode Map(Func<Context, string> select, IEnumerable<INode?> paths, IGuidProvider? guid = null)
        => _factory.Select(select, paths, guid);

    public static IMapNode Map(string id, Func<Context, string> select, IEnumerable<INode?> paths)
        => _factory.Select(id, select, paths);

    public static IActionNode Do(Action<Context> action, INode? next = null, IPolicy? policy = null, IGuidProvider? guid = null)
        => _factory.Do(action, next, policy, guid);

    public static IActionNode Do(string id, Action<Context> action, INode? next = null, IPolicy? policy = null)
        => _factory.Do(id, action, next, policy);

    public static INode Void
        => _factory.Void;

    #endregion

    public string Id { get; }
    protected List<INode?> Paths { get; init; } = [];

    public virtual Result Validate(ICollection<INode>? validatedNodes = null) {
        validatedNodes ??= new HashSet<INode>();
        if (validatedNodes.Contains(this))
            return Success();

        var result = IsValid();
        if (result.IsInvalid)
            return result;

        validatedNodes.Add(this);
        return Paths.Where(node => node is not null)
                    .Aggregate(result, (current, node) => current + node!.Validate(validatedNodes));
    }

    protected virtual Result IsValid() => Success();

    public virtual INode? Run(Context context) {
        OnEntry(context);
        UpdateState(context);
        var exitPath = GetNext(context);
        OnExit(context);
        return exitPath;
    }

    protected virtual void OnEntry(Context state) { }
    protected abstract void UpdateState(Context context);
    protected abstract INode? GetNext(Context context);
    protected virtual void OnExit(Context state) { }

    public override int GetHashCode() => Id.GetHashCode();
}
