using Application.Feathers.Product.GetAllProducts;

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpGet("")]
    public async Task<IActionResult> GetAll([FromQuery] RequestFilters filters, [FromQuery] bool includeNotAvailable, CancellationToken cancellationToken)
        => Ok(await _sender.Send(new GetAllProductsQuery(filters, includeNotAvailable), cancellationToken));
}
