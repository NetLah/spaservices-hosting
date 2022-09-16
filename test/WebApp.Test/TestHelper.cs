using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;

namespace WebApp.Test;

internal static class TestHelper
{
    public static readonly string ContentRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../.."));

    public static WebApplicationBuilder CreateWebApplicationBuilder(string environmentName = DefaultConfig.TestingEnvironmentConst)
    {
        var builder = WebApplication.CreateBuilder(
            new[] {
                $"--environment={environmentName}",
                $"--contentRoot={ContentRoot}",
                "--applicationNAME=WebApp",
                }
            );

        builder.WebHost.UseTestServer();

        return builder;
    }
}
