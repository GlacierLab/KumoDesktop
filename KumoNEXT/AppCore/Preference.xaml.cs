using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace KumoNEXT.AppCore
{
    /// <summary>
    /// Interaction logic for Preference.xaml
    /// </summary>
    public partial class Preference : Window
    {
        Scheme.PkgManifest? ParsedManifest;
        Scheme.PkgLocalData[] ParsedLocalData = new Scheme.PkgLocalData[1];
        public Preference(Scheme.PkgManifest Manifest, ref Scheme.PkgLocalData LocalData)
        {
            InitializeComponent();
            ParsedManifest = Manifest;
            ParsedLocalData[0] = LocalData;
            Title = "设置-" + Manifest.DisplayName;
            //渲染设置图标
            var SettingsIcon=this.FindResource("settings") as DrawingImage; DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawImage(SettingsIcon, new Rect(0, 0, 512, 512));
            }
        }



        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        public class PreferenceBridge
        {
            Preference Window;
            public PreferenceBridge(Preference Window)
            {
                this.Window = Window;
            }

            public void Close()
            {
                Window.Close();
            }
            public string ReadPreference()
            {
                return "";
            }
        }
        private async void WebView_Loaded(object sender, RoutedEventArgs e)
        {
            await WebView.EnsureCoreWebView2Async(App.WebView2Environment).ConfigureAwait(true);
            WebView.CoreWebView2.AddHostObjectToScript("PreferenceBridge", new PreferenceBridge(this));
            WebView.CoreWebView2.NavigateToString(Properties.Resources.Preference);
        }
    }
}
