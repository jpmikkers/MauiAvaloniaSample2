#if WINDOWS10_0_19041_0_OR_GREATER
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.XamlTypeInfo;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;
using System.Threading;

namespace Avalonia.Maui.Platforms.Windows
{
    public class WinUIApp : Microsoft.UI.Xaml.Application, IXamlMetadataProvider
    {
        private Window _mainWindow;
        private XamlControlsXamlMetaDataProvider provider;

        public WinUIApp() { }

        //[STAThread]
        public static void Start()
        {
            Microsoft.UI.Xaml.Application.Start(
                (ApplicationInitializationCallbackParams callback) =>
                {
                    var uiContext = new DispatcherQueueSynchronizationContext(DispatcherQueue.GetForCurrentThread());
                    SynchronizationContext.SetSynchronizationContext(uiContext);
                    new WinUIApp();
                }
            );
        }

        public IXamlType GetXamlType(string fullName)
        {
            return provider.GetXamlType(fullName);
        }

        public IXamlType GetXamlType(Type type)
        {
            return provider.GetXamlType(type);
        }

        public XmlnsDefinition[] GetXmlnsDefinitions()
        {
            return provider.GetXmlnsDefinitions();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            XamlControlsXamlMetaDataProvider.Initialize();
            provider = new();

            Resources.MergedDictionaries.Add(new XamlControlsResources());

            _mainWindow = new Window();
            _mainWindow.Activate();
        }
    }
}
#endif
