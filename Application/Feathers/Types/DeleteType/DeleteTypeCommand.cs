namespace Application.Feathers.Types.DeleteType;

public record DeleteTypeCommand(int Id) : IRequest<Result>;
