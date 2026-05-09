public class ReportRecord
{
    public int Id { get; set; }
    public string TaskId { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
    public string BlobUrl { get; set; } = string.Empty;
    public string OutputHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}