using Application.Common.Extensions;
using Application.Common.Models.Commands;
using FluentValidation;

namespace Application.Products.Commands.CreateProduct;

public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .NotNull()
            .MaximumLength(128);

        RuleFor(x => x.Description)
            .NotEmpty()
            .NotNull();

        RuleFor(x => x.Brand)
            .NullOrNotWhitespace()
            .MaximumLength(64);

        RuleFor(x => x.Weight)
            .NullOrNotWhitespace()
            .MaximumLength(32);

        RuleFor(x => x.Dimensions)
            .NullOrNotWhitespace()
            .MaximumLength(32);

        RuleFor(x => x.NewImages)
            .NotEmpty()
            .NotNull()
            .Must(x => x.Count <= 5).WithMessage("最多上傳 5 張圖片。")
            .ForEach(x =>
            {
                x.NotNull();
                x.SetValidator(new UploadedFileDTOValidator());
            });

        RuleFor(x => x.ProductItems)
            .NotEmpty()
            .NotNull()
            .Must(x => x.Count <= 10).WithMessage("最多新增 10 個商品項目。")
            .ForEach(x =>
            {
                x.NotNull();
                x.ChildRules(y =>
                {
                    y.RuleFor(z => z.Name)
                        .NotEmpty()
                        .NotNull()
                        .MaximumLength(64);

                    y.RuleFor(z => z.Price)
                        .NotEmpty()
                        .NotNull()
                        .GreaterThanOrEqualTo(1);

                    y.RuleFor(z => z.StockQuantity)
                        .NotEmpty()
                        .NotNull()
                        .GreaterThanOrEqualTo(0);

                    y.RuleFor(z => z.IsActive)
                        .NotEmpty()
                        .NotNull()
                        .NullOrBoolean();

                    y.When(z => z.NewImage is not null, () =>
                    {
                        y.RuleFor(z => z.NewImage!)
                            .SetValidator(new UploadedFileDTOValidator());
                    });
                });
            });
    }
}
