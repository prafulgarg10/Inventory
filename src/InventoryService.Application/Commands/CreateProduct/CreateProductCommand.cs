namespace InventoryService.Application.Commands.CreateProduct;
public sealed record CreateProductCommand(string Name, decimal UnitPrice, int AvailableQuantity, int ReservedQuantity); 
