using Conesoft.Hosting;
using Conesoft.PwaGenerator;
using Conesoft.Services.HomeDevicesControl;
using Conesoft.Services.HomeDevicesControl.Components;
using Conesoft.Services.HomeDevicesControl.Features.DeviceApi.Extensions;
using Conesoft.Services.HomeDevicesControl.Features.Events.Extensions;
using Conesoft.Services.HomeDevicesControl.Features.Events.Interfaces;
using Conesoft.Services.HomeDevicesControl.Features.WebDevices.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddHostConfigurationFiles()
    .AddHostEnvironmentInfo()
    .AddLoggingService()
    .AddUsersWithStorage()
    .AddNotificationService()
    ;

builder.Services
    .AddCompiledHashCacheBuster()
    .AddHttpClient()
    .AddAntiforgery()
    .AddSingleton<ActionsList>()
    .AddEventQueue(events => events.AddErrorHandler(item =>
    {
        Log.Error("event not handled: {@event}", item);
        return Task.CompletedTask;
    }))
    .AddWebDevicesEvents()
    .AddRazorComponents().AddInteractiveServerComponents();

var app = builder.Build();

app.MapDevicesHttpApi();
app.MapPwaInformationFromAppSettings();
app.MapUsersWithStorage();
app.MapStaticAssets();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Map("/test", (IAddEvent addEvent) =>
{
    addEvent.Add(new Conesoft.Services.HomeDevicesControl.Features.WebDevices.Events.WebButtonIdEvent("C82B962E1F73-4"));
});

app.Run();




