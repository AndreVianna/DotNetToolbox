﻿namespace DotNetToolbox.AI.Models;

public class Model
    : IModel {
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required uint MaximumContextSize { get; init; }
    public required uint MaximumOutputTokens { get; init; }
    public DateOnly TrainingDataCutOff { get; init; }
    public string ProviderId { get; init; }
}
