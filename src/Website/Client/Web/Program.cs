﻿using Tonrich.Client.Shared;
#if BlazorElectron
using ElectronNET.API;
using ElectronNET.API.Entities;
#endif
#if BlazorServer
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
#elif BlazorWebAssembly
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
#endif

namespace Tonrich.Client.Web;

public class Program
{
    public static async Task Main(string[] args)
    {
#if !BlazorWebAssembly && !BlazorServer
        throw new InvalidOperationException("Please switch to either blazor web assembly or server as described in readme.md");
#else
        await CreateHostBuilder(args)
            .RunAsync();
#endif
    }

#if BlazorWebAssembly
    public static WebAssemblyHost CreateHostBuilder(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault();
        builder.Configuration.AddJsonStream(typeof(MainLayout).Assembly.GetManifestResourceStream("Tonrich.Client.Shared.appsettings.json"));

        var apiServerAddressConfig = builder.Configuration.GetApiServerAddress();

        var apiServerAddress = new Uri($"{builder.HostEnvironment.BaseAddress}{apiServerAddressConfig}");

        builder.Services.AddSingleton(sp => new HttpClient(sp.GetRequiredService<AppHttpClientHandler>()) { BaseAddress = apiServerAddress });

        builder.Services.AddScoped<Microsoft.AspNetCore.Components.WebAssembly.Services.LazyAssemblyLoader>();
        builder.Services.AddTransient<IAuthTokenProvider, ClientSideAuthTokenProvider>();

        builder.Services.AddSharedServices();
        builder.Services.AddClientSharedServices(builder.Configuration);
        builder.Services.AddClientWebServices();

        var host = builder.Build();

#if MultilingualEnabled
        var preferredCultureCookie = ((IJSInProcessRuntime)host.Services.GetRequiredService<IJSRuntime>()).Invoke<string?>("window.App.getCookie", ".AspNetCore.Culture");
        CultureInfoManager.SetCurrentCulture(preferredCultureCookie);
#endif

        return host;
    }
#elif BlazorServer
    public static WebApplication CreateHostBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Configuration.AddJsonStream(typeof(MainLayout).Assembly.GetManifestResourceStream("Tonrich.Client.Shared.appsettings.json")!);
#if BlazorElectron
        builder.WebHost.UseElectron(args);
        builder.Services.AddElectron();
#endif

#if DEBUG
        if (OperatingSystem.IsWindows())
        {
            // The following line (using the * in the URL), allows the emulators and mobile devices to access the app using the host IP address.
            builder.WebHost.UseUrls("https://localhost:4001", "http://localhost:4000", "https://*:4001", "http://*:4000");
        }
#elif BlazorElectron
        builder.WebHost.UseUrls("https://localhost:4001", "http://localhost:4000");
#endif

        Startup.Services.Add(builder.Services, builder.Configuration);

        var app = builder.Build();

        Startup.Middlewares.Use(app, builder.Environment);

#if BlazorElectron
        Task.Run(async () =>
        {
            var window = await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions
            {
                AutoHideMenuBar = true,
                BackgroundColor = "#0D2960",
                WebPreferences = new WebPreferences
                {
                    NodeIntegration = false
                }
            }, "https://localhost:4001");

            window.OnClosed += delegate
            {
                app.Services.GetRequiredService<IHostApplicationLifetime>().StopApplication();
                Electron.App.Quit();
            };
        });
#endif

        return app;
    }
#endif
}
