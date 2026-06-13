using InventoryService.Application.Common.Models;
using InventoryService.Application.Products.Requests;
using InventoryService.Application.Products.Responses;
using InventoryService.Tests.Integration.Support;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace InventoryService.Tests.Integration.Products;

public sealed class ProductEndpointsTests(
    InventoryServiceWebApplicationFactory factory)
    : IClassFixture<InventoryServiceWebApplicationFactory>, IAsyncLifetime
{
    private readonly HttpClient _client = factory.CreateClient();

    public async ValueTask InitializeAsync()
    {
        await factory.ResetDatabaseAsync();
    }

    public ValueTask DisposeAsync()
    {
        _client.Dispose();

        return ValueTask.CompletedTask;
    }

    [Fact]
    public async Task CreateProduct_WithValidRequest_ReturnsCreatedProduct()
    {
        var request = new CreateProductRequest
        {
            Name = "Mechanical Keyboard",
            Description = "Compact keyboard with hot-swappable switches",
            Sku = "KEYBOARD-001",
            Price = 129.99m,
            QuantityInStock = 25
        };

        using var response = await _client.PostAsJsonAsync("/api/products", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var product = await response.Content.ReadFromJsonAsync<ProductResponse>();

        Assert.NotNull(product);
        Assert.NotEqual(Guid.Empty, product.Id);
        Assert.Equal(request.Name, product.Name);
        Assert.Equal(request.Description, product.Description);
        Assert.Equal(request.Sku, product.Sku);
        Assert.Equal(request.Price, product.Price);
        Assert.Equal(request.QuantityInStock, product.QuantityInStock);
        Assert.True(product.IsActive);
    }

    [Fact]
    public async Task CreateProduct_WithInvalidRequest_ReturnsValidationProblemDetails()
    {
        var request = new
        {
            Name = "",
            Sku = "",
            Price = -1,
            QuantityInStock = -5
        };

        using var response = await _client.PostAsJsonAsync("/api/products", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.NotNull(problemDetails);
        Assert.Equal(StatusCodes.Status400BadRequest, problemDetails.Status);
        Assert.NotEmpty(problemDetails.Errors);
    }

    [Fact]
    public async Task CreateProduct_WithDuplicateSku_ReturnsConflict()
    {
        var firstRequest = new CreateProductRequest
        {
            Name = "Mechanical Keyboard",
            Description = "Compact keyboard",
            Sku = "KEYBOARD-001",
            Price = 129.99m,
            QuantityInStock = 25
        };

        var secondRequest = new CreateProductRequest
        {
            Name = "Another Keyboard",
            Description = "Duplicate SKU test",
            Sku = "KEYBOARD-001",
            Price = 99.99m,
            QuantityInStock = 10
        };

        using var firstResponse = await _client.PostAsJsonAsync("/api/products", firstRequest);
        using var secondResponse = await _client.PostAsJsonAsync("/api/products", secondRequest);

        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, secondResponse.StatusCode);

        var problemDetails = await secondResponse.Content.ReadFromJsonAsync<ProblemDetails>();

        Assert.NotNull(problemDetails);
        Assert.Equal(StatusCodes.Status409Conflict, problemDetails.Status);
    }

    [Fact]
    public async Task ListProducts_AfterProductWasCreated_ReturnsPagedResult()
    {
        var request = new CreateProductRequest
        {
            Name = "Wireless Mouse",
            Description = "Ergonomic wireless mouse",
            Sku = "MOUSE-001",
            Price = 49.99m,
            QuantityInStock = 50
        };

        using var createResponse = await _client.PostAsJsonAsync("/api/products", request);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        using var listResponse = await _client.GetAsync("/api/products?page=1&pageSize=10");

        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);

        var result = await listResponse.Content.ReadFromJsonAsync<PagedResult<ProductResponse>>();

        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal(1, result.Page);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(1, result.TotalCount);
        Assert.Equal("Wireless Mouse", result.Items.Single().Name);
    }

    [Fact]
    public async Task DeleteProduct_WhenProductExists_SoftDeletesProduct()
    {
        var request = new CreateProductRequest
        {
            Name = "USB-C Dock",
            Description = "Docking station",
            Sku = "DOCK-001",
            Price = 89.99m,
            QuantityInStock = 15
        };

        using var createResponse = await _client.PostAsJsonAsync("/api/products", request);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createdProduct = await createResponse.Content.ReadFromJsonAsync<ProductResponse>();

        Assert.NotNull(createdProduct);

        using var deleteResponse = await _client.DeleteAsync($"/api/products/{createdProduct.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        using var getResponse = await _client.GetAsync($"/api/products/{createdProduct.Id}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var deletedProduct = await getResponse.Content.ReadFromJsonAsync<ProductResponse>();

        Assert.NotNull(deletedProduct);
        Assert.False(deletedProduct.IsActive);
    }
}