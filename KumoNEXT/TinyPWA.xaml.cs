using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KumoNEXT
{
    /// <summary>
    /// Interaction logic for TinyPWA.xaml
    /// </summary>
    public partial class TinyPWA : Window
    {
        public TinyPWA()
        {
            InitializeComponent();
            Init();
        }

        private async void Init()
        {
            var WebviewArgu = "--disable-features=msSmartScreenProtection --enable-features=msEdgeAVIF --in-process-gpu --renderer-process-limit=1 --single-process";
            CoreWebView2EnvironmentOptions options = new CoreWebView2EnvironmentOptions()
            {
                AdditionalBrowserArguments = WebviewArgu
            };
            Directory.CreateDirectory(System.Environment.CurrentDirectory + @"\QinliliWebview2\");
            var webView2Environment = await CoreWebView2Environment.CreateAsync(null, System.Environment.CurrentDirectory + @"\QinliliWebview2\", options);
            await WebView.EnsureCoreWebView2Async(webView2Environment);
            WebView.IsEnabled = true;
            WebView.CoreWebView2.DocumentTitleChanged += (a, b) =>
            {

                this.Title.Content = WebView.CoreWebView2.DocumentTitle;
            };
            WebView.CoreWebView2.Navigate("https://545.qinlili.bid");
            SetThemeColor("#FADCBB");
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton ==MouseButton.Left) {
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
            }
            else
            {
                this.BorderThickness = new Thickness(0);
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
            this.Background= new BrushConverter().ConvertFromString(Color) as SolidColorBrush;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            WebView.Dispose();
        }
    }
}
