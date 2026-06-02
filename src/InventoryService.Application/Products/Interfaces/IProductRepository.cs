using InventoryService.Application.Common.Models;
using InventoryService.Application.Products.Requests;
using InventoryService.Domain.Products;

namespace InventoryService.Application.Products.Interfaces;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<bool> ExistsBySkuAsync(string sku, CancellationToken cancellationToken);

    Task<PagedResult<Product>> ListAsync(ProductListRequest request, CancellationToken cancellationToken);

    Task AddAsync(Product product, CancellationToken cancellationToken);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}