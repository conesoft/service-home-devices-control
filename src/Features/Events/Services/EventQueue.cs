using Conesoft.Services.HomeDevicesControl.Features.Events.Interfaces;
using Conesoft.Services.HomeDevicesControl.Features.Events.Types;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Conesoft.Services.HomeDevicesControl.Features.Events.Services;

public class EventQueue(IEnumerable<IProcessEvent> eventProcessors) : BackgroundService, IAddEvent
{
    readonly Channel<Event> channel = Channel.CreateUnbounded<Event>(new()
    {
        SingleWriter = true,
        SingleReader = true
    });

    readonly Dictionary<string, IProcessEvent> eventProcessorTypes = eventProcessors.ToDictionary(p => p.Type);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await channel.Reader.ReadAsync(stoppingToken).AsTask().NullIfCancelled() is Event item)
        {
            if (eventProcessorTypes.TryGetValue(item.Type, out var processor))
            {
                await processor.Process(item);
            }
            else if (eventProcessorTypes.TryGetValue(nameof(ErrorEventHandler), out var defaultProcessor))
            {
                await defaultProcessor.Process(item);
            }
            else
            {
                throw new NotImplementedException($"no handler found for event of type {item.Type}");
            }
        }
    }

    async Task IAddEvent.Add(Event item) => await channel.Writer.WriteAsync(item);
}
