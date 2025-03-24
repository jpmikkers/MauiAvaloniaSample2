#if WINDOWS10_0_19041_0_OR_GREATER
using Avalonia.Platform;
using Microsoft.Maui;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Hosting;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.Maui.Platforms.Windows
{
    public class AppThemeWindow : MauiWinUIWindow
    {
        public AppThemeWindow()
        {
            Avalonia.Application.Current.ActualThemeVariantChanged += (_, _) => SetAppTheme();
            SetAppTheme();

            void SetAppTheme()
            {
                SetTheme((PlatformThemeVariant?)Avalonia.Application.Current.ActualThemeVariant == PlatformThemeVariant.Dark
                    ? ElementTheme.Dark
                    : ElementTheme.Light);
            }
        }

        public void SetTheme(ElementTheme theme)
        {
            //var window = Microsoft.UI.Xaml.Window.Current;
            if (this.Content is FrameworkElement rootElement)
            {
                rootElement.RequestedTheme = theme;
            }
        }
    }
}
#endif
