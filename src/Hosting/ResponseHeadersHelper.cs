using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace NetLah.Extensions.SpaServices.Hosting;

internal static class ResponseHeadersHelper
{
    private static readonly StringComparison DefaultStringComparison = StringComparison.OrdinalIgnoreCase;

    private static readonly StringComparer DefaultStringComparer = StringComparer.OrdinalIgnoreCase;

    private static readonly HashSet<string> PropertySet = new(DefaultStringComparer) {
        nameof(BaseResponseHeadersConfigurationOptions.ContentType),
        nameof(BaseResponseHeadersConfigurationOptions.ContentTypeContain),
        nameof(BaseResponseHeadersConfigurationOptions.ContentTypeStartWith),
        nameof(BaseResponseHeadersConfigurationOptions.Headers),
        nameof(BaseResponseHeadersConfigurationOptions.StatusCode),
        nameof(ResponseHeadersConfigurationOptions.IsEnabled),
        "IsAnyContentType",             // legacy value
        "IsContentTypeContainsMatch",   // legacy value
    };

    //private static readonly string[] PropertyNames = [.. PropertySet];

    public static ResponseHeadersOptions Parse(IConfigurationRoot? configurationRoot, string sectionName)
    {
        ResponseHandlerEntry? defaultHandlerEntry = null;
        var isEnabled = false;
        var handlers = new List<ResponseHandlerEntry>();

        if (configurationRoot != null)
        {
            var configuration = configurationRoot.GetSection(sectionName);
            var configOptions = new ResponseHeadersConfigurationOptions();
            configuration.Bind(configOptions);
            if (configOptions.IsEnabled)
            {
                isEnabled = true;
                defaultHandlerEntry = ParseHandler(configOptions, configuration);
            }

            foreach (var item in configuration.GetChildren())
            {
                var key = item.Key;
                if (item.Value == null && int.TryParse(key, out var _))
                {
                    var configOptions1 = new BaseResponseHeadersConfigurationOptions();
                    item.Bind(configOptions1);
                    var handlerEntry = ParseHandler(configOptions1, item);
                    if (handlerEntry.Headers.Length > 0)
                    {
                        handlers.Add(handlerEntry);
                    }
                }
            }
        }

        return new ResponseHeadersOptions
        {
            IsEnabled = isEnabled,
            DefaultHandler = defaultHandlerEntry?.Headers.Length > 0 ? defaultHandlerEntry : default,
            Handlers = [.. handlers],
        };
    }

    private static ResponseHandlerEntry ParseHandler(BaseResponseHeadersConfigurationOptions options, IConfigurationSection configuration)
    {
        var headers = new Dictionary<string, string>();

        foreach (var item in options.Headers ?? [])
        {
            if (TryParseKeyValue(item, out var key, out var value)
                && !string.IsNullOrEmpty(key)
                && !string.IsNullOrEmpty(value))
            {
                headers[key] = value;
            }
        }

        foreach (var item in configuration.GetChildren())
        {
            var key = item.Key;
            if (item.Value is { } value && !string.IsNullOrEmpty(value) && !int.TryParse(key, out var _))
            {
                if (nameof(BaseResponseHeadersConfigurationOptions.ContentType).Equals(key, DefaultStringComparison))
                {
                    if (options.ContentType == null || options.ContentType.Count == 0)
                    {
                        options.ContentType = [value];
                    }
                }
                else if (nameof(BaseResponseHeadersConfigurationOptions.ContentTypeContain).Equals(key, DefaultStringComparison))
                {
                    if (options.ContentTypeContain == null || options.ContentTypeContain.Count == 0)
                    {
                        options.ContentTypeContain = [value];
                    }
                }
                else if (nameof(BaseResponseHeadersConfigurationOptions.ContentTypeStartWith).Equals(key, DefaultStringComparison))
                {
                    if (options.ContentTypeStartWith == null || options.ContentTypeStartWith.Count == 0)
                    {
                        options.ContentTypeStartWith = [value];
                    }
                }
                else if (nameof(BaseResponseHeadersConfigurationOptions.StatusCode).Equals(key, DefaultStringComparison) && int.TryParse(value, out var statusCode))
                {
                    if (options.StatusCode == null || options.StatusCode.Count == 0)
                    {
                        options.StatusCode = [statusCode];
                    }
                }
                else if (!PropertySet.Contains(key))
                {
                    headers[key] = value;
                }
            }
            else if (nameof(BaseResponseHeadersConfigurationOptions.Headers).Equals(key, DefaultStringComparison))
            {
                foreach (var item1 in item.GetChildren())
                {
                    if (item1.Value is { } value1 && item1.Key is { } key1 && !string.IsNullOrEmpty(key1) && !string.IsNullOrEmpty(value1) && !int.TryParse(key1, out var _))
                    {
                        headers[key1] = value1;
                    }
                }
            }
        }

        var headersStringValues = headers
            .Select(kv => new KeyValuePair<string, StringValues>(kv.Key, new StringValues(kv.Value)))
            .ToArray();

        HashSet<string> contentTypesSet = [.. options.ContentType ?? [], .. options.ContentTypeStartWith ?? [], .. options.ContentTypeContain ?? []];

        var handlerEntry = new ResponseHandlerEntry(
            options.ContentType?.Where(v => !string.IsNullOrEmpty(v)).ToHashSet() ?? [],
            options.ContentTypeStartWith?.Where(v => !string.IsNullOrEmpty(v)).ToArray() ?? [],
            options.ContentTypeContain?.Where(v => !string.IsNullOrEmpty(v)).ToArray() ?? [],
            options.StatusCode?.Where(v => v > 0).ToHashSet() ?? [],
            headersStringValues,
            [.. headers.Keys.OrderBy(s => s, DefaultStringComparer)],
            [.. contentTypesSet.OrderBy(s => s, DefaultStringComparer)]);

        return handlerEntry;

        static bool TryParseKeyValue(string keyValue, out string key, out string? value)
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
    }
}
