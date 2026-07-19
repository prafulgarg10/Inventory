namespace InventoryService.Application.Interfaces;
public interface IProductRepository
{
    Task<List<Product>> GetByIdsAsync(IEnumerable<int> productIds, CancellationToken cancellationToken);
    Task AddAsync(Product product, CancellationToken cancellationToken);
}