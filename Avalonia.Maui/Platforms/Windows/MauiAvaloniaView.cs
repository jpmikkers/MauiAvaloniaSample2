#if WINDOWS10_0_19041_0_OR_GREATER
using Avalonia.Maui.Controls;
using System;
using System.Runtime.InteropServices;
using Avalonia.Platform; // Add this using directive
using AvaloniaControl = Avalonia.Controls.Control;
using Avalonia.Controls.Embedding;
using System.Reflection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Hosting;
using Avalonia.Media;
using Microsoft.UI.Xaml.Media;
using Microsoft.Web.WebView2.Core;
using Microsoft.Maui.Controls;
using SkiaSharp;
using Microsoft.Maui;

namespace Avalonia.Maui.Windows
{
    public partial class MauiAvaloniaView : Microsoft.UI.Xaml.Controls.ContentControl
    {        

        readonly AvaloniaView _mauiView;

        public MauiAvaloniaView(AvaloniaView mauiView)
        {
            _mauiView = mauiView;
        }
     
        public void UpdateContent()
        {
            Content = Content;
        }
    }
}
#endif