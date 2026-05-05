// ============================================================
// Module 2: Research Service — web content fetcher
// Owner: Hamza Khaliq (02-131232-059)
// 
// TODO (Hamza): Replace the stub below with actual HTTP search calls.
// Suggested approach:
//   - Use DuckDuckGo Instant Answer API (free, no key needed)
//     GET https://api.duckduckgo.com/?q={topic}&format=json
//   - Or use SerpAPI free tier / ScraperAPI for richer results
//   - Strip HTML tags, keep first ~3000 characters of useful text
// ============================================================

public class ResearchService
{
    private readonly HttpClient _http;

    public ResearchService(HttpClient http)
    {
        _http = http;
    }

    /// <summary>
    /// Fetches raw text content relevant to the topic.
    /// Returns a cleaned string that will be passed to the Summarizer.
    /// </summary>
    public async Task<string> FetchContentAsync(string topic)
    {
        // ── STUB — replace with real search logic ──────────────────────
        // Example using DuckDuckGo:
        //
        // var url = $"https://api.duckduckgo.com/?q={Uri.EscapeDataString(topic)}&format=json&no_html=1";
        // var json = await _http.GetStringAsync(url);
        // var doc  = System.Text.Json.JsonDocument.Parse(json);
        // var abstract_ = doc.RootElement.GetProperty("Abstract").GetString() ?? "";
        // var related   = doc.RootElement.GetProperty("RelatedTopics")...
        // return abstract_ + "\n" + related;

        await Task.Delay(100); // simulate async
        return $"[STUB] Raw research content about: {topic}. " +
               "Replace this with actual web content fetched from a search API.";
    }
}
