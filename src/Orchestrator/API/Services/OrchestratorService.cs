// ============================================================
// Module 1: Orchestrator Service — pipeline coordinator
// Owner: M. Khizar Akram (Team Lead)
// ============================================================
using System.Net.Http.Json;

public class OrchestratorService
{
    private readonly IHttpClientFactory _httpFactory;

    public OrchestratorService(IHttpClientFactory httpFactory)
    {
        _httpFactory = httpFactory;
    }

    /// <summary>
    /// Runs the full pipeline in sequence:
    ///   1. Research + Summarize  (Module 2 — Hamza)
    ///   2. Report + Store        (Module 3 — Hassan)
    ///   3. Notify + Blockchain   (Module 4 — Shamraiz)
    /// </summary>
    public async Task<TaskResult> RunPipelineAsync(TaskRequest request)
    {
        var taskId = Guid.NewGuid().ToString();

        // ── Step 1: Research & Summarize ───────────────────────────────
        var researchClient = _httpFactory.CreateClient("Research");
        var summaryResponse = await researchClient.PostAsJsonAsync("/api/research", new
        {
            TaskId = taskId,
            Topic  = request.Topic
        });
        summaryResponse.EnsureSuccessStatusCode();
        var summaryResult = await summaryResponse.Content.ReadFromJsonAsync<SummaryResult>()
            ?? throw new Exception("Research agent returned null");

        // ── Step 2: Generate Report & Store in Blob ─────────────────────
        var reporterClient = _httpFactory.CreateClient("Reporter");
        var reportResponse = await reporterClient.PostAsJsonAsync("/api/report", new
        {
            TaskId  = taskId,
            Topic   = request.Topic,
            Summary = summaryResult.Summary
        });
        reportResponse.EnsureSuccessStatusCode();
        var reportResult = await reportResponse.Content.ReadFromJsonAsync<ReportResult>()
            ?? throw new Exception("Reporter agent returned null");

        // ── Step 3: Notify User & Log to Blockchain ─────────────────────
        var notifierClient = _httpFactory.CreateClient("Notifier");
        var notifyResponse = await notifierClient.PostAsJsonAsync("/api/notify", new
        {
            TaskId      = taskId,
            Email       = request.NotifyEmail,
            ReportUrl   = reportResult.BlobUrl,
            OutputHash  = reportResult.OutputHash
        });
        notifyResponse.EnsureSuccessStatusCode();
        var notifyResult = await notifyResponse.Content.ReadFromJsonAsync<NotifyResult>()
            ?? throw new Exception("Notifier agent returned null");

        return new TaskResult
        {
            TaskId    = taskId,
            Status    = "Completed",
            ReportUrl = reportResult.BlobUrl,
            TxHash    = notifyResult.TxHash,
            Message   = "Pipeline completed successfully."
        };
    }
}

// ── Shared inter-service response models ────────────────────
public record SummaryResult(string Summary);
public record ReportResult(string BlobUrl, string OutputHash);
public record NotifyResult(string TxHash);
