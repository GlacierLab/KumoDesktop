using Microsoft.WindowsAPICodePack.Taskbar;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace KumoNEXT.AppCore
{
    /// <summary>
    /// Interaction logic for Preference.xaml
    /// </summary>
    public partial class Preference : Window
    {
        public Scheme.PkgManifest? ParsedManifest;
        public Scheme.PkgLocalData[] ParsedLocalData = new Scheme.PkgLocalData[1];


        public Preference(Scheme.PkgManifest Manifest, ref Scheme.PkgLocalData LocalData)
        {
            InitializeComponent(); 
            ParsedManifest = Manifest;
            ParsedLocalData[0] = LocalData;
            Title = "设置-" + Manifest.DisplayName;
            //渲染设置图标
            var SettingsIcon=this.FindResource("settings") as DrawingImage; 
            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawImage(SettingsIcon, new Rect(0, 0, 512, 512));
                var PkgPath = "Package\\" + ParsedManifest.Name.Replace(".", "\\") + "\\";
                if (File.Exists(PkgPath + "icon.png"))
                {
                    Stream imageStreamSource = new FileStream(PkgPath + "icon.png", FileMode.Open, FileAccess.Read, FileShare.Read);
                    PngBitmapDecoder decoder = new PngBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                    BitmapSource bitmapSource = decoder.Frames[0];
                    drawingContext.DrawImage(bitmapSource, new Rect(192, 192, 320, 320));
                }
                else
                {
                    var bitmap = new BitmapImage(new Uri("pack://application:,,,/KumoNEXT;component/Res/Launch.png"));
                    drawingContext.DrawImage(bitmap, new Rect(192, 192, 320, 320));
                }
            }
            RenderTargetBitmap bmp = new RenderTargetBitmap(512,512, 96, 96, PixelFormats.Pbgra32);
            bmp.Render(drawingVisual);
            Icon = bmp;
            //设置AppID
            SourceInitialized += (s, e) =>
            {
                TaskbarManager.Instance.SetApplicationIdForSpecificWindow(new WindowInteropHelper(this).Handle, "Kumo." + ParsedManifest.Name + "Settings");
            };
            AsyncInit();
        }

        private async void AsyncInit()
        {
            await WebView.EnsureCoreWebView2Async(App.WebView2Environment).ConfigureAwait(true);
            WebView.CoreWebView2.AddHostObjectToScript("PreferenceBridge", new PreferenceBridge(this));
            WebView.CoreWebView2.Settings.IsBuiltInErrorPageEnabled = false;
            WebView.CoreWebView2.Settings.IsSwipeNavigationEnabled = false;
            WebView.CoreWebView2.Settings.IsZoomControlEnabled = false;
            WebView.CoreWebView2.Settings.IsPinchZoomEnabled = false;
            if (App.MainConfig.EnableDebug == false)
            {
                WebView.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;
            }
            WebView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            WebView.CoreWebView2.NavigateToString(Properties.Resources.Preference); 
            WebView.CoreWebView2.OpenDevToolsWindow();
        }

        //同步设置后再关闭窗口
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }



    }
}
