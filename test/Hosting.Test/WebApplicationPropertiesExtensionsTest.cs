using Microsoft.AspNetCore.Builder;

namespace NetLah.Extensions.SpaServices.Hosting.Test;

public class WebApplicationPropertiesExtensionsTest
{
    [Fact]
    public void ClassSuccess()
    {
        var builder = WebApplication.CreateBuilder();

        var p1Null = WebApplicationPropertiesExtensions.GetProperty<PropertyValue>(builder);
        Assert.Null(p1Null);

        var p2 = new PropertyValue();
        WebApplicationPropertiesExtensions.SetProperty<PropertyValue>(builder, p2);
        var p3 = WebApplicationPropertiesExtensions.GetProperty<PropertyValue>(builder);

        Assert.NotNull(p3);
        Assert.Same(p3, p2);
    }

    [Fact]
    public void KeyClassSuccess()
    {
        var builder = WebApplication.CreateBuilder();
        var key = typeof(int);

        var p1Null = WebApplicationPropertiesExtensions.GetProperty<PropertyValue>(builder, key);
        Assert.Null(p1Null);

        var p2 = new PropertyValue();
        WebApplicationPropertiesExtensions.SetProperty<PropertyValue>(builder, key, p2);
        var p3 = WebApplicationPropertiesExtensions.GetProperty<PropertyValue>(builder, key);

        Assert.NotNull(p3);
        Assert.Same(p3, p2);
    }

    [Fact]
    public void StructSuccess()
    {
        var builder = WebApplication.CreateBuilder();

        var p1Default = WebApplicationPropertiesExtensions.GetProperty<double>(builder);
        Assert.Equal(default, p1Default);

        var p2 = 1.234D;
        WebApplicationPropertiesExtensions.SetProperty<double>(builder, p2);
        var p3 = WebApplicationPropertiesExtensions.GetProperty<double>(builder);

        Assert.NotEqual(default, p3);
        Assert.Equal(p3, p2);
    }

    [Fact]
    public void KeyStructSuccess()
    {
        var builder = WebApplication.CreateBuilder();
        var key = "theKey123";

        double p1Default = WebApplicationPropertiesExtensions.GetProperty<double>(builder, key);
        Assert.Equal(default, p1Default);

        var p2 = 1.234D;
        WebApplicationPropertiesExtensions.SetProperty<double>(builder, key, p2);
        var p3 = WebApplicationPropertiesExtensions.GetProperty<double>(builder, key);

        Assert.NotEqual(default, p3);
        Assert.Equal(p3, p2);
    }


    [Fact]
    public void NullableStructSuccess()
    {
        var builder = WebApplication.CreateBuilder();

        var p1Default = WebApplicationPropertiesExtensions.GetPropertyValue<decimal>(builder);
        Assert.Null(p1Default);

        var p2 = 1.2345M;
        WebApplicationPropertiesExtensions.SetProperty<decimal>(builder, p2);
        var p3 = WebApplicationPropertiesExtensions.GetPropertyValue<decimal>(builder);

        Assert.NotNull(p3);
        Assert.Equal(p3, p2);
    }

    [Fact]
    public void KeyNullableStructSuccess()
    {
        var builder = WebApplication.CreateBuilder();
        var key = "theKeyForNullable123";

        var p1Default = WebApplicationPropertiesExtensions.GetPropertyValue<decimal>(builder, key);
        Assert.Null(p1Default);

        var p2 = 1.2345M;
        WebApplicationPropertiesExtensions.SetProperty<decimal>(builder, key, p2);
        var p3 = WebApplicationPropertiesExtensions.GetPropertyValue<decimal>(builder, key);

        Assert.NotNull(p3);
        Assert.Equal(p3, p2);
    }

    internal class PropertyValue
    {
    }
}
