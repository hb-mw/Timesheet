namespace Timesheet.Shared.Contracts;

public class ApiError
{
    public string? Type { get; set; }
    public string? Title { get; set; }
    public int? Status { get; set; }
    public string? Code { get; set; }
    public string? TraceId { get; set; }
    public Dictionary<string, object>? Details { get; set; }
    
    public Dictionary<string, object>? Errors { get; set; }
}