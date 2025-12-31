namespace Application.Feathers.Types.AddType;

public record AddTypeCommand(TypeRequest Request) : IRequest<Result<TypeResponse>>;
