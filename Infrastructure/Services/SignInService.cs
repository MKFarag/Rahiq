//using Application.Interfaces;

//namespace Infrastructure.Services;

//public class SignInService(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager) : ISignInService
//{
//    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
//    private readonly UserManager<ApplicationUser> _userManager = userManager;

//    public async Task<Result<User>> PasswordSignInAsync(string identifier, string password, bool isPersistent, bool lockoutOnFailure)
//    {
//        if (string.IsNullOrWhiteSpace(identifier) || string.IsNullOrWhiteSpace(password))
//            return Result.Failure<User>(UserErrors.InvalidCredentials);

//        var user = await GetUserByIdentifierAsync(identifier);

//        if (user is null)
//            return Result.Failure<User>(UserErrors.InvalidCredentials);

//        var result = await _signInManager.PasswordSignInAsync(user, password, isPersistent, lockoutOnFailure);

//        return result.Succeeded
//            ? Result.Success(user.Adapt<User>())
//            : Result.Failure<User>(MapSignInError(result));
//    }

//    private async Task<ApplicationUser?> GetUserByIdentifierAsync(string identifier)
//        => IsEmail(identifier)
//            ? await _userManager.FindByEmailAsync(identifier)
//            : await _userManager.FindByNameAsync(identifier);

//    private static bool IsEmail(string identifier)
//    {
//        try
//        {
//            var mailAddress = new System.Net.Mail.MailAddress(identifier);
//            return mailAddress.Address == identifier;
//        }
//        catch
//        {
//            return false;
//        }
//    }

//    private static Error MapSignInError(SignInResult result)
//        => result.IsNotAllowed ? UserErrors.EmailNotConfirmed
//             : result.IsLockedOut ? UserErrors.LockedUser
//             : UserErrors.InvalidCredentials;
//}

