// ============================================================
// Module 2: Summarizer Service — OpenRouter API + Llama 3
// Owner: Hamza Khaliq (02-131232-059)
//
// OpenRouter docs: https://openrouter.ai/docs
// Free tier available via community models
// ============================================================
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

public class SummarizerService
{
    private readonly HttpClient _http;
    private readonly string _model;

    public SummarizerService(HttpClient http)
    {
        _http  = http;
        _model = Environment.GetEnvironmentVariable("OPENROUTER_MODEL") ?? "meta-llama/llama-3.1-70b-instruct:free";

        var apiKey = Environment.GetEnvironmentVariable("OPENROUTER_API_KEY")
            ?? throw new Exception("OPENROUTER_API_KEY env var not set");

        _http.BaseAddress = new Uri("https://openrouter.ai");
        _http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", apiKey);
    }

    /// <summary>
    /// Sends rawContent to OpenRouter Llama3 and returns a structured summary.
    /// </summary>
    public async Task<string> SummarizeAsync(string topic, string rawContent)
    {
        var systemPrompt =
            "You are a professional research analyst. Given raw research content, " +
            "produce a clear, structured summary with: an executive overview, " +
            "3-5 key findings, and a brief conclusion. Use plain English.";

        var userPrompt =
            $"Topic: {topic}\n\n" +
            $"Raw content:\n{rawContent}\n\n" +
            "Produce the structured summary now.";

        var requestBody = new
        {
            model    = _model,
            messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user",   content = userPrompt   }
            },
            max_tokens  = 1024,
            temperature = 0.3
        };

        var response = await _http.PostAsJsonAsync("/openai/v1/chat/completions", requestBody);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        return doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString() ?? "Summary unavailable.";
    }
}
