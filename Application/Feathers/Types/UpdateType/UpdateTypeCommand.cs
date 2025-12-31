namespace Application.Feathers.Types.UpdateType;

public record UpdateTypeCommand(int Id, TypeRequest Request) : IRequest<Result>;
