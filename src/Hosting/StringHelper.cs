namespace NetLah.Extensions.SpaServices.Hosting;

internal static class StringHelper
{
    public static string GetOrDefault(params string?[] values)
    {
        foreach (var value in values)
        {
            if (!string.IsNullOrWhiteSpace(value))
                return value;
        }

        return string.Empty;
    }
}
