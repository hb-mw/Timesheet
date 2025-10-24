using FluentValidation;
using Timesheet.Shared.Contracts;

namespace Timesheet.App.Validators;

public class UpsertTimesheetEntryRequestValidator : AbstractValidator<UpsertTimesheetEntryRequest>
{
    public UpsertTimesheetEntryRequestValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.")
            .GreaterThan(0).WithMessage("User ID must be greater than 0.");

        RuleFor(x => x.ProjectId)
            .GreaterThan(0).WithMessage("Project ID must be greater than 0.");

        RuleFor(x => x.Hours)
            .InclusiveBetween(0.1m, 12m).WithMessage("Hours must be between 0.1 and 12.");

        RuleFor(x => x.Date)
            .NotNull()
            .NotEmpty().WithMessage("Date is required.")
            .Must(date => date <= DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Date cannot be in the future.")
            .Must(date => date >= DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-14)))
            .WithMessage("Date cannot be more than 14 days in the past.");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Description cannot exceed 500 characters.");
    }
}