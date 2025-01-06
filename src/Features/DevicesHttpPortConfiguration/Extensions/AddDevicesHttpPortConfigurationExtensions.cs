using Conesoft.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Linq;

namespace Conesoft.Services.HomeDevicesControl.Features.DevicesHttpPortConfiguration.Extensions;

static class AddDevicesHttpPortConfigurationExtensions
{
    public static WebApplicationBuilder AddDevicesHttpPortConfiguration(this WebApplicationBuilder builder)
    {
        var urls = builder.Configuration["urls"];
        var devicePort = builder.Configuration.GetValue<int>("devices:port");
        var deviceDevPort = builder.Configuration.GetValue<int>("devices:dev-port");

        var environment = CreateHostEnvironmentFromConfigurationManually(builder.Configuration);

        var port = new DevicePort(environment.IsInHostedEnvironment ? devicePort : deviceDevPort);

        var httpsUrl = urls?.Split(";").FirstOrDefault(u => u.StartsWith("https")) ?? "https://127.0.0.1:0";
        builder.WebHost.UseUrls(httpsUrl, $"http://+:{port.Port}");
        builder.Services.AddSingleton(port);

        return builder;
    }

    private static HostEnvironment CreateHostEnvironmentFromConfigurationManually(ConfigurationManager configurationManager)
    {
        // can't use DI yet, but can access configuration already.
        var options = configurationManager.GetRequiredSection("hosting").Get<HostingOptions>();
        if (options == null)
        {
            throw new Exception("can't read the required hosting section");
        }
        return new HostEnvironment(Options.Create(options));
    }
}

public record DevicePort(int Port);