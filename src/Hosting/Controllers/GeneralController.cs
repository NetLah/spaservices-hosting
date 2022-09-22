using Microsoft.AspNetCore.Mvc;

namespace NetLah.Extensions.SpaServices.Hosting.Controllers;

//[Route("api/v1/[controller]/[action]")]
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[ApiExplorerSettings(IgnoreApi = true)]
public class GeneralController : ControllerBase
{
    private readonly IAppInfo _appInfo;

    public GeneralController(IAppInfo appInfo)
    {
        _appInfo = appInfo;
    }

    //[HttpGet]
    //[ExcludeFromCodeCoverage]
    //public ContentResult SysInfo()
    //{
    //    return Content(string.Join(Environment.NewLine, _infoCollector.Logs), "text/plain; charset=utf-8");
    //}

    [HttpGet]
#pragma warning disable CA1822 // Mark members as static
    public string SysInfo()
#pragma warning restore CA1822 // Mark members as static
    {
        return $"This is GetInfo {DateTimeOffset.Now:O}";
    }

    [HttpGet]
    public string GetInfo()
    {
        var request = HttpContext.Request;
        var remote = HttpContext?.Connection;
        var appVer = $"App:{_appInfo.Title}; Version:{_appInfo.Version}; BuildTime:{_appInfo.BuildTimestampLocal} Scheme:{request.Scheme} Host:{request.Host} {remote?.RemoteIpAddress}:{remote?.RemotePort}";
        return appVer;
    }

    [HttpGet]
    public string GetVersion() => _appInfo.Version;
}
