namespace Timesheet.UI.Models;

public class ServiceResult<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public List<string?> Errors { get; set; } = new();

    public static ServiceResult<T> Ok(T data, string? message = null)
    {
        return new ServiceResult<T> { Success = true, Data = data, Message = message };
    }

    public static ServiceResult<T> Fail(string error)
    {
        return new ServiceResult<T> { Success = false, Errors = [error] };
    }

    public static ServiceResult<T> Fail(IEnumerable<string?> errors)
    {
        return new ServiceResult<T> { Success = false, Errors = errors.ToList() };
    }
}