using System.Security.Cryptography;

namespace Application.Helpers;

internal static class RefreshTokenHandler
{
    private const int _refreshTokenExpiryDays = 14;

    internal static string GenerateNewToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

    internal static int ExpiryDays => _refreshTokenExpiryDays;
}
