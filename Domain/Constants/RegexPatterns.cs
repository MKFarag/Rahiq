namespace Domain.Constants;

public static class RegexPatterns
{
    // Complex password pattern
    public const string Password = "(?=(.*[0-9]))(?=.*[\\!@#$%^&*()\\\\[\\]{}\\-_+=~`|:;\"'<>,./?])(?=.*[a-z])(?=(.*[A-Z]))(?=(.*)).{8,}";

    // Only contain letters, numbers, and underscores
    public const string AlphanumericUnderscorePattern = @"^[a-zA-Z0-9_]+$";

    // Only contain numbers
    public const string OnlyNumbers = @"^[0-9]+$";

    // Only contain letters, numbers, underscores, hyphens, and periods
    public const string SafeFileNamePattern = @"^[A-Za-z0-9_\-\.]*$";
}
