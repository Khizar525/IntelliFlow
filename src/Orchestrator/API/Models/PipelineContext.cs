namespace Orchestrator.API.Models;

/// <summary>
/// Shared execution context that flows through the pipeline.
/// Allows services to share state and track pipeline progress.
/// </summary>
public class PipelineContext
{
    /// <summary>
    /// Unique identifier for this pipeline execution.
    /// </summary>
    public string TaskId { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// The research topic submitted by the user.
    /// </summary>
    public string Topic { get; set; } = string.Empty;

    /// <summary>
    /// Email address for notification delivery.
    /// </summary>
    public string NotifyEmail { get; set; } = string.Empty;

    /// <summary>
    /// Raw content fetched from research sources.
    /// </summary>
    public string? Research { get; set; }

    /// <summary>
    /// LLM-generated summary of the research content.
    /// </summary>
    public string? Summary { get; set; }

    /// <summary>
    /// URL of the generated report in Supabase Storage.
    /// </summary>
    public string? ReportUrl { get; set; }

    /// <summary>
    /// SHA-256 hash of the report content.
    /// </summary>
    public string? BlockchainHash { get; set; }

    /// <summary>
    /// Ethereum transaction hash from blockchain logging.
    /// </summary>
    public string? TransactionHash { get; set; }

    /// <summary>
    /// Current status of the pipeline execution.
    /// </summary>
    public PipelineStatus Status { get; set; } = new();

    /// <summary>
    /// Timestamp when the pipeline was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Timestamp when the pipeline completed.
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Error message if the pipeline failed.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// List of events that occurred during pipeline execution.
    /// </summary>
    public List<PipelineEvent> Events { get; set; } = new();

    /// <summary>
    /// Metadata for extensibility.
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();

    /// <summary>
    /// Duration of the entire pipeline execution.
    /// </summary>
    public TimeSpan? Duration => CompletedAt.HasValue ? CompletedAt.Value - CreatedAt : null;

    /// <summary>
    /// Mark the pipeline as completed.
    /// </summary>
    public void Complete()
    {
        Status.MarkCompleted();
        CompletedAt = DateTime.UtcNow;
        AddEvent("Pipeline", "Pipeline execution completed");
    }

    /// <summary>
    /// Mark the pipeline as failed.
    /// </summary>
    public void Fail(string error)
    {
        Status.MarkFailed(error);
        CompletedAt = DateTime.UtcNow;
        ErrorMessage = error;
        AddEvent("Pipeline", $"Pipeline failed: {error}");
    }

    /// <summary>
    /// Add an event to the pipeline log.
    /// </summary>
    public void AddEvent(string source, string message)
    {
        Events.Add(new PipelineEvent
        {
            Timestamp = DateTime.UtcNow,
            Source = source,
            Message = message
        });
    }

    /// <summary>
    /// Convert to dictionary for inter-service communication.
    /// </summary>
    public Dictionary<string, string> ToDictionary()
    {
        return new Dictionary<string, string>
        {
            ["taskId"] = TaskId,
            ["topic"] = Topic,
            ["notifyEmail"] = NotifyEmail,
            ["research"] = Research ?? "",
            ["summary"] = Summary ?? "",
            ["reportUrl"] = ReportUrl ?? "",
            ["blockchainHash"] = BlockchainHash ?? "",
            ["transactionHash"] = TransactionHash ?? "",
            ["status"] = Status.CurrentStage
        };
    }

    /// <summary>
    /// Create from dictionary received from inter-service communication.
    /// </summary>
    public static PipelineContext FromDictionary(Dictionary<string, string> dict)
    {
        return new PipelineContext
        {
            TaskId = dict.GetValueOrDefault("taskId") ?? Guid.NewGuid().ToString(),
            Topic = dict.GetValueOrDefault("topic") ?? "",
            NotifyEmail = dict.GetValueOrDefault("notifyEmail") ?? "",
            Research = dict.GetValueOrDefault("research"),
            Summary = dict.GetValueOrDefault("summary"),
            ReportUrl = dict.GetValueOrDefault("reportUrl"),
            BlockchainHash = dict.GetValueOrDefault("blockchainHash"),
            TransactionHash = dict.GetValueOrDefault("transactionHash")
        };
    }
}

/// <summary>
/// Tracks the current stage and status of pipeline execution.
/// </summary>
public class PipelineStatus
{
    /// <summary>
    /// Current stage of the pipeline.
    /// </summary>
    public string CurrentStage { get; set; } = "Pending";

    /// <summary>
    /// Progress percentage (0-100).
    /// </summary>
    public int ProgressPercent { get; set; }

    /// <summary>
    /// Whether the pipeline is currently processing.
    /// </summary>
    public bool IsProcessing => CurrentStage == "Processing";

    /// <summary>
    /// Whether the pipeline completed successfully.
    /// </summary>
    public bool IsCompleted => CurrentStage == "Completed";

    /// <summary>
    /// Whether the pipeline failed.
    /// </summary>
    public bool IsFailed => CurrentStage == "Failed";

    /// <summary>
    /// Mark the pipeline as started.
    /// </summary>
    public void MarkStarted()
    {
        CurrentStage = "Processing";
        ProgressPercent = 0;
    }

    /// <summary>
    /// Update the current stage.
    /// </summary>
    public void UpdateStage(string stage, int progress)
    {
        CurrentStage = stage;
        ProgressPercent = progress;
    }

    /// <summary>
    /// Mark the pipeline as completed.
    /// </summary>
    public void MarkCompleted()
    {
        CurrentStage = "Completed";
        ProgressPercent = 100;
    }

    /// <summary>
    /// Mark the pipeline as failed.
    /// </summary>
    public void MarkFailed(string error)
    {
        CurrentStage = "Failed";
        ProgressPercent = 0;
    }
}

/// <summary>
/// Represents an event that occurred during pipeline execution.
/// </summary>
public class PipelineEvent
{
    /// <summary>
    /// When the event occurred.
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Which service or component generated the event.
    /// </summary>
    public string Source { get; set; } = string.Empty;

    /// <summary>
    /// Description of the event.
    /// </summary>
    public string Message { get; set; } = string.Empty;
}
