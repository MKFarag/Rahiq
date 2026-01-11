using Application.Contracts.Products;
using Application.Feathers.Products.AddProduct;
using Application.Feathers.Products.AddProductDiscount;
using Application.Feathers.Products.ChangeProductStatus;
using Application.Feathers.Products.DeleteProductImage;
using Application.Feathers.Products.GetAllProducts;
using Application.Feathers.Products.GetProduct;
using Presentation.DTOs.Products;

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

    [HttpPost("")]
    public async Task<IActionResult> Add([FromForm] ProductWithImageRequest request, [FromServices] IValidator<ProductWithImageRequest> validator, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return this.ToProblem(validationResult);

        using var image = request.Image?.ToFileData();

        var result = await _sender.Send(new AddProductCommand(request.Product, image), cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(Get), new { result.Value.Id }, result.Value)
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

    [HttpDelete("image/{id}")]
    public async Task<IActionResult> DeleteImage([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteProductImageCommand(id), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }
}
