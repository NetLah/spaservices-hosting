namespace NetLah.Extensions.SpaServices.Hosting;

internal class MountFileProviderOptions
{
    public IDictionary<string, string> Files { get; set; } = null!;

    public IDictionary<string, string> Folders { get; set; } = null!;
}
