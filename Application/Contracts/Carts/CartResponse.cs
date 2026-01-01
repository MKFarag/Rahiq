using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Contracts.Carts;

public record CartResponse(
    IEnumerable<CartProductResponse> CartProducts,
    decimal TotalPrice
);
