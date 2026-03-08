#region Usings

using Application.Contracts.Categories;
using Application.Feathers.Category.AddCategory;
using Application.Feathers.Category.DeleteCategory;
using Application.Feathers.Category.GetAllCategories;
using Application.Feathers.Category.GetCategory;
using Application.Feathers.Category.UpdateCategory;

#endregion

namespace Presentation.Controllers;

/// <summary>
/// Manage product categories.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize]
[EnableRateLimiting(RateLimitingOptions.PolicyNames.Concurrency)]
public class CategoriesController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    /// <summary>
    /// Retrieves all categories.
    /// </summary>
    /// <remarks>
    /// Returns a list of all available product categories.
    /// </remarks>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of categories.</returns>
    /// <response code="200">Returns the list of categories.</response>
    [HttpGet("")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        => Ok(await _sender.Send(new GetAllCategoriesQuery(), cancellationToken));

    /// <summary>
    /// Retrieves a specific category by ID.
    /// </summary>
    /// <remarks>
    /// Retrieves the details of a specific category based on its unique identifier.
    /// </remarks>
    /// <param name="id">The unique identifier of the category.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The category details.</returns>
    /// <response code="200">Returns the category details.</response>
    /// <response code="404">If the category is not found.</response>
    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetCategoryQuery(id), cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    /// <summary>
    /// Creates a new category.
    /// </summary>
    /// <remarks>
    /// Adds a new product category to the system.
    /// 
    /// Sample request:
    /// 
    ///     POST /api/Categories
    ///     {
    ///       "name": "Electronics"
    ///     }
    /// 
    /// </remarks>
    /// <param name="request">The category creation request details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The newly created category details.</returns>
    /// <response code="201">Returns the newly created category.</response>
    /// <response code="400">If the request data is invalid.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="403">If the user does not have permission to add categories.</response>
    [HttpPost("")]
    [HasPermission(Permissions.AddCategory)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Add([FromBody] CategoryRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new AddCategoryCommand(request), cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { result.Value.Id }, result.Value)
            : result.ToProblem();
    }

    /// <summary>
    /// Updates an existing category.
    /// </summary>
    /// <remarks>
    /// Updates the details of an existing category by ID.
    /// 
    /// Sample request:
    /// 
    ///     PUT /api/Categories/1
    ///     {
    ///       "name": "Home Appliances"
    ///     }
    /// 
    /// </remarks>
    /// <param name="id">The unique identifier of the category to update.</param>
    /// <param name="request">The updated category data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">If the category was successfully updated.</response>
    /// <response code="400">If the request data is invalid.</response>
    /// <response code="404">If the category is not found.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="403">If the user does not have permission to update categories.</response>
    [HttpPut("{id}")]
    [HasPermission(Permissions.UpdateCategory)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] CategoryRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new UpdateCategoryCommand(id, request), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }

    /// <summary>
    /// Deletes a specific category by ID.
    /// </summary>
    /// <remarks>
    /// Removes a category from the system. Ensure no products are dependent on this category before deletion.
    /// </remarks>
    /// <param name="id">The unique identifier of the category to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">If the category was successfully deleted.</response>
    /// <response code="404">If the category is not found.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="403">If the user does not have permission to delete categories.</response>
    [HttpDelete("{id}")]
    [HasPermission(Permissions.DeleteCategory)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteCategoryCommand(id), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }
}