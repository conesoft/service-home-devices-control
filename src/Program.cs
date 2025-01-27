using Conesoft.Hosting;
using Conesoft.PwaGenerator;
using Conesoft.Services.HomeDevicesControl;
using Conesoft.Services.HomeDevicesControl.Components;
using Conesoft.Services.HomeDevicesControl.Features.DeviceApi.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddHostConfigurationFiles()
    .AddHostEnvironmentInfo()
    .AddLoggingService()
    .AddUsersWithStorage()
    ;

builder.Services
    .AddCompiledHashCacheBuster()
    .AddHttpClient()
    .AddAntiforgery()
    .AddSingleton<ActionsList>()
    .AddRazorComponents().AddInteractiveServerComponents();

var app = builder.Build();

app.MapDevicesHttpApi();
app.MapPwaInformationFromAppSettings();
app.MapUsersWithStorage();
app.MapStaticAssets();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();




