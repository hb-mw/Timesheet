using System.ComponentModel.DataAnnotations;

namespace Timesheet.UI.Models.Forms;

public class TimesheetEntryFormModel
{
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "User ID is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "User ID must be greater than 0.")]
    public int UserId { get; set; }

    [Required(ErrorMessage = "Project ID is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Project ID must be greater than 0.")]
    public int ProjectId { get; set; }

    [Required(ErrorMessage = "Date is required.")]
    [CustomValidation(typeof(TimesheetEntryFormModel), nameof(ValidateDateNotInFuture))]
    public DateTime? Date { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "Hours are required.")]
    [Range(0.1, 24, ErrorMessage = "Hours must be greater than 0 and up to 24.")]
    public decimal Hours { get; set; }

    public string? Description { get; set; }

    // ✅ Custom validator for date
    public static ValidationResult? ValidateDateNotInFuture(DateTime date, ValidationContext context)
    {
        return date > DateTime.Today ? new ValidationResult("Date cannot be in the future.") : ValidationResult.Success;
    }
}