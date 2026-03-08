#region Usings

using Application.Contracts.Users;
using Application.Feathers.Users.ChangeUserEmailRequest;
using Application.Feathers.Users.ChangeUserPassword;
using Application.Feathers.Users.ConfirmChangeUserEmail;
using Application.Feathers.Users.GetUserProfile;
using Application.Feathers.Users.UpdateUserProfile;

#endregion

namespace Presentation.Controllers;

/// <summary>
/// Manage authenticated user's profile and account settings.
/// </summary>
[Route("me")]
[ApiController]
[Authorize]
[EnableRateLimiting(RateLimitingOptions.PolicyNames.UserLimit)]
public class ProfileController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    /// <summary>
    /// Retrieves the current user's profile.
    /// </summary>
    /// <remarks>
    /// Returns the profile details of the currently authenticated user.
    /// </remarks>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The user profile details.</returns>
    /// <response code="200">Returns the user profile.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="404">If the user profile is not found.</response>
    [HttpGet("")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

    /// <summary>
    /// Updates the current user's profile.
    /// </summary>
    /// <remarks>
    /// Updates general profile information of the authenticated user.
    /// 
    /// Sample request:
    /// 
    ///     PUT /me
    ///     {
    ///       "firstName": "John",
    ///       "lastName": "Doe"
    ///     }
    /// 
    /// </remarks>
    /// <param name="request">The profile update request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">If the profile was successfully updated.</response>
    /// <response code="400">If the request data is invalid.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="404">If the user is not found.</response>
    [HttpPut("")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

    /// <summary>
    /// Changes the current user's password.
    /// </summary>
    /// <remarks>
    /// Changes the password of the authenticated user. Requires current password verification.
    /// 
    /// Sample request:
    /// 
    ///     PUT /me/change-password
    ///     {
    ///       "currentPassword": "OldPassword123!",
    ///       "newPassword": "NewPassword123!"
    ///     }
    /// 
    /// </remarks>
    /// <param name="request">The change password request containing current and new password.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">If the password was successfully changed.</response>
    /// <response code="400">If the request data or current password is invalid.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="404">If the user is not found.</response>
    [HttpPut("change-password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

    /// <summary>
    /// Requests an email change.
    /// </summary>
    /// <remarks>
    /// Initiates an email change request for the authenticated user. A confirmation code will be sent to the new email address.
    /// 
    /// Sample request:
    /// 
    ///     POST /me/change-email
    ///     {
    ///       "newEmail": "new.email@example.com"
    ///     }
    /// 
    /// </remarks>
    /// <param name="request">The change email request containing the new email.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Ok on success.</returns>
    /// <response code="200">If the email change request was successfully initiated.</response>
    /// <response code="400">If the request data is invalid or email already exists.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="404">If the user is not found.</response>
    [HttpPost("change-email")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

    /// <summary>
    /// Confirms an email change request.
    /// </summary>
    /// <remarks>
    /// Verifies the confirmation token to apply the new email address to the user's account.
    /// 
    /// Sample request:
    /// 
    ///     POST /me/confirm-change-email
    ///     {
    ///       "newEmail": "new.email@example.com",
    ///       "token": "CfDJ8..."
    ///     }
    /// 
    /// </remarks>
    /// <param name="request">The request containing the new email and confirmation token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Ok on success.</returns>
    /// <response code="200">If the email was successfully changed.</response>
    /// <response code="400">If the token is invalid.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="404">If the user is not found.</response>
    [HttpPost("confirm-change-email")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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