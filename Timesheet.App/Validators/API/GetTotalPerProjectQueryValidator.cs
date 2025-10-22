using FluentValidation;
using Timesheet.App.Models.Requests.Queries;

namespace Timesheet.App.Validators;

public class GetTotalPerProjectQueryValidator : AbstractValidator<GetTotalPerProjectQuery>
{
    public GetTotalPerProjectQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.")
            .GreaterThan(0).WithMessage("User ID must be greater than 0.");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Date is required.")
            .Must(date => date > DateOnly.MinValue)
            .WithMessage("Date must be a valid, non-default value.")
            .Must(date => date <= DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Date cannot be in the future.")
            .Must(date => date >= DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-14)))
            .WithMessage("Date cannot be more than 14 days in the past.");
    }
}