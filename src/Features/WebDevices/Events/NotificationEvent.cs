using Conesoft.Services.HomeDevicesControl.Features.Events.Types;

namespace Conesoft.Services.HomeDevicesControl.Features.WebDevices.Events;

public record NotificationEvent(string Message) : Event
{
    public string? Title { get; set; }
    public string? ImagePath { get; set; }
    public string? To { get; set; }
}