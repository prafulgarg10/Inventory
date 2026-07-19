using InventoryService.Application.Interfaces;

public sealed class UnitofWorkRepository : IUnitofWorkRepository
{
    private readonly AppDbContext _dbContext;

    public UnitofWorkRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}