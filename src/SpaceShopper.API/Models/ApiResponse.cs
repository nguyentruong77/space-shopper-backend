namespace SpaceShopper.API.Models
{
    public sealed class ApiError
    {
        public string Code { get; init; } = string.Empty;
        public string Message { get; init; } = string.Empty;
        public object? Details { get; init; }
    }

    public sealed class ApiResponse<T>
    {
        public bool Success { get; init; }
        public T? Data { get; init; }
        public ApiError? Error { get; init; }

        public static ApiResponse<T> Ok(T data) =>
            new() { Success = true, Data = data };

        public static ApiResponse<T> Fail(string code, string message, object? details = null) =>
            new() { Success = false, Error = new ApiError { Code = code, Message = message, Details = details } };
    }
}
