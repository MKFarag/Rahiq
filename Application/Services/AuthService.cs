//using Microsoft.AspNetCore.WebUtilities;
//using System.Security.Cryptography;

//namespace Application.Services;

//public class AuthService(IEmailTemplateService emailTemplateService, ISignInService signInService,
//    IJwtProvider jwtProvider, IUnitOfWork unitOfWork) : IAuthService
//{
//    private readonly IEmailTemplateService _emailTemplateService = emailTemplateService;
//    private readonly ISignInService _signInService = signInService;
//    private readonly IJwtProvider _jwtProvider = jwtProvider;
//    private readonly IUnitOfWork _unitOfWork = unitOfWork;

//    private readonly int _refreshTokenExpiryDays = 14;

//    #region Login

//    public async Task<Result<AuthResponse>> GetTokenAsync(string identifier, string password, CancellationToken cancellationToken = default)
//    {
//        var result = await _signInService.PasswordSignInAsync(identifier, password, false, true);

//        if (result.IsFailure)
//            return Result.Failure<AuthResponse>(result.Error);

//        var user = result.Value;

//        if (user.IsDisabled)
//            return Result.Failure<AuthResponse>(UserErrors.DisabledUser);

//        var (userRoles, userPermissions) = await _unitOfWork.Users.GetRolesAndPermissionsAsync(user, cancellationToken);

//        var (token, expiresIn) = _jwtProvider.GenerateToken(user, userRoles, userPermissions);

//        var refreshToken = GenerateRefreshToken();
//        var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

//        await _unitOfWork.Users.AddRefreshTokenAsync(user, new RefreshToken
//        {
//            Token = refreshToken,
//            ExpiresOn = refreshTokenExpiration
//        }, cancellationToken);

//        var response = new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, token, expiresIn, refreshToken, refreshTokenExpiration);

//        return Result.Success(response);
//    }

//    public async Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
//    {
//        if (_jwtProvider.ValidateToken(token) is not { } userId)
//            return Result.Failure<AuthResponse>(UserErrors.InvalidJwtToken);

//        if (await _unitOfWork.Users.FindByIdAsync(userId, cancellationToken) is not { } user)
//            return Result.Failure<AuthResponse>(UserErrors.InvalidJwtToken);

//        if (user.IsDisabled)
//            return Result.Failure<AuthResponse>(UserErrors.DisabledUser);

//        if (await _unitOfWork.Users.IsLockedOutAsync(user))
//            return Result.Failure<AuthResponse>(UserErrors.LockedUser);

//        var revokeResult = await _unitOfWork.Users.RevokeRefreshTokenAsync(user, refreshToken, cancellationToken);

//        if (revokeResult.IsFailure)
//            return Result.Failure<AuthResponse>(revokeResult.Error);

//        var (userRoles, userPermissions) = await _unitOfWork.Users.GetRolesAndPermissionsAsync(user, cancellationToken);

//        var (newToken, expiresIn) = _jwtProvider.GenerateToken(user, userRoles, userPermissions);

//        var newRefreshToken = GenerateRefreshToken();
//        var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

//        await _unitOfWork.Users.AddRefreshTokenAsync(user, new RefreshToken
//        {
//            Token = newRefreshToken,
//            ExpiresOn = refreshTokenExpiration
//        }, cancellationToken);

//        var response = new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, newToken, expiresIn, newRefreshToken, refreshTokenExpiration);

//        return Result.Success(response);
//    }

//    public async Task<Result> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
//    {
//        if (_jwtProvider.ValidateToken(token) is not { } userId)
//            return Result.Failure(UserErrors.InvalidJwtToken);

//        if (await _unitOfWork.Users.FindByIdAsync(userId, cancellationToken) is not { } user)
//            return Result.Failure(UserErrors.InvalidJwtToken);

//        var revokeResult = await _unitOfWork.Users.RevokeRefreshTokenAsync(user, refreshToken, cancellationToken);

//        if (revokeResult.IsFailure)
//            return Result.Failure<AuthResponse>(revokeResult.Error);

//        return Result.Success();
//    }

//    #endregion

//    #region Register

//    public async Task<Result> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
//    {
//        if (await _unitOfWork.Users.EmailExistsAsync(request.Email, cancellationToken))
//            return Result.Failure(UserErrors.DuplicatedEmail);

//        var user = request.Adapt<User>();

//        var result = await _unitOfWork.Users.CreateAsync(user, request.Password);

//        if (result.IsFailure)
//            return Result.Failure(result.Error);

//        await SendEmailConfirmationCodeAsync(user);

//        return Result.Success();
//    }

//    public async Task<Result> ConfirmEmailAsync(string userId, string code)
//    {
//        if (await _unitOfWork.Users.FindByIdAsync(userId) is not { } user)
//            return Result.Failure(UserErrors.InvalidCode);

//        if (await _unitOfWork.Users.IsEmailConfirmedAsync(user))
//            return Result.Failure(UserErrors.DuplicatedConfirmation);

//        try
//        {
//            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
//        }
//        catch (FormatException)
//        {
//            return Result.Failure(UserErrors.InvalidCode);
//        }
//        var result = await _unitOfWork.Users.ConfirmEmailWithTokenAsync(user, code);

//        if (result.IsFailure)
//            return Result.Failure(result.Error);

//        await _unitOfWork.Users.AddToRoleAsync(user, DefaultRoles.Customer.Name);

//        return Result.Success();
//    }

//    public async Task<Result> ResendConfirmationEmailAsync(string email)
//    {
//        if (await _unitOfWork.Users.FindByEmailAsync(email) is not { } user)
//            return Result.Success();

//        if (await _unitOfWork.Users.IsEmailConfirmedAsync(user))
//            return Result.Failure(UserErrors.DuplicatedConfirmation);

//        await SendEmailConfirmationCodeAsync(user);

//        return Result.Success();
//    }

//    #endregion

//    #region Forget Password

//    public async Task<Result> SendResetPasswordCodeAsync(string email)
//    {
//        if (await _unitOfWork.Users.FindByEmailAsync(email) is not { } user)
//            return Result.Success();

//        if (!await _unitOfWork.Users.IsEmailConfirmedAsync(user))
//            return Result.Failure(UserErrors.EmailNotConfirmed with { StatusCode = StatusCodes.Status400BadRequest });

//        var code = await _unitOfWork.Users.GeneratePasswordResetTokenAsync(user);
//        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

//        _emailTemplateService.SendResetPassword(user, code);

//        return Result.Success();

//    }

//    public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request)
//    {
//        var user = await _unitOfWork.Users.FindByEmailAsync(request.Email);

//        if (user is null || !await _unitOfWork.Users.IsEmailConfirmedAsync(user))
//            return Result.Failure(UserErrors.InvalidCode);

//        Result result;

//        try
//        {
//            var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));
//            result = await _unitOfWork.Users.ResetPasswordAsync(user, code, request.NewPassword);
//        }
//        catch (FormatException)
//        {
//            result = Result.Failure(UserErrors.InvalidToken);
//        }

//        if (result.IsFailure)
//            return Result.Failure(result.Error);

//        return Result.Success();
//    }

//    #endregion

//    #region PrivatesMethods

//    private static string GenerateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

//    private async Task SendEmailConfirmationCodeAsync(User user)
//    {
//        var code = await _unitOfWork.Users.GenerateEmailConfirmationTokenAsync(user);
//        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
//        _emailTemplateService.SendConfirmationLink(user, code);
//    }

//    #endregion
//}
