using InventoryService.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Infrastructure.Persistence.Repositories;
public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _dbContext;
    public ProductRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<List<Product>> GetByIdsAsync(IEnumerable<int> productIds, CancellationToken cancellationToken)
    {
        return await _dbContext.Products.Where(p => productIds.Contains(p.ProductId)).ToListAsync(cancellationToken);
    }
    public async Task AddAsync(Product product, CancellationToken cancellationToken)
    {
        await _dbContext.Products.AddAsync(product, cancellationToken);
    }
}