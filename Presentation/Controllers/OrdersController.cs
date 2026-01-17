using Application.Contracts.Order;
using Application.Feathers.Orders.AddOrder;
using Application.Feathers.Orders.GetAllMyOrders;
using Application.Feathers.Orders.GetMyOrder;
using Application.Feathers.Orders.GetAllOrdersByStatus;

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class OrdersController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpGet("me")]
    public async Task<IActionResult> GetMyOrders([FromQuery] int year, CancellationToken cancellationToken)
        => Ok(await _sender.Send(new GetAllMyOrdersQuery(User.GetId()!, year), cancellationToken));

    [HttpGet("me/{id}")]
    public async Task<IActionResult> GetMyOrder([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetMyOrderQuery(id, User.GetId()!), cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAllByStatus([FromQuery] SimpleRequestFilters filters, [FromBody] OrderStatusRequest request, CancellationToken cancellationToken)
        => Ok(await _sender.Send(new GetAllOrdersByStatusQuery(filters, request.Status), cancellationToken));

    [HttpPost("")]
    public async Task<IActionResult> Add(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new AddOrderCommand(User.GetId()!), cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(Add), new { result.Value.Id }, result.Value) // Fix it when we finish the Get EP
            : result.ToProblem();
    }
}
