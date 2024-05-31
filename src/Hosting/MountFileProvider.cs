using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;
using Microsoft.Extensions.Primitives;

namespace NetLah.Extensions.SpaServices.Hosting;

internal class MountFileProvider(IFileProvider fileProvider, MountFileProviderOptions options) : IFileProvider
{
    private readonly IFileProvider _fileProvider = fileProvider;
    private readonly IDictionary<string, string> _fileMap = options?.Files ?? new Dictionary<string, string>();
    private readonly (string path, Entry value)[] _folderMap = options?
        .Folders?
        .Select(kv => (kv.Key, new Entry(kv.Key.Length - 1, new PhysicalFileProvider(kv.Value))))
        .ToArray()
        ?? [];

    private class Entry(int length, IFileProvider fileProvider)
    {
        public int Length { get; } = length;
        public IFileProvider FileProvider { get; } = fileProvider;
    }

    public IDirectoryContents GetDirectoryContents(string subpath)
    {
        return _fileProvider.GetDirectoryContents(subpath);
    }

    public IFileInfo GetFileInfo(string subpath)
    {
        if (!string.IsNullOrEmpty(subpath))
        {
            if (_fileMap.TryGetValue(subpath, out var source))
            {
                var fileInfo = new FileInfo(source);
                return fileInfo.Exists ? new PhysicalFileInfo(fileInfo) : new NotFoundFileInfo(subpath);
            }

            foreach (var (path, entry) in _folderMap)
            {
                if (subpath.StartsWith(path))
                {
                    return entry.FileProvider.GetFileInfo(subpath[entry.Length..]);
                }
            }
        }

        var result = _fileProvider.GetFileInfo(subpath);
        return result;
    }

    public IChangeToken Watch(string filter)
    {
        return _fileProvider.Watch(filter);
    }
}
