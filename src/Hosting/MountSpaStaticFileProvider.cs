using Microsoft.AspNetCore.SpaServices.StaticFiles;
using Microsoft.Extensions.FileProviders;

namespace NetLah.Extensions.SpaServices.Hosting;

internal class MountSpaStaticFileProvider : ISpaStaticFileProvider
{
    public MountSpaStaticFileProvider(IDecorator<ISpaStaticFileProvider> decorator, MountFileProviderOptions options)
    {
        FileProvider = decorator.Instance.FileProvider == null
            ? decorator.Instance.FileProvider
            : new MountFileProvider(decorator.Instance.FileProvider, options);
    }

    public IFileProvider? FileProvider { get; }
}
