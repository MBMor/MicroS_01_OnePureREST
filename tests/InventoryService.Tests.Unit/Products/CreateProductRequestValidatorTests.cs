using InventoryService.Application.Products.Requests;
using InventoryService.Application.Products.Validators;
using InventoryService.Domain.Products;
using Xunit;

namespace InventoryService.Tests.Unit.Products;

public sealed class CreateProductRequestValidatorTests
{
    private readonly CreateProductRequestValidator _validator = new();

    [Fact]
    public void Validate_WithValidRequest_ReturnsValidResult()
    {
        var request = new CreateProductRequest
        {
            Name = "Mechanical Keyboard",
            Description = "Compact keyboard",
            Sku = "KEYBOARD-001",
            Price = 129.99m,
            QuantityInStock = 25
        };

        var result = _validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithMissingRequiredFields_ReturnsValidationErrors()
    {
        var request = new CreateProductRequest
        {
            Name = "",
            Sku = "",
            Price = 10,
            QuantityInStock = 1
        };

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateProductRequest.Name));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateProductRequest.Sku));
    }

    [Fact]
    public void Validate_WithNegativeNumbers_ReturnsValidationErrors()
    {
        var request = new CreateProductRequest
        {
            Name = "Mechanical Keyboard",
            Sku = "KEYBOARD-001",
            Price = -1,
            QuantityInStock = -5
        };

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateProductRequest.Price));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateProductRequest.QuantityInStock));
    }

    [Fact]
    public void Validate_WithTooLongName_ReturnsValidationError()
    {
        var request = new CreateProductRequest
        {
            Name = new string('A', ProductConstraints.NameMaxLength + 1),
            Sku = "KEYBOARD-001",
            Price = 10,
            QuantityInStock = 1
        };

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateProductRequest.Name));
    }
}