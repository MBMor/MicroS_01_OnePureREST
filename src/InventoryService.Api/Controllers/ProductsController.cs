using InventoryService.Application.Common.Models;
using InventoryService.Application.Products.Interfaces;
using InventoryService.Application.Products.Requests;
using InventoryService.Application.Products.Responses;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.Api.Controllers;

[ApiController]
[Route("api/products")]
[Produces("application/json")]
public sealed class ProductsController(IProductService productService) : ControllerBase
{
    /// <summary>
    /// Creates a new product.
    /// </summary>
    /// <param name="request">Product creation request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The newly created product.</returns>
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProductResponse>> Create(
        [FromBody] CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        var product = await productService.CreateAsync(request, cancellationToken);

        return Created($"/api/products/{product.Id}", product);
    }

    /// <summary>
    /// Gets a product by its ID.
    /// </summary>
    /// <param name="id">Product ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The product if it exists.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var product = await productService.GetByIdAsync(id, cancellationToken);

        if (product is null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    /// <summary>
    /// Lists products with optional filtering, pagination, and sorting.
    /// </summary>
    /// <param name="request">Product list query parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paged list of products.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ProductResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResult<ProductResponse>>> List(
        [FromQuery] ProductListRequest request,
        CancellationToken cancellationToken)
    {
        var products = await productService.ListAsync(request, cancellationToken);

        return Ok(products);
    }

    /// <summary>
    /// Updates an existing product.
    /// </summary>
    /// <param name="id">Product ID.</param>
    /// <param name="request">Product update request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated product if it exists.</returns>
    [HttpPut("{id:guid}")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductResponse>> Update(
        Guid id,
        [FromBody] UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        var product = await productService.UpdateAsync(id, request, cancellationToken);

        if (product is null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    /// <summary>
    /// Soft deletes a product by deactivating it.
    /// </summary>
    /// <param name="id">Product ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content if the product was deactivated.</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var wasDeleted = await productService.DeactivateAsync(id, cancellationToken);

        if (!wasDeleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}