namespace Application.Feathers.Types.DeleteType;

public class DeleteTypeCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteTypeCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(DeleteTypeCommand request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Types.GetAsync([request.Id], cancellationToken) is not { } type)
            return Result.Failure(TypeErrors.NotFound);

        if (await _unitOfWork.Products.AnyAsync(x => x.TypeId == request.Id, cancellationToken))
            return Result.Failure(TypeErrors.InUse);

        _unitOfWork.Types.Delete(type);

        await _unitOfWork.CompleteAsync(cancellationToken);
        
        return Result.Success();
    }
}
