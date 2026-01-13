#region Usings

using Application.Contracts.Auth;
using Application.Feathers.Auth.ConfirmEmail;
using Application.Feathers.Auth.GetRefreshToken;
using Application.Feathers.Auth.GetToken;
using Application.Feathers.Auth.Register;
using Application.Feathers.Auth.ResendConfirmationEmail;
using Application.Feathers.Auth.ResetPassword;
using Application.Feathers.Auth.RevokeRefreshToken;
using Application.Feathers.Auth.SendResetPasswordCode;

#endregion

namespace Presentation.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpPost("")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var command = new GetTokenCommand(request.Email, request.Password);
        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> GetRefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var command = new GetRefreshTokenCommand(request.Token, request.RefreshToken);
        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    [HttpPost("revoke-refresh")]
    public async Task<IActionResult> RevokeRefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var command = new RevokeRefreshTokenCommand(request.Token, request.RefreshToken);
        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Ok()
            : result.ToProblem();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var command = new RegisterCommand(request);
        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Ok()
            : result.ToProblem();
    }

    [HttpPost("confirm")]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request, CancellationToken cancellationToken)
    {
        var command = new ConfirmEmailCommand(request.UserId, request.Token);
        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Ok()
            : result.ToProblem();
    }

    [HttpPost("resend-confirm")]
    public async Task<IActionResult> ResendConfirmEmail([FromBody] EmailRequest request, CancellationToken cancellationToken)
    {
        var command = new ResendConfirmationEmailCommand(request.Email);
        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Ok()
            : result.ToProblem();
    }

    [HttpPost("forget-password")]
    public async Task<IActionResult> ForgetPassword([FromBody] EmailRequest request, CancellationToken cancellationToken)
    {
        var command = new SendResetPasswordCodeCommand(request.Email);
        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Ok()
            : result.ToProblem();
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        var command = new ResetPasswordCommand(request);
        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Ok()
            : result.ToProblem();
    }
}
