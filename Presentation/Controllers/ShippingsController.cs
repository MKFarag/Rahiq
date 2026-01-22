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

[Route("api/[controller]")]
[ApiController]
[Authorize]
[EnableRateLimiting(RateLimitingOptions.PolicyNames.)]
public class ShippingsController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpGet("")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        => Ok(await _sender.Send(new GetAllShippingsQuery(), cancellationToken));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetShippingQuery(id), cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] ShippingRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new UpdateShippingCommand(id, request), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteShippingCommand(id), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }

    [HttpPost("")]
    public async Task<IActionResult> AddCustomerShipping([FromBody] CustomerShippingRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new AddCustomerShippingCommand(request), cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { result.Value.Id }, result.Value)
            : result.ToProblem();
    }

    [HttpPut("{id}/assign-details")]
    public async Task<IActionResult> AssignDetails([FromRoute] int id, [FromBody] AdminShippingRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new AssignShippingDetailsCommand(id, request), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }
}

