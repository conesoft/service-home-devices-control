using Conesoft.Files;
using Conesoft.Hosting;
using Conesoft.Services.HomeDevicesControl.Features.Events.Interfaces;
using Conesoft.Services.HomeDevicesControl.Features.WebDevices.Events;
using Serilog;
using System.Threading.Tasks;

namespace Conesoft.Services.HomeDevicesControl.Features.WebDevices;

public class ProcessHomeStateEvent(HostEnvironment environment) : IProcessEvent<HomeStateEvent>
{
    public async Task Process(HomeStateEvent item)
    {
        var file = File.From(System.IO.Path.Combine(environment.Global.Storage.Path, System.IO.Path.ChangeExtension(item.Path, "txt")));
        await file.WriteText(item.Value);
        Log.Information("write {value} to {path} in {file}", item.Value, item.Path, file.Path);
    }
}