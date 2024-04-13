namespace DotNetToolbox.Data.Repositories;

public class InMemoryRepositoryStrategy<TItem>(IItemSet<TItem> repository)
    : IRepositoryStrategy<InMemoryRepositoryStrategy<TItem>> {
    protected ItemSet<TItem> Repository { get; } = (ItemSet<TItem>)repository;

    public TResult ExecuteQuery<TResult>(LambdaExpression expression)
        => throw new NotImplementedException();

    public IItemSet Create(LambdaExpression expression)
        => ItemSet.Create(expression.Type, expression, this);
    // ReSharper disable once SuspiciousTypeConversion.Global
    public IItemSet<TResult> Create<TResult>(LambdaExpression expression)
        => (IItemSet<TResult>)ItemSet.Create(typeof(TResult), expression);

    public TResult ExecuteFunction<TResult>(string command, Expression? expression = null, object? input = null) {
        object? result = command switch {
            "Any" when expression is not null => Repository.Data.Any(ToDelegate<Func<TItem, bool>>(expression)),
            "Any" => Repository.Data.Any(),
            "Count" when expression is not null => Repository.Data.Count(ToDelegate<Func<TItem, bool>>(expression)),
            "Count" => Repository.Data.Count(),
            "FindFirst" when expression is not null => Repository.Data.FirstOrDefault(ToDelegate<Func<TItem, bool>>(expression)),
            "FindFirst" => Repository.Data.FirstOrDefault(),
            "GetList" => Repository.Data.ToArray(),
            "Create" when typeof(TItem).IsClass && input is Action<TItem> set => CreateSetAndAddItem(set),
            _ when expression is not null => Repository.Query.Provider.Execute<TItem>(expression),
            _ => throw new NotImplementedException($"Command '{command}' is not implemented for '{Repository.GetType().Name}<{typeof(TItem).Name}>'."),
        };
        return IsOfTypeOrDefault<TResult>(result)!;
    }

    private TResult ToDelegate<TResult>(Expression expression)
        where TResult : Delegate {
        if (expression is LambdaExpression lambda)
            return (TResult)lambda.Compile(true);
        throw new NotImplementedException($"Command expression '{expression}' is not implemented for '{Repository.GetType().Name}<{typeof(TItem).Name}>'.");
    }

    public void ExecuteAction(string command, Expression? expression = null, object? input = null) {
        var source = expression is LambdaExpression lambda ? Repository.Create(lambda) : Repository.Query;
        switch (command) {
            case "Add" when input is TItem i:
                DataAsCollection.Add(i);
                return;
        }
        throw new NotSupportedException($"Command '{command}' is not supported.");
    }

    private ICollection<TItem> DataAsCollection
        // ReSharper disable once SuspiciousTypeConversion.Global
        => Repository.Data as ICollection<TItem>
        ?? throw new NotSupportedException($"Repository is not a collection. Found '{Repository.Data.GetType().Name}'.");

    private TItem CreateSetAndAddItem(Action<TItem> set) {
        var item = Activator.CreateInstance<TItem>();
        set(item);
        DataAsCollection.Add(item);
        return item;
    }
}
