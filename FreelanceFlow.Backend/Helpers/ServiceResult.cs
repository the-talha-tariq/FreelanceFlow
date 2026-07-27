namespace FreelanceFlow.Backend.Helpers;

/// <summary>
/// Uniform success/failure envelope returned by service methods, so
/// controllers don't need to catch exceptions for expected failure cases
/// (bad credentials, not found, validation, etc.).
/// </summary>
public class ServiceResult<T>
{
    public bool Success { get; init; }
    public T? Data { get; init; }
    public List<string> Errors { get; init; } = new();

    public static ServiceResult<T> SuccessResult(T data) =>
        new() { Success = true, Data = data };

    public static ServiceResult<T> FailureResult(string error) =>
        new() { Success = false, Errors = new List<string> { error } };

    public static ServiceResult<T> FailureResult(IEnumerable<string> errors) =>
        new() { Success = false, Errors = errors.ToList() };
}