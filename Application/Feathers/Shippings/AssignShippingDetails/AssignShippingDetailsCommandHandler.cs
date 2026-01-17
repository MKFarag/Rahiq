namespace Application.Feathers.Shippings.AssignShippingDetails;

public class AssignShippingDetailsCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<AssignShippingDetailsCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(AssignShippingDetailsCommand command, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Shipping.GetAsync([command.Id], cancellationToken) is not { } shipping)
            return Result.Failure(ShippingErrors.NotFound);

        if (await _unitOfWork.Shipping.AnyAsync(s => s.Code == command.Request.Code && s.Id != command.Id, cancellationToken))
            return Result.Failure(ShippingErrors.DuplicatedCode);

        shipping = command.Request.Adapt(shipping);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}

