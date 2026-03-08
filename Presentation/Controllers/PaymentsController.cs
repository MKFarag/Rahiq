#region Usings

using Application.Feathers.Payments.AddOrderPayment;
using Application.Feathers.Payments.GetAllNotVerifiedPayments;
using Application.Feathers.Payments.VerifyPayment;
using Presentation.DTOs.Payments;

#endregion

namespace Presentation.Controllers;

/// <summary>
/// Manage payments for orders.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize]
[EnableRateLimiting(RateLimitingOptions.PolicyNames.Concurrency)]
public class PaymentsController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    /// <summary>
    /// Retrieves all unverified payments.
    /// </summary>
    /// <remarks>
    /// Returns a list of all payments that have not yet been verified by an admin.
    /// </remarks>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of unverified payments.</returns>
    /// <response code="200">Returns the list of unverified payments.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="403">If the user does not have permission to verify payments.</response>
    [HttpGet("")]
    [HasPermission(Permissions.PaymentVerify)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAllNotVerified(CancellationToken cancellationToken)
        => Ok(await _sender.Send(new GetAllNotVerifiedPaymentsCommand(), cancellationToken));

    /// <summary>
    /// Adds a new payment to an order.
    /// </summary>
    /// <remarks>
    /// Customers can use this endpoint to add a payment receipt to their order.
    /// </remarks>
    /// <param name="orderId">The unique identifier of the order.</param>
    /// <param name="request">The payment request details including amount and receipt image.</param>
    /// <param name="validator">Validator for the request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Created status if successful.</returns>
    /// <response code="201">If the payment was successfully added.</response>
    /// <response code="400">If the request validation fails.</response>
    /// <response code="404">If the order is not found.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="403">If the user is not a customer.</response>
    [HttpPost("{orderId}")]
    [Authorize(Roles = DefaultRoles.Customer.Name)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
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

    /// <summary>
    /// Verifies a payment.
    /// </summary>
    /// <remarks>
    /// Marks a specific payment as verified by an administrator.
    /// </remarks>
    /// <param name="paymentId">The unique identifier of the payment to verify.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">If the payment was successfully verified.</response>
    /// <response code="404">If the payment is not found.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="403">If the user does not have permission to verify payments.</response>
    [HttpPut("{paymentId}/verify")]
    [HasPermission(Permissions.PaymentVerify)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Verify([FromRoute] int paymentId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new VerifyPaymentCommand(paymentId), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }
}