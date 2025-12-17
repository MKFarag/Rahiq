namespace Domain.Constants;

internal static class StatusCodes
{
    // 2xx Success
    public const int Ok = 200;
    public const int Created = 201;
    public const int Accepted = 202;
    public const int NoContent = 204;

    // 3xx Redirection
    public const int MovedPermanently = 301;
    public const int Found = 302;
    public const int SeeOther = 303;
    public const int NotModified = 304;
    public const int TemporaryRedirect = 307;
    public const int PermanentRedirect = 308;

    // 4xx Client Error
    public const int BadRequest = 400;
    public const int Unauthorized = 401;
    public const int PaymentRequired = 402;
    public const int Forbidden = 403;
    public const int NotFound = 404;
    public const int MethodNotAllowed = 405;
    public const int NotAcceptable = 406;
    public const int RequestTimeout = 408;
    public const int Conflict = 409;
    public const int Gone = 410;
    public const int UnprocessableEntity = 422;
    public const int TooManyRequests = 429;

    // 5xx Server Error
    public const int InternalServerError = 500;
    public const int NotImplemented = 501;
    public const int BadGateway = 502;
    public const int ServiceUnavailable = 503;
    public const int GatewayTimeout = 504;
}
