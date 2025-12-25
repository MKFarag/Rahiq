namespace Application.Feathers.Auth.ResetPassword;

public record ResetPasswordCommand(ResetPasswordRequest Request) : IRequest<Result>;
