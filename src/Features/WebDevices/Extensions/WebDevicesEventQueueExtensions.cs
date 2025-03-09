using Conesoft.Services.HomeDevicesControl.Features.Events.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Conesoft.Services.HomeDevicesControl.Features.WebDevices.Extensions;

public static class WebDevicesEventQueueExtensions
{
    public static IServiceCollection AddWebDevicesEvents(this IServiceCollection services)
    {
        return services.AddEventQueueEvents(events =>
         {
             events.Add<ProcessWebButtonIdEvent>();
             events.Add<ProcessHomeStateEvent>();
             events.Add<ProcessNoEvent>();
             events.Add<ProcessNotificationEvent>();
         });
    }
}