using Microsoft.Web.WebView2.Core;
using System.IO;
using System.Windows;

namespace KumoNEXT.AppCore
{
    /// <summary>
    /// Interaction logic for WebRender.xaml
    /// </summary>
    public partial class WebRender : Window
    {
        public WebRender(string Config="{}")
        {
            InitializeComponent();
            InitContent();
        }
        private async void InitContent()
        {
            await WebView.EnsureCoreWebView2Async(App.WebView2Environment);
            WebView.CoreWebView2.Navigate("https://ctfile.qinlili.bid/");
        }
    }
}
