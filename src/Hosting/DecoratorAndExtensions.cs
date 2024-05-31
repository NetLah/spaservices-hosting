using Microsoft.Extensions.DependencyInjection;

namespace NetLah.Extensions.SpaServices.Hosting;

public interface IDecorator<out TService>
{
    TService Instance { get; }
}

internal class Decorator<TService>(TService instance) : IDecorator<TService>
{
    public TService Instance { get; } = instance;
}

internal class Decorator<TService, TImplementation>(TImplementation instance) : Decorator<TService>(instance)
    where TImplementation : class, TService
{
}

internal sealed class DisposableDecorator<TService>(TService instance) : Decorator<TService>(instance), IDisposable
{
    public void Dispose()
    {
        (Instance as IDisposable)?.Dispose();
    }
}

internal sealed class AsyncDisposableDecorator<TService>(TService instance) : Decorator<TService>(instance), IAsyncDisposable
{
    public ValueTask DisposeAsync()
    {
        return Instance is IAsyncDisposable asyncDisposable ? asyncDisposable.DisposeAsync() : ValueTask.CompletedTask;
    }
}

public static class DecoratorExtensions
{
    public static ServiceLifetime AddDecoratorAsLifetime<TService, TImplementation>(this IServiceCollection services)
        where TService : class
        where TImplementation : class, TService
    {
        var lifetime = services.AddDecorator<TService>();
        services.Add(new ServiceDescriptor(typeof(TService), typeof(TImplementation), lifetime));
        return lifetime;
    }

    private static ServiceLifetime AddDecorator<TService>(this IServiceCollection services)
    {
        var registration = services.LastOrDefault(x => x.ServiceType == typeof(TService))
            ?? throw new InvalidOperationException("Service type: " + typeof(TService).Name + " not registered.");

        if (services.Any(x => x.ServiceType == typeof(IDecorator<TService>)))
        {
            throw new InvalidOperationException("Decorator already registered for type: " + typeof(TService).Name + ".");
        }

        services.Remove(registration);

        var lifetime = registration.Lifetime;

        if (registration.ImplementationInstance != null)
        {
            var type = registration.ImplementationInstance.GetType();
            var innerType = typeof(Decorator<,>).MakeGenericType(typeof(TService), type);
            services.Add(new ServiceDescriptor(typeof(IDecorator<TService>), innerType, lifetime));
            services.Add(new ServiceDescriptor(type, registration.ImplementationInstance));
        }
        else if (registration.ImplementationFactory != null)
        {
            if (typeof(IAsyncDisposable).IsAssignableFrom(typeof(TService)))
            {
                services.Add(new ServiceDescriptor(typeof(IDecorator<TService>), provider =>
                {
                    return new AsyncDisposableDecorator<TService>((TService)registration.ImplementationFactory(provider));
                }, lifetime));
            }
            else
            {
                services.Add(new ServiceDescriptor(typeof(IDecorator<TService>), provider =>
                {
                    return new DisposableDecorator<TService>((TService)registration.ImplementationFactory(provider));
                }, lifetime));
            }
        }
        else
        {
            var type = registration.ImplementationType ?? throw new InvalidOperationException("ImplementationType is required");
            var innerType = typeof(Decorator<,>).MakeGenericType(typeof(TService), type);
            services.Add(new ServiceDescriptor(typeof(IDecorator<TService>), innerType, lifetime));
            services.Add(new ServiceDescriptor(type, type, lifetime));
        }

        return lifetime;
    }
}
