using Microsoft.Extensions.Primitives;

namespace NetLah.Extensions.SpaServices.Hosting;

internal class BaseResponseHeadersConfigurationOptions
{
    public HashSet<int>? StatusCode { get; set; }
    public HashSet<string>? ContentType { get; set; }
    public HashSet<string>? ContentTypeContain { get; set; }
    public HashSet<string>? ContentTypeStartWith { get; set; }
    public List<string>? Headers { get; set; }
}

internal class ResponseHeadersConfigurationOptions : BaseResponseHeadersConfigurationOptions
{
    public bool IsEnabled { get; set; } = true;

}

internal class ResponseHeadersOptions
{
    public bool IsEnabled { get; set; } = true;
    public ResponseHandlerEntry[] Handlers { get; set; } = [];
    public ResponseHandlerEntry? DefaultHandler { get; set; }
}

internal class ResponseHandlerEntry(HashSet<string> contentType,
    string[] contentTypeMatchStartWith,
    string[] contentTypeMatchContain,
    HashSet<int> statusCode,
    KeyValuePair<string, StringValues>[] headers,
    string[] headerNames,
    string[] contentTypes)
{
    public HashSet<string> ContentTypeMatchEq { get; set; } = contentType;
    public string[] ContentTypeMatchStartWith { get; set; } = contentTypeMatchStartWith;
    public string[] ContentTypeMatchContain { get; set; } = contentTypeMatchContain;
    public HashSet<int> StatusCode { get; set; } = statusCode;
    public KeyValuePair<string, StringValues>[] Headers { get; set; } = headers;
    public string[] HeaderNames { get; set; } = headerNames;
    public string[] ContentTypes { get; set; } = contentTypes;
}
