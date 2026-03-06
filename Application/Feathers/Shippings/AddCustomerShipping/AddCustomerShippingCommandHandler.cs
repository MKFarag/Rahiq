namespace Application.Feathers.Shippings.AddCustomerShipping;

public class AddCustomerShippingCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<AddCustomerShippingCommand, Result<ShippingResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<ShippingResponse>> Handle(AddCustomerShippingCommand command, CancellationToken cancellationToken = default)
    {
        if (!await _unitOfWork.Orders.AnyAsync(x => x.Id == command.Request.OrderId, cancellationToken))
            return Result.Failure<ShippingResponse>(OrderErrors.NotFound);

        var shipping = command.Request.Adapt<Shipping>();

        await _unitOfWork.Shipping.AddAsync(shipping, cancellationToken);

        await _unitOfWork.CompleteAsync(cancellationToken);

        await _unitOfWork.Orders.ExecuteUpdateAsync(x => x.Id == command.Request.OrderId, nameof(Order.ShippingId), shipping.Id, cancellationToken);

        return Result.Success(shipping.Adapt<ShippingResponse>());
    }
}

