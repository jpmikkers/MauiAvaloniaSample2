using Avalonia;
using Avalonia.Maui;
using AvaloniaSample;
using AvaloniaSample.Maui;
using AvaloniaSample.Views;
using CommunityToolkit.Maui;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Dispatching;
using Microsoft.Maui.Embedding;
using Microsoft.Maui.Hosting;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.XamlTypeInfo;
using Syncfusion.Maui.Core.Hosting;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace ClassLibray1
{
    internal sealed class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args)
        {
            var thread = new System.Threading.Thread(() => Avalonia.Maui.Platforms.Windows.WinUIApp.Start());
            thread.SetApartmentState(System.Threading.ApartmentState.STA);
            thread.Start();

            var builder = BuildAvaloniaApp();

            builder.StartWithClassicDesktopLifetime(args);
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
        {
            return AppBuilder.Configure<App>()
                .UseMaui<MauiApplication>(b =>
                {
                    b.Services.AddLogging(logging =>
                    {
                        //logging.AddDebug();
                    });
                    b.ConfigureSyncfusionCore()
                .UseMauiCommunityToolkit()
                .UseMauiCommunityToolkitMediaElement();

                    
                })
                .UsePlatformDetect();
        }
    }

    public class MauiApplication : Microsoft.Maui.Controls.Application
    {
    }
}
