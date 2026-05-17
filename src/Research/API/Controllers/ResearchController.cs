// ============================================================
// Module 2: Research + LLM Summarizer Agent
// Owner: Hamza Khaliq (02-131232-059)
// ============================================================
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ResearchController : ControllerBase
{
    private readonly ResearchService    _research;
    private readonly SummarizerService  _summarizer;

    public ResearchController(ResearchService research, SummarizerService summarizer)
    {
        _research   = research;
        _summarizer = summarizer;
    }

    /// <summary>
    /// POST /api/research
    /// Receives topic → fetches raw content → summarizes with OpenRouter/Llama3
    /// Returns: { "summary": "..." }
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Research([FromBody] ResearchRequest request)
    {
        // Step 1 — fetch raw content from the web
        var rawContent = await _research.FetchContentAsync(request.Topic);

        // Step 2 — summarize with LLM
        var summary = await _summarizer.SummarizeAsync(request.Topic, rawContent);

        return Ok(new { summary });
    }

    [HttpGet("health")]
    public IActionResult Health() => Ok(new { status = "ResearchSummarizer OK" });
}

public class ResearchRequest
{
    public string TaskId { get; set; } = string.Empty;
    public string Topic  { get; set; } = string.Empty;
}
