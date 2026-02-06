#region Usings

using Application.Contracts.Products;
using Application.Feathers.Products.AddProduct;
using Application.Feathers.Products.AddProductDiscount;
using Application.Feathers.Products.AddProductImage;
using Application.Feathers.Products.ChangeProductStatus;
using Application.Feathers.Products.DeleteProductImage;
using Application.Feathers.Products.GetAllProducts;
using Application.Feathers.Products.GetProduct;
using Application.Feathers.Products.UpdateProduct;
using Presentation.DTOs.Products;

#endregion

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
[EnableRateLimiting(RateLimitingOptions.PolicyNames.Concurrency)]
public class ProductsController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpGet("")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll([FromQuery] RequestFilters filters, [FromQuery] bool includeNotAvailable, CancellationToken cancellationToken)
        => Ok(await _sender.Send(new GetAllProductsQuery(filters, includeNotAvailable), cancellationToken));

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> Get([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetProductQuery(id), cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    [HttpPost("")]
    [HasPermission(Permissions.AddProduct)]
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

    [HttpPut("{id}")]
    [HasPermission(Permissions.UpdateProduct)]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] ProductRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new UpdateProductCommand(id, request), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }

    [HttpPut("{id}/change-status")]
    [HasPermission(Permissions.ChangeProductStatus)]
    public async Task<IActionResult> ChangeStatus([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new ChangeProductStatusCommand(id), cancellationToken);

        return result.IsSuccess
            ? Ok()
            : result.ToProblem();
    }

    [HttpPut("{id}/discount")]
    [HasPermission(Permissions.DiscountProduct)]
    public async Task<IActionResult> Discount([FromRoute] int id, [FromBody] ProductDiscountRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new AddProductDiscountCommand(id, request.DiscountPercentage), cancellationToken);

        return result.IsSuccess
            ? Ok()
            : result.ToProblem();
    }

    [HttpPut("{id}/image")]
    [HasPermission(Permissions.UpdateProduct)]
    public async Task<IActionResult> AddImage([FromRoute] int id, [FromForm] UploadImageRequest request, [FromServices] IValidator<UploadImageRequest> validator, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return this.ToProblem(validationResult);

        using var image = request.Image.ToFileData();

        var result = await _sender.Send(new AddProductImageCommand(id, image), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }

    [HttpDelete("{id}/image")]
    [HasPermission(Permissions.UpdateProduct)]
    public async Task<IActionResult> DeleteImage([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteProductImageCommand(id), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }
}
