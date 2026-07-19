namespace InventoryService.Application.Queries.GetProducts;
public sealed record GetProductsResult(List<ProductDTO> Products);

public sealed record ProductDTO(int ProductId, decimal UnitPrice);