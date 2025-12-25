namespace Application.Feathers.Auth.GetRefreshToken;

public record GetRefreshTokenCommand(string Token, string RefreshToken) : IRequest<Result<AuthResponse>>;
