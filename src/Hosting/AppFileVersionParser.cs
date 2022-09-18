using System.Text.RegularExpressions;

namespace NetLah.Extensions.SpaServices.Hosting;

internal class AppFileVersionParser
{
    private static readonly Regex KeyValueRegex = new("^\\s*(?<key>app|version|buildTime|description)\\s*:(?<value>.{1,100})$", RegexOptions.Compiled);

#pragma warning disable CA1822 // Mark members as static
    public AppFileVersionInfo? Parse(TextReader reader)
#pragma warning restore CA1822 // Mark members as static
    {
        var result = new AppFileVersionInfo();
        string? line;
        string? version = null;
        var firstLine = true;
        var hasAnyProperty = false;
        while ((line = reader.ReadLine()) != null)
        {
            if (!string.IsNullOrWhiteSpace(line))
            {
                if (line.IndexOf(":") is { } index &&
                    index > 0 &&
                    KeyValueRegex.Match(line) is { } m &&
                    m.Success)
                {
                    var key = m.Groups["key"].Value;
                    var value = m.Groups["value"].Value.Trim();
                    if (!string.IsNullOrEmpty(value))
                    {
                        switch (key.ToLower())
                        {
                            case "app": hasAnyProperty = true; SetIf<int>(value, a => a.Title, (a, v) => a.Title = v); break;
                            case "version": hasAnyProperty = true; SetIf<int>(value, a => a.Version, (a, v) => a.Version = v); break;
                            case "buildtime":
                                var buildTime = SetIf(value, a => a.BuildTimeString, (a, v) => a.BuildTimeString = v, v => BuildTimeHelper.ParseBuildTime(value));
                                if (buildTime.HasValue)
                                {
                                    result.BuildTime = buildTime;
                                    hasAnyProperty = true;
                                }
                                break;
                            case "description": hasAnyProperty = true; SetIf<int>(value, a => a.Description, (a, v) => a.Description = v); break;
                        }
                    }
                }

                if (index < 0 && firstLine)
                {
                    version = line.Trim();
                }
            }

            firstLine = false;
        }

        if (!hasAnyProperty && !string.IsNullOrEmpty(version))
        {
            result.Version = version;
        }

        return result.IsValid() ? result : default;

        TValue? SetIf<TValue>(string value,
            Func<AppFileVersionInfo, string?> funcGet,
            Action<AppFileVersionInfo, string> setValue,
            Func<string, TValue?>? convert = null) where TValue : struct
        {
            TValue? conversionValue = default;

            var currentValue = funcGet(result);
            if (currentValue != null)
            {
                return conversionValue;
            }

            if (convert != null)
            {
                conversionValue = convert(value);
                if (conversionValue.HasValue)
                {
                    setValue(result, value);
                }
            }
            else
            {
                setValue(result, value);
            }

            return conversionValue;
        }
    }
}
