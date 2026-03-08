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

/// <summary>
/// Manage products and catalog.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize]
[EnableRateLimiting(RateLimitingOptions.PolicyNames.Concurrency)]
public class ProductsController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    /// <summary>
    /// Retrieves all products.
    /// </summary>
    /// <remarks>
    /// Returns a paginated list of all products based on the provided filters. Allows optionally including unavailable products.
    /// </remarks>
    /// <param name="filters">Pagination and generic filtering options.</param>
    /// <param name="includeNotAvailable">A boolean flag to include unavailable products.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of products.</returns>
    /// <response code="200">Returns the list of products.</response>
    [HttpGet("")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] RequestFilters filters, [FromQuery] bool includeNotAvailable, CancellationToken cancellationToken)
        => Ok(await _sender.Send(new GetAllProductsQuery(filters, includeNotAvailable), cancellationToken));

    /// <summary>
    /// Retrieves a specific product by ID.
    /// </summary>
    /// <remarks>
    /// Retrieves the details of a specific product based on its unique identifier.
    /// </remarks>
    /// <param name="id">The unique identifier of the product.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The product details.</returns>
    /// <response code="200">Returns the product details.</response>
    /// <response code="404">If the product is not found.</response>
    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetProductQuery(id), cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    /// <summary>
    /// Creates a new product.
    /// </summary>
    /// <remarks>
    /// Adds a new product to the catalog, optionally including an image.
    /// </remarks>
    /// <param name="request">The product creation request with details and image.</param>
    /// <param name="validator">Validator for the request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The newly created product details.</returns>
    /// <response code="201">Returns the newly created product.</response>
    /// <response code="400">If the request data is invalid.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="403">If the user does not have permission to add products.</response>
    [HttpPost("")]
    [HasPermission(Permissions.AddProduct)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
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

    /// <summary>
    /// Updates an existing product.
    /// </summary>
    /// <remarks>
    /// Updates the details of an existing product by ID.
    /// 
    /// Sample request:
    /// 
    ///     PUT /api/Products/1
    ///     {
    ///       "name": "Smartphone",
    ///       "brand": "Samsung",
    ///       "description": "Latest model",
    ///       "categoryId": 2,
    ///       "typeId": 3,
    ///       "price": 999.99,
    ///       "discountPercentage": 10,
    ///       "isAvailable": true
    ///     }
    /// 
    /// </remarks>
    /// <param name="id">The unique identifier of the product to update.</param>
    /// <param name="request">The updated product data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">If the product was successfully updated.</response>
    /// <response code="400">If the request data is invalid.</response>
    /// <response code="404">If the product is not found.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="403">If the user does not have permission to update products.</response>
    [HttpPut("{id}")]
    [HasPermission(Permissions.UpdateProduct)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] ProductRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new UpdateProductCommand(id, request), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }

    /// <summary>
    /// Toggles a product's availability status.
    /// </summary>
    /// <remarks>
    /// Changes the active/inactive status of a product. Inactive products won't be shown to regular users.
    /// </remarks>
    /// <param name="id">The unique identifier of the product.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Ok on success.</returns>
    /// <response code="200">If the product status was successfully changed.</response>
    /// <response code="404">If the product is not found.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="403">If the user does not have permission to change product status.</response>
    [HttpPut("{id}/change-status")]
    [HasPermission(Permissions.ChangeProductStatus)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ChangeStatus([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new ChangeProductStatusCommand(id), cancellationToken);

        return result.IsSuccess
            ? Ok()
            : result.ToProblem();
    }

    /// <summary>
    /// Applies a discount to a product.
    /// </summary>
    /// <remarks>
    /// Adds or updates the discount percentage applied to a product.
    /// 
    /// Sample request:
    /// 
    ///     PUT /api/Products/1/discount
    ///     {
    ///       "discountPercentage": 15
    ///     }
    /// 
    /// </remarks>
    /// <param name="id">The unique identifier of the product.</param>
    /// <param name="request">The request containing the new discount percentage.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Ok on success.</returns>
    /// <response code="200">If the product discount was successfully applied.</response>
    /// <response code="400">If the discount percentage is invalid.</response>
    /// <response code="404">If the product is not found.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="403">If the user does not have permission to discount products.</response>
    [HttpPut("{id}/discount")]
    [HasPermission(Permissions.DiscountProduct)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Discount([FromRoute] int id, [FromBody] ProductDiscountRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new AddProductDiscountCommand(id, request.DiscountPercentage), cancellationToken);

        return result.IsSuccess
            ? Ok()
            : result.ToProblem();
    }

    /// <summary>
    /// Uploads an image to an existing product.
    /// </summary>
    /// <remarks>
    /// Adds or replaces the image associated with the product.
    /// </remarks>
    /// <param name="id">The unique identifier of the product.</param>
    /// <param name="request">The image upload request.</param>
    /// <param name="validator">Validator for the request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">If the image was successfully added.</response>
    /// <response code="400">If the request data is invalid.</response>
    /// <response code="404">If the product is not found.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="403">If the user does not have permission to update products.</response>
    [HttpPut("{id}/image")]
    [HasPermission(Permissions.UpdateProduct)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
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

    /// <summary>
    /// Deletes the image associated with a product.
    /// </summary>
    /// <remarks>
    /// Removes the image from a specific product.
    /// </remarks>
    /// <param name="id">The unique identifier of the product.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">If the image was successfully deleted.</response>
    /// <response code="404">If the product is not found.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="403">If the user does not have permission to update products.</response>
    [HttpDelete("{id}/image")]
    [HasPermission(Permissions.UpdateProduct)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteImage([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteProductImageCommand(id), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }
}