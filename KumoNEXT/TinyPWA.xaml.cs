﻿using Microsoft.Web.WebView2.Core;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace KumoNEXT
{
    /// <summary>
    /// Interaction logic for TinyPWA.xaml
    /// </summary>
    public partial class TinyPWA : Window
    {
        public TinyPWA()
        {
            Init(null, false, null);
        }
        public TinyPWA(string? PkgName = null, bool NewWindow = false, string? NewWindowLink = null)
        {
            Init(PkgName, NewWindow, NewWindowLink);
        }
        public void Init(string? PkgName = null, bool NewWindow = false, string? NewWindowLink = null)
        {
            InitializeComponent();
            InitWebView(NewWindow, NewWindowLink);
            if (PkgName == null)
            {
                PkgName = App.ParsedArgu.package;
            }
            string PkgPath = "Package\\" + PkgName.Replace(".", "\\") + "\\";
            try
            {
                ParsedManifest = JsonSerializer.Deserialize<Scheme.PkgManifest>(File.OpenRead(PkgPath + "manifest.json"));
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
            Title = ParsedManifest.DisplayName;
            SetThemeColor(ParsedManifest.ThemeColor);
            if (ParsedManifest.SaveWindowSize)
            {
                try
                {
                    Stream FileStream =File.OpenRead("PackageData\\" + PkgName + ".json");
                    ParsedLocalData = JsonSerializer.Deserialize<Scheme.PkgLocalData>(FileStream);
                    FileStream.Close();
                }
                catch (Exception)
                {
                    ParsedLocalData = new Scheme.PkgLocalData();
                    using FileStream createStream = File.Create("PackageData\\" + PkgName + ".json");
                    {
                        JsonSerializer.Serialize(createStream, ParsedLocalData);
                        createStream.Dispose();
                    }
                }
                if (ParsedLocalData.Height > 0 && ParsedLocalData.Width > 0)
                {
                    Height = ParsedLocalData.Height;
                    Width = ParsedLocalData.Width;
                }
            }
            else
            {
                Height = ParsedManifest.Height;
                Width = ParsedManifest.Width;
            }
            if (File.Exists(PkgPath + "icon.png"))
            {
                Stream imageStreamSource = new FileStream(PkgPath + "icon.png", FileMode.Open, FileAccess.Read, FileShare.Read);
                PngBitmapDecoder decoder = new PngBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                BitmapSource bitmapSource = decoder.Frames[0];
                this.Icon = bitmapSource;
            }
            if (NewWindow && new Uri(NewWindowLink).DnsSafeHost != ParsedManifest.Domain)
            {
                MenuIcon.Icon = FontAwesome.Sharp.IconChar.ArrowUpRightFromSquare;
            }
            if (WebView.CoreWebView2 != null)
            {
                if (NewWindow)
                {
                    WebView.CoreWebView2.Navigate(NewWindowLink);
                }
                else
                {
                    WebView.CoreWebView2.Navigate("https://" + ParsedManifest.Domain + "/" + ParsedManifest.Entry);
                }
            }
        }

        Scheme.PkgManifest? ParsedManifest;
        Scheme.PkgLocalData? ParsedLocalData;
        private async void InitWebView(bool NewWindow = false, string? NewWindowLink = null)
        {
            var WebviewArgu = "--disable-features=msSmartScreenProtection --in-process-gpu --renderer-process-limit=1 --pull-to-refresh";
            CoreWebView2EnvironmentOptions options = new CoreWebView2EnvironmentOptions()
            {
                AdditionalBrowserArguments = WebviewArgu
            };
            Directory.CreateDirectory(System.Environment.CurrentDirectory + @"\WebviewCache\PWA\");
            App.WebView2Environment = await CoreWebView2Environment.CreateAsync(null, System.Environment.CurrentDirectory + @"\WebviewCache\PWA\", options);
            await WebView.EnsureCoreWebView2Async(App.WebView2Environment);
            WebView.IsEnabled = true;
            WebView.CoreWebView2.Settings.IsBuiltInErrorPageEnabled = false;
            WebView.CoreWebView2.DocumentTitleChanged += (a, b) =>
            {
                this.TitleText.Content = WebView.CoreWebView2.DocumentTitle;
            };
            //域名白名单机制
            Func<string, bool> CheckWhitelist = (string u) =>
            {
                string CurrentDomain = new Uri(u).DnsSafeHost;
                return ((CurrentDomain == ParsedManifest.Domain) || ParsedManifest.TrustedDomain.Contains(CurrentDomain) || u.StartsWith("data"));
            };
            Storyboard sb = this.FindResource("Loading") as Storyboard;
            WebView.CoreWebView2.NavigationStarting += (a, e) =>
            {
                RefreshIcon.Icon = FontAwesome.Sharp.IconChar.CircleNotch;
                sb.Begin();
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
                sb.Stop();
                RefreshIcon.Icon = FontAwesome.Sharp.IconChar.RotateRight;
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
            if (ParsedManifest != null)
            {
                if (WebView.CoreWebView2 != null)
                {
                    if (NewWindow)
                    {
                        WebView.CoreWebView2.Navigate(NewWindowLink);
                    }
                    else
                    {
                        WebView.CoreWebView2.Navigate("https://" + ParsedManifest.Domain + "/" + ParsedManifest.Entry);
                    }
                }
            }
        }



        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (e.ClickCount == 2)
                {
                    WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
                }
                else
                {
                    //最大化恢复逻辑，有待优化，暂不开放
                    if (WindowState == WindowState.Maximized)
                    {
                        //double currentHeight = this.Height;
                        //double currentWidth = this.Width;
                        //double currentLeft = Left;
                        //double currentTop = Top;
                        //WindowState = WindowState.Normal;
                        //Left = currentLeft+10;
                        //Top= currentTop+10;
                        //this.Height= currentHeight-20;
                        //this.Width= currentWidth-20;
                    }
                    DragMove();
                }
            }
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                WindowStyle = WindowStyle.SingleBorderWindow;
                WindowStyle = WindowStyle.None;
                this.BorderThickness = new Thickness(8);
                WebView.Margin = new Thickness(0, 30, 0, 0);
            }
            else
            {
                this.BorderThickness = new Thickness(0);
                WebView.Margin = new Thickness(1, 30, 1, 1);
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            WebView.CoreWebView2.Reload();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            WebView.CoreWebView2.GoBack();
        }

        private void SetThemeColor(string Color)
        {
            this.Background = new BrushConverter().ConvertFromString(Color) as SolidColorBrush;
        }

        bool ReadyToExit = false;
        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!ReadyToExit && MenuIcon.Icon != FontAwesome.Sharp.IconChar.ArrowUpRightFromSquare)
            {
                e.Cancel = true;
                TitleText.Content = "正在保存数据...";
                if (ParsedManifest.SaveWindowSize)
                {
                    ParsedLocalData.Height = (int)Math.Round(Height);
                    ParsedLocalData.Width = (int)Math.Round(Width);
                    MinHeight = 30;
                    Height = 30;
                    using FileStream createStream = File.Create("PackageData\\" + ParsedManifest.Name + ".json");
                    await JsonSerializer.SerializeAsync(createStream, ParsedLocalData);
                    await createStream.DisposeAsync();
                }
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

        private void Menu_Click(object sender, RoutedEventArgs e)
        {
            //菜单里面塞什么还没想好
            //如果是新窗口打开，允许使用外部浏览器打开页面
            if (MenuIcon.Icon == FontAwesome.Sharp.IconChar.ArrowUpRightFromSquare)
            {
                ProcessStartInfo startInfo = new(WebView.CoreWebView2.Source);
                startInfo.UseShellExecute = true;
                Process.Start(startInfo);
            }
        }
    }
}
