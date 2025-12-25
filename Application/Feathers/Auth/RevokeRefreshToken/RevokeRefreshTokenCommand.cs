namespace Application.Feathers.Auth.RevokeRefreshToken;

public record RevokeRefreshTokenCommand(string Token, string RefreshToken) : IRequest<Result>;
