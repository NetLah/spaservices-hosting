using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace NetLah.Extensions.SpaServices.Hosting;

internal partial class AppFileVersionParser
{
#if NET7_0_OR_GREATER
    private static readonly Regex KeyValueRegex = KeyValueRegexClass();

    [GeneratedRegex("^\\s*(?<key>app|version|buildTime|description)\\s*:(?<value>.{1,100})$", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex KeyValueRegexClass();
#else
    private static readonly Regex KeyValueRegex = new("^\\s*(?<key>app|version|buildTime|description)\\s*:(?<value>.{1,100})$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
#endif

#pragma warning disable CA1822 // Mark members as static
    public AppFileVersionInfo? Parse(TextReader reader)
#pragma warning restore CA1822 // Mark members as static
    {
        var result = new AppFileVersionInfo();
        string? line;
        string version = default!;
        var firstLine = true;
        var hasAnyProperty = false;
        while ((line = reader.ReadLine()) != null)
        {
            if (!string.IsNullOrWhiteSpace(line))
            {
                var index = line.IndexOf(':');
                if (index > 0 &&
                    KeyValueRegex.Match(line) is { } m &&
                    m.Success)
                {
                    var key = m.Groups["key"].Value;
                    var value = m.Groups["value"].Value.Trim();
                    if (!string.IsNullOrEmpty(value))
                    {
                        switch (key.ToLower())
                        {
                            case "app": hasAnyProperty = true; SetIf<int>(value, a => a.Title); break;
                            case "version": hasAnyProperty = true; SetIf<int>(value, a => a.Version); break;
                            case "buildtime":
                                var buildTime = SetIf(value, a => a.BuildTimeString, v => BuildTimeHelper.ParseBuildTime(value));
                                if (buildTime.HasValue)
                                {
                                    result.BuildTime = buildTime;
                                    hasAnyProperty = true;
                                }
                                break;
                            case "description": hasAnyProperty = true; SetIf<int>(value, a => a.Description); break;
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
            Expression<Func<AppFileVersionInfo, string?>> selector,
            Func<string, TValue?>? convert = null) where TValue : struct
        {
            TValue? conversionValue = default;

            var currentValue = selector.Compile().Invoke(result);
            if (currentValue != null)
            {
                return conversionValue;
            }

            if (convert != null)
            {
                conversionValue = convert(value);
                if (conversionValue.HasValue)
                {
                    SetValue();
                }
            }
            else
            {
                SetValue();
            }

            return conversionValue;

            void SetValue()
            {
                var propertyInfo = (PropertyInfo)((MemberExpression)selector.Body).Member;
                propertyInfo.SetValue(result, value);
            }
        }
    }
}
