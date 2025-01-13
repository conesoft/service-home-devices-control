using Conesoft.Hosting;
using Conesoft.Services.HomeDevicesControl;
using Conesoft.Services.HomeDevicesControl.Components;
using Conesoft.Services.HomeDevicesControl.Features.DevicesHttpPortConfiguration.Extensions;
using Conesoft.Services.HomeDevicesControl.Features.PwaInformation.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections;
using System.IO;

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

int imageIndex = 0;

app.MapGet("/device/image", (HttpContext context) =>
{
    var userAgent = context.Request.GetTypedHeaders().Headers.UserAgent;
    if (userAgent == "Conesoft-Web-Image")
    {
        if (context.Request.Headers.TryGetValue("Conesoft-Web-Image-Id", out var buttonid))
        {
            BitArray bits = new(200 * 200);
            Log.Information("preparing image for {device}", buttonid);
            using (var image = SixLabors.ImageSharp.Image.Load<L8>(@$"wwwroot/images/{imageIndex}.png"))
            {
                image.Mutate(x => x.Resize(200, 200));
                image.ProcessPixelRows(accessor =>
                {
                    for (int y = 0; y < 200; y++)
                    {
                        Span<L8> pixelRow = accessor.GetRowSpan(y);
                        for (int x = 0; x < 200; x++)
                        {
                            ref L8 pixel = ref pixelRow[x];
                            bits[x + 200 * y] = pixel.PackedValue > byte.MaxValue / 2;
                        }
                    }
                });
            }
            imageIndex = (imageIndex + 1) % Directory.GetFiles("wwwroot/images", "*.png").Length;
            Log.Information("sending image to {device}", buttonid);
            var bytes = bits.ToBytes();
            for (var i = 0; i < bytes.Length; i ++)
            {
                bytes[i] = bytes[i].Reverse();
            }
            return Results.Bytes(bytes);
        }
    }
    Log.Information("request from {device} with {*headers}", userAgent, context.Request.Headers);
    return Results.BadRequest();
});

app
    .MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddEndpointFilter(async (context, next) => context.HttpContext.Request.IsHttps ? await next(context) : Results.BadRequest());

app.Run();

static class Ext
{
    public static byte ToByte(this BitArray bits)
    {
        if (bits.Count != 8)
        {
            throw new ArgumentException(null, nameof(bits));
        }
        byte[] bytes = new byte[1];
        bits.CopyTo(bytes, 0);
        return bytes[0];
    }
    public static byte[] ToBytes(this BitArray bits)
    {
        byte[] bytes = new byte[bits.Length / 8];
        bits.CopyTo(bytes, 0);
        return bytes;
    }

    public static byte Reverse(this byte b)
    {
        int a = 0;
        for (int i = 0; i < 8; i++)
            if ((b & (1 << i)) != 0)
                a |= 1 << (7 - i);
        return (byte)a;
    }
}





