using FluentValidation;
using InventoryService.Application.Products.Requests;
using InventoryService.Domain.Products;

namespace InventoryService.Application.Products.Validators;

public sealed class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductRequestValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .MaximumLength(ProductConstraints.NameMaxLength);

        RuleFor(request => request.Description)
            .MaximumLength(ProductConstraints.DescriptionMaxLength)
            .When(request => !string.IsNullOrWhiteSpace(request.Description));

        RuleFor(request => request.Price)
            .GreaterThanOrEqualTo(0);

        RuleFor(request => request.QuantityInStock)
            .GreaterThanOrEqualTo(0);
    }
}