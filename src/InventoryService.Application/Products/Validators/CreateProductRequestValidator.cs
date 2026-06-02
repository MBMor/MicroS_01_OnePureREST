using FluentValidation;
using InventoryService.Application.Products.Requests;
using InventoryService.Domain.Products;

namespace InventoryService.Application.Products.Validators;

public sealed class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .MaximumLength(ProductConstraints.NameMaxLength);

        RuleFor(request => request.Description)
            .MaximumLength(ProductConstraints.DescriptionMaxLength)
            .When(request => !string.IsNullOrWhiteSpace(request.Description));

        RuleFor(request => request.Sku)
            .NotEmpty()
            .MaximumLength(ProductConstraints.SkuMaxLength);

        RuleFor(request => request.Price)
            .GreaterThanOrEqualTo(0);

        RuleFor(request => request.QuantityInStock)
            .GreaterThanOrEqualTo(0);
    }
}