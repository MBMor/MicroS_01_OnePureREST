namespace InventoryService.Domain.Products;

public sealed class Product
{
    private Product()
    {
    }

    public Product(
        Guid id,
        string name,
        string? description,
        string sku,
        decimal price,
        int quantityInStock,
        DateTime createdAt)
    {
        EnsureNotEmpty(id, nameof(id));

        Id = id;
        Name = NormalizeRequiredText(name, ProductConstraints.NameMaxLength, nameof(name));
        Description = NormalizeOptionalText(description, ProductConstraints.DescriptionMaxLength, nameof(description));
        Sku = NormalizeRequiredText(sku, ProductConstraints.SkuMaxLength, nameof(sku));
        Price = EnsureNonNegative(price, nameof(price));
        QuantityInStock = EnsureNonNegative(quantityInStock, nameof(quantityInStock));
        IsActive = true;
        CreatedAt = EnsureUtc(createdAt, nameof(createdAt));
        UpdatedAt = CreatedAt;
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public string Sku { get; private set; } = string.Empty;

    public decimal Price { get; private set; }

    public int QuantityInStock { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    public void Update(
    string name,
    string? description,
    decimal price,
    int quantityInStock,
    bool isActive,
    DateTime updatedAt)
    {
        Name = NormalizeRequiredText(name, ProductConstraints.NameMaxLength, nameof(name));
        Description = NormalizeOptionalText(description, ProductConstraints.DescriptionMaxLength, nameof(description));
        Price = EnsureNonNegative(price, nameof(price));
        QuantityInStock = EnsureNonNegative(quantityInStock, nameof(quantityInStock));
        IsActive = isActive;
        UpdatedAt = EnsureUtc(updatedAt, nameof(updatedAt));
    }

    public void Deactivate(DateTime updatedAt)
    {
        if (!IsActive)
        {
            return;
        }

        IsActive = false;
        UpdatedAt = EnsureUtc(updatedAt, nameof(updatedAt));
    }

    private static string NormalizeRequiredText(string value, int maxLength, string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, parameterName);

        var normalizedValue = value.Trim();

        EnsureMaxLength(normalizedValue, maxLength, parameterName);

        return normalizedValue;
    }

    private static string? NormalizeOptionalText(string? value, int maxLength, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var normalizedValue = value.Trim();

        EnsureMaxLength(normalizedValue, maxLength, parameterName);

        return normalizedValue;
    }

    private static decimal EnsureNonNegative(decimal value, string parameterName)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(value, parameterName);

        return value;
    }

    private static int EnsureNonNegative(int value, string parameterName)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(value, parameterName);

        return value;
    }

    private static DateTime EnsureUtc(DateTime value, string parameterName)
    {
        if (value.Kind != DateTimeKind.Utc)
        {
            throw new ArgumentException("DateTime value must be in UTC.", parameterName);
        }

        return value;
    }

    private static void EnsureNotEmpty(Guid value, string parameterName)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("Product ID must not be empty.", parameterName);
        }
    }

    private static void EnsureMaxLength(string value, int maxLength, string parameterName)
    {
        if (value.Length > maxLength)
        {
            throw new ArgumentException($"Value must not exceed {maxLength} characters.", parameterName);
        }
    }
}