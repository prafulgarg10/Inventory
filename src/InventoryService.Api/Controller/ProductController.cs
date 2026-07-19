using System.Threading.Tasks;
using InventoryService.Application.Commands.CreateProduct;
using InventoryService.Application.Queries.GetProducts;
using Microsoft.AspNetCore.Mvc;


[Route("/api")]
public class ProductController : ControllerBase
{
    private readonly GetProductsHandler _getProductsHandler;
    private readonly CreateProductCommandHandler _createProductCommandHandler;
    public ProductController(GetProductsHandler getProductsHandler, CreateProductCommandHandler createProductCommandHandler)
    {
        _getProductsHandler = getProductsHandler;
        _createProductCommandHandler = createProductCommandHandler;
    }

    [HttpPost("products")]
    public async Task<IActionResult> GetProducts([FromBody] GetProductsQuery getProductsQuery, CancellationToken cancellationToken)
    {
        var response = await _getProductsHandler.HandleAsync(getProductsQuery, cancellationToken);
        return Ok(response);
    }

    [HttpPost("product")]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand createProductCommand, CancellationToken cancellationToken)
    {
        var response = await _createProductCommandHandler.HandleAsync(createProductCommand, cancellationToken);
        return Ok(response);
    }
}