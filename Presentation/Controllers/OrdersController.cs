#region Usings

using Application.Contracts.Order;
using Application.Feathers.Orders.AddOrder;
using Application.Feathers.Orders.GetAllMyOrders;
using Application.Feathers.Orders.GetMyOrder;
using Application.Feathers.Orders.GetAllOrdersByStatus;
using Application.Feathers.Orders.GetAllOrdersByYear;
using Application.Feathers.Orders.GetAllOrdersByMonth;
using Application.Feathers.Orders.GetOrder;
using Application.Feathers.Orders.CancelOrder;
using Application.Feathers.Orders.StartProcessingOrder;
using Application.Feathers.Orders.ShipOrder;
using Application.Feathers.Orders.DeliverOrder;

#endregion

namespace Presentation.Controllers;

/// <summary>
/// Manage customer orders and order fulfillment.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize]
[EnableRateLimiting(RateLimitingOptions.PolicyNames.Concurrency)]
public class OrdersController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    /// <summary>
    /// Retrieves the current customer's orders.
    /// </summary>
    /// <remarks>
    /// Returns a paginated list of orders placed by the authenticated customer, optionally filtered by year.
    /// </remarks>
    /// <param name="filters">Pagination and filtering options.</param>
    /// <param name="year">The year to filter orders by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of the customer's orders.</returns>
    /// <response code="200">Returns the list of orders.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="403">If the user is not a customer.</response>
    [HttpGet("me")]
    [Authorize(Roles = DefaultRoles.Customer.Name)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetMyOrders([FromQuery] SimpleRequestFilters filters, [FromQuery] int year, CancellationToken cancellationToken)
        => Ok(await _sender.Send(new GetAllMyOrdersQuery(filters, User.GetId()!, year), cancellationToken));
     
    /// <summary>
    /// Retrieves a specific order for the current customer.
    /// </summary>
    /// <remarks>
    /// Returns the details of a specific order belonging to the authenticated customer.
    /// </remarks>
    /// <param name="id">The unique identifier of the order.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The order details.</returns>
    /// <response code="200">Returns the order details.</response>
    /// <response code="404">If the order is not found or does not belong to the user.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="403">If the user is not a customer.</response>
    [HttpGet("me/{id}")]
    [Authorize(Roles = DefaultRoles.Customer.Name)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetMyOrder([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetMyOrderQuery(id, User.GetId()!), cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    /// <summary>
    /// Retrieves a specific order by ID (Admin).
    /// </summary>
    /// <remarks>
    /// Retrieves the details of any specific order based on its unique identifier.
    /// </remarks>
    /// <param name="id">The unique identifier of the order.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The order details.</returns>
    /// <response code="200">Returns the order details.</response>
    /// <response code="404">If the order is not found.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="403">If the user does not have permission to read orders.</response>
    [HttpGet("{id}")]
    [HasPermission(Permissions.ReadOrder)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetOrderQuery(id), cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    /// <summary>
    /// Retrieves all orders filtered by status (Admin).
    /// </summary>
    /// <remarks>
    /// Returns a list of orders that match a specific status (e.g., Pending, Shipped).
    /// 
    /// Sample request:
    /// 
    ///     GET /api/Orders/status
    ///     {
    ///       "status": "Pending"
    ///     }
    /// 
    /// </remarks>
    /// <param name="filters">Pagination and filtering options.</param>
    /// <param name="request">The order status request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of orders.</returns>
    /// <response code="200">Returns the list of orders.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="403">If the user does not have permission to read orders.</response>
    [HttpGet("status")]
    [HasPermission(Permissions.ReadOrder)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAllByStatus([FromQuery] SimpleRequestFilters filters, [FromBody] OrderStatusRequest request, CancellationToken cancellationToken)
        => Ok(await _sender.Send(new GetAllOrdersByStatusQuery(filters, request.Status), cancellationToken));

    /// <summary>
    /// Retrieves all orders filtered by year (Admin).
    /// </summary>
    /// <remarks>
    /// Returns a list of all orders placed in a specific year.
    /// </remarks>
    /// <param name="filters">Pagination and filtering options.</param>
    /// <param name="year">The year to filter orders by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of orders.</returns>
    /// <response code="200">Returns the list of orders.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="403">If the user does not have permission to read orders.</response>
    [HttpGet("year")]
    [HasPermission(Permissions.ReadOrder)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAllByYear([FromQuery] SimpleRequestFilters filters, [FromQuery] int year, CancellationToken cancellationToken)
        => Ok(await _sender.Send(new GetAllOrdersByYearQuery(filters, year), cancellationToken));

    /// <summary>
    /// Retrieves all orders filtered by month (Admin).
    /// </summary>
    /// <remarks>
    /// Returns a list of all orders placed in a specific month of the current year.
    /// </remarks>
    /// <param name="filters">Pagination and filtering options.</param>
    /// <param name="month">The month to filter orders by (1-12).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of orders.</returns>
    /// <response code="200">Returns the list of orders.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="403">If the user does not have permission to read orders.</response>
    [HttpGet("month")]
    [HasPermission(Permissions.ReadOrder)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAllByMonth([FromQuery] SimpleRequestFilters filters, [FromQuery] int month, CancellationToken cancellationToken)
        => Ok(await _sender.Send(new GetAllOrdersByMonthQuery(filters, month), cancellationToken));

    /// <summary>
    /// Places a new order from the cart.
    /// </summary>
    /// <remarks>
    /// Converts the current customer's shopping cart into a new order.
    /// </remarks>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The newly created order details.</returns>
    /// <response code="201">Returns the newly created order.</response>
    /// <response code="400">If the cart is empty or items are unavailable.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="403">If the user is not a customer.</response>
    [HttpPost("")]
    [Authorize(Roles = DefaultRoles.Customer.Name)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Add(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new AddOrderCommand(User.GetId()!), cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { result.Value.Id }, result.Value)
            : result.ToProblem();
    }

    /// <summary>
    /// Cancels an order.
    /// </summary>
    /// <remarks>
    /// Cancels a specific order. Can be performed by admins or the customer who placed it if it's still pending.
    /// </remarks>
    /// <param name="id">The unique identifier of the order to cancel.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">If the order was successfully cancelled.</response>
    /// <response code="400">If the order cannot be cancelled in its current status.</response>
    /// <response code="404">If the order is not found.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="403">If the user does not have permission to cancel orders.</response>
    [HttpPut("{id}/cancel")]
    [HasPermission(Permissions.CancelOrder)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Cancel([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new CancelOrderCommand(id, User.GetId()!), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }

    /// <summary>
    /// Marks an order as processing (Admin).
    /// </summary>
    /// <remarks>
    /// Updates the status of an order to Processing, indicating it is being prepared for shipment.
    /// </remarks>
    /// <param name="id">The unique identifier of the order.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">If the order was successfully marked as processing.</response>
    /// <response code="400">If the order cannot be marked as processing in its current status.</response>
    /// <response code="404">If the order is not found.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="403">If the user does not have permission to change order status.</response>
    [HttpPut("{id}/start-processing")]
    [HasPermission(Permissions.ChangeOrderStatus)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> StartProcessing([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new StartProcessingOrderCommand(id), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }

    /// <summary>
    /// Marks an order as shipped (Admin).
    /// </summary>
    /// <remarks>
    /// Updates the status of an order to Shipped and associates it with a shipping record.
    /// </remarks>
    /// <param name="orderId">The unique identifier of the order.</param>
    /// <param name="shippingId">The unique identifier of the shipping record.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">If the order was successfully marked as shipped.</response>
    /// <response code="400">If the order cannot be marked as shipped in its current status.</response>
    /// <response code="404">If the order or shipping record is not found.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="403">If the user does not have permission to change order status.</response>
    [HttpPut("{orderId}/ship/{shippingId}")]
    [HasPermission(Permissions.ChangeOrderStatus)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Ship([FromRoute] int orderId, [FromRoute] int shippingId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new ShipOrderCommand(orderId, shippingId), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }

    /// <summary>
    /// Marks an order as delivered (Admin).
    /// </summary>
    /// <remarks>
    /// Updates the status of an order to Delivered, indicating it has reached the customer.
    /// </remarks>
    /// <param name="id">The unique identifier of the order.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">If the order was successfully marked as delivered.</response>
    /// <response code="400">If the order cannot be marked as delivered in its current status.</response>
    /// <response code="404">If the order is not found.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="403">If the user does not have permission to change order status.</response>
    [HttpPut("{id}/deliver")]
    [HasPermission(Permissions.ChangeOrderStatus)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Deliver([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeliverOrderCommand(id), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }
}