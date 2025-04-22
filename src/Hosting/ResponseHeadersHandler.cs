using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;

namespace NetLah.Extensions.SpaServices.Hosting;

internal class ResponseHeadersHandler(ILogger logger, ResponseHeadersOptions responseHeaders, Action<StaticFileResponseContext> originalResponseHandler)
{
    private readonly ILogger _logger = logger;
    private readonly WeakReference _originalResponseHandler = new(originalResponseHandler ?? (_ => { }));
    private readonly ResponseHandlerEntry[] _handlers = [
        .. responseHeaders.DefaultHandler is { } handler ? [handler] : Array.Empty<ResponseHandlerEntry>(),
        .. responseHeaders.Handlers,
    ];

    public void PrepareResponse(StaticFileResponseContext context)
    {
        var contentType = context.Context.Response.ContentType;
        var statusCode = context.Context.Response.StatusCode;

        if (string.IsNullOrEmpty(contentType) || statusCode > 399)
        {
            GetOriginalResponseHandler()?.Invoke(context);
            return;
        }

        foreach (var handler in _handlers)
        {
            if ((handler.StatusCode.Count == 0 || handler.StatusCode.Contains(statusCode))
                && (
                (handler.ContentTypeMatchEq.Count == 0 && handler.ContentTypeMatchContain.Length == 0 && handler.ContentTypeMatchStartWith.Length == 0)
                || (handler.ContentTypeMatchEq.Count > 0 && handler.ContentTypeMatchEq.Contains(contentType))
                || (handler.ContentTypeMatchContain.Length > 0 && MatchContain(handler.ContentTypeMatchContain))
                || (handler.ContentTypeMatchStartWith.Length > 0 && MatchStartWith(handler.ContentTypeMatchStartWith))
                ))
            {
                if (handler.StatusCode.Count == 0)
                {
                    _logger.LogDebug("Add headers for {contentType}", contentType);
                }
                else
                {
                    _logger.LogDebug("Add headers for {contentType} statusCode={statusCode}", contentType, statusCode);
                }

                foreach (var item in handler.Headers)
                {
                    context.Context.Response.Headers.Add(item);
                }

                GetOriginalResponseHandler()?.Invoke(context);
                return;
            }
        }

        bool MatchContain(string[] contentTypeMatchContain)
        {
            foreach (var item in contentTypeMatchContain)
            {
                if (contentType.Contains(item))
                {
                    return true;
                }
            }
            return false;
        }

        bool MatchStartWith(string[] contentTypeMatchStartWith)
        {
            foreach (var item in contentTypeMatchStartWith)
            {
                if (contentType.StartsWith(item))
                {
                    return true;
                }
            }
            return false;
        }

        GetOriginalResponseHandler()?.Invoke(context);

        Action<StaticFileResponseContext>? GetOriginalResponseHandler()
        {
            return _originalResponseHandler.IsAlive ? _originalResponseHandler.Target as Action<StaticFileResponseContext> : null;
        }
    }
}
