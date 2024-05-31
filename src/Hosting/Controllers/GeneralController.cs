using Microsoft.AspNetCore.Mvc;

namespace NetLah.Extensions.SpaServices.Hosting.Controllers;

//[Route("api/v1/[controller]/[action]")]
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[ApiExplorerSettings(IgnoreApi = true)]
public class GeneralController(IAppInfo appInfo) : ControllerBase
{
    private readonly IAppInfo _appInfo = appInfo;

    public string Version() => _appInfo.Version;

    public string Info()
    {
        var request = HttpContext.Request;
        var remote = HttpContext?.Connection;
        var appVer = $"App:{_appInfo.Title}; Version:{_appInfo.Version}; BuildTime:{_appInfo.BuildTimestampLocal} Scheme:{request.Scheme} Host:{request.Host} {remote?.RemoteIpAddress}:{remote?.RemotePort}";
        return appVer;
    }

    public ContentResult Sys([FromServices] IInfoCollector infoCollector)
        => Content(string.Join(Environment.NewLine, infoCollector.Logs.Append($"Uptime: {_appInfo.Uptime}")), "text/plain; charset=utf-8");
}
