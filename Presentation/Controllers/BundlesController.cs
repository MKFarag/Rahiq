#region Usings

using Application.Contracts.Bundles;
using Application.Feathers.Bundles.AddBundle;
using Application.Feathers.Bundles.AddBundleImage;
using Application.Feathers.Bundles.DeactivateBundle;
using Application.Feathers.Bundles.DeleteBundleImage;
using Application.Feathers.Bundles.GetAllBundles;
using Application.Feathers.Bundles.GetBundle;
using Application.Feathers.Bundles.ReactivateBundle;
using Application.Feathers.Bundles.UpdateBundle;
using Presentation.DTOs.Bundles;

#endregion

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

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] BundleRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new UpdateBundleCommand(id, request), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }

    [HttpPut("image/{id}")]
    public async Task<IActionResult> AddImage([FromRoute] int id, [FromForm] UploadImageRequest request, [FromServices] IValidator<UploadImageRequest> validator, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return this.ToProblem(validationResult);

        using var image = request.Image.ToFileData();

        var result = await _sender.Send(new AddBundleImageCommand(id, image), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }

    [HttpDelete("image/{id}")]
    public async Task<IActionResult> DeleteImage([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteBundleImageCommand(id), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }

    [HttpPut("{id}/deactivate")]
    public async Task<IActionResult> Deactivate([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeactivateBundleCommand(id), cancellationToken);

        return result.IsSuccess
            ? Ok()
            : result.ToProblem();
    }

    [HttpPut("{id}/reactivate")]
    public async Task<IActionResult> Reactivate([FromRoute] int id, [FromBody] ReactivateBundleRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new ReactivateBundleCommand(id, request.EndAt, request.QuantityAvailable), cancellationToken);

        return result.IsSuccess
            ? Ok()
            : result.ToProblem();
    }
}
