using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }
    public DbSet<Product> Products {get; set;}
    public DbSet<ProcessedMessage> ProcessedMessages {get; set;}
    public DbSet<OutboxMessage> OutboxMessages {get; set;}
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AppDbContext).Assembly);
    }
}