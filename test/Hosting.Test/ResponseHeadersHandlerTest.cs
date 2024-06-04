using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using Moq;

namespace NetLah.Extensions.SpaServices.Hosting.Test;
public class ResponseHeadersHandlerTest
{
    private static (StaticFileResponseContext responseContext, Mock<HttpContext> httpContext) MockContext(string contentType = "text/html", int statusCode = 200)
    {
        var httpContext = new Mock<HttpContext>();
        httpContext.SetupAllProperties();
        httpContext.SetupGet(x => x.Response).Returns(Mock.Of<HttpResponse>());
        httpContext.SetupGet(x => x.Response.Headers).Returns(new HeaderDictionary());
        Assert.NotNull(httpContext.Object.Response);
        httpContext.Object.Response.ContentType = contentType;
        httpContext.Object.Response.StatusCode = statusCode;
        var responseContext = new StaticFileResponseContext(httpContext.Object, Mock.Of<IFileInfo>());
        return (responseContext, httpContext);
    }

    private static ResponseHeadersHandler GetService(ResponseHeadersOptions? options = null, Action<StaticFileResponseContext> originalResponseHandler = null!)
    {
        options ??= new ResponseHeadersOptions();
        return new ResponseHeadersHandler(Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance, options, originalResponseHandler);
    }

    private static ResponseHandlerEntry GetHandler(string[]? contentTypes = null,
        string[]? contentTypeStartWith = null,
        string[]? contentTypeContain = null,
        int[]? statusCode = null,
        IEnumerable<KeyValuePair<string, StringValues>>? headers = null)
    {
        headers ??= new Dictionary<string, StringValues>
        {
            ["x-header"] = "x--value",
        };

        HashSet<string> contentTypesSet = [.. contentTypes ?? [], .. contentTypeStartWith ?? [], .. contentTypeContain ?? []];

        Assert.NotNull(headers);
        Assert.NotEmpty(headers);

        var handlerEntry = new ResponseHandlerEntry(
            contentTypes?.ToHashSet() ?? [],
            contentTypeStartWith ?? [],
            contentTypeContain ?? [],
            statusCode?.ToHashSet() ?? [],
            headers.ToArray(),
            [.. headers.Select(h => h.Key).OrderBy(s => s, StringComparer.OrdinalIgnoreCase)],
            [.. contentTypesSet.OrderBy(s => s, StringComparer.OrdinalIgnoreCase)]);

        return handlerEntry;
    }

    [Fact]
    public void DisabledTest()
    {
        var (responseContext, _) = MockContext();

        var service = GetService(new ResponseHeadersOptions { IsEnabled = false });

        service.PrepareResponse(responseContext);

        Assert.Equal([], responseContext.Context.Response.Headers);
    }

    [Fact]
    public void EnabledTest()
    {
        var (responseContext, _) = MockContext();

        var service = GetService();

        service.PrepareResponse(responseContext);

        Assert.Equal([], responseContext.Context.Response.Headers);
    }

    [Fact]
    public void ApplyDefaultSimpleTest()
    {
        var (responseContext, _) = MockContext();

        var service = GetService(new ResponseHeadersOptions
        {
            DefaultHandler = GetHandler()
        });

        service.PrepareResponse(responseContext);

        Assert.Equal(new Dictionary<string, StringValues>
        {
            ["x-header"] = "x--value",
        }, responseContext.Context.Response.Headers);
    }

    [Fact]
    public void FallbackTest()
    {
        var (responseContext, _) = MockContext("text/html");

        var service = GetService(new ResponseHeadersOptions
        {
            DefaultHandler = GetHandler(["text/plain"], headers: new Dictionary<string, StringValues>
            {
                ["expires"] = "Thu, 01 Jan 1970 00:00:00 GMT",
                ["pragma"] = "no-cache",
            })
        }, c => c.Context.Response.Headers["x-header1"] = "x--value1");

        service.PrepareResponse(responseContext);

        Assert.Equal(new Dictionary<string, StringValues>
        {
            ["x-header1"] = "x--value1",
        }, responseContext.Context.Response.Headers);

        responseContext.Context.Response.ContentType = "text/plain";
        responseContext.Context.Response.Headers.Clear();
        service.PrepareResponse(responseContext);

        Assert.Equal(new Dictionary<string, StringValues>
        {
            ["expires"] = "Thu, 01 Jan 1970 00:00:00 GMT",
            ["pragma"] = "no-cache",
            ["x-header1"] = "x--value1",
        }, responseContext.Context.Response.Headers);
    }

