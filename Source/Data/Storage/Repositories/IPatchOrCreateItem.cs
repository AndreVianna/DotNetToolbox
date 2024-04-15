﻿namespace DotNetToolbox.Data.Repositories;

public interface IPatchOrCreateItem<TItem>
    : ICanPatchItem<TItem>,
      ICanCreateItem<TItem>
    where TItem : class {
    Task<TItem> PatchOrCreate(Expression<Func<TItem, bool>> predicate, Action<TItem> setModel, CancellationToken ct = default);
}