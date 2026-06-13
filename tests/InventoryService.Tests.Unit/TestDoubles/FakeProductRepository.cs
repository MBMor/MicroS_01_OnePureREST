using InventoryService.Application.Common.Models;
using InventoryService.Application.Products.Interfaces;
using InventoryService.Application.Products.Requests;
using InventoryService.Domain.Products;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryService.Tests.Unit.TestDoubles;

internal sealed class FakeProductRepository : IProductRepository
{
    private readonly List<Product> _products = [];

    public bool SaveChangesWasCalled { get; private set; }

    public IReadOnlyCollection<Product> Products => _products;

    public void Seed(Product product)
    {
        _products.Add(product);
    }

    public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var product = _products.FirstOrDefault(product => product.Id == id);

        return Task.FromResult(product);
    }

    public Task<Product?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken)
    {
        var product = _products.FirstOrDefault(product => product.Id == id);

        return Task.FromResult(product);
    }

    public Task<PagedResult<Product>> ListAsync(
        ProductListRequest request,
        CancellationToken cancellationToken)
    {
        var result = new PagedResult<Product>(
            _products,
            request.Page,
            request.PageSize,
            _products.Count);

        return Task.FromResult(result);
    }

    public Task<bool> ExistsBySkuAsync(string sku, CancellationToken cancellationToken)
    {
        var exists = _products.Any(product => product.Sku == sku);

        return Task.FromResult(exists);
    }

    public Task AddAsync(Product product, CancellationToken cancellationToken)
    {
        _products.Add(product);

        return Task.CompletedTask;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        SaveChangesWasCalled = true;

        return Task.FromResult(1);
    }
}