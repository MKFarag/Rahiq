#region Usings

using Application.Contracts.Users;
using Application.Feathers.Users.ChangeUserEmailRequest;
using Application.Feathers.Users.ChangeUserPassword;
using Application.Feathers.Users.ConfirmChangeUserEmail;
using Application.Feathers.Users.GetUserProfile;
using Application.Feathers.Users.UpdateUserProfile;

#endregion

namespace Presentation.Controllers;

[Route("me")]
[ApiController]
[Authorize]
[EnableRateLimiting(RateLimitingOptions.PolicyNames.UserLimit)]
public class ProfileController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpGet("")]
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

    [HttpPut("")]
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
}
