using Microsoft.Web.WebView2.Core;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace KumoNEXT
{
    public partial class Init : Window
    {
        public Init()
        {
            InitializeComponent();
            InitAsync(App.ParsedArgu);
        }
        public void ChangeProgress(int Value, string Text)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                Progress.Value = Value;
                Description.Content = Text;
            }));
        }
        private async Task<bool> EnsurePackage(string Name)
        {
            if (!await PackageManager.EnsureInstall(Name))
            {
                var result = MessageBox.Show("安装" + Name + "时遇到错误，是否重试？跳过此包体可能导致程序运行异常", "包体缺失", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    return (await EnsurePackage(Name));
                }
                return false;
            }
            return true;
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
                    if ((App.MainConfig.MaxRuntimeVersion > 0 && App.MainConfig.MaxRuntimeVersion < App.MainConfig.RuntimeVersion) || (App.MainConfig.MinRuntimeVersion > 0 && App.MainConfig.MinRuntimeVersion > App.MainConfig.RuntimeVersion))
                    {
                        //删除版本要求不符的配置文件
                        App.MainConfig = new Scheme.MainConfig();
                        try
                        {
                            File.Delete("config.json");
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                catch (Exception)
                {
                    //自动删除损坏的配置文件
                    openStream.Close();
                    try
                    {
                        File.Delete("config.json");
                    }
                    catch (Exception)
                    {
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
            ChangeProgress(10, "准备核心包体...");
#if RELEASE
            if (PackageManager.CheckInstall("CorePkg.Update"))
            {

            }
            
#endif
            foreach (var PkgName in App.MainConfig.RequirePkg)
            {
                await EnsurePackage(PkgName);
            }
            ChangeProgress(50, "准备目标包体...");
            await EnsurePackage(Argu.package);
            ChangeProgress(65, "准备目标包体...");
            Scheme.PkgManifest ParsedManifest;
            try
            {
                ParsedManifest = await JsonSerializer.DeserializeAsync<Scheme.PkgManifest>(File.OpenRead("Package\\" + Argu.package.Replace(".", "\\") + "\\manifest.json"));
            }
            catch (Exception)
            {
                var result = MessageBox.Show(Argu.package + "包体信息无法解析，是否打开更新模块尝试修复？", "包体异常", MessageBoxButton.YesNo);
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
#if DEBUG
            await Task.Delay(200);
            //初始化IPC服务进程，目前暂不启用，后期会支持跨进程任务传递和后台任务
            ChangeProgress(70, "检查服务进程...");
            if (!File.Exists(@"\\.\pipe\KumoDesktop"))
            {
                Process.Start(Environment.ProcessPath, "--type=service");
            }
            Service.ClientCore.Main();
            await Task.Delay(200);
#endif
            //初始化WebView组件
            ChangeProgress(90, "准备渲染器...");
            var WebviewArgu = "--disable-features=msSmartScreenProtection --enable-features=msWebView2EnableDraggableRegions --in-process-gpu --disable-web-security --no-sandbox";
            CoreWebView2EnvironmentOptions options = new CoreWebView2EnvironmentOptions()
            {
                AdditionalBrowserArguments = WebviewArgu
            };
            Directory.CreateDirectory(Environment.CurrentDirectory + @"\WebviewCache\App\");
            App.WebView2Environment = await CoreWebView2Environment.CreateAsync(null, Environment.CurrentDirectory + "\\WebviewCache\\App\\", options);
#if DEBUG
            await Task.Delay(200);
#endif
            new AppCore.WebRender(ParsedManifest).Show();
            ChangeProgress(100, "准备就绪");
            await Task.Delay(100);
            this.Close();
        }
    }
}