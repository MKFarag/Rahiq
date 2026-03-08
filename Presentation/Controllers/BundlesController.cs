#region Usings

using Application.Contracts.Bundles;
using Application.Feathers.Bundles.AddBundle;
using Application.Feathers.Bundles.AddBundleImage;
using Application.Feathers.Bundles.DeactivateBundle;
using Application.Feathers.Bundles.DeleteBundleImage;
using Application.Feathers.Bundles.GetAllBundles;
using Application.Feathers.Bundles.GetBundle;
using Application.Feathers.Bundles.ReactivateBundle;
using Application.Feathers.Bundles.UpdateBundle;
using Presentation.DTOs.Bundles;

#endregion

namespace Presentation.Controllers;

/// <summary>
/// Manage product bundles.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize]
[EnableRateLimiting(RateLimitingOptions.PolicyNames.Concurrency)]
public class BundlesController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    /// <summary>
    /// Retrieves all bundles.
    /// </summary>
    /// <remarks>
    /// Returns a list of all available bundles. Allows filtering by availability.
    /// </remarks>
    /// <param name="includeNotAvailable">A boolean flag to include bundles that are not currently available.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of bundles.</returns>
    /// <response code="200">Returns the list of bundles.</response>
    [HttpGet("")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] bool includeNotAvailable, CancellationToken cancellationToken)
        => Ok(await _sender.Send(new GetAllBundlesQuery(includeNotAvailable), cancellationToken));

    /// <summary>
    /// Retrieves a specific bundle by ID.
    /// </summary>
    /// <remarks>
    /// Retrieves the details of a specific bundle based on its unique identifier.
    /// </remarks>
    /// <param name="id">The unique identifier of the bundle.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The bundle details.</returns>
    /// <response code="200">Returns the bundle details.</response>
    /// <response code="404">If the bundle is not found.</response>
    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetBundleQuery(id), cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    /// <summary>
    /// Creates a new bundle.
    /// </summary>
    /// <remarks>
    /// Creates a new bundle with its associated image.
    /// </remarks>
    /// <param name="request">The bundle creation request containing details and an optional image.</param>
    /// <param name="validator">Validator for the request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The newly created bundle details.</returns>
    /// <response code="201">Returns the newly created bundle.</response>
    /// <response code="400">If the request is invalid.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="403">If the user does not have permission to add bundles.</response>
    [HttpPost("")]
    [HasPermission(Permissions.AddBundle)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Add([FromForm] BundleWithImageRequest request, [FromServices] IValidator<BundleWithImageRequest> validator, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return this.ToProblem(validationResult);

        using var image = request.Image?.ToFileData();

        var result = await _sender.Send(new AddBundleCommand(request.Bundle, image), cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(Get), new { result.Value.Bundle.Id }, result.Value)
            : result.ToProblem();
    }

    /// <summary>
    /// Updates an existing bundle.
    /// </summary>
    /// <remarks>
    /// Updates the details of an existing bundle by ID.
    /// 
    /// Sample request:
    /// 
    ///     PUT /api/Bundles/1
    ///     {
    ///       "name": "Summer Collection",
    ///       "quantityAvailable": 100,
    ///       "discountPercentage": 15,
    ///       "endAt": "2026-12-31",
    ///       "productsId": [1, 2, 3]
    ///     }
    /// 
    /// </remarks>
    /// <param name="id">The unique identifier of the bundle to update.</param>
    /// <param name="request">The updated bundle data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">If the bundle was successfully updated.</response>
    /// <response code="400">If the request data is invalid.</response>
    /// <response code="404">If the bundle is not found.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="403">If the user does not have permission to update bundles.</response>
    [HttpPut("{id}")]
    [HasPermission(Permissions.UpdateBundle)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] BundleRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new UpdateBundleCommand(id, request), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }

    /// <summary>
    /// Uploads an image to an existing bundle.
    /// </summary>
    /// <remarks>
    /// Adds or replaces the image associated with the bundle.
    /// </remarks>
    /// <param name="id">The unique identifier of the bundle.</param>
    /// <param name="request">The image upload request.</param>
    /// <param name="validator">Validator for the request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">If the image was successfully added.</response>
    /// <response code="400">If the request data is invalid.</response>
    /// <response code="404">If the bundle is not found.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="403">If the user does not have permission to update bundles.</response>
    [HttpPut("{id}/image")]
    [HasPermission(Permissions.UpdateBundle)]
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

        var result = await _sender.Send(new AddBundleImageCommand(id, image), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }

    /// <summary>
    /// Deletes the image associated with a bundle.
    /// </summary>
    /// <remarks>
    /// Removes the image from a specific bundle.
    /// </remarks>
    /// <param name="id">The unique identifier of the bundle.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">If the image was successfully deleted.</response>
    /// <response code="404">If the bundle is not found.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="403">If the user does not have permission to update bundles.</response>
    [HttpDelete("{id}/image")]
    [HasPermission(Permissions.UpdateBundle)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteImage([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteBundleImageCommand(id), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }

    /// <summary>
    /// Deactivates a bundle.
    /// </summary>
    /// <remarks>
    /// Marks a bundle as deactivated so it is no longer available.
    /// </remarks>
    /// <param name="id">The unique identifier of the bundle.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Ok on success.</returns>
    /// <response code="200">If the bundle was successfully deactivated.</response>
    /// <response code="404">If the bundle is not found.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="403">If the user does not have permission to change bundle activation status.</response>
    [HttpPut("{id}/deactivate")]
    [HasPermission(Permissions.BundleActivation)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Deactivate([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeactivateBundleCommand(id), cancellationToken);

        return result.IsSuccess
            ? Ok()
            : result.ToProblem();
    }

    /// <summary>
    /// Reactivates a deactivated bundle.
    /// </summary>
    /// <remarks>
    /// Marks a deactivated bundle as active again with a specified end date and quantity.
    /// 
    /// Sample request:
    /// 
    ///     PUT /api/Bundles/1/reactivate
    ///     {
    ///       "quantityAvailable": 50,
    ///       "endAt": "2026-12-31"
    ///     }
    /// 
    /// </remarks>
    /// <param name="id">The unique identifier of the bundle.</param>
    /// <param name="request">The reactivation request details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Ok on success.</returns>
    /// <response code="200">If the bundle was successfully reactivated.</response>
    /// <response code="404">If the bundle is not found.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="403">If the user does not have permission to change bundle activation status.</response>
    [HttpPut("{id}/reactivate")]
    [HasPermission(Permissions.BundleActivation)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Reactivate([FromRoute] int id, [FromBody] ReactivateBundleRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new ReactivateBundleCommand(id, request.EndAt, request.QuantityAvailable), cancellationToken);

        return result.IsSuccess
            ? Ok()
            : result.ToProblem();
    }
}