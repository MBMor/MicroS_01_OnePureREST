using InventoryService.Application.Common.Models;
using InventoryService.Application.Products.Interfaces;
using InventoryService.Application.Products.Models;
using InventoryService.Application.Products.Requests;
using InventoryService.Domain.Products;
using InventoryService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Infrastructure.Repositories;

public sealed class ProductRepository(InventoryDbContext dbContext) : IProductRepository
{
    public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(product => product.Id == id, cancellationToken);
    }

    public Task<Product?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.Products
            .FirstOrDefaultAsync(product => product.Id == id, cancellationToken);
    }

    public async Task<PagedResult<Product>> ListAsync(
        ProductListRequest request,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Products
            .AsNoTracking()
            .AsQueryable();

        query = ApplyFilters(query, request);

        var totalCount = await query.CountAsync(cancellationToken);

        var products = await ApplySorting(query, request)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToArrayAsync(cancellationToken);

        return new PagedResult<Product>(
            products,
            request.Page,
            request.PageSize,
            totalCount);
    }
    public Task<bool> ExistsBySkuAsync(string sku, CancellationToken cancellationToken)
    {
        return dbContext.Products.AnyAsync(
            product => product.Sku == sku,
            cancellationToken);
    }

    public async Task AddAsync(Product product, CancellationToken cancellationToken)
    {
        await dbContext.Products.AddAsync(product, cancellationToken);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }

    private static IQueryable<Product> ApplyFilters(
    IQueryable<Product> query,
    ProductListRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            var name = request.Name.Trim().ToLower();

            query = query.Where(product =>
                product.Name.ToLower().Contains(name));
        }

        if (!string.IsNullOrWhiteSpace(request.Sku))
        {
            var sku = request.Sku.Trim().ToLower();

            query = query.Where(product =>
                product.Sku.ToLower().Contains(sku));
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(product =>
                product.IsActive == request.IsActive.Value);
        }

        return query;
    }

    private static IQueryable<Product> ApplySorting(
        IQueryable<Product> query,
        ProductListRequest request)
    {
        return (request.SortBy, request.SortDirection) switch
        {
            (ProductSortBy.Name, SortDirection.Asc) =>
                query.OrderBy(product => product.Name),

            (ProductSortBy.Name, SortDirection.Desc) =>
                query.OrderByDescending(product => product.Name),

            (ProductSortBy.Price, SortDirection.Asc) =>
                query.OrderBy(product => product.Price),

            (ProductSortBy.Price, SortDirection.Desc) =>
                query.OrderByDescending(product => product.Price),

            (ProductSortBy.QuantityInStock, SortDirection.Asc) =>
                query.OrderBy(product => product.QuantityInStock),

            (ProductSortBy.QuantityInStock, SortDirection.Desc) =>
                query.OrderByDescending(product => product.QuantityInStock),

            (ProductSortBy.CreatedAt, SortDirection.Asc) =>
                query.OrderBy(product => product.CreatedAt),

            _ =>
                query.OrderByDescending(product => product.CreatedAt)
        };
    }
}