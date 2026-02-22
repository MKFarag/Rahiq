#region Usings

using Application.Contracts.Carts;
using Application.Feathers.Carts.AddToCart;
using Application.Feathers.Carts.ClearMyCart;
using Application.Feathers.Carts.GetMyCart;
using Application.Feathers.Carts.UpdateCart;

#endregion

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = DefaultRoles.Customer.Name)]
[EnableRateLimiting(RateLimitingOptions.PolicyNames.UserLimit)]
public class CartsController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpGet("")]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
        => Ok(await _sender.Send(new GetMyCartQuery(User.GetId()!), cancellationToken));

    [HttpPost()]
    public async Task<IActionResult> AddToCart([FromBody] CartQuantityRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new AddToCartCommand(User.GetId()!, request.ItemId, request.Quantity, request.IsBundle), cancellationToken);

        return result.IsSuccess
            ? Created()
            : result.ToProblem();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCartProduct([FromRoute] int id, [FromBody] UpdateCartRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new UpdateCartCommand(User.GetId()!, id, request.Quantity), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }

    [HttpDelete()]
    public async Task<IActionResult> Clear(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new ClearMyCartCommand(User.GetId()!), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }
}
