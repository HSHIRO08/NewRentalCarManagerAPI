namespace NewRentalCarManagerAPI.Common;

public class UserFriendlyException : Exception
{
    public int StatusCode { get; }

    public UserFriendlyException(int statusCode, string message) : base(message)
    {
        StatusCode = statusCode;
    }
}