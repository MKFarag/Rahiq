using Application.Contracts.Carts;
using Application.Feathers.Carts.ClearMyCart;
using Application.Feathers.Carts.GetMyCart;
using Application.Feathers.Carts.RemoveCartProduct;
using Application.Feathers.Carts.UpdateCartProduct;

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CartsController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpGet("")]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
        => Ok(await _sender.Send(new GetMyCartQuery(User.GetId()!), cancellationToken));

    [HttpPut("")]
    public async Task<IActionResult> UpdateCartProduct([FromBody] UpdateCartProductRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new UpdateCartProductCommand(User.GetId()!, request.ProductId, request.Quantity), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }

    [HttpPut("remove-product/{productId}")]
    public async Task<IActionResult> RemoveProduct([FromRoute] int productId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new RemoveCartProductCommand(User.GetId()!, productId), cancellationToken);

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
