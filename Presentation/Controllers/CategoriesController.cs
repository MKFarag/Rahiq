using Application.Contracts.Categories;
using Application.Feathers.Category.AddCategory;
using Application.Feathers.Category.DeleteCategory;
using Application.Feathers.Category.GetAllCategories;
using Application.Feathers.Category.GetCategory;
using Application.Feathers.Category.UpdateCategory;

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpGet("")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        => Ok(await _sender.Send(new GetAllCategoriesQuery(), cancellationToken));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetCategoryQuery(id), cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    [HttpPost("")]
    public async Task<IActionResult> Add([FromBody] CategoryRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new AddCategoryCommand(request), cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { result.Value.Id }, result.Value)
            : result.ToProblem();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] CategoryRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new UpdateCategoryCommand(id, request), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Update([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteCategoryCommand(id), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }
}
