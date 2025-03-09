using BusinessLogicLayer.DTO;
using FluentValidation;

namespace BusinessLogicLayer.Validators;

public class ProductUpdateValidator : AbstractValidator<ProductUpdateRequest>
{
    public ProductUpdateValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty().WithMessage("Product Id can't be blank");
        RuleFor(x => x.ProductName).NotEmpty().WithMessage("Product Name can't be blank");
        RuleFor(x => x.Category).IsInEnum();
        RuleFor(x => x.UnitPrice).InclusiveBetween(0, double.MaxValue).WithMessage($"Unit Price should between 0 to {double.MaxValue}");
        RuleFor(x => x.QuantityInStock).InclusiveBetween(0, int.MaxValue).WithMessage($"Quantity in stock should between 0 to {int.MaxValue}");
    }
}
