namespace InventoryService.Application.Products.Requests;

public sealed class CreateProductRequest
{
    public required string Name { get; init; }

    public string? Description { get; init; }

    public required string Sku { get; init; }

    public decimal Price { get; init; }

    public int QuantityInStock { get; init; }
}