using Application.Contracts.Products;
using Application.Feathers.Products.AddProductDiscount;
using Application.Feathers.Products.ChangeProductStatus;
using Application.Feathers.Products.GetAllProducts;
using Application.Feathers.Products.GetProduct;

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpGet("")]
    public async Task<IActionResult> GetAll([FromQuery] RequestFilters filters, [FromQuery] bool includeNotAvailable, CancellationToken cancellationToken)
        => Ok(await _sender.Send(new GetAllProductsQuery(filters, includeNotAvailable), cancellationToken));

    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetProductQuery(id), cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    [HttpPut("change-status/{id}")]
    public async Task<IActionResult> ChangeStatus([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new ChangeProductStatusCommand(id), cancellationToken);

        return result.IsSuccess
            ? Ok()
            : result.ToProblem();
    }

    [HttpPut("discount/{id}")]
    public async Task<IActionResult> AddDiscount([FromRoute] int id, [FromBody] ProductDiscountRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new AddProductDiscountCommand(id, request.DiscountPercentage), cancellationToken);

        return result.IsSuccess
            ? Ok()
            : result.ToProblem();
    }
}
