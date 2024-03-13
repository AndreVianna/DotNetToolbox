﻿namespace DotNetToolbox.AI.OpenAI.Models;

internal class OpenAIModelsHandler(IHttpClientProvider httpClientProvider, ILogger<OpenAIModelsHandler> logger)
        : IModelsHandler {
    private readonly HttpClient _httpClient = httpClientProvider.GetHttpClient();

    public async Task<string[]> GetIds(string? type = null) {
        try {
            logger.LogDebug("Getting list of models...");
            var models = await GetModelsAsync().ConfigureAwait(false);
            var result = models
                        .Where(m => GetModelType(m.Id) == (type ?? "chat"))
                        .Select(m => m.Id)
                        .ToArray();
            logger.LogDebug("A list of {numberOfModels} models was found.", result.Length);
            return result;
        }
        catch (Exception ex) {
            logger.LogError(ex, "Failed to get list of models.");
            throw;
        }
    }

    private async Task<OpenAIModel[]> GetModelsAsync() {
        var response = await _httpClient.GetAsync("models").ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<OpenAIModelsResponse>().ConfigureAwait(false);
        return result!.Data;
    }

    public static string GetModelType(string id) {
        var name = id.StartsWith("ft:") ? id[3..] : id;
        return name switch {
            _ when name.StartsWith("dall-e") => "image",
            _ when name.StartsWith("whisper") => "stt",
            _ when name.StartsWith("tts") => "tts",
            _ when name.StartsWith("text-embedding") => "embedding",
            _ when name.StartsWith("text-moderation") => "moderation",
            _ => "chat",
        };
    }
}