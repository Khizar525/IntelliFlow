using Microsoft.EntityFrameworkCore;

public class IntelliFlowDbContext : DbContext
{
    public IntelliFlowDbContext(DbContextOptions<IntelliFlowDbContext> options) 
        : base(options) { }

    public DbSet<ReportRecord> Reports { get; set; }
}