using Conesoft.Files;
using Conesoft.Notifications;
using Conesoft.Services.HomeDevicesControl.Features.Events.Interfaces;
using Conesoft.Services.HomeDevicesControl.Features.WebDevices.Events;
using System.Threading.Tasks;

namespace Conesoft.Services.HomeDevicesControl.Features.WebDevices;

public class ProcessNotificationEvent(Notifier notifier) : IProcessEvent<NotificationEvent>
{
    public Task Process(NotificationEvent item)
    {
        var path = item.ImagePath != null ? Directory.Common.Current / "wwwroot" / "cooper" / Filename.FromExtended(item.ImagePath) : null;
        notifier.Notify(item.Title, item.Message, image: path, to: item.To);
        return Task.CompletedTask;
    }
}