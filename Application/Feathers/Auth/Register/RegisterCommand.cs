namespace Application.Feathers.Auth.Register;

public record RegisterCommand(RegisterRequest Request) : IRequest<Result>;
