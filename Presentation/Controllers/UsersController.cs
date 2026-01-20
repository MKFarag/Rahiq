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
[Authorize]
public class UsersController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    #region Admin Endpoints

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

    [HttpPut("toggle-status/{id}")]
    public async Task<IActionResult> ToggleStatus([FromRoute] string id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new ToggleStatusUserCommand(id), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }

    #endregion

    #region Profile Endpoints

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
    {
        var userId = User.GetId();

        if (userId is null)
            return Unauthorized();

        var result = await _sender.Send(new GetUserProfileQuery(userId), cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request, CancellationToken cancellationToken)
    {
        var userId = User.GetId();

        if (userId is null)
            return Unauthorized();

        var result = await _sender.Send(new UpdateUserProfileCommand(userId, request), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }

    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        var userId = User.GetId();

        if (userId is null)
            return Unauthorized();

        var result = await _sender.Send(new ChangeUserPasswordCommand(userId, request), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }

    [HttpPost("change-email")]
    public async Task<IActionResult> ChangeEmailRequest([FromBody] ChangeEmailRequest request, CancellationToken cancellationToken)
    {
        var userId = User.GetId();

        if (userId is null)
            return Unauthorized();

        var result = await _sender.Send(new ChangeUserEmailRequestCommand(userId, request.NewEmail), cancellationToken);

        return result.IsSuccess
            ? Ok()
            : result.ToProblem();
    }

    [HttpPost("confirm-change-email")]
    public async Task<IActionResult> ConfirmChangeEmail([FromBody] ConfirmChangeEmailRequest request, CancellationToken cancellationToken)
    {
        var userId = User.GetId();

        if (userId is null)
            return Unauthorized();

        var result = await _sender.Send(new ConfirmChangeUserEmailCommand(userId, request), cancellationToken);

        return result.IsSuccess
            ? Ok()
            : result.ToProblem();
    }

    #endregion
}
