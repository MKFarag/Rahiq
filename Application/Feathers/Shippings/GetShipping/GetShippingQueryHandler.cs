namespace Application.Feathers.Shippings.GetShipping;

public class GetShippingQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetShippingQuery, Result<ShippingResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<ShippingResponse>> Handle(GetShippingQuery request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Shipping.GetAsync([request.Id], cancellationToken) is not { } shipping)
            return Result.Failure<ShippingResponse>(ShippingErrors.NotFound);

        return Result.Success(shipping.Adapt<ShippingResponse>());
    }
}
