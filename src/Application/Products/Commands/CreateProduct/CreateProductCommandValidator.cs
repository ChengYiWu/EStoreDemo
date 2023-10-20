using FluentValidation;

namespace Application.Products.Commands.CreateProduct;

public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .NotNull()
            .MaximumLength(50).WithMessage("Name must not exceed 50 characters.");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required.")
            .NotNull()
            .MaximumLength(200).WithMessage("Content must not exceed 200 characters.");
    }
}
