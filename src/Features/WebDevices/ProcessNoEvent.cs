using Conesoft.Services.HomeDevicesControl.Features.Events.Interfaces;
using Conesoft.Services.HomeDevicesControl.Features.WebDevices.Events;
using Serilog;
using System.Threading.Tasks;

namespace Conesoft.Services.HomeDevicesControl.Features.WebDevices;

public class ProcessNoEvent : IProcessEvent<NoEvent>
{
    public Task Process(NoEvent item)
    {
        Log.Error("no event registered for {id}", item.Id);
        return Task.CompletedTask;
    }
}