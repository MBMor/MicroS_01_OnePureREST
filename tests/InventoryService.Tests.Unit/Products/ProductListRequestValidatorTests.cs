using InventoryService.Application.Products.Requests;
using InventoryService.Application.Products.Validators;
using Xunit;

namespace InventoryService.Tests.Unit.Products;

public sealed class ProductListRequestValidatorTests
{
    private readonly ProductListRequestValidator _validator = new();

    [Fact]
    public void Validate_WithDefaultRequest_ReturnsValidResult()
    {
        var request = new ProductListRequest();

        var result = _validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithInvalidPage_ReturnsValidationError()
    {
        var request = new ProductListRequest
        {
            Page = 0,
            PageSize = ProductListRequest.DefaultPageSize
        };

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(ProductListRequest.Page));
    }

    [Fact]
    public void Validate_WithTooLargePageSize_ReturnsValidationError()
    {
        var request = new ProductListRequest
        {
            Page = 1,
            PageSize = ProductListRequest.MaxPageSize + 1
        };

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(ProductListRequest.PageSize));
    }
}