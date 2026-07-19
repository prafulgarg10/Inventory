using System.Threading.Tasks;
using InventoryService.Application.Queries.GetProducts;
using Microsoft.AspNetCore.Mvc;


[Route("/api")]
public class ProductController : ControllerBase
{
    private GetProductsHandler _getProductsHandler;
    public ProductController(GetProductsHandler getProductsHandler)
    {
        _getProductsHandler = getProductsHandler;
    }

    [HttpPost("products")]
    public async Task<IActionResult> GetProducts([FromBody] GetProductsQuery getProductsQuery, CancellationToken cancellationToken)
    {
        var response = await _getProductsHandler.HandleAsync(getProductsQuery, cancellationToken);
        return Ok(response);
    }
}