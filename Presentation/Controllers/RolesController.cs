#region Usings

using Application.Contracts.Roles;
using Application.Feathers.Roles.AddRole;
using Application.Feathers.Roles.GetAllRoles;
using Application.Feathers.Roles.GetRole;
using Application.Feathers.Roles.ToggleRoleStatus;
using Application.Feathers.Roles.UpdateRole;


#endregion

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
[EnableRateLimiting(RateLimitingOptions.PolicyNames.Concurrency)]
public class RolesController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpGet("")]
    [HasPermission(Permissions.ReadRole)]
    public async Task<IActionResult> GetAll([FromQuery] bool includeDisabled = false, CancellationToken cancellationToken = default)
        => Ok(await _sender.Send(new GetAllRolesQuery(includeDisabled), cancellationToken));

    [HttpGet("{id}")]
    [HasPermission(Permissions.ReadRole)]
    public async Task<IActionResult> Get([FromRoute] string id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetRoleQuery(id), cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    [HttpPost("")]
    [HasPermission(Permissions.AddRole)]
    public async Task<IActionResult> Add([FromBody] RoleRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new AddRoleCommand(request), cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(Get), new { result.Value.Id }, result.Value)
            : result.ToProblem();
    }

    [HttpPut("{id}")]
    [HasPermission(Permissions.UpdateRole)]
    public async Task<IActionResult> Update([FromRoute] string id, [FromBody] RoleRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new UpdateRoleCommand(id, request), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }

    [HttpPut("toggle-status/{id}")]
    [HasPermission(Permissions.ToggleRoleStatus)]
    public async Task<IActionResult> ToggleStatus([FromRoute] string id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new ToggleRoleStatusCommand(id), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }
}