    [Fact]
    public void ContentTypeMatchDefaultNextAndNotMatchTest()
    {
        var (responseContext, _) = MockContext("application/json");

        var service = GetService(new ResponseHeadersOptions
        {
            DefaultHandler = GetHandler(["text/html"], headers: new Dictionary<string, StringValues>
            {
                ["expires"] = "Thu, 01 Jan 1970 00:00:00 GMT",
                ["pragma"] = "no-cache",
            }),
            Handlers = [GetHandler(["text/plain"], headers: new Dictionary<string, StringValues>
            {
                ["x-header2"] = "x--value2",
            })]
        });

        service.PrepareResponse(responseContext);
        Assert.Equal([], responseContext.Context.Response.Headers);

        responseContext.Context.Response.ContentType = "text/plain";
        responseContext.Context.Response.Headers.Clear();
        service.PrepareResponse(responseContext);

        Assert.Equal(new Dictionary<string, StringValues>
        {
            ["x-header2"] = "x--value2",
        }, responseContext.Context.Response.Headers);

        responseContext.Context.Response.ContentType = "text/html";
        responseContext.Context.Response.Headers.Clear();
        service.PrepareResponse(responseContext);

        Assert.Equal(new Dictionary<string, StringValues>
        {
            ["expires"] = "Thu, 01 Jan 1970 00:00:00 GMT",
            ["pragma"] = "no-cache",
        }, responseContext.Context.Response.Headers);
    }

    [Fact]
    public void StatusCodeMatchDefaultNextAndNotMatchTest()
    {
        var (responseContext, _) = MockContext("text/plain");

        var service = GetService(new ResponseHeadersOptions
        {
            DefaultHandler = GetHandler(["text/plain"], statusCode: [201], headers: new Dictionary<string, StringValues>
            {
                ["expires"] = "Thu, 01 Jan 1970 00:00:00 GMT",
                ["pragma"] = "no-cache",
            }),
            Handlers = [GetHandler(["text/plain"], statusCode: [202],headers: new Dictionary<string, StringValues>
            {
                ["x-header3"] = "x--value3",
            })]
        });

        service.PrepareResponse(responseContext);

        Assert.Equal([], responseContext.Context.Response.Headers);

        responseContext.Context.Response.StatusCode = 201;
        service.PrepareResponse(responseContext);

        Assert.Equal(new Dictionary<string, StringValues>
        {
            ["expires"] = "Thu, 01 Jan 1970 00:00:00 GMT",
            ["pragma"] = "no-cache",
        }, responseContext.Context.Response.Headers);

        responseContext.Context.Response.StatusCode = 202;
        responseContext.Context.Response.Headers.Clear();
        service.PrepareResponse(responseContext);

        Assert.Equal(new Dictionary<string, StringValues>
        {
            ["x-header3"] = "x--value3",
        }, responseContext.Context.Response.Headers);
    }

    [Fact]
    public void ContentTypeContainMatchTest()
    {
        var (responseContext, _) = MockContext("application/json");

        var service = GetService(new ResponseHeadersOptions
        {
            DefaultHandler = GetHandler(contentTypeContain: ["ext/"], headers: new Dictionary<string, StringValues>
            {
                ["expires"] = "Thu, 01 Jan 1970 00:00:00 GMT",
                ["pragma"] = "no-cache",
            })
        });

        service.PrepareResponse(responseContext);

        Assert.Equal([], responseContext.Context.Response.Headers);

        responseContext.Context.Response.ContentType = "text/html";
        responseContext.Context.Response.Headers.Clear();
        service.PrepareResponse(responseContext);

        Assert.Equal(new Dictionary<string, StringValues>
        {
            ["expires"] = "Thu, 01 Jan 1970 00:00:00 GMT",
            ["pragma"] = "no-cache",
        }, responseContext.Context.Response.Headers);

        responseContext.Context.Response.ContentType = "text/plain";
        responseContext.Context.Response.Headers.Clear();
        service.PrepareResponse(responseContext);

        Assert.Equal(new Dictionary<string, StringValues>
        {
            ["expires"] = "Thu, 01 Jan 1970 00:00:00 GMT",
            ["pragma"] = "no-cache",
        }, responseContext.Context.Response.Headers);
    }

    [Fact]
    public void ContentTypeStartWithMatchTest()
    {
        var (responseContext, _) = MockContext("application/json");

        var service = GetService(new ResponseHeadersOptions
        {
            DefaultHandler = GetHandler(contentTypeStartWith: ["text/"], headers: new Dictionary<string, StringValues>
            {
                ["expires"] = "Thu, 01 Jan 1970 00:00:00 GMT",
                ["pragma"] = "no-cache",
            })
        });

        service.PrepareResponse(responseContext);

        Assert.Equal([], responseContext.Context.Response.Headers);

        responseContext.Context.Response.ContentType = "text/html";
        responseContext.Context.Response.Headers.Clear();
        service.PrepareResponse(responseContext);

        Assert.Equal(new Dictionary<string, StringValues>
        {
            ["expires"] = "Thu, 01 Jan 1970 00:00:00 GMT",
            ["pragma"] = "no-cache",
        }, responseContext.Context.Response.Headers);

        responseContext.Context.Response.ContentType = "text/plain";
        responseContext.Context.Response.Headers.Clear();
        service.PrepareResponse(responseContext);

        Assert.Equal(new Dictionary<string, StringValues>
        {
            ["expires"] = "Thu, 01 Jan 1970 00:00:00 GMT",
            ["pragma"] = "no-cache",
        }, responseContext.Context.Response.Headers);
    }
}
