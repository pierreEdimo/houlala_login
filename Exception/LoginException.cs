using Microsoft.AspNetCore.Identity;

namespace user_service.exception;

public class LoginException : Exception
{
    public LoginException()
    {
    }

    public LoginException(string message, int status) : base(message)
    {
        Status = status;
    }

    public LoginException(string message, Exception innerException, int status) : base(message, innerException)
    {
        Status = status;
    }

    public LoginException(IEnumerable<IdentityError> errors, int status)
    {
        Status = status;
        Errors = errors;
    }

    public int Status { get; set; }
    public string? TimeStamp { get; set; }
    public IEnumerable<IdentityError>? Errors { get; set; }
}