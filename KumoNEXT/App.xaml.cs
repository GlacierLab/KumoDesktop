﻿using Microsoft.Web.WebView2.Core;
using System.Reflection;
using System.Text.Json;
using System.Windows;

namespace KumoNEXT
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //WebView2运行时为整个进程共享
        //安全级别相同的包可以运行在同一进程下
        public static CoreWebView2Environment? WebView2Environment;
        //0-无安全措施，适用于可信任的包，无进程/目录隔离，无沙盒，可跨域，无限制调用本地API
        //1-标准安全措施，适用于大部分扩展包，无进程/目录隔离，启用沙盒，禁止跨域，调用受限API需请求权限
        //2-严格安全措施，适用于对安全要求较高的扩展包，进程/目录隔离，启用沙盒，禁止跨域，禁止调用本地API
        public static int SecurityLevel = 0;
        //启动参数
        public static Scheme.LaunchArgu? ParsedArgu;

        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent(params string[] Args)
        {
            //解析参数
            ParsedArgu = new Scheme.LaunchArgu();
            Array.ForEach(Args, (string argu) => {
                if (argu.StartsWith("--"))
                {
                    string[] parsed = argu.Substring(2).Split("=");
                    PropertyInfo? entry = ParsedArgu.GetType().GetProperty(parsed[0]);
                    if (entry != null)
                    {
                        entry.SetValue(ParsedArgu, parsed[1]);
                    }
                }
            });
            Console.WriteLine("Launch Argu:" + JsonSerializer.Serialize(ParsedArgu));
            switch (ParsedArgu.type){
                case "ui":
                    this.StartupUri= new System.Uri("Init.xaml", System.UriKind.Relative);
                    break;
                case "browser":
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
            KumoNEXT.App app = new KumoNEXT.App();
            app.InitializeComponent(Args);
            app.Run();
        }
    }

}
