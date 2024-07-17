﻿namespace DotNetToolbox.Graph.Nodes;

public interface INodeFactory {
    IIfNode If(Func<Context, bool> predicate, INode truePath, INode? falsePath = null, IGuidProvider? guid = null);
    IIfNode If(string id, Func<Context, bool> predicate, INode truePath, INode? falsePath = null);

    ISelectNode<TKey> Select<TKey>(Func<Context, TKey> select, IReadOnlyDictionary<TKey, INode?> paths, IGuidProvider? guid = null)
        where TKey : notnull;
    ISelectNode<TKey> Select<TKey>(string id, Func<Context, TKey> select, IReadOnlyDictionary<TKey, INode?> paths)
        where TKey : notnull;

    ISelectNode Select(Func<Context, string> select, IEnumerable<INode?> paths, IGuidProvider? guid = null);
    ISelectNode Select(string id, Func<Context, string> select, IEnumerable<INode?> paths);

    IActionNode Do(Action<Context> action, INode? node, IGuidProvider? guid = null);
    IActionNode Do(string id, Action<Context> action, INode? node);

    INode Void { get; }
}
