#region Usings

using Application.Contracts.Shippings;
using Application.Feathers.Shippings.AddCustomerShipping;
using Application.Feathers.Shippings.AssignShippingDetails;
using Application.Feathers.Shippings.DeleteShipping;
using Application.Feathers.Shippings.GetAllShippings;
using Application.Feathers.Shippings.GetShipping;
using Application.Feathers.Shippings.UpdateShipping;

#endregion

namespace Presentation.Controllers;

/// <summary>
/// Manage shippings and delivery information.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize]
[EnableRateLimiting(RateLimitingOptions.PolicyNames.Concurrency)]
public class ShippingsController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    /// <summary>
    /// Retrieves all shippings.
    /// </summary>
    /// <remarks>
    /// Returns a list of all available shippings.
    /// </remarks>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of shippings.</returns>
    /// <response code="200">Returns the list of shippings.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="403">If the user does not have permission to read shippings.</response>
    [HttpGet("")]
    [HasPermission(Permissions.ReadShipping)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        => Ok(await _sender.Send(new GetAllShippingsQuery(), cancellationToken));

    /// <summary>
    /// Retrieves a specific shipping by ID.
    /// </summary>
    /// <remarks>
    /// Retrieves the details of a specific shipping based on its unique identifier.
    /// </remarks>
    /// <param name="id">The unique identifier of the shipping.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The shipping details.</returns>
    /// <response code="200">Returns the shipping details.</response>
    /// <response code="404">If the shipping is not found.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="403">If the user does not have permission to read shippings.</response>
    [HttpGet("{id}")]
    [HasPermission(Permissions.ReadShipping)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetShippingQuery(id), cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    /// <summary>
    /// Updates an existing shipping.
    /// </summary>
    /// <remarks>
    /// Updates the details of an existing shipping by ID.
    /// 
    /// Sample request:
    /// 
    ///     PUT /api/Shippings/1
    ///     {
    ///       "address": "123 Main St",
    ///       "phone": "01000000000",
    ///       "cost": 50.00,
    ///       "code": "SHP123"
    ///     }
    /// 
    /// </remarks>
    /// <param name="id">The unique identifier of the shipping to update.</param>
    /// <param name="request">The updated shipping data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">If the shipping was successfully updated.</response>
    /// <response code="400">If the request data is invalid.</response>
    /// <response code="404">If the shipping is not found.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="403">If the user does not have permission to update shippings.</response>
    [HttpPut("{id}")]
    [HasPermission(Permissions.UpdateShipping)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] ShippingRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new UpdateShippingCommand(id, request), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }

    /// <summary>
    /// Deletes a specific shipping by ID.
    /// </summary>
    /// <remarks>
    /// Removes a shipping from the system.
    /// </remarks>
    /// <param name="id">The unique identifier of the shipping to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">If the shipping was successfully deleted.</response>
    /// <response code="404">If the shipping is not found.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="403">If the user does not have permission to delete shippings.</response>
    [HttpDelete("{id}")]
    [HasPermission(Permissions.DeleteShipping)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteShippingCommand(id), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }

    /// <summary>
    /// Adds a new customer shipping.
    /// </summary>
    /// <remarks>
    /// Customers can use this endpoint to add their shipping information.
    /// 
    /// Sample request:
    /// 
    ///     POST /api/Shippings
    ///     {
    ///       "orderId": 5,
    ///       "address": "123 Main St",
    ///       "phone": "01000000000"
    ///     }
    /// 
    /// </remarks>
    /// <param name="request">The shipping request details containing customer info.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The newly created shipping details.</returns>
    /// <response code="201">Returns the newly created shipping.</response>
    /// <response code="400">If the request validation fails.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="403">If the user is not a customer.</response>
    [HttpPost("")]
    [Authorize(Roles = DefaultRoles.Customer.Name)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> AddCustomerShipping([FromBody] CustomerShippingRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new AddCustomerShippingCommand(request), cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { result.Value.Id }, result.Value)
            : result.ToProblem();
    }

    /// <summary>
    /// Assigns shipping details to an existing shipping record.
    /// </summary>
    /// <remarks>
    /// Administrators can use this endpoint to assign specific shipping details like tracking information.
    /// 
    /// Sample request:
    /// 
    ///     PUT /api/Shippings/1/assign-details
    ///     {
    ///       "cost": 50.00,
    ///       "code": "SHP123"
    ///     }
    /// 
    /// </remarks>
    /// <param name="id">The unique identifier of the shipping.</param>
    /// <param name="request">The shipping request details containing admin info.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">If the shipping details were successfully assigned.</response>
    /// <response code="400">If the request data is invalid.</response>
    /// <response code="404">If the shipping is not found.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="403">If the user does not have permission to assign shipping details.</response>
    [HttpPut("{id}/assign-details")]
    [HasPermission(Permissions.AssignShippingDetails)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> AssignDetails([FromRoute] int id, [FromBody] AdminShippingRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new AssignShippingDetailsCommand(id, request), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }
}