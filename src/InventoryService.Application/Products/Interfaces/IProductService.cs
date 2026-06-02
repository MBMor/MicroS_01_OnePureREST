using InventoryService.Application.Common.Models;
using InventoryService.Application.Products.Requests;
using InventoryService.Application.Products.Responses;

namespace InventoryService.Application.Products.Interfaces;

public interface IProductService
{
    Task<ProductResponse> CreateAsync(
        CreateProductRequest request,
        CancellationToken cancellationToken);

    Task<ProductResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<PagedResult<ProductResponse>> ListAsync(
        ProductListRequest request,
        CancellationToken cancellationToken);

    Task<ProductResponse?> UpdateAsync(
        Guid id,
        UpdateProductRequest request,
        CancellationToken cancellationToken);

    Task<bool> DeactivateAsync(
        Guid id,
        CancellationToken cancellationToken);
}