﻿namespace AI.Sample.Models.Handlers;

public interface IModelHandler {
    ModelEntity[] List();
    ModelEntity? GetByKey(string key);
    ModelEntity Create(Action<ModelEntity> setUp);
    void Register(ModelEntity model);
    void Update(ModelEntity model);
    void Remove(string key);
    ModelEntity[] ListByProvider(string provider);
    void RemoveByProvider(string provider);

    void Select(string key);
    ModelEntity? Internal { get; }
}