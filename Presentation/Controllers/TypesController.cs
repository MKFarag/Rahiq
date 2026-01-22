#region Usings

using Application.Contracts.Types;
using Application.Feathers.Types.AddType;
using Application.Feathers.Types.DeleteType;
using Application.Feathers.Types.GetAllTypes;
using Application.Feathers.Types.GetType;
using Application.Feathers.Types.UpdateType;

#endregion

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
[EnableRateLimiting(RateLimitingOptions.PolicyNames.)]
public class TypesController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpGet("")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        => Ok(await _sender.Send(new GetAllTypesQuery(), cancellationToken));

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetTypeQuery(id), cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    [HttpPost("")]
    public async Task<IActionResult> Add([FromBody] TypeRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new AddTypeCommand(request), cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { result.Value.Id }, result.Value)
            : result.ToProblem();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] TypeRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new UpdateTypeCommand(id, request), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteTypeCommand(id), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }
}
