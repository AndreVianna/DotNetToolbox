namespace DotNetToolbox.Data.Repositories;

public interface IAsyncRepositoryStrategy<TItem>
    : IRepositoryStrategy
    where TItem : class {
    Task<bool> HaveAny(CancellationToken ct = default);
    Task<int> Count(CancellationToken ct = default);
    Task<TItem[]> ToArray(CancellationToken ct = default);
    Task<TItem?> GetFirst(CancellationToken ct = default);
    Task Add(TItem newItem, CancellationToken ct = default);
    Task Update(Expression<Func<TItem, bool>> predicate, TItem updatedItem, CancellationToken ct = default);
    Task Remove(Expression<Func<TItem, bool>> predicate, CancellationToken ct = default);
}