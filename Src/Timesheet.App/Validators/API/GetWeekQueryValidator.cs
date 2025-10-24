using FluentValidation;
using Timesheet.Shared.Contracts;

namespace Timesheet.App.Validators;

public class GetWeekQueryValidator : AbstractValidator<GetWeekQuery>
{
    public GetWeekQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.")
            .GreaterThan(0).WithMessage("User ID must be greater than 0.");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Date is required.");
    }
}