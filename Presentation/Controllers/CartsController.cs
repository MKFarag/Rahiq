#region Usings

using Application.Contracts.Carts;
using Application.Feathers.Carts.AddToCart;
using Application.Feathers.Carts.ClearMyCart;
using Application.Feathers.Carts.GetMyCart;
using Application.Feathers.Carts.UpdateCart;

#endregion

namespace Presentation.Controllers;

/// <summary>
/// Manage customer shopping carts.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = DefaultRoles.Customer.Name)]
[EnableRateLimiting(RateLimitingOptions.PolicyNames.UserLimit)]
public class CartsController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    /// <summary>
    /// Retrieves the current customer's cart.
    /// </summary>
    /// <remarks>
    /// Returns the contents of the shopping cart for the authenticated customer.
    /// </remarks>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The cart details including items and total price.</returns>
    /// <response code="200">Returns the customer's cart.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="403">If the user is not a customer.</response>
    [HttpGet("")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
        => Ok(await _sender.Send(new GetMyCartQuery(User.GetId()!), cancellationToken));

    /// <summary>
    /// Adds an item to the cart.
    /// </summary>
    /// <remarks>
    /// Adds a product or bundle to the customer's shopping cart.
    /// 
    /// Sample request:
    /// 
    ///     POST /api/Carts
    ///     {
    ///       "itemId": 1,
    ///       "quantity": 2,
    ///       "isBundle": false
    ///     }
    /// 
    /// </remarks>
    /// <param name="request">The request containing the item ID, quantity, and whether it's a bundle.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Created status on success.</returns>
    /// <response code="201">If the item was successfully added to the cart.</response>
    /// <response code="400">If the item is not available or quantity is invalid.</response>
    /// <response code="404">If the item is not found.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="403">If the user is not a customer.</response>
    [HttpPost()]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> AddToCart([FromBody] CartQuantityRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new AddToCartCommand(User.GetId()!, request.ItemId, request.Quantity, request.IsBundle), cancellationToken);

        return result.IsSuccess
            ? Created()
            : result.ToProblem();
    }

    /// <summary>
    /// Updates an item's quantity in the cart.
    /// </summary>
    /// <remarks>
    /// Updates the quantity of a specific cart item.
    /// 
    /// Sample request:
    /// 
    ///     PUT /api/Carts/5
    ///     {
    ///       "quantity": 3
    ///     }
    /// 
    /// </remarks>
    /// <param name="id">The unique identifier of the cart item.</param>
    /// <param name="request">The request containing the new quantity.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">If the cart item was successfully updated.</response>
    /// <response code="400">If the quantity is invalid or exceeds available stock.</response>
    /// <response code="404">If the cart item is not found.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="403">If the user is not a customer.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateCartProduct([FromRoute] int id, [FromBody] UpdateCartRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new UpdateCartCommand(User.GetId()!, id, request.Quantity), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }

    /// <summary>
    /// Clears the current customer's cart.
    /// </summary>
    /// <remarks>
    /// Removes all items from the authenticated customer's shopping cart.
    /// </remarks>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">If the cart was successfully cleared.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="403">If the user is not a customer.</response>
    [HttpDelete()]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Clear(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new ClearMyCartCommand(User.GetId()!), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }
}