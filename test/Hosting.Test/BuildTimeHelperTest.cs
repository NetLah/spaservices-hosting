namespace NetLah.Extensions.SpaServices.Hosting.Test;

public class BuildTimeHelperTest
{
    public static IEnumerable<object?[]> DateTimeFormats =>
        new List<object?[]>
        {
        new object?[] { "2021-05-08T05:25:59", new DateTimeOffset(2021, 5, 8, 5, 25, 59, TimeSpan.Zero) },
        new object?[] { "2021-05-08T05:25:59Z", new DateTimeOffset(2021, 5, 8, 5, 25, 59, TimeSpan.Zero) },
        new object?[] { "2021-05-08T05:25:59.1", new DateTimeOffset(2021, 5, 8, 5, 25, 59, 100, TimeSpan.Zero) },
        new object?[] { "2021-05-08T05:25:59.1Z", new DateTimeOffset(2021, 5, 8, 5, 25, 59, 100, TimeSpan.Zero) },
        new object?[] { "2021-05-08T05:25:59.12", new DateTimeOffset(2021, 5, 8, 5, 25, 59, 120, TimeSpan.Zero) },
        new object?[] { "2021-05-08T05:25:59.12Z", new DateTimeOffset(2021, 5, 8, 5, 25, 59, 120, TimeSpan.Zero) },
        new object?[] { "2021-05-08T05:25:59.123", new DateTimeOffset(2021, 5, 8, 5, 25, 59, 123, TimeSpan.Zero) },
        new object?[] { "2021-05-08T05:25:59.123Z", new DateTimeOffset(2021, 5, 8, 5, 25, 59, 123, TimeSpan.Zero) },
        new object?[] { "2021-05-08T05:25:59.1234", new DateTimeOffset(2021, 5, 8, 5, 25, 59, TimeSpan.Zero).Add(TimeSpan.FromMilliseconds( 123.4D)) },
        new object?[] { "2021-05-08T05:25:59.1234Z", new DateTimeOffset(2021, 5, 8, 5, 25, 59, TimeSpan.Zero).Add(TimeSpan.FromMilliseconds( 123.4D)) },
        new object?[] { "2021-05-08T05:25:59.12345", new DateTimeOffset(2021, 5, 8, 5, 25, 59, TimeSpan.Zero).Add(TimeSpan.FromMilliseconds( 123.45D)) },
        new object?[] { "2021-05-08T05:25:59.12345Z", new DateTimeOffset(2021, 5, 8, 5, 25, 59, TimeSpan.Zero).Add(TimeSpan.FromMilliseconds( 123.45D)) },
        new object?[] { "2021-05-08T05:25:59.123456", new DateTimeOffset(2021, 5, 8, 5, 25, 59, TimeSpan.Zero).Add(TimeSpan.FromMilliseconds( 123.456D)) },
        new object?[] { "2021-05-08T05:25:59.123456Z", new DateTimeOffset(2021, 5, 8, 5, 25, 59, TimeSpan.Zero).Add(TimeSpan.FromMilliseconds( 123.456D)) },
        new object?[] { "2021-05-08T05:25:59.1234567", new DateTimeOffset(2021, 5, 8, 5, 25, 59, TimeSpan.Zero).Add(TimeSpan.FromMilliseconds( 123.4567D)) },
        new object?[] { "2021-05-08T05:25:59.1234567Z", new DateTimeOffset(2021, 5, 8, 5, 25, 59, TimeSpan.Zero).Add(TimeSpan.FromMilliseconds( 123.4567D)) },
        new object?[] { "2021-05-08T05:25:59:1", null },
        new object?[] { "2021-05-08T05:25:59:1Z", null },
        new object?[] { "2021-05-08T05:25:59:12", null },
        new object?[] { "2021-05-08T05:25:59:12Z", null },
        new object?[] { "2021-05-08T05:25:59:123", null },
        new object?[] { "2021-05-08T05:25:59:123Z", null },
        new object?[] { "2021-05-08T05:25:59:1234", null },
        new object?[] { "2021-05-08T05:25:59:1234Z", null },
        new object?[] { "2021-05-08T05:25:59:12345", null },
        new object?[] { "2021-05-08T05:25:59:12345Z", null },
        new object?[] { "2021-05-08T05:25:59:123456", null },
        new object?[] { "2021-05-08T05:25:59:123456Z", null },
        new object?[] { "2021-05-08T05:25:59:1234567", null },
        new object?[] { "2021-05-08T05:25:59:1234567Z", null },
        };

    [Theory]
    [MemberData(nameof(DateTimeFormats))]
    public void ParseDateTimeUtcToDateTimeOffsetSuccess(string value, DateTimeOffset? expected)
    {
        var buildTime = BuildTimeHelper.ParseBuildTime(value);

        Assert.Equal(expected, buildTime);
        Assert.Equal(expected?.Offset, buildTime?.Offset);
    }
}
