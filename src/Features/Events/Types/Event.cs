namespace Conesoft.Services.HomeDevicesControl.Features.Events.Types;

public record Event
{
    public string Type { get; private set; }
    public Event()
    {
        Type = GetType().Name;
    }
};