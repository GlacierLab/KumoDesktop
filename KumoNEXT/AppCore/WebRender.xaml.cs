using Microsoft.Web.WebView2.Core;
using Microsoft.WindowsAPICodePack.Taskbar;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

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
            AppID = "Kumo." + PkgManifest.Name + "." + new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString()[3..];
            Init(PkgManifest);
        }
        public WebRender(Scheme.PkgManifest PkgManifest,string ID,string Entry)
        {
            AppID = ID;
            Init(PkgManifest);
            //TODO
            //单一包多窗口扩展实现
        }

        public Scheme.PkgManifest? ParsedManifest;
        public Scheme.PkgLocalData? ParsedLocalData;
        string PkgPath = "";
        string AppID = "Kumo.Init";

        private async void Init(Scheme.PkgManifest PkgManifest)
        {
            InitializeComponent();
            ParsedManifest = PkgManifest;
            PkgPath = "Package\\" + ParsedManifest.Name.Replace(".", "\\") + "\\";
            //读取配置文件
            try
            {
                using Stream JsonStream=File.OpenRead("PackageData\\" + ParsedManifest.Name + ".json");
                {
                    ParsedLocalData = JsonSerializer.Deserialize<Scheme.PkgLocalData>(JsonStream);
                    JsonStream.Dispose();
                }
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
            if (ParsedManifest.AllowResize)
            {
                this.ResizeMode = ResizeMode.CanResize;
            }
            if (File.Exists(PkgPath + "icon.png"))
            {
                Stream imageStreamSource = new FileStream(PkgPath + "icon.png", FileMode.Open, FileAccess.Read, FileShare.Read);
                PngBitmapDecoder decoder = new PngBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                BitmapSource bitmapSource = decoder.Frames[0];
                this.Icon = bitmapSource;
            }
            //设置AppID
            SourceInitialized += (s, e) =>
            {
                TaskbarManager.Instance.SetApplicationIdForSpecificWindow(new WindowInteropHelper(this).Handle, AppID);
            };
            await App.InitAppWebView();
            await WebView.EnsureCoreWebView2Async(App.WebView2Environment);
            WebView.CoreWebView2.AddHostObjectToScript("KumoBridge", new KumoBridge(this));
            WebView.CoreWebView2.SetVirtualHostNameToFolderMapping(ParsedManifest.Domain,
        PkgPath, CoreWebView2HostResourceAccessKind.DenyCors);
            WebView.CoreWebView2.Settings.IsBuiltInErrorPageEnabled = false;
            WebView.CoreWebView2.Settings.IsSwipeNavigationEnabled = false;
            WebView.CoreWebView2.Settings.IsZoomControlEnabled = false;
            WebView.CoreWebView2.Settings.IsPinchZoomEnabled = false;
            if (App.MainConfig.EnableDebug == false)
            {
                WebView.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;
            }
            WebView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            //域名白名单机制
            Func<string, bool> CheckWhitelist = (string u) =>
            {
                string CurrentDomain = new Uri(u).DnsSafeHost;
                return ((CurrentDomain == ParsedManifest.Domain) || ParsedManifest.TrustedDomain.Contains(CurrentDomain) || u.StartsWith("data"));
            };
            WebView.CoreWebView2.NavigationStarting += (a, e) =>
            {
                if (!CheckWhitelist(e.Uri))
                {
                    e.Cancel = true;
                    ProcessStartInfo startInfo = new(e.Uri);
                    startInfo.UseShellExecute = true;
                    Process.Start(startInfo);
                }
            };
            //新窗口打开
            WebView.CoreWebView2.NewWindowRequested += (a, e) =>
            {
                e.Handled = true;
                if (!CheckWhitelist(e.Uri))
                {
                    ProcessStartInfo startInfo = new(e.Uri);
                    startInfo.UseShellExecute = true;
                    Process.Start(startInfo);
                }
                else
                {
                    new TinyPWA(ParsedManifest.Name, true, e.Uri).Show();
                }
            };
            //错误页面
            WebView.CoreWebView2.NavigationCompleted += (a, e) =>
            {
                if (e.IsSuccess)
                {
                }
                else
                {
                    if (e.WebErrorStatus != CoreWebView2WebErrorStatus.OperationCanceled)
                    {
                        WebView.CoreWebView2.NavigateToString(Properties.Resources.PWA_Error);
                    }
                }
            };
            WebView.CoreWebView2.Navigate("https://" + ParsedManifest.Domain + "/" + ParsedManifest.Entry);
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                WindowStyle = WindowStyle.SingleBorderWindow;
                WindowStyle = WindowStyle.None;
                this.BorderThickness = new Thickness(8);
                WebView.CoreWebView2.ExecuteScriptAsync("Callback.Window_State?Callback.Window_State(true):null;");
                WebView.Margin = new Thickness(0, 0, 0, 0);
            }
            else
            {
                this.BorderThickness = new Thickness(0);
                WebView.CoreWebView2.ExecuteScriptAsync("Callback.Window_State?Callback.Window_State(false):null;");
                WebView.Margin = new Thickness(1, 1, 1, 1);
            }
            if (this.WindowState == WindowState.Minimized)
            {
                WebView.CoreWebView2.TrySuspendAsync();
            };
        }

        bool ReadyToExit = false;
        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
            if (!ReadyToExit)
            {
                e.Cancel = true;
                if (ParsedManifest.SaveWindowSize)
                {
                    ParsedLocalData.Height = (int)Math.Round(Height);
                    ParsedLocalData.Width = (int)Math.Round(Width);
                    MinHeight = 30;
                    Height = 30;
                }
                FileStream? createStream=null;
                if (File.Exists("PackageData\\" + ParsedManifest.Name + ".json"))
                {
                   createStream = File.OpenWrite("PackageData\\" + ParsedManifest.Name + ".json");
                }
                else
                {
                    createStream = File.Create("PackageData\\" + ParsedManifest.Name + ".json");
                }
                await JsonSerializer.SerializeAsync(createStream, ParsedLocalData);
                await createStream.DisposeAsync();
                WebView.Dispose();
                ReadyToExit = true;
                Task.Run(async () =>
                {
                    await Task.Delay(1);
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        this.Close();
                    }));
                });
            }
        }
    }
}
