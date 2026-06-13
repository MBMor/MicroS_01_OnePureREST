using FluentValidation;
using InventoryService.Application.Common.Exceptions;
using InventoryService.Application.Products.Requests;
using InventoryService.Application.Products.Services;
using InventoryService.Application.Products.Validators;
using InventoryService.Domain.Products;
using InventoryService.Tests.Unit.TestDoubles;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace InventoryService.Tests.Unit.Products;

public sealed class ProductServiceTests
{
    private static readonly DateTime FixedUtcNow =
        new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public async Task CreateAsync_WithValidRequest_AddsProductAndReturnsResponse()
    {
        var repository = new FakeProductRepository();
        var service = CreateService(repository);

        var request = new CreateProductRequest
        {
            Name = "  Mechanical Keyboard  ",
            Description = "  Compact keyboard  ",
            Sku = "  KEYBOARD-001  ",
            Price = 129.99m,
            QuantityInStock = 25
        };

        var response = await service.CreateAsync(request, CancellationToken.None);

        Assert.NotEqual(Guid.Empty, response.Id);
        Assert.Equal("Mechanical Keyboard", response.Name);
        Assert.Equal("Compact keyboard", response.Description);
        Assert.Equal("KEYBOARD-001", response.Sku);
        Assert.Equal(129.99m, response.Price);
        Assert.Equal(25, response.QuantityInStock);
        Assert.True(response.IsActive);
        Assert.Equal(FixedUtcNow, response.CreatedAt);
        Assert.Equal(FixedUtcNow, response.UpdatedAt);

        Assert.Single(repository.Products);
        Assert.True(repository.SaveChangesWasCalled);
    }

    [Fact]
    public async Task CreateAsync_WithInvalidRequest_ThrowsValidationException()
    {
        var repository = new FakeProductRepository();
        var service = CreateService(repository);

        var request = new CreateProductRequest
        {
            Name = "",
            Sku = "",
            Price = -1,
            QuantityInStock = -5
        };

        await Assert.ThrowsAsync<ValidationException>(() =>
            service.CreateAsync(request, CancellationToken.None));
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateSku_ThrowsDuplicateSkuException()
    {
        var repository = new FakeProductRepository();

        repository.Seed(new Product(
            Guid.NewGuid(),
            "Existing Product",
            null,
            "KEYBOARD-001",
            100,
            10,
            FixedUtcNow));

        var service = CreateService(repository);

        var request = new CreateProductRequest
        {
            Name = "Mechanical Keyboard",
            Sku = "KEYBOARD-001",
            Price = 129.99m,
            QuantityInStock = 25
        };

        await Assert.ThrowsAsync<DuplicateSkuException>(() =>
            service.CreateAsync(request, CancellationToken.None));
    }

    private static ProductService CreateService(FakeProductRepository repository)
    {
        return new ProductService(
            repository,
            new CreateProductRequestValidator(),
            new ProductListRequestValidator(),
            new UpdateProductRequestValidator(),
            new FakeClock(FixedUtcNow));
    }
}