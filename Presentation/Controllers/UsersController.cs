#region Usings

using Application.Contracts.Users;
using Application.Feathers.Users.AddUser;
using Application.Feathers.Users.ChangeUserEmailRequest;
using Application.Feathers.Users.ChangeUserPassword;
using Application.Feathers.Users.ConfirmChangeUserEmail;
using Application.Feathers.Users.GetAllUsers;
using Application.Feathers.Users.GetUser;
using Application.Feathers.Users.GetUserProfile;
using Application.Feathers.Users.ToggleStatusUser;
using Application.Feathers.Users.UpdateUser;
using Application.Feathers.Users.UpdateUserProfile;

#endregion

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = DefaultRoles.Admin.Name)]
[EnableRateLimiting(RateLimitingOptions.PolicyNames.Concurrency)]
public class UsersController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpGet("")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        => Ok(await _sender.Send(new GetAllUsersQuery(), cancellationToken));

    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute] string id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetUserQuery(id), cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    [HttpPost("")]
    public async Task<IActionResult> Add([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new AddUserCommand(request), cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(Get), new { result.Value.Id }, result.Value)
            : result.ToProblem();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] string id, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new UpdateUserCommand(id, request), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }

    [HttpPut("{id}/toggle-status")]
    public async Task<IActionResult> ToggleStatus([FromRoute] string id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new ToggleStatusUserCommand(id), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }
}
