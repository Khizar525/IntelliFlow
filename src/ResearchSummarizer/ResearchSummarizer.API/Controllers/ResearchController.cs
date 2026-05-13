using Microsoft.AspNetCore.Mvc;
using ResearchSummarizer.API.Models;
using ResearchSummarizer.API.Services;

namespace ResearchSummarizer.API.Controllers;

[ApiController]
[Route("api/research")]
public class ResearchController : ControllerBase
{
    private readonly ResearchService _researchService;
    private readonly SummarizerService _summarizerService;
    private readonly ILogger<ResearchController> _logger;

    public ResearchController(ResearchService researchService, SummarizerService summarizerService, ILogger<ResearchController> logger)
    {
        _researchService = researchService;
        _summarizerService = summarizerService;
        _logger = logger;
    }

    // POST /api/research
    [HttpPost]
    public async Task<IActionResult> Research([FromBody] ResearchRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Topic))
            return BadRequest(new { error = "Topic is required" });

        _logger.LogInformation("Research request received. TaskId: {TaskId}, Topic: {Topic}",
            request.TaskId, request.Topic);

        // Step 1: Fetch raw content from DuckDuckGo
        var rawContent = await _researchService.FetchRawContentAsync(request.Topic);

        // Step 2: Summarize using OpenRouter LLM
        var summary = await _summarizerService.SummarizeAsync(rawContent, request.Topic);

        // Must return exactly this shape (Orchestrator reads .summary)
        return Ok(new ResearchResponse { Summary = summary });
    }

    // GET /api/research/health
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { status = "healthy", service = "ResearchSummarizer", timestamp = DateTime.UtcNow });
    }
}