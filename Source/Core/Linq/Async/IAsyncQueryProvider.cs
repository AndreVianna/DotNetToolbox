namespace System.Linq.Async;

public interface IAsyncQueryProvider {
    IAsyncQueryable<TElement> CreateAsyncQuery<TElement>(Expression expression);
    Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken ct = default);
}
