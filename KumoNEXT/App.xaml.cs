using Microsoft.Web.WebView2.Core;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Windows;
using Windows.Management.Deployment;

namespace KumoNEXT
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //WebView2运行时为整个进程共享
        //安全级别相同的包可以运行在同一进程下，但这得等到进程间通信写完再说，先挖一个坑在这里
        public static CoreWebView2Environment? WebView2Environment;
        //0-无安全措施，适用于可信任的包，无进程/目录隔离，无沙盒，可跨域，无限制调用本地API
        //1-标准安全措施，适用于大部分扩展包，无进程/目录隔离，启用沙盒，禁止跨域，调用受限API需请求权限
        //2-严格安全措施，适用于对安全要求较高的扩展包，进程/目录隔离，启用沙盒，禁止跨域，禁止调用本地API
        public static int SecurityLevel = 0;
        //启动参数
        public static Scheme.LaunchArgu? ParsedArgu;
        //配置文件
        public static Scheme.MainConfig? MainConfig;

        //载入常规的WebView环境
        public async static Task InitAppWebView()
        {
            if (WebView2Environment != null)
            {
                //避免重复初始化
                return;
            }
            else
            {
                var WebviewArgu = "--disable-features=msSmartScreenProtection,ElasticOverscroll --enable-features=msWebView2EnableDraggableRegions --in-process-gpu --disable-web-security --no-sandbox --single-process";
                CoreWebView2EnvironmentOptions options = new()
                {
                    AdditionalBrowserArguments = WebviewArgu
                };
                Directory.CreateDirectory(Environment.CurrentDirectory + @"\WebviewCache\App\");
                WebView2Environment = await CoreWebView2Environment.CreateAsync(null, Environment.CurrentDirectory + "\\WebviewCache\\App\\", options);
                WebView2Environment.BrowserProcessExited += (o, e) =>
                {
                    Console.WriteLine("WebView Environment Dead");
                    WebView2Environment = null;
                    InitAppWebView();
                };
            }
        }


        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent(params string[] Args)
        {
            //Task.Run(Utils.WindowsPackageHelper.CacheWindowsPackages);
            //解析参数
            ParsedArgu = new Scheme.LaunchArgu();
            Array.ForEach(Args, (string argu) =>
            {
                if (argu.StartsWith("--"))
                {
                    string[] parsed = argu.Substring(2).Split("=");
                    PropertyInfo? entry = ParsedArgu.GetType().GetProperty(parsed[0]);
                    entry?.SetValue(ParsedArgu, parsed[1]);
                }
            });
#if DEBUG
            Console.WriteLine("Launch Argu:" + JsonSerializer.Serialize(ParsedArgu));
#endif
            switch (ParsedArgu.type)
            {
                case "ui":
                    this.StartupUri = new System.Uri("Init.xaml", System.UriKind.Relative);
                    break;
                case "pwa":
                    this.StartupUri = new System.Uri("TinyPWA.xaml", System.UriKind.Relative);
                    break;
                case "service":
                    Service.ServiceCore.Main();
                    break;
                default:
                    Current.Shutdown();
                    break;
            }
        }
        [System.STAThreadAttribute()]
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public static void Main(params string[] Args)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            KumoNEXT.App app = new KumoNEXT.App();
            app.InitializeComponent(Args);
            app.Run();
        }


        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            string str = GetExceptionMsg(e.ExceptionObject as Exception, e.ToString());
            Directory.CreateDirectory("Logs");
            File.WriteAllText("Logs\\Error" + new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString() + ".log", str);
            MessageBox.Show(str);
        }
        static string GetExceptionMsg(Exception ex, string backStr)
        {
            StringBuilder sb = new();
            sb.AppendLine("***************************************************************");
            sb.AppendLine("************************似乎哪里有点不对劲************************");
            sb.AppendLine("[时间]：" + DateTime.Now.ToString());
            if (ex != null)
            {
                sb.AppendLine("[错误]：" + ex.GetType().Name);
                sb.AppendLine("[信息]：" + ex.Message);
                sb.AppendLine("[堆栈]：" + ex.StackTrace);
            }
            else
            {
                sb.AppendLine("[异常]：" + backStr);
            }
            sb.AppendLine("***************************************************************");
            return sb.ToString();
        }
    }
}


