namespace NetLah.Extensions.SpaServices.Hosting.Test;

public class BuildTimeHelperTest
{
    public class TestData : TheoryData<string, DateTimeOffset?>
    {
        public TestData()
        {
            Add("2021-05-08T05:25:59", new DateTimeOffset(2021, 5, 8, 5, 25, 59, TimeSpan.Zero));
            Add("2021-05-08T05:25:59Z", new DateTimeOffset(2021, 5, 8, 5, 25, 59, TimeSpan.Zero));
            Add("2021-05-08T05:25:59.1", new DateTimeOffset(2021, 5, 8, 5, 25, 59, 100, TimeSpan.Zero));
            Add("2021-05-08T05:25:59.1Z", new DateTimeOffset(2021, 5, 8, 5, 25, 59, 100, TimeSpan.Zero));
            Add("2021-05-08T05:25:59.12", new DateTimeOffset(2021, 5, 8, 5, 25, 59, 120, TimeSpan.Zero));
            Add("2021-05-08T05:25:59.12Z", new DateTimeOffset(2021, 5, 8, 5, 25, 59, 120, TimeSpan.Zero));
            Add("2021-05-08T05:25:59.123", new DateTimeOffset(2021, 5, 8, 5, 25, 59, 123, TimeSpan.Zero));
            Add("2021-05-08T05:25:59.123Z", new DateTimeOffset(2021, 5, 8, 5, 25, 59, 123, TimeSpan.Zero));
            Add("2021-05-08T05:25:59.1234", new DateTimeOffset(2021, 5, 8, 5, 25, 59, TimeSpan.Zero).Add(TimeSpan.FromMilliseconds(123.4D)));
            Add("2021-05-08T05:25:59.1234Z", new DateTimeOffset(2021, 5, 8, 5, 25, 59, TimeSpan.Zero).Add(TimeSpan.FromMilliseconds(123.4D)));
            Add("2021-05-08T05:25:59.12345", new DateTimeOffset(2021, 5, 8, 5, 25, 59, TimeSpan.Zero).Add(TimeSpan.FromMilliseconds(123.45D)));
            Add("2021-05-08T05:25:59.12345Z", new DateTimeOffset(2021, 5, 8, 5, 25, 59, TimeSpan.Zero).Add(TimeSpan.FromMilliseconds(123.45D)));
            Add("2021-05-08T05:25:59.123456", new DateTimeOffset(2021, 5, 8, 5, 25, 59, TimeSpan.Zero).Add(TimeSpan.FromMilliseconds(123.456D)));
            Add("2021-05-08T05:25:59.123456Z", new DateTimeOffset(2021, 5, 8, 5, 25, 59, TimeSpan.Zero).Add(TimeSpan.FromMilliseconds(123.456D)));
            Add("2021-05-08T05:25:59.1234567", new DateTimeOffset(2021, 5, 8, 5, 25, 59, TimeSpan.Zero).Add(TimeSpan.FromMilliseconds(123.4567D)));
            Add("2021-05-08T05:25:59.1234567Z", new DateTimeOffset(2021, 5, 8, 5, 25, 59, TimeSpan.Zero).Add(TimeSpan.FromMilliseconds(123.4567D)));
            Add("2021-05-08T05:25:59:1", null);
            Add("2021-05-08T05:25:59:1Z", null);
            Add("2021-05-08T05:25:59:12", null);
            Add("2021-05-08T05:25:59:12Z", null);
            Add("2021-05-08T05:25:59:123", null);
            Add("2021-05-08T05:25:59:123Z", null);
            Add("2021-05-08T05:25:59:1234", null);
            Add("2021-05-08T05:25:59:1234Z", null);
            Add("2021-05-08T05:25:59:12345", null);
            Add("2021-05-08T05:25:59:12345Z", null);
            Add("2021-05-08T05:25:59:123456", null);
            Add("2021-05-08T05:25:59:123456Z", null);
            Add("2021-05-08T05:25:59:1234567", null);
            Add("2021-05-08T05:25:59:1234567Z", null);
        }
    }

    [Theory]
    [ClassData(typeof(TestData))]
    public void ParseDateTimeUtcToDateTimeOffsetSuccess(string value, DateTimeOffset? expected)
    {
        var buildTime = BuildTimeHelper.ParseBuildTime(value);

        Assert.Equal(expected, buildTime);
        Assert.Equal(expected?.Offset, buildTime?.Offset);
    }
}
