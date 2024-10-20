namespace user_service.exception;

public class HttpExceptionResult
{
    public int Status { get; set; }
    public string? Message { get; set; }
    public DateTime? TimeStamps { get; set; } = DateTime.Now;
    public string? Source { get; set; }
}