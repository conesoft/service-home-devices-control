using Conesoft.Services.HomeDevicesControl.Features.Events.Types;

namespace Conesoft.Services.HomeDevicesControl.Features.WebDevices.Events;

public record WebButtonIdEvent(string Id) : Event
{
    public string SafeId => Id.Replace(":", "");
}
