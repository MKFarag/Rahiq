namespace Application.Feathers.Shippings.AddCustomerShipping;

public class AddCustomerShippingCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<AddCustomerShippingCommand, Result<ShippingResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<ShippingResponse>> Handle(AddCustomerShippingCommand command, CancellationToken cancellationToken = default)
    {
        var shipping = command.Request.Adapt<Shipping>();

        await _unitOfWork.Shipping.AddAsync(shipping, cancellationToken);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(shipping.Adapt<ShippingResponse>());
    }
}

