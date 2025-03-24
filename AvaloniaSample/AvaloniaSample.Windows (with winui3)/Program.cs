using Avalonia;
using Avalonia.Maui;
using AvaloniaSample;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core.Handlers;
using CommunityToolkit.Maui.Core.Views;
using CommunityToolkit.Maui.Views;
using Microsoft.UI.Xaml.Controls;
using Syncfusion.Maui.Core.Hosting;
using ZXing.Net.Maui.Controls;

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
            var builder = BuildAvaloniaApp();

            builder.StartWithClassicDesktopLifetime(args);
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
        {
            return AppBuilder.Configure<App>()
                .UseMaui<MauiApplication>((b) =>
                {
                    b.Services.AddLogging(logging =>
                    {
                        //logging.AddDebug();
                    });
                    b.ConfigureSyncfusionCore()
                    .UseMauiCommunityToolkit();
                    //.UseMauiCommunityToolkitMediaElement();

                    b.UseBarcodeReader();
                    b.ConfigureMauiHandlers(handlers =>
                    {
                        handlers.AddHandler<MediaElement, MediaElementHandlerEx>();
                    });
                })
                .UsePlatformDetect();
        }

        public static void Prefix()
        {
            
        }
    }

    public class MauiApplication : Microsoft.Maui.Controls.Application
    {

    }

    public class MediaElementHandlerEx : MediaElementHandler
    {
        public MediaElementHandlerEx() 
            : base(PropertyMapper, CommandMapper)
        {
        }

        protected override MauiMediaElement CreatePlatformView()
        {
            var view = new MediaPlayerElement()
            {
                //Source = UriMediaSource.FromUri("https://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4")
            };
            
            view.Source = Windows.Media.Core.MediaSource.CreateFromUri(new Uri("https://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4"));
            view.AutoPlay = true;
            return new MauiMediaElement(view);
        }
    }
}
