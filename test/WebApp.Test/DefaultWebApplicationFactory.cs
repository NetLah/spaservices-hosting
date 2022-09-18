using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace WebApp.Test;

public class DefaultWebApplicationFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint> where TEntryPoint : class
{
    public virtual string EnvironmentName { get; set; } = DefaultConfig.TestingEnvironmentName;

    public virtual bool SetContentRoot { get; set; } = true;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        if (string.IsNullOrWhiteSpace(EnvironmentName))
        {
            throw new ArgumentException("EnvironmentName is required");
        }

        base.ConfigureWebHost(builder);

        builder.UseEnvironment(EnvironmentName);

        if (SetContentRoot)
        {
            builder.RelocateContentRoot();
        }
    }
}

public sealed class PublicCustomWebApplicationFactory : DefaultWebApplicationFactory<Program>
{
    public override string EnvironmentName { get; set; } = "Public";
}

public sealed class Public2CustomWebApplicationFactory : DefaultWebApplicationFactory<Program>
{
    public override string EnvironmentName { get; set; } = "Public2";
}

public sealed class NoFolderCustomWebApplicationFactory : DefaultWebApplicationFactory<Program>
{
    public override string EnvironmentName { get; set; } = "NoFolder";
}
