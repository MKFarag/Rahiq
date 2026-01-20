namespace Application.Feathers.Users.UpdateUserProfile;

public record UpdateUserProfileCommand(string UserId, UpdateProfileRequest Request) : IRequest<Result>;

