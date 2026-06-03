using FluentValidation;
using InventoryService.Application.Common.Exceptions;
using InventoryService.Application.Common.Interfaces;
using InventoryService.Application.Products.Interfaces;
using InventoryService.Application.Products.Requests;
using InventoryService.Application.Products.Responses;
using InventoryService.Domain.Products;

namespace InventoryService.Application.Products.Services;

public sealed class ProductService(
    IProductRepository productRepository,
    IValidator<CreateProductRequest> createProductValidator,
    IClock clock) : IProductService
{
    public async Task<ProductResponse> CreateAsync(
        CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var validationResult = await createProductValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var normalizedSku = request.Sku.Trim();

        if (await productRepository.ExistsBySkuAsync(normalizedSku, cancellationToken))
        {
            throw new DuplicateSkuException(normalizedSku);
        }

        var product = new Product(
            Guid.NewGuid(),
            request.Name,
            request.Description,
            normalizedSku,
            request.Price,
            request.QuantityInStock,
            clock.UtcNow);

        await productRepository.AddAsync(product, cancellationToken);
        await productRepository.SaveChangesAsync(cancellationToken);

        return ToResponse(product);
    }

    public async Task<ProductResponse?> GetByIdAsync(
    Guid id,
    CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(id, cancellationToken);

        return product is null
            ? null
            : ToResponse(product);
    }

    private static ProductResponse ToResponse(Product product)
    {
        return new ProductResponse(
            product.Id,
            product.Name,
            product.Description,
            product.Sku,
            product.Price,
            product.QuantityInStock,
            product.IsActive,
            product.CreatedAt,
            product.UpdatedAt);
    }
}