using FluentValidation;
using InventoryService.Application.Common.Exceptions;
using InventoryService.Application.Common.Interfaces;
using InventoryService.Application.Common.Models;
using InventoryService.Application.Products.Interfaces;
using InventoryService.Application.Products.Requests;
using InventoryService.Application.Products.Responses;
using InventoryService.Domain.Products;

namespace InventoryService.Application.Products.Services;

public sealed class ProductService(
    IProductRepository productRepository,
    IValidator<CreateProductRequest> createProductValidator,
    IValidator<ProductListRequest> productListValidator,
    IValidator<UpdateProductRequest> updateProductValidator,
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

    public async Task<PagedResult<ProductResponse>> ListAsync(
    ProductListRequest request,
    CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var validationResult = await productListValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var products = await productRepository.ListAsync(request, cancellationToken);

        var items = products.Items
            .Select(ToResponse)
            .ToArray();

        return new PagedResult<ProductResponse>(
            items,
            products.Page,
            products.PageSize,
            products.TotalCount);
    }

    public async Task<ProductResponse?> UpdateAsync(
    Guid id,
    UpdateProductRequest request,
    CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var validationResult = await updateProductValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var product = await productRepository.GetByIdForUpdateAsync(id, cancellationToken);

        if (product is null)
        {
            return null;
        }

        product.Update(
            request.Name,
            request.Description,
            request.Price,
            request.QuantityInStock,
            request.IsActive,
            clock.UtcNow);

        await productRepository.SaveChangesAsync(cancellationToken);

        return ToResponse(product);
    }

    public async Task<bool> DeactivateAsync(
    Guid id,
    CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdForUpdateAsync(id, cancellationToken);

        if (product is null)
        {
            return false;
        }

        product.Deactivate(clock.UtcNow);

        await productRepository.SaveChangesAsync(cancellationToken);

        return true;
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