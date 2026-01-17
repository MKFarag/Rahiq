namespace Application.Feathers.Shippings.GetAllShippings;

public class GetAllShippingsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllShippingsQuery, IEnumerable<ShippingResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<IEnumerable<ShippingResponse>> Handle(GetAllShippingsQuery request, CancellationToken cancellationToken = default)
        => await _unitOfWork.Shipping.GetAllProjectionAsync<ShippingResponse>(cancellationToken);
}

