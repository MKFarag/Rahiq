namespace Application.Feathers.Users.GetUserProfile;

public record GetUserProfileQuery(string UserId) : IRequest<Result<UserProfileResponse>>;
