using InventoryService.Domain.Exceptions;
public class Product
{
    public int ProductId {get; private set;}
    public Guid ProductNumber {get; private set;}
    public string Name {get; private set;} = string.Empty;
    public decimal UnitPrice {get; private set;}
    public int AvailableQuantity {get; private set;}
    public int ReservedQuantity {get; private set;}
    public DateTime CreatedAt {get; private set;}
    public DateTime? ModifiedAt {get; private set;} 
    public byte[] RowVersion {get; private set;} = default;
    
    private Product(){}
    private Product(string name, decimal unitPrice, int quantity)
    {
        Name = name;
        ProductNumber = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        UnitPrice = unitPrice;
        AvailableQuantity = quantity;
        ReservedQuantity = 0;
    }

    public static Product Create(string name, decimal unitPrice, int quantity)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Product name is required.");
        }
        if (unitPrice <= 0)
        {
            throw new ArgumentException("Unit price must be greater than zero.");
        }
        if (quantity < 0)
        {
            throw new ArgumentException("Quantity cannot be negative.");
        }
        return new Product(name, unitPrice, quantity);
    }

    public void Reserve(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentException("Quantity must be greater than zero.");
        }
        ReservedQuantity += quantity;
        AvailableQuantity -= quantity;
        ModifiedAt = DateTime.UtcNow;
    }

    public void EnsureCanReserve(int quantity)
    {
        if (AvailableQuantity < quantity)
        {
            throw new InsufficientStockException(ProductId, quantity, AvailableQuantity);
        }
    }

    public void Release(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentException("Quantity must be greater than zero.");
        }
        if (ReservedQuantity < quantity)
        {
            throw new InvalidOperationException("Reserved quantity is insufficient.");
        }
        ReservedQuantity -= quantity;
        AvailableQuantity += quantity;
        ModifiedAt = DateTime.UtcNow;
    }

    public void Commit(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentException("Quantity must be greater than zero.");
        }
        if (ReservedQuantity < quantity)
        {
            throw new InvalidOperationException("Reserved quantity is insufficient.");
        }
        ReservedQuantity -= quantity;
        ModifiedAt = DateTime.UtcNow;
    }

    public void IncreaseStock(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentException("Quantity must be greater than zero.");
        }
        AvailableQuantity += quantity;
        ModifiedAt = DateTime.UtcNow;
    }

    public void UpdatePrice(decimal price)
    {
        if (price <= 0)
        {
            throw new ArgumentException("Price must be greater than zero.");
        }
        UnitPrice = price;
        ModifiedAt = DateTime.UtcNow;
    }
}