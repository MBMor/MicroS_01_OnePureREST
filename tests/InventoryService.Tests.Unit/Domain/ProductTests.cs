using InventoryService.Domain.Products;
using System;
using Xunit;

namespace InventoryService.Tests.Unit.Domain;

public sealed class ProductTests
{
    private static readonly DateTime CreatedAt =
        new(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Constructor_WithValidValues_CreatesActiveProduct()
    {
        var product = new Product(
            Guid.NewGuid(),
            "  Mechanical Keyboard  ",
            "  Compact keyboard  ",
            "  KEYBOARD-001  ",
            129.99m,
            25,
            CreatedAt);

        Assert.Equal("Mechanical Keyboard", product.Name);
        Assert.Equal("Compact keyboard", product.Description);
        Assert.Equal("KEYBOARD-001", product.Sku);
        Assert.Equal(129.99m, product.Price);
        Assert.Equal(25, product.QuantityInStock);
        Assert.True(product.IsActive);
        Assert.Equal(CreatedAt, product.CreatedAt);
        Assert.Equal(CreatedAt, product.UpdatedAt);
    }

    [Fact]
    public void Constructor_WithLocalDateTime_ThrowsArgumentException()
    {
        var localDateTime = new DateTime(2026, 1, 1, 10, 0, 0, DateTimeKind.Local);

        var exception = Assert.Throws<ArgumentException>(() =>
            new Product(
                Guid.NewGuid(),
                "Mechanical Keyboard",
                null,
                "KEYBOARD-001",
                129.99m,
                25,
                localDateTime));

        Assert.Equal("createdAt", exception.ParamName);
    }

    [Fact]
    public void Update_WithValidValues_UpdatesEditableFields()
    {
        var product = CreateProduct();
        var updatedAt = CreatedAt.AddHours(1);

        product.Update(
            "Mechanical Keyboard Pro",
            "Updated keyboard",
            149.99m,
            20,
            false,
            updatedAt);

        Assert.Equal("Mechanical Keyboard Pro", product.Name);
        Assert.Equal("Updated keyboard", product.Description);
        Assert.Equal("KEYBOARD-001", product.Sku);
        Assert.Equal(149.99m, product.Price);
        Assert.Equal(20, product.QuantityInStock);
        Assert.False(product.IsActive);
        Assert.Equal(updatedAt, product.UpdatedAt);
    }

    [Fact]
    public void Deactivate_WhenProductIsActive_SetsIsActiveToFalse()
    {
        var product = CreateProduct();
        var updatedAt = CreatedAt.AddHours(1);

        product.Deactivate(updatedAt);

        Assert.False(product.IsActive);
        Assert.Equal(updatedAt, product.UpdatedAt);
    }

    private static Product CreateProduct()
    {
        return new Product(
            Guid.NewGuid(),
            "Mechanical Keyboard",
            null,
            "KEYBOARD-001",
            129.99m,
            25,
            CreatedAt);
    }
}