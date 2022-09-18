namespace NetLah.Extensions.SpaServices.Hosting;

public class AppFileVersionInfo
{
    private string? _title;
    private string? _version;
    private string? _buildTimeString;
    private string? _description;

    public string? Title { get => _title; set => _title = NormalizeEmptyNull(value); }
    public string? Version { get => _version; set => _version = NormalizeEmptyNull(value); }
    public string? BuildTimeString { get => _buildTimeString; set => _buildTimeString = NormalizeEmptyNull(value); }
    public string? Description { get => _description; set => _description = NormalizeEmptyNull(value); }
    public DateTimeOffset? BuildTime { get; set; }

    public bool IsValid() => Version != null;

    public bool IsFullValid() => IsValid() && BuildTime != null;

    private static string? NormalizeEmptyNull(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
