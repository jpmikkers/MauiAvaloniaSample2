using CommunityToolkit.Maui;
using Syncfusion.Maui.Core.Hosting;
using ZXing.Net.Maui.Controls;

namespace AvaloniaSample.WinUI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder.UseMauiApp<AvaloniaSample.WinUI.App>().ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
            .ConfigureSyncfusionCore()            
            .UseBarcodeReader()            
            .UseMauiCommunityToolkit()
            .UseMauiCommunityToolkitMediaElement();

            return builder.Build();
        }
    }
}