using FluentValidation;
using InventoryService.Application.Products.Requests;
using InventoryService.Domain.Products;

namespace InventoryService.Application.Products.Validators;

public sealed class ProductListRequestValidator : AbstractValidator<ProductListRequest>
{
    public ProductListRequestValidator()
    {
        RuleFor(request => request.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(request => request.PageSize)
            .InclusiveBetween(1, ProductListRequest.MaxPageSize);

        RuleFor(request => request.Name)
            .MaximumLength(ProductConstraints.NameMaxLength)
            .When(request => !string.IsNullOrWhiteSpace(request.Name));

        RuleFor(request => request.Sku)
            .MaximumLength(ProductConstraints.SkuMaxLength)
            .When(request => !string.IsNullOrWhiteSpace(request.Sku));

        RuleFor(request => request.SortBy)
            .IsInEnum();

        RuleFor(request => request.SortDirection)
            .IsInEnum();
    }
}