using Application.Feathers.Bundles;

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BundlesController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpGet("")]
    public async Task<IActionResult> GetAll([FromQuery] bool includeNotAvailable, CancellationToken cancellationToken)
        => Ok(await _sender.Send(new GetAllBundlesQuery(includeNotAvailable), cancellationToken));
}
