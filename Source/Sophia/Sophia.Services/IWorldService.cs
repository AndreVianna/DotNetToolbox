﻿namespace Sophia.Services;

public interface IWorldService {
    Task<WorldData> GetWorld();
    Task UpdateWorld(WorldData world);
}