namespace InventoryService.Application.Queries.GetProducts;
public sealed record GetProductsQuery(List<ProductDTO> Products);

public sealed record ProductDTO(int ProductId, decimal Amount);