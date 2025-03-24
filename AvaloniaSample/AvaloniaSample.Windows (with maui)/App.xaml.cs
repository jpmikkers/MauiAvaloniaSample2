using Avalonia.Controls.ApplicationLifetimes;
using Avalonia;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Application = Microsoft.Maui.Controls.Application;

namespace AvaloniaSample.WinUI
{
    public partial class App : Application
    {
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        ClassicDesktopStyleApplicationLifetime lifetime;

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = new Window()
            {
                Height = 0,
                Width = 0,
                Page = new ContentPage()
            };
            window.Activated += Window_Activated;

            return window;
        }

        private void Window_Activated(object? sender, EventArgs e)
        {
            var window = (Window)sender;
            var view = (MauiWinUIWindow)window.Handler.PlatformView;
            ShowWindow(view.WindowHandle, 0);
        }

        public App()
        {
            InitializeComponent();

            //MainPage = new AppShell();

            var builder = AppBuilder.Configure<AvaloniaSample.App>()
            .UsePlatformDetect()
            .LogToTrace();

            builder.AfterSetup(_ =>
            {

            });
            lifetime = new ClassicDesktopStyleApplicationLifetime
            {
                Args = null,
            };

            builder.SetupWithLifetime(lifetime);

            var process = Process.GetCurrentProcess();


            EventHandler handler = null;
            lifetime.MainWindow.Closed += handler = (sender, e) =>
            {
                lifetime.MainWindow.Closed -= handler;
                process.Kill();
            };
            
        }

        protected override void OnHandlerChanged()
        {
            lifetime.MainWindow.Show();
            base.OnHandlerChanged();
        }
    }
}
