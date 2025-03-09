using Conesoft.Services.HomeDevicesControl.Features.Events.Interfaces;
using Conesoft.Services.HomeDevicesControl.Features.Events.Services;
using Conesoft.Services.HomeDevicesControl.Features.Events.Types;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Conesoft.Services.HomeDevicesControl.Features.Events.Extensions;

public static class EventQueueExtensions
{
    public static IServiceCollection AddEventQueue(this IServiceCollection services, Action<EventConfigurator>? events = default)
    {
        services.AddSingleton<EventQueue>();
        services.AddSingleton<IAddEvent>(s => s.GetRequiredService<EventQueue>());
        services.AddHostedService(s => s.GetRequiredService<EventQueue>());

        events?.Invoke(new(services));

        return services;
    }

    public static IServiceCollection AddEventQueueEvents(this IServiceCollection services, Action<EventConfigurator> events)
    {
        events(new(services));
        return services;
    }

    public class EventConfigurator(IServiceCollection services)
    {
        public void Add<T>() where T : class, IProcessEvent => services.AddTransient<IProcessEvent, T>();
        public void AddErrorHandler(Func<Event, Task> handler) => services.AddTransient<IProcessEvent>(s => new ErrorEventHandler(handler));
    }

    private class ErrorEventHandler(Func<Event, Task> handler) : IProcessEvent
    {
        public string Type => nameof(ErrorEventHandler);

        public Task Process(Event item) => handler(item);
    }
}