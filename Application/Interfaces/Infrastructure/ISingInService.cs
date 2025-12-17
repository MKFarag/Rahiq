namespace Application.Interfaces.Infrastructure;

public interface ISignInService
{
    Task<Result<User>> PasswordSignInAsync(string identifier, string password, bool isPersistent, bool lockoutOnFailure);
}
