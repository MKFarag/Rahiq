namespace Application.Feathers.Shippings.DeleteShipping;

public class DeleteShippingCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteShippingCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(DeleteShippingCommand command, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Shipping.GetAsync([command.Id], cancellationToken) is not { } shipping)
            return Result.Failure(ShippingErrors.NotFound);

        if (await _unitOfWork.Orders.AnyAsync(o => o.ShippingId == command.Id, cancellationToken))
            return Result.Failure(ShippingErrors.InUse);

        _unitOfWork.Shipping.Delete(shipping);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}

