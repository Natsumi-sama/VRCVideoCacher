﻿using EmbedIO;
using EmbedIO.Actions;
using EmbedIO.Files;
using EmbedIO.WebApi;
using Serilog;
using Swan.Logging;
using ILogger = Serilog.ILogger;

namespace VRCVideoCacher;

public class WebServer
{
    public static EmbedIO.WebServer server;
    public static ILogger Log = Program.Logger.ForContext("SourceContext", "WebServer");
    public static void Init()
    {
        server = CreateWebServer(ConfigManager.config.ytdlWebServerURL);
        server.RunAsync();  
    }
    
    private static EmbedIO.WebServer CreateWebServer(string url)
    {
        Swan.Logging.Logger.UnregisterLogger<ConsoleLogger>();
        Swan.Logging.Logger.RegisterLogger<WebServerLogger>();
        var server = new EmbedIO.WebServer(o => o
                .WithUrlPrefix(url)
                .WithMode(HttpListenerMode.EmbedIO))
            // First, we will configure our web server by adding Modules.
            .WithWebApi("/api", m => m
                .WithController<APIController>())
            .WithStaticFolder("/", ConfigManager.config.CachedAssetPath, true, m => m
                .WithContentCaching(true));

        // Listen for state changes.
        server.StateChanged += (s, e) => $"WebServer New State - {e.NewState}".Info();
        server.OnUnhandledException += OnUnhandledException;
        server.OnHttpException += OnHttpException;  
        return server;
    }

    private static Task OnHttpException(IHttpContext context, IHttpException httpexception)
    {
        Log.Information("[WEBSERVER]: " + httpexception.Message);
        return Task.CompletedTask;
    }

    private static Task OnUnhandledException(IHttpContext context, Exception exception)
    {
        Log.Information("[WEBSERVER]: " + exception.Message);
        return Task.CompletedTask;
    }
}