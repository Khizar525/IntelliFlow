using System.Text.Json;
using ResearchSummarizer.API.Models;

namespace ResearchSummarizer.API.Services;

public class ResearchService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ResearchService> _logger;

    public ResearchService(HttpClient httpClient, ILogger<ResearchService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<string> FetchRawContentAsync(string topic)
    {
        var url = $"https://api.duckduckgo.com/?q={Uri.EscapeDataString(topic)}&format=json&no_html=1&skip_disambig=1";

        _logger.LogInformation("Fetching DuckDuckGo data for topic: {Topic}", topic);

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        var ddgResponse = JsonSerializer.Deserialize<DuckDuckGoResponse>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        var parts = new List<string>();

        if (!string.IsNullOrWhiteSpace(ddgResponse?.Abstract))
            parts.Add(ddgResponse.Abstract);

        if (!string.IsNullOrWhiteSpace(ddgResponse?.AbstractText) &&
            ddgResponse.AbstractText != ddgResponse.Abstract)
            parts.Add(ddgResponse.AbstractText);

        if (ddgResponse?.RelatedTopics != null)
        {
            foreach (var topic2 in ddgResponse.RelatedTopics.Take(5))
            {
                if (!string.IsNullOrWhiteSpace(topic2.Text))
                    parts.Add(topic2.Text);
            }
        }

        var combined = string.Join(" ", parts);

        // Keep under 3000 chars as per spec
        if (combined.Length > 3000)
            combined = combined[..3000];

        if (string.IsNullOrWhiteSpace(combined))
            combined = $"Topic: {topic}. Please provide a general summary of this subject.";

        _logger.LogInformation("Fetched {Length} characters of content", combined.Length);
        return combined;
    }
}