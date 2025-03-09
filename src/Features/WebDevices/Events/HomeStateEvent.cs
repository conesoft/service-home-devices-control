using Conesoft.Services.HomeDevicesControl.Features.Events.Types;

namespace Conesoft.Services.HomeDevicesControl.Features.WebDevices.Events; 

public record HomeStateEvent(string Path, string Value) : Event;
