// ============================================================
// Module 3: Entity Framework Core — DbContext + Entities
// Owner: Hassan Asif (02-131232-113)
//
// After setting AZURE_SQL_CONNECTION, run:
//   dotnet ef migrations add InitialCreate
//   dotnet ef database update
// ============================================================
using Microsoft.EntityFrameworkCore;

public class IntelliFlowDbContext : DbContext
{
    public IntelliFlowDbContext(DbContextOptions<IntelliFlowDbContext> options)
        : base(options) { }

    public DbSet<TaskEntity>   Tasks      { get; set; }
    public DbSet<AgentLog>     AgentLogs  { get; set; }
    public DbSet<ReportEntity> Reports    { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskEntity>(e =>
        {
            e.HasKey(t => t.Id);
            e.Property(t => t.Status).HasMaxLength(50).HasDefaultValue("Pending");
        });

        modelBuilder.Entity<ReportEntity>(e =>
        {
            e.HasKey(r => r.Id);
            e.HasOne<TaskEntity>().WithOne().HasForeignKey<ReportEntity>(r => r.TaskId);
        });

        modelBuilder.Entity<AgentLog>(e =>
        {
            e.HasKey(a => a.Id);
            e.HasOne<TaskEntity>().WithMany().HasForeignKey(a => a.TaskId);
        });
    }
}

// ── Entities ────────────────────────────────────────────────
public class TaskEntity
{
    public Guid     Id          { get; set; } = Guid.NewGuid();
    public string   Topic       { get; set; } = string.Empty;
    public string   Status      { get; set; } = "Pending";
    public string   RequestedBy { get; set; } = string.Empty;
    public DateTime CreatedAt   { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt   { get; set; } = DateTime.UtcNow;
}

public class AgentLog
{
    public Guid     Id         { get; set; } = Guid.NewGuid();
    public Guid     TaskId     { get; set; }
    public string   AgentName  { get; set; } = string.Empty;
    public string   Status     { get; set; } = string.Empty;
    public string?  Message    { get; set; }
    public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;
}

public class ReportEntity
{
    public Guid    Id               { get; set; } = Guid.NewGuid();
    public Guid    TaskId           { get; set; }
    public string  BlobUrl          { get; set; } = string.Empty;
    public string  OutputHash       { get; set; } = string.Empty;
    public string? BlockchainTxHash { get; set; }
    public DateTime CreatedAt       { get; set; } = DateTime.UtcNow;
}
