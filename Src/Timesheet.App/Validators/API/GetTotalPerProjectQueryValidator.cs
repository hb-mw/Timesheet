using FluentValidation;
using Timesheet.Shared.Contracts;

namespace Timesheet.App.Validators;

public class GetTotalPerProjectQueryValidator : AbstractValidator<GetTotalPerProjectQuery>
{
    public GetTotalPerProjectQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.")
            .GreaterThan(0).WithMessage("User ID must be greater than 0.");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Date is required.");
    }
}