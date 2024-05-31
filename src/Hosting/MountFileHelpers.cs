using Microsoft.AspNetCore.SpaServices.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NetLah.Extensions.Logging;

namespace NetLah.Extensions.SpaServices.Hosting;

internal static class MountFileHelpers
{
    private static readonly Lazy<ILogger?> _loggerLazy = new(() => AppLogReference.GetAppLogLogger(typeof(AppOptions).Namespace + ".MountFile"));
    private static readonly StringComparer _stringComparer = OperatingSystem.IsWindows() ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;
    // private static readonly StringComparison _stringComparison = OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal

    internal static Func<string, bool> ValidateDirectoryExistsDelegate { get; set; } = Directory.Exists;

    private static ILogger GetLogger()
    {
        return _loggerLazy.Value ?? NullLogger.Instance;
    }

    public static void Configure(IServiceCollection services, IConfigurationRoot configuration)
    {
        var options = ParserOptions(configuration, GetLogger());

        if (options.Files.Count > 0 || options.Folders.Count > 0)
        {
            services.AddSingleton(options);
            services.AddDecoratorAsLifetime<ISpaStaticFileProvider, MountSpaStaticFileProvider>();
        }
    }

    public static MountFileProviderOptions ParserOptions(IConfigurationRoot configuration, ILogger logger)
    {
        var options = new MountFileProviderOptions
        {
            Files = new Dictionary<string, string>(_stringComparer),
            Folders = new Dictionary<string, string>(_stringComparer),
        };

        foreach (var item in configuration.GetSection("MountFile").GetChildren().Concat(configuration.GetSection("MountFiles").GetChildren()))
        {
            if (item.Value is { } keyValue)
            {
                if (TryParseKeyValue(keyValue, out var target, out var source) && !string.IsNullOrEmpty(target) && !string.IsNullOrEmpty(source))
                {
                    TryAddFile(target, source);
                }
                else
                {
                    throw new InvalidOperationException("Invalid MountFile '" + keyValue + "'");
                }
            }
            else
            {
                var source = item["From"] ?? item["Source"] ?? item["Src"];
                var target = item["Target"] ?? item["To"] ?? item["Destination"] ?? item["Dest"] ?? item["Dst"];
                TryAddFile(target, source);
            }
        }

        foreach (var item in configuration.GetSection("MountFolder").GetChildren().Concat(configuration.GetSection("MountFolders").GetChildren()))
        {
            if (item.Value is { } keyValue)
            {
                if (TryParseKeyValue(keyValue, out var target, out var source) && !string.IsNullOrEmpty(target) && !string.IsNullOrEmpty(source))
                {
                    TryAddFolder(target, source);
                }
                else
                {
                    throw new InvalidOperationException("Invalid MountFolder '" + keyValue + "'");
                }
            }
            else
            {
                var source = item["From"] ?? item["Source"] ?? item["Src"];
                var target = item["Target"] ?? item["To"] ?? item["Destination"] ?? item["Dest"] ?? item["Dst"];
                TryAddFolder(target, source);
            }
        }

        bool TryParseKeyValue(string keyValue, out string key, out string? value)
        {
            var pos = keyValue.IndexOf('=');
            if (pos > 0 && pos < keyValue.Length - 1)
            {
                key = keyValue[..pos];
                value = keyValue[(pos + 1)..];
                return true;
            }

            key = string.Empty;
            value = null;
            return false;
        }

        void TryAddFile(string? target, string? source)
        {
            if (string.IsNullOrEmpty(target) || !target.StartsWith('/'))
            {
                throw new InvalidOperationException("target has to start with / '" + target + "'");
            }

            if (string.IsNullOrEmpty(source))
            {
                throw new InvalidOperationException("source is required");
            }

            if (options.Files.ContainsKey(target))
            {
                throw new InvalidOperationException("Duplicated target '" + target + "'");
            }

            var sourceFullPath = Path.GetFullPath(source);

            logger.LogDebug("MountFile target={target} source={source} {sourceFullPath}", target, source, sourceFullPath);

            options.Files[target] = sourceFullPath;
        }

        void TryAddFolder(string? target, string? source)
        {
            if (string.IsNullOrEmpty(target) || !target.StartsWith('/'))
            {
                throw new InvalidOperationException("target has to start with / '" + target + "'");
            }

            if (string.IsNullOrEmpty(source))
            {
                throw new InvalidOperationException("source is required");
            }

            if (!target.EndsWith('/'))
            {
                target += '/';
            }

            if (options.Folders.ContainsKey(target))
            {
                throw new InvalidOperationException("Duplicated target '" + target + "'");
            }

            var sourceFullPath = Path.GetFullPath(source);

            if (!ValidateDirectoryExistsDelegate(sourceFullPath))
            {
                throw new DirectoryNotFoundException(sourceFullPath);
            }

            logger.LogDebug("MountFolder target={target} source={source} {sourceFullPath}", target, source, sourceFullPath);

            options.Folders[target] = sourceFullPath;
        }

        return options;
    }
}
