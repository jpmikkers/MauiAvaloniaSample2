using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Runtime.Loader;
using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Maui.Handlers;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Dispatching;
using Microsoft.Maui.Embedding;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Platform;
using Moq;
using MauiWindow = Microsoft.Maui.Controls.Window;
#if IOS
using PlatformWindow = UIKit.UIWindow;
using UIKit;
#elif ANDROID
using PlatformWindow = global::Android.Content.Context;
#else
using PlatformWindow = System.Object;
#endif

namespace Avalonia.Maui;

public static class AvaloniaAppBuilderExtensions
{
#if WINDOWS10_0_19041_0_OR_GREATER
    public static MauiWinUIWindow WinUIWindow { get; set; }
#endif

    public static MauiContext Context { get; set; }

#if ANDROID
    public static AppBuilder UseMaui<TMauiApplication>(this AppBuilder appBuilder, global::Android.App.Activity activity, Action<MauiAppBuilder>? configure = null)
        where TMauiApplication : Microsoft.Maui.Controls.Application
#elif IOS
    public static AppBuilder UseMaui<TMauiApplication>(this AppBuilder appBuilder, IUIApplicationDelegate applicationDelegate, Action<MauiAppBuilder>? configure  = null)
        where TMauiApplication : Microsoft.Maui.Controls.Application
#elif WINDOWS10_0_19041_0_OR_GREATER
    public static AppBuilder UseMaui<TMauiApplication>(this AppBuilder appBuilder, Action<MauiAppBuilder>? configure = null)
        where TMauiApplication : Microsoft.Maui.Controls.Application
#else
    public static AppBuilder UseMaui<TMauiApplication>(this AppBuilder appBuilder, Action<MauiAppBuilder>? configure  = null)
        where TMauiApplication : Microsoft.Maui.Controls.Application
#endif
    {
        return appBuilder
            .AfterSetup(appBuilder =>
            {
                var builder = MauiApp.CreateBuilder()
                    .UseMauiEmbedding<TMauiApplication>();

                builder.Services.AddSingleton(appBuilder.Instance!)
#if ANDROID
                    .AddSingleton(activity.Application!)
                    .AddSingleton<global::Android.Content.Context>(activity)
                    .AddSingleton(activity)
#elif IOS
	                .AddSingleton(applicationDelegate ?? UIApplication.SharedApplication.Delegate)
	                .AddSingleton<UIWindow>(static p => p.GetService<IUIApplicationDelegate>()!.GetWindow())
#endif
                    .AddSingleton<IMauiInitializeService, MauiEmbeddingInitializer>();

#if WINDOWS10_0_19041_0_OR_GREATER

                Microsoft.UI.Dispatching.DispatcherQueueController.CreateOnCurrentThread();


                Microsoft.UI.Xaml.Hosting.WindowsXamlManager.InitializeForCurrentThread();

                WinUIWindow = new MauiWinUIWindow();
                
                Microsoft.Maui.ApplicationModel.Platform.OnPlatformWindowInitialized(WinUIWindow);

#endif

                configure?.Invoke(builder);

                Context = new MauiContext(builder.Services.BuildServiceProvider());

#if !WINDOWS10_0_19041_0_OR_GREATER
                var mauiApp = builder.Build();
                    InitializeMauiEmbeddingApp(mauiApp);
#endif

            });
    }

    private static void InitializeMauiEmbeddingApp(this MauiApp mauiApp)
    {
        var iApp = mauiApp.Services.GetRequiredService<IApplication>();

#if ANDROID
        var window = mauiApp.Services.GetRequiredService<global::Android.App.Activity>();
        var scope = mauiApp.Services.CreateScope();
        var platformApplication = window.Application!;
        var services = scope.ServiceProvider;
        var rootContext = new MauiContext(scope.ServiceProvider, window);

        Microsoft.Maui.ApplicationModel.Platform.Init(window, null);
#else
		var rootContext = new MauiContext(mauiApp.Services);
	    var services = mauiApp.Services;
#if IOS
	    var platformApplication = mauiApp.Services.GetRequiredService<IUIApplicationDelegate>();
	    var window = platformApplication.GetWindow();
	    if (window == null)
	    {   // hack for older Avalonia versions.
		    window = new UIWindow();
		    platformApplication.SetWindow(window);
	    }

		Microsoft.Maui.ApplicationModel.Platform.Init(() => platformApplication.GetWindow().RootViewController!);
#else
	    var platformApplication = new object();
	    var window = new object();
#endif
#endif

        var scopedServices = rootContext.Services.GetServices<IMauiInitializeScopedService>();
        foreach (var service in scopedServices)
        {
            service.Initialize(rootContext.Services);
        }
#if !WINDOWS10_0_19041_0_OR_GREATER
        platformApplication.SetApplicationHandler(iApp, rootContext);
#endif
        IPlatformApplication.Current = new EmbeddingApplication(services, iApp);

        if (iApp is Microsoft.Maui.Controls.Application app
            && iApp.Windows is List<MauiWindow> windows)
        {
            var virtualWindow = CreateVirtualWindow(app, window);
            windows.Add(virtualWindow);
        }
    }

    private static Microsoft.Maui.Controls.Window CreateVirtualWindow(Microsoft.Maui.Controls.Application app, PlatformWindow? window)
    {
#if ANDROID
        var services = app.Handler!.MauiContext!.Services;
        var context = new MauiContext(services, services.GetRequiredService<global::Android.App.Activity>());
#elif WINDOWS10_0_19041_0_OR_GREATER
        var context = AvaloniaAppBuilderExtensions.Context;
#else
        var context = app.Handler.MauiContext;
#endif

        // Create a Maui Window and initialize a Handler shim. This will expose the actual Application Window
        var virtualWindow = new MauiWindow();
        virtualWindow.Handler = new EmbeddedWindowHandler
        {
            PlatformView = window,
            VirtualView = virtualWindow,
            MauiContext = context
        };
        // This ContentPage is necessary only to fool the Xaml Hot Reload.
        virtualWindow.Page = new ContentPage();
        return virtualWindow;
    }

    private record EmbeddingApplication(IServiceProvider Services, IApplication Application) : IPlatformApplication;
}