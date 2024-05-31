using Microsoft.AspNetCore.SpaServices.StaticFiles;
using Microsoft.Extensions.FileProviders;

namespace NetLah.Extensions.SpaServices.Hosting;

internal class MountSpaStaticFileProvider(IDecorator<ISpaStaticFileProvider> decorator, MountFileProviderOptions options) : ISpaStaticFileProvider
{
    public IFileProvider? FileProvider { get; } = decorator.Instance.FileProvider == null
        ? decorator.Instance.FileProvider
        : new MountFileProvider(decorator.Instance.FileProvider, options);
}
