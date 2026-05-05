// ============================================================
// Module 1: Task Controller — entry point for all task requests
// Owner: M. Khizar Akram (Team Lead)
// ============================================================
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly OrchestratorService _orchestrator;

    public TasksController(OrchestratorService orchestrator)
    {
        _orchestrator = orchestrator;
    }

    /// <summary>
    /// Submit a new task. Triggers the full agent pipeline.
    /// POST /api/tasks
    /// Body: { "topic": "...", "notifyEmail": "..." }
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> SubmitTask([FromBody] TaskRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Topic))
            return BadRequest(new { error = "Topic is required." });

        var result = await _orchestrator.RunPipelineAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// Health check — no auth required
    /// GET /api/tasks/health
    /// </summary>
    [HttpGet("health")]
    [AllowAnonymous]
    public IActionResult Health() => Ok(new { status = "Orchestrator OK" });
}

// ── Request / Response Models ────────────────────────────────
public class TaskRequest
{
    public string Topic       { get; set; } = string.Empty;
    public string NotifyEmail { get; set; } = string.Empty;
}

public class TaskResult
{
    public string TaskId       { get; set; } = string.Empty;
    public string Status       { get; set; } = string.Empty;
    public string ReportUrl    { get; set; } = string.Empty;
    public string TxHash       { get; set; } = string.Empty;  // Blockchain tx hash
    public string Message      { get; set; } = string.Empty;
}
