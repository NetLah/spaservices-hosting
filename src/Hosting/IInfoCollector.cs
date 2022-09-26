namespace NetLah.Extensions.SpaServices.Hosting;

public interface IInfoCollector
{
    List<string> Logs { get; }

    IInfoCollector Add<T>(string key, T value);
}
