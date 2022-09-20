namespace NetLah.Extensions.SpaServices.Hosting;

public class AppFileVersionInfo
{
    private string? _title;
    private string _version = default!;
    private string? _buildTimeString;
    private string? _description;

    public string? Title { get => _title; set => _title = NormalizeNull(value); }
    public string Version { get => _version; set => _version = NormalizeNull(value); }
    public string? BuildTimeString { get => _buildTimeString; set => _buildTimeString = NormalizeNull(value); }
    public string? Description { get => _description; set => _description = NormalizeNull(value); }
    public DateTimeOffset? BuildTime { get; set; }

    public bool IsValid() => Version != null;

    public bool IsFullValid() => IsValid() && BuildTime != null;

    private static string NormalizeNull(string? value)
        => string.IsNullOrWhiteSpace(value) ? default! : value.Trim();
}
