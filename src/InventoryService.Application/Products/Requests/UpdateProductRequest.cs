namespace InventoryService.Application.Products.Requests;

public sealed class UpdateProductRequest
{
    public required string Name { get; init; }

    public string? Description { get; init; }

    public decimal Price { get; init; }

    public int QuantityInStock { get; init; }

    public bool IsActive { get; init; }
}