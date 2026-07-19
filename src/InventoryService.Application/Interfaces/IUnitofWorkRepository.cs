namespace InventoryService.Application.Interfaces;
public interface IUnitofWorkRepository
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}