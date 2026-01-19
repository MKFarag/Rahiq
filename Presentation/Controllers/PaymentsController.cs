#region Usings

using Application.Feathers.Payments.AddOrderPayment;
using Application.Feathers.Payments.VerifyPayment;
using Presentation.DTOs.Payments;

#endregion

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PaymentsController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpPost("{orderId}")]
    public async Task<IActionResult> Add(
        [FromRoute] int orderId,
        [FromForm] AddPaymentRequest request,
        [FromServices] IValidator<AddPaymentRequest> validator,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return this.ToProblem(validationResult);

        using var image = request.Image.ToFileData();

        var result = await _sender.Send(new AddOrderPaymentCommand(orderId, request.Amount, image), cancellationToken);

        return result.IsSuccess
            ? Created()
            : result.ToProblem();
    }

    [HttpPut("{paymentId}/verify")]
    [Authorize(Roles = DefaultRoles.Admin.Name)]
    public async Task<IActionResult> Verify([FromRoute] int paymentId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new VerifyPaymentCommand(paymentId), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }
}
