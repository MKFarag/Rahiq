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

[Route("api/[controller]")]
[ApiController]
[Authorize]
[EnableRateLimiting(RateLimitingOptions.PolicyNames.Concurrency)]
public class OrdersController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpGet("me")]
    [Authorize(Roles = DefaultRoles.Customer.Name)]
    public async Task<IActionResult> GetMyOrders([FromQuery] SimpleRequestFilters filters, [FromQuery] int year, CancellationToken cancellationToken)
        => Ok(await _sender.Send(new GetAllMyOrdersQuery(filters, User.GetId()!, year), cancellationToken));

    [HttpGet("me/{id}")]
    [Authorize(Roles = DefaultRoles.Customer.Name)]
    public async Task<IActionResult> GetMyOrder([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetMyOrderQuery(id, User.GetId()!), cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    [HttpGet("{id}")]
    [HasPermission(Permissions.ReadOrder)]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetOrderQuery(id), cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    [HttpGet("status")]
    [HasPermission(Permissions.ReadOrder)]
    public async Task<IActionResult> GetAllByStatus([FromQuery] SimpleRequestFilters filters, [FromBody] OrderStatusRequest request, CancellationToken cancellationToken)
        => Ok(await _sender.Send(new GetAllOrdersByStatusQuery(filters, request.Status), cancellationToken));

    [HttpGet("year")]
    [HasPermission(Permissions.ReadOrder)]
    public async Task<IActionResult> GetAllByYear([FromQuery] SimpleRequestFilters filters, [FromQuery] int year, CancellationToken cancellationToken)
        => Ok(await _sender.Send(new GetAllOrdersByYearQuery(filters, year), cancellationToken));

    [HttpGet("month")]
    [HasPermission(Permissions.ReadOrder)]
    public async Task<IActionResult> GetAllByMonth([FromQuery] SimpleRequestFilters filters, [FromQuery] int month, CancellationToken cancellationToken)
        => Ok(await _sender.Send(new GetAllOrdersByMonthQuery(filters, month), cancellationToken));

    [HttpPost("")]
    [HasPermission(Permissions.AddOrder)]
    public async Task<IActionResult> Add(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new AddOrderCommand(User.GetId()!), cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { result.Value.Id }, result.Value)
            : result.ToProblem();
    }

    [HttpPut("{id}/cancel")]
    [HasPermission(Permissions.CancelOrder)]
    public async Task<IActionResult> Cancel([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new CancelOrderCommand(id, User.GetId()!), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }

    [HttpPut("{id}/start-processing")]
    [HasPermission(Permissions.ChangeOrderStatus)]
    public async Task<IActionResult> StartProcessing([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new StartProcessingOrderCommand(id), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }

    [HttpPut("{orderId}/ship/{shippingId}")]
    [HasPermission(Permissions.ChangeOrderStatus)]
    public async Task<IActionResult> Ship([FromRoute] int orderId, [FromRoute] int shippingId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new ShipOrderCommand(orderId, shippingId), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }

    [HttpPut("{id}/deliver")]
    [HasPermission(Permissions.ChangeOrderStatus)]
    public async Task<IActionResult> Deliver([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeliverOrderCommand(id), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }
}
