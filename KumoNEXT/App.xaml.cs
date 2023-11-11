using Microsoft.Web.WebView2.Core;
using System.Configuration;
using System.Data;
using System.Security.Permissions;
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

        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent()
        {
            this.StartupUri = new System.Uri("Init.xaml", System.UriKind.Relative);
        }
        [System.STAThreadAttribute()]
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public static void Main()
        {
            KumoNEXT.App app = new KumoNEXT.App();
            app.InitializeComponent();
            app.Run();
        }
    }

}
