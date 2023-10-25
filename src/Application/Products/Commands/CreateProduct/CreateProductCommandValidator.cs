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
            .Must(y => y == null || !string.IsNullOrWhiteSpace(y)).WithMessage("不可輸入空字串。")
            .MaximumLength(64);

        RuleFor(x => x.Weight)
            .Must(y => y == null || !string.IsNullOrWhiteSpace(y)).WithMessage("不可輸入空字串。")
            .MaximumLength(32);

        RuleFor(x => x.Dimensions)
            .Must(y => y == null || !string.IsNullOrWhiteSpace(y)).WithMessage("不可輸入空字串。")
            .MaximumLength(32);

        RuleFor(x => x.Images)
            .NotEmpty()
            .NotNull()
            .Must(x => x.Count <= 5).WithMessage("最多上傳 5 張圖片。")
            .ForEach(x =>
            {
                x.ChildRules(y =>
                {
                    y.RuleFor(z => z.TmpFileName)
                        .NotEmpty()
                        .NotNull();

                    y.RuleFor(z => z.OriFileName)
                        .NotEmpty()
                        .NotNull();
                });
            });

        RuleFor(x => x.ProductItems)
            .NotEmpty()
            .NotNull()
            .Must(x => x.Count <= 10).WithMessage("最多新增 10 個商品項目。")
            .ForEach(x =>
            {
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
                        .Must(a => a == false || a == true).WithMessage("請傳入 true 或 false");

                    y.RuleFor(z => z.Image)
                        .ChildRules(z =>
                        {
                            z.RuleFor(a => a.TmpFileName)
                                .NotEmpty()
                                .NotNull();

                            z.RuleFor(a => a.OriFileName)
                                .NotEmpty()
                                .NotNull();
                        });
                });
            });
    }
}
