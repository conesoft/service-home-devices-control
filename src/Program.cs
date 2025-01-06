using Conesoft.Hosting;
using Conesoft.Services.HomeDevicesControl;
using Conesoft.Services.HomeDevicesControl.Components;
using Conesoft.Services.HomeDevicesControl.Features.DevicesHttpPortConfiguration.Extensions;
using Conesoft.Services.HomeDevicesControl.Features.PwaInformation.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddHostConfigurationFiles()
    .AddHostEnvironmentInfo()
    .AddLoggingService()
    .AddUsersWithStorage()
    .AddDevicesHttpPortConfiguration()
    ;

builder.Services
    .AddCompiledHashCacheBuster()
    .AddHttpClient()
    .AddAntiforgery()
    .AddSingleton<ActionsList>()
    .AddRazorComponents().AddInteractiveServerComponents();

var app = builder.Build();

app
    .UseDefaultFiles()
    .UseStaticFiles()
    .UseAntiforgery();

app.MapPwaInformationFromAppSettings();

app.MapUsersWithStorage();

app.MapGet("/device/action", (HttpContext context, ActionsList actions) =>
{
    var userAgent = context.Request.GetTypedHeaders().Headers.UserAgent;
    if (userAgent == "Conesoft-Web-Button")
    {
        if (context.Request.Headers.TryGetValue("Conesoft-Web-Button-Id", out var buttonid))
        {
            var id = buttonid.ToString();
            actions.Add(id);
            return Results.Ok();
        }
    }
    return Results.BadRequest();
}).AddEndpointFilter(async (context, next) => context.HttpContext.Request.IsHttps ? Results.BadRequest() : await next(context));

app
    .MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddEndpointFilter(async (context, next) => context.HttpContext.Request.IsHttps ? await next(context) : Results.BadRequest());

app.Run();