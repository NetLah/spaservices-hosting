using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace NetLah.Extensions.SpaServices.Hosting;

public static class WebApplicationPropertiesExtensions
{
    public static TValue? GetProperty<TValue>(this WebApplicationBuilder builder)
        => builder.Host.GetProperty<TValue>();

    public static TValue? GetProperty<TValue>(this IHostBuilder builder)
        => builder.Properties.GetProperty<TValue>();

    public static TValue? GetProperty<TValue>(this WebApplicationBuilder builder, object key)
     => builder.Host.GetProperty<TValue>(key);

    public static TValue? GetProperty<TValue>(this IHostBuilder builder, object key)
        => builder.Properties.GetProperty<TValue>(key);

    public static TValue? GetProperty<TValue>(this IDictionary<object, object> properties)
       => properties.GetProperty<TValue>(typeof(TValue));

    public static TValue? GetProperty<TValue>(this IDictionary<object, object> properties, object key)
       => properties.TryGetValue(key ?? throw new ArgumentNullException(nameof(key)), out var valueObject) && valueObject is TValue value ? value : default;


    public static TValue? GetPropertyValue<TValue>(this WebApplicationBuilder builder) where TValue : struct
    => builder.Host.GetPropertyValue<TValue>();

    public static TValue? GetPropertyValue<TValue>(this IHostBuilder builder) where TValue : struct
        => builder.Properties.GetPropertyValue<TValue>();

    public static TValue? GetPropertyValue<TValue>(this WebApplicationBuilder builder, object key) where TValue : struct
     => builder.Host.GetPropertyValue<TValue>(key);

    public static TValue? GetPropertyValue<TValue>(this IHostBuilder builder, object key) where TValue : struct
        => builder.Properties.GetPropertyValue<TValue>(key);

    public static TValue? GetPropertyValue<TValue>(this IDictionary<object, object> properties) where TValue : struct
       => properties.GetPropertyValue<TValue>(typeof(TValue));

    public static TValue? GetPropertyValue<TValue>(this IDictionary<object, object> properties, object key) where TValue : struct
       => properties.TryGetValue(key ?? throw new ArgumentNullException(nameof(key)), out var valueObject) && valueObject is TValue value ? value : new TValue?();


    public static TValue SetProperty<TValue>(this WebApplicationBuilder builder, TValue value)
        => builder.Host.SetProperty(value);

    public static TValue SetProperty<TValue>(this IHostBuilder builder, TValue value)
        => builder.Properties.SetProperty(value);

    public static TValue SetProperty<TValue>(this WebApplicationBuilder builder, object key, TValue value)
        => builder.Host.SetProperty(key, value);

    public static TValue SetProperty<TValue>(this IHostBuilder builder, object key, TValue value)
        => builder.Properties.SetProperty(key, value);

    public static TValue SetProperty<TValue>(this IDictionary<object, object> properties, TValue value)
       => properties.SetProperty(typeof(TValue), value);

    public static TValue SetProperty<TValue>(this IDictionary<object, object> properties, object key, TValue value)
    {
        properties[key ?? throw new ArgumentNullException(nameof(key))] = value ?? throw new ArgumentNullException(nameof(value));
        return value;
    }

    internal static AppInfo GetAppInfoOrDefault(this WebApplicationBuilder builder)
    {
        if (builder.GetProperty<AppInfo>() is not AppInfo appInfo)
        {
            appInfo = new AppInfo();
            SetAppInfo(builder, appInfo);
        }
        return appInfo;
    }

    internal static WebApplicationBuilder SetAppInfo(this WebApplicationBuilder builder, AppInfo appInfo)
    {
        builder.Host.Properties.SetProperty(appInfo);
        return builder;
    }

    public static AppOptions GetAppOptionsOrDefault(this WebApplicationBuilder builder)
    {
        if (builder.GetProperty<AppOptions>() is not AppOptions appOptions)
        {
            appOptions = new AppOptions();
            SetAppOptions(builder, appOptions);
        }
        return appOptions;
    }

    public static WebApplicationBuilder SetAppOptions(this WebApplicationBuilder builder, AppOptions appOptions)
    {
        builder.Host.Properties.SetProperty(appOptions);
        return builder;
    }
}
