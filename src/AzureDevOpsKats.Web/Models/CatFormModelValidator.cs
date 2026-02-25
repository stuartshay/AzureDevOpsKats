using FluentValidation;

namespace AzureDevOpsKats.Web.Models;

public class CatFormModelValidator : AbstractValidator<CatFormModel>
{
    public CatFormModelValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must be 200 characters or fewer.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must be 1,000 characters or fewer.");
    }
}
