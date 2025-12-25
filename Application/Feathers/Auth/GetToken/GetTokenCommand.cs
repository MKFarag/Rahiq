namespace Application.Feathers.Auth.GetToken;

public record GetTokenCommand(string Identifier, string Password) : IRequest<Result<AuthResponse>>;
