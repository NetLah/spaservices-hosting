namespace NetLah.Extensions.SpaServices.Hosting;

public class ResponseHeadersOptions
{
    public bool IsEnabled { get; set; } = true;

    public bool IsContentTypeContainsMatch { get; set; } = true;

    public bool IsAnyContentType { get; set; } = false;

    public Dictionary<string, string?> Headers { get; set; } = [];

    public List<string?>? FilterContentType { get; set; }

    public List<int>? FilterHttpStatusCode { get; set; }
}
