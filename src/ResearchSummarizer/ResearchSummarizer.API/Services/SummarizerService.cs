using System.Text;
using System.Text.Json;

namespace ResearchSummarizer.API.Services;

public class SummarizerService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;
    private readonly ILogger<SummarizerService> _logger;

    public SummarizerService(HttpClient httpClient, IConfiguration config, ILogger<SummarizerService> logger)
    {
        _httpClient = httpClient;
        _config = config;
        _logger = logger;
    }

    private static readonly string[] FallbackModels =
    [
        "google/gemma-4-26b-a4b-it:free",
        "meta-llama/llama-3.2-3b-instruct:free",
        "cognitivecomputations/dolphin3.0-r1-mistral-24b:free",
    ];

    public async Task<string> SummarizeAsync(string rawContent, string topic)
    {
        var apiKey = _config["OPENROUTER_API_KEY"];

        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException("OPENROUTER_API_KEY not set");

        var configuredModel = _config["OPENROUTER_MODEL"];
        var modelsToTry = string.IsNullOrWhiteSpace(configuredModel)
            ? FallbackModels
            : [configuredModel, ..FallbackModels.Where(m => m != configuredModel)];

        foreach (var model in modelsToTry)
        {
            var requestBody = new
            {
                model,
                messages = new[]
                {
                    new
                    {
                        role = "system",
                        content = "You are a research summarizer. Write a clear 3-4 sentence summary. Return only the summary."
                    },
                    new
                    {
                        role = "user",
                        content = $"Topic: {topic}\n\nRaw content:\n{rawContent}\n\nSummarize it."
                    }
                }
            };

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                "https://openrouter.ai/api/v1/chat/completions"
            );

            request.Headers.Add("Authorization", $"Bearer {apiKey}");
            request.Headers.Add("HTTP-Referer", "http://localhost");

            request.Content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json"
            );

            _logger.LogInformation("Calling OpenRouter model: {Model}", model);

            var response = await _httpClient.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Model {Model} returned {Status}, trying next fallback", model, (int)response.StatusCode);
                continue;
            }

            using var doc = JsonDocument.Parse(responseBody);

            if (!doc.RootElement.TryGetProperty("choices", out var choices) ||
                choices.GetArrayLength() == 0)
            {
                _logger.LogWarning("No choices in response from model {Model}, trying next fallback", model);
                continue;
            }

            var summary =
                choices[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            if (!string.IsNullOrWhiteSpace(summary))
                return summary.Trim();
        }

        return $"Topic: {topic}. {rawContent}";
    }
}