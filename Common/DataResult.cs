namespace NewRentalCarManagerAPI.Common;

public class DataResult
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public object? Data { get; set; }
    public int? TotalCount { get; set; }
    public int StatusCode { get; set; }

    public static DataResult ResultSuccess(object? data, string message = "Success", int? totalCount = null, int statusCode = 200)
    {
        return new DataResult
        {
            Success = true,
            Message = message,
            Data = data,
            TotalCount = totalCount,
            StatusCode = statusCode
        };
    }

    public static DataResult ResultError(int statusCode, string message)
    {
        return new DataResult
        {
            Success = false,
            Message = message,
            StatusCode = statusCode
        };
    }
}