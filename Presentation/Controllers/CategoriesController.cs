using Application.Feathers.Category.GetAllCategories;

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpGet("")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        => Ok(await _sender.Send(new GetAllCategoriesQuery(), cancellationToken));
}
