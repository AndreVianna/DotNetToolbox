﻿namespace DotNetToolbox.AI.Anthropic.Chats;

public class AnthropicImageData {
    [JsonPropertyName("type")]
    public required string Type { get; init; }

    [JsonPropertyName("media_type")]
    public required string MediaType { get; init; }

    [JsonPropertyName("data")]
    public required string Data { get; init; }
}