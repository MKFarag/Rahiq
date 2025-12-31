namespace Application.Feathers.Types.GetType;

public record GetTypeQuery(int Id) : IRequest<Result<TypeResponse>>;
