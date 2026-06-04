namespace InventoryService.Application.Common.Exceptions;

public sealed class DuplicateSkuException : Exception
{
    public DuplicateSkuException()
        : base("A product with the same SKU already exists.")
    {
    }

    public DuplicateSkuException(string sku)
        : base($"A product with SKU '{sku}' already exists.")
    {
        Sku = sku;
    }

    public string? Sku { get; }
}