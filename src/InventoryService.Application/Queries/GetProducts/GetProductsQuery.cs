namespace InventoryService.Application.Queries.GetProducts;
public sealed record GetProductsQuery(IEnumerable<int> ProductIds);