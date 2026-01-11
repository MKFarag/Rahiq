using Application.Contracts.Bundles;
using Application.Feathers.Bundles.AddBundle;
using Application.Feathers.Bundles.GetAllBundles;
using Application.Feathers.Bundles.GetBundle;
using Presentation.DTOs.Bundles;

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BundlesController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpGet("")]
    public async Task<IActionResult> GetAll([FromQuery] bool includeNotAvailable, CancellationToken cancellationToken)
        => Ok(await _sender.Send(new GetAllBundlesQuery(includeNotAvailable), cancellationToken));

    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetBundleQuery(id), cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    [HttpPost("")]
    public async Task<IActionResult> Add([FromForm] BundleWithImageRequest request, [FromServices] IValidator<BundleWithImageRequest> validator, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return this.ToProblem(validationResult);

        using var image = request.Image?.ToFileData(); 

        var result = await _sender.Send(new AddBundleCommand(request.Bundle, image), cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(Get), new { result.Value.Bundle.Id }, result.Value)
            : result.ToProblem();
    }
}
