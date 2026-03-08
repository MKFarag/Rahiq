#region Usings

using Application.Contracts.Types;
using Application.Feathers.Types.AddType;
using Application.Feathers.Types.DeleteType;
using Application.Feathers.Types.GetAllTypes;
using Application.Feathers.Types.GetType;
using Application.Feathers.Types.UpdateType;

#endregion

namespace Presentation.Controllers;

/// <summary>
/// Manage product types.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize]
[EnableRateLimiting(RateLimitingOptions.PolicyNames.Concurrency)]
public class TypesController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    /// <summary>
    /// Retrieves all product types.
    /// </summary>
    /// <remarks>
    /// Returns a list of all available product types in the system.
    /// </remarks>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of product types.</returns>
    /// <response code="200">Returns the list of product types.</response>
    [HttpGet("")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        => Ok(await _sender.Send(new GetAllTypesQuery(), cancellationToken));

    /// <summary>
    /// Retrieves a specific product type by ID.
    /// </summary>
    /// <remarks>
    /// Retrieves the details of a specific product type based on its unique identifier.
    /// </remarks>
    /// <param name="id">The unique identifier of the product type.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The product type details.</returns>
    /// <response code="200">Returns the product type details.</response>
    /// <response code="404">If the product type is not found.</response>
    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetTypeQuery(id), cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    /// <summary>
    /// Creates a new product type.
    /// </summary>
    /// <remarks>
    /// Adds a new product type to the system.
    /// 
    /// Sample request:
    /// 
    ///     POST /api/Types
    ///     {
    ///       "name": "Electronics"
    ///     }
    /// 
    /// </remarks>
    /// <param name="request">The product type creation request details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The newly created product type details.</returns>
    /// <response code="201">Returns the newly created product type.</response>
    /// <response code="400">If the request data is invalid.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="403">If the user does not have permission to add product types.</response>
    /// <response code="409">If a product type with the same name already exists.</response>
    [HttpPost("")]
    [HasPermission(Permissions.AddType)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Add([FromBody] TypeRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new AddTypeCommand(request), cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { result.Value.Id }, result.Value)
            : result.ToProblem();
    }

    /// <summary>
    /// Updates an existing product type.
    /// </summary>
    /// <remarks>
    /// Updates the details of an existing product type by ID.
    /// 
    /// Sample request:
    /// 
    ///     PUT /api/Types/1
    ///     {
    ///       "name": "Home Appliances"
    ///     }
    /// 
    /// </remarks>
    /// <param name="id">The unique identifier of the product type to update.</param>
    /// <param name="request">The updated product type data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">If the product type was successfully updated.</response>
    /// <response code="400">If the request data is invalid.</response>
    /// <response code="404">If the product type is not found.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="403">If the user does not have permission to update product types.</response>
    /// <response code="409">If a product type with the same name already exists.</response>
    [HttpPut("{id}")]
    [HasPermission(Permissions.UpdateType)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] TypeRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new UpdateTypeCommand(id, request), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }

    /// <summary>
    /// Deletes a specific product type by ID.
    /// </summary>
    /// <remarks>
    /// Removes a product type from the system. Ensure no products are dependent on this type before deletion.
    /// </remarks>
    /// <param name="id">The unique identifier of the product type to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">If the product type was successfully deleted.</response>
    /// <response code="404">If the product type is not found.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="403">If the user does not have permission to delete product types.</response>
    /// <response code="409">If the product type is associated with existing products.</response>
    [HttpDelete("{id}")]
    [HasPermission(Permissions.DeleteType)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteTypeCommand(id), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }
}