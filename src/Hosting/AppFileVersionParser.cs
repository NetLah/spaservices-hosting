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
        string? version1 = null;
        var firstLine = true;
        var hasProperty = false;
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
                        hasProperty = true;
                        switch (key.ToLower())
                        {
                            case "app": SetIf(value, a => a.Title, (a, v) => a.Title = v); break;
                            case "version": SetIf(value, a => a.Version, (a, v) => a.Version = v); break;
                            case "buildtime":
                                var hasSetBuildTimeString = SetIf(value, a => a.BuildTimeString, (a, v) => a.BuildTimeString = v);
                                if (hasSetBuildTimeString)
                                {
                                    result.BuildTime = BuildTimeHelper.ParseBuildTime(value);
                                }
                                break;
                            case "description": SetIf(value, a => a.Description, (a, v) => a.Description = v); break;
                        }
                    }
                }

                if (index < 0 && firstLine)
                {
                    version1 = line.Trim();
                }
            }

            firstLine = false;
        }

        if (!hasProperty && !string.IsNullOrEmpty(version1))
        {
            result.Version = version1;
        }

        return result.IsValid() ? result : default;

        bool SetIf(string value, Func<AppFileVersionInfo, string?> funcGet, Action<AppFileVersionInfo, string> setValue)
        {
            var currentValue = funcGet(result);
            if (currentValue != null)
            {
                return false;
            }

            setValue(result, value);
            return true;
        }
    }
}
