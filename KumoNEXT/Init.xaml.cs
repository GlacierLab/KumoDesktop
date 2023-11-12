using Microsoft.Web.WebView2.Core;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
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
            InitAsync(App.ParsedArgu);
        }
        public void ChangeProgress(int Value,string Text)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                Progress.Value = Value;
                Description.Content = Text;
            }));
        }
        private async void InitAsync(Scheme.LaunchArgu Argu)
        {

            Progress.IsIndeterminate = false;
            //解析启动参数
            ChangeProgress(0, "解析启动参数...");
            if (File.Exists("config.json"))
            {
                using FileStream openStream = File.OpenRead("config.json");
                try
                {
                    App.MainConfig = await JsonSerializer.DeserializeAsync<Scheme.MainConfig>(openStream);
                }
                catch (Exception)
                {
                    //自动删除损坏的配置文件
                    openStream.Close();
                    try
                    {
                        File.Delete("config.json");
                    }catch (Exception) {
                    }
                    App.MainConfig = new Scheme.MainConfig();
                }
            }
            else
            {
                App.MainConfig = new Scheme.MainConfig();
            }
#if DEBUG
            Console.WriteLine("Main Config:" + JsonSerializer.Serialize(App.MainConfig));
            await Task.Delay(200);
#endif
            //检查是否存在必要的包
            ChangeProgress(10, "检查必要包体...");
            if (PackageManager.CheckInstall("CorePkg.Update") == 0)
            {

            }
#if DEBUG
            await Task.Delay(200);
#endif
            //初始化IPC服务进程
            ChangeProgress(70, "检查服务进程...");
            if (!File.Exists(@"\\.\pipe\KumoDesktop"))
            {
#pragma warning disable CS8604 // Possible null reference argument.
                Process.Start(Environment.ProcessPath, "--type=service");
#pragma warning restore CS8604 // Possible null reference argument.
            }
            Service.ClientCore.Main();
#if DEBUG
            await Task.Delay(200);
#endif
            //初始化WebView组件
            ChangeProgress(90, "准备渲染器...");
            var WebviewArgu = "--disable-features=msSmartScreenProtection --enable-features=msEdgeAVIF --in-process-gpu --disable-web-security --no-sandbox";
            CoreWebView2EnvironmentOptions options = new CoreWebView2EnvironmentOptions()
            {
                AdditionalBrowserArguments = WebviewArgu
            };
            Directory.CreateDirectory(Environment.CurrentDirectory + @"\QinliliWebview2\");
            App.WebView2Environment = await CoreWebView2Environment.CreateAsync(null, Environment.CurrentDirectory + "\\QinliliWebview2\\", options);
#if DEBUG
            await Task.Delay(200);
#endif
            new AppCore.WebRender().Show();
            new AppCore.WebRender().Show();
            new AppCore.WebRender().Show();
            ChangeProgress(100, "准备就绪");
            await Task.Delay(100);
            this.Close();
        }
    }
}