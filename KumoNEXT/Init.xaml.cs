using Microsoft.Web.WebView2.Core;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KumoNEXT
{
    public partial class Init : Window
    {
        public Init()
        {
            InitializeComponent();
            InitAsync();
        }
        private async void InitAsync()
        {
            Progress.IsIndeterminate = false;
            //解析启动参数
            Progress.Value = 0;
            Description.Content = "解析启动参数...";
#if DEBUG
            await Task.Delay(200);
#endif
            //检查是否存在必要的包
            Progress.Value = 10;
            Description.Content = "检查必要包体...";
            if (PackageManager.CheckInstall("CorePkg.Update") == 0)
            {

            }
#if DEBUG
            await Task.Delay(200);
#endif
            //初始化IPC服务进程
            Progress.Value = 70;
            Description.Content = "检查服务进程...";
#if DEBUG
            await Task.Delay(200);
#endif
            //初始化WebView组件
            Progress.Value = 90;
            Description.Content = "准备渲染器...";
            var WebviewArgu = "--disable-features=msSmartScreenProtection --enable-features=msEdgeAVIF --in-process-gpu --disable-web-security --no-sandbox --renderer-process-limit=1 --single-process";
            CoreWebView2EnvironmentOptions options = new CoreWebView2EnvironmentOptions()
            {
                AdditionalBrowserArguments = WebviewArgu
            };
            Directory.CreateDirectory(Environment.CurrentDirectory + @"\QinliliWebview2\");
            App.WebView2Environment = await CoreWebView2Environment.CreateAsync(null, Environment.CurrentDirectory + "\\QinliliWebview2\\", options);
#if DEBUG
            await Task.Delay(200);
#endif
            new WebRender().Show();
            new WebRender().Show();
            new WebRender().Show();
            Progress.Value = 100;
            Description.Content = "准备就绪";
            await Task.Delay(100);
            this.Close();
        }
    }
}