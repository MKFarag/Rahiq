namespace Application.Contracts.Users;

public record UserProfileResponse(
    string FirstName,
    string LastName,
    string Email
);
