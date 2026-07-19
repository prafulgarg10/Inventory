using InventoryService.Application.Interfaces;

namespace InventoryService.Application.Commands.CreateProduct;

public sealed class CreateProductCommandHandler
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitofWorkRepository _unitOfWorkRepository;

    public CreateProductCommandHandler(IProductRepository productRepository, IUnitofWorkRepository unitOfWorkRepository)
    {
        _productRepository = productRepository;
        _unitOfWorkRepository = unitOfWorkRepository;
    }

    public async Task<CreateProductResult> HandleAsync(CreateProductCommand command, CancellationToken cancellationToken)
    {
        Product product = Product.Create(command.Name, command.UnitPrice, command.AvailableQuantity);
        await _productRepository.AddAsync(product, cancellationToken);
        await _unitOfWorkRepository.SaveChangesAsync(cancellationToken);
        return new CreateProductResult(product.ProductId);
    }
}
