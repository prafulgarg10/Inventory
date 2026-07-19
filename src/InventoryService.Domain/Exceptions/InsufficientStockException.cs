namespace InventoryService.Domain.Exceptions;
public sealed class InsufficientStockException : Exception
{
    public int ProductId {get;}
    public int ReservedQuantity {get;}
    public int AvailableQuantity {get;}
    public InsufficientStockException(int productId, int reservedQuantity, int availableQuantity) : base($"Insufficient stock for product {productId}. Reserved quantity: {reservedQuantity}, Available quantity: {availableQuantity}.")
    {
        ProductId = productId;
        ReservedQuantity = reservedQuantity;
        AvailableQuantity = availableQuantity;
    }
}