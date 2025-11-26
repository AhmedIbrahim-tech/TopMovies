namespace TopMovies.ViewModels;

public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    public int? StatusCode { get; set; }

    public string? ExceptionMessage { get; set; }
}
