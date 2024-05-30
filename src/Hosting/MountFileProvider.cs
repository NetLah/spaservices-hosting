using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;
using Microsoft.Extensions.Primitives;

namespace NetLah.Extensions.SpaServices.Hosting;

internal class MountFileProvider : IFileProvider
{
    private readonly IFileProvider _fileProvider;
    private readonly MountFileProviderOptions _options;

    public MountFileProvider(IFileProvider fileProvider, MountFileProviderOptions options)
    {
        _fileProvider = fileProvider;
        _options = options;
    }

    public IDirectoryContents GetDirectoryContents(string subpath)
    {
        return _fileProvider.GetDirectoryContents(subpath);
    }

    public IFileInfo GetFileInfo(string subpath)
    {
        if (_options.Files.TryGetValue(subpath, out var source))
        {
            var fileInfo = new FileInfo(source);
            return fileInfo.Exists ? new PhysicalFileInfo(fileInfo) : new NotFoundFileInfo(subpath);
        }

        var result = _fileProvider.GetFileInfo(subpath);
        return result;
    }

    public IChangeToken Watch(string filter)
    {
        return _fileProvider.Watch(filter);
    }
}
