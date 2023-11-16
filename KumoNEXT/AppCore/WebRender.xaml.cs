using Microsoft.Web.WebView2.Core;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Media.Imaging;

namespace KumoNEXT.AppCore
{
    /// <summary>
    /// Interaction logic for WebRender.xaml
    /// </summary>
    public partial class WebRender : Window
    {
        public WebRender(string PkgName, bool HotInstall = false)
        {
            if (HotInstall)
            {
                //使用动态安装模式
            }
            else
            {
                Scheme.PkgManifest ParsedManifest;
                try
                {
                    ParsedManifest = JsonSerializer.Deserialize<Scheme.PkgManifest>(File.OpenRead("Package\\" + PkgName.Replace(".", "\\") + "\\manifest.json"));
                }
                catch (Exception)
                {
                    var result = MessageBox.Show(PkgName + "包体信息无法解析，是否打开更新模块尝试修复？", "包体异常", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        Process.Start(Environment.ProcessPath, "--type=ui --package=CorePkg.Update");
                    }
                    Environment.Exit(0);
                    return;
                }
                //简单PWA包直接启动PWA模块
                if (ParsedManifest.PWA)
                {
                    Process.Start(Environment.ProcessPath, "--type=pwa --package=" + ParsedManifest.Name);
                    Environment.Exit(0);
                    return;
                }
                Init(ParsedManifest);
            }
        }

        public WebRender(Scheme.PkgManifest PkgManifest)
        {
            Init(PkgManifest);
        }
        Scheme.PkgManifest? ParsedManifest;
        Scheme.PkgLocalData? ParsedLocalData;
        string PkgPath = "";
        private async void Init(Scheme.PkgManifest PkgManifest)
        {
            InitializeComponent();
            ParsedManifest = PkgManifest;
            PkgPath = "Package\\" + ParsedManifest.Name.Replace(".", "\\") + "\\";
            //读取配置文件
            try
            {
                ParsedLocalData = JsonSerializer.Deserialize<Scheme.PkgLocalData>(File.OpenRead("PackageData\\" + ParsedManifest.Name + ".json"));
            }
            catch (Exception)
            {
                ParsedLocalData = new Scheme.PkgLocalData();
                using FileStream createStream = File.Create("PackageData\\" + ParsedManifest.Name + ".json");
                {
                    JsonSerializer.Serialize(createStream, ParsedLocalData);
                    createStream.Dispose();
                }
            }
            //设置窗口大小
            if (ParsedLocalData.Height > 0 && ParsedLocalData.Width > 0 && ParsedManifest.SaveWindowSize)
            {
                Height = ParsedLocalData.Height;
                Width = ParsedLocalData.Width;
            }
            else
            {
                Height = ParsedManifest.Height;
                Width = ParsedManifest.Width;
            }
            Title = ParsedManifest.DisplayName;
            if (File.Exists(PkgPath + "icon.png"))
            {
                Stream imageStreamSource = new FileStream(PkgPath + "icon.png", FileMode.Open, FileAccess.Read, FileShare.Read);
                PngBitmapDecoder decoder = new PngBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                BitmapSource bitmapSource = decoder.Frames[0];
                this.Icon = bitmapSource;
            }
            await WebView.EnsureCoreWebView2Async(App.WebView2Environment);
            WebView.CoreWebView2.SetVirtualHostNameToFolderMapping(ParsedManifest.Domain,
        PkgPath, CoreWebView2HostResourceAccessKind.DenyCors);
            WebView.CoreWebView2.Navigate("https://" + ParsedManifest.Domain + "/" + ParsedManifest.Entry);
        }


    }
}
