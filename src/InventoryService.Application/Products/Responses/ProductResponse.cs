namespace InventoryService.Application.Products.Responses;

public sealed record ProductResponse(
    Guid Id,
    string Name,
    string? Description,
    string Sku,
    decimal Price,
    int QuantityInStock,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt);