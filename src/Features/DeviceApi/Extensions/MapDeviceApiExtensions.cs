using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Serilog;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace Conesoft.Services.HomeDevicesControl.Features.DeviceApi.Extensions;

public static class MapDeviceApiExtensions
{
    public static WebApplication MapDevicesHttpApi(this WebApplication app)
    {
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
        });

        int imageIndex = 0;

        app.MapGet("/device/image-old", (HttpContext context) =>
        {
            var userAgent = context.Request.GetTypedHeaders().Headers.UserAgent;
            if (userAgent == "Conesoft-Web-Image")
            {
                if (context.Request.Headers.TryGetValue("Conesoft-Web-Image-Id", out var deviceid))
                {
                    BitArray bits = new(200 * 200);
                    Log.Information("preparing image for {device}", deviceid);
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
                    Log.Information("sending image to {device}", deviceid);
                    var bytes = bits.ToBytes();
                    for (var i = 0; i < bytes.Length; i++)
                    {
                        bytes[i] = bytes[i].Reverse();
                    }
                    return Results.Bytes(bytes);
                }
            }
            Log.Information("request from {device} with {*headers}", userAgent, context.Request.Headers);
            return Results.BadRequest();
        });

        app.MapGet("/device/image", async (HttpContext context) =>
        {
            var userAgent = context.Request.GetTypedHeaders().Headers.UserAgent;
            if (userAgent == "Conesoft-Web-Image")
            {
                if (context.Request.Headers.TryGetValue("Conesoft-Web-Image-Id", out var deviceid))
                {
                    BitArray bits = new(200 * 200);
                    Log.Information("preparing image for {device}", deviceid);

                    var font = SystemFonts.Get("Segoe UI Emoji").CreateFont(20, FontStyle.Bold);
                    HttpClient client = new();
                    var rssresponse = await client.GetStringAsync(@"https://bsky.app/profile/did:plc:jsfqqy7p34wxeqycehtdellg/rss");
                    var posturl = rssresponse.Split("<item>").Skip(1).First().Split("<link>").Skip(1).First().Split("</link>").FirstOrDefault();
                    var postresponse = await client.GetStringAsync(posturl);
                    var postcontent = postresponse.Split("<p id=\"bsky_post_text\">").Skip(1).First().Split("</p>").First();

                    using (Image<L8> image = new(200, 200))
                    {
                        image.Mutate(x =>
                            x
                            .Clear(Color.White)
                            .DrawText(
                                postcontent,
                                font,
                                Color.Black,
                                new(5, 25)
                            )
                            .Grayscale(GrayscaleMode.Bt709)
                            .Dither()
                        );
                        image.ProcessPixelRows(accessor =>
                        {
                            for (int y = 0; y < 200; y++)
                            {
                                Span<L8> pixelRow = accessor.GetRowSpan(y);
                                for (int x = 0; x < 200; x++)
                                {
                                    ref L8 pixel = ref pixelRow[x];
                                    bits[x + 200 * y] = pixel.PackedValue > (byte.MaxValue / 2 + byte.MaxValue / 16);
                                }
                            }
                        });
                    }

                    Log.Information("sending image to {device}", deviceid);
                    var bytes = bits.ToBytes();
                    for (var i = 0; i < bytes.Length; i++)
                    {
                        bytes[i] = bytes[i].Reverse();
                    }
                    return Results.Bytes(bytes);
                }
            }
            Log.Information("request from {device} with {*headers}", userAgent, context.Request.Headers);
            return Results.BadRequest();
        });

        return app;
    }
}
