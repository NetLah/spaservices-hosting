using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace NetLah.Extensions.SpaServices.Hosting;

internal class ResponseHeadersHandler
{
    private readonly ILogger _logger;
    private readonly List<KeyValuePair<string, StringValues>> _headers;
    private readonly HashSet<string?>? _contentTypeHashSet;
    private readonly List<string?>? _contentTypeList;
    private readonly Func<int, bool> _matchStatusCode;
    private readonly Func<string, bool> _matchContentType;
    private readonly HashSet<int>? _statusCodes;

    public ResponseHeadersHandler(ILogger logger, ResponseHeadersOptions responseHeaders)
    {
        _logger = logger;
        _headers = responseHeaders.Headers
            .Select(kv => new KeyValuePair<string, StringValues>(kv.Key, new StringValues(kv.Value)))
            .ToList();

        if (!responseHeaders.IsEnabled || !_headers.Any())
        {
            _matchStatusCode = _ => false;
            _matchContentType = _ => false;
            logger.LogWarning("ResponseHeadersHandler is disabled");
        }
        else
        {
            if (responseHeaders.FilterHttpStatusCode is { } listHttpStatusCode
                && listHttpStatusCode.Any())
            {
                _statusCodes = listHttpStatusCode.ToHashSet();
                _matchStatusCode = statusCode => _statusCodes.Contains(statusCode);
                logger.LogInformation("ResponseHeadersHandler StatusCode in list");
            }
            else
            {
                _matchStatusCode = _ => true;
            }

            if (responseHeaders.FilterContentType is not { } listContentType
                || !listContentType.Any()
                || responseHeaders.IsAnyContentType)
            {
                _matchContentType = _ => true;
                logger.LogInformation("ResponseHeadersHandler all content-type");
            }
            else
            {
                _contentTypeHashSet = listContentType.ToHashSet(StringComparer.InvariantCultureIgnoreCase);
                _contentTypeList = _contentTypeHashSet.ToList();

                if (responseHeaders.IsContentTypeContainsMatch)
                {
                    _matchContentType = contentType => _contentTypeList.Any(item => !string.IsNullOrEmpty(item) && item.Contains(contentType));
                    logger.LogInformation("ResponseHeadersHandler content-type -contains {contentTypes}", _contentTypeList);
                }
                else
                {
                    _matchContentType = contentType => _contentTypeHashSet.Contains(contentType);
                    logger.LogInformation("ResponseHeadersHandler content-type -eq {contentTypes}", _contentTypeList);
                }
            }
        }
    }

    public void PrepareResponse(StaticFileResponseContext context)
    {
        var contentType = context.Context.Response.ContentType;
        var statusCode = context.Context.Response.StatusCode;

        if (string.IsNullOrEmpty(contentType)
            || statusCode > 399
            || !_matchStatusCode(statusCode)
            || !_matchContentType(contentType))
        {
            return;
        }

        _logger.LogDebug("Add headers");
        foreach (var item in _headers)
        {
            context.Context.Response.Headers.Add(item);
        }
    }
}
