using System.Collections.Immutable;
using InventoryService.Application.Interfaces;
using InventoryService.Application.Queries.GetProducts;

namespace InventoryService.Application.Queries.GetProducts;
public sealed class GetProductsHandler
{
    private readonly IProductRepository _productRepository;
    public GetProductsHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<GetProductsResult> HandleAsync(GetProductsQuery query, CancellationToken cancellationToken)
    {
        var response = await _productRepository.GetByIdsAsync(query.Products.Select(p => p.ProductId).ToList(), cancellationToken);
        return new GetProductsResult(response.Select(p => new ProductDTO(p.ProductId, p.UnitPrice)).ToList());
    }
}