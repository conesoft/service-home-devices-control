using Conesoft.Files;
using Conesoft.Hosting;
using Conesoft.Services.HomeDevicesControl.Features.Events.Interfaces;
using Conesoft.Services.HomeDevicesControl.Features.Events.Types;
using Conesoft.Services.HomeDevicesControl.Features.WebDevices.Events;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text.Json;
using System.Threading.Tasks;

class ProcessWebButtonIdEvent(HostEnvironment environment, IServiceProvider services) : IProcessEvent<WebButtonIdEvent>
{
    readonly JsonSerializerOptions options = Json.DefaultOptions.WithPolymorphicTypesFor<Event>(configure =>
    {
        configure
            .IncludeTypesFromNamespaceWith<HomeStateEvent>()
            ;
    });

    public async Task Process(WebButtonIdEvent item)
    {
        var addEvent = services.GetRequiredService<IAddEvent>();
        var file = environment.Local.Storage / "configuration" / Filename.From(item.SafeId, "json");
        if ((await file.ReadFromJson<Event[]>(options)) is Event[] events)
        {
            foreach (var _event in events)
            {
                await addEvent.Add(_event);
            }
        }
        else
        {
            await file.WriteAsJson<Event[]>([new NoEvent(item.SafeId)], options);
        }
    }
}
