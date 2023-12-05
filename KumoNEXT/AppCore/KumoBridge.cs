using System.Text.Json;
using System.Windows;

namespace KumoNEXT.AppCore
{
    //云酱原生功能调用
    //每个方法前面的[]内注明了需要请求的权限
    public class KumoBridge
    {
        WebRender CurrentWindow;
        public KumoBridge(WebRender Window)
        {
            CurrentWindow = Window;
        }

        //Window类方法，控制基本的窗口状态

        //[]切换最大化，并返回最大化状态
        public bool Window_Maximize()
        {
            CurrentWindow.WindowState = CurrentWindow.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            return CurrentWindow.WindowState == WindowState.Maximized;
        }
        //[]切换最小化，无返回
        public void Window_Minimize()
        {
            CurrentWindow.WindowState = WindowState.Minimized;
        }
        //[]关闭窗口，无返回
        public void Window_Close()
        {
            CurrentWindow.Close();
        }
        //[]设置窗口大小，返回设置后的大小
        public int[] Window_Size(int Width, int Height)
        {
            CurrentWindow.Width = Width;
            CurrentWindow.Height = Height;
            return [(int)Math.Round(CurrentWindow.Width), (int)Math.Round(CurrentWindow.Height)];
        }
        //[AlwaysTop]请求窗口置顶，返回置顶状态
        //0:取消置顶
        //1:进入置顶
        //-1:权限不足
        public async Task<int> Window_PinTop(bool Pin)
        {
            //TODO
            return 0;
        }
        //[]打开WebView调试工具，无返回
        //该方法无视运行时配置的EnableDebug值
        public void Window_OpenDevTools()
        {
            CurrentWindow.WebView.CoreWebView2.OpenDevToolsWindow();
        }



        //Kumo类方法，云酱运行时与当前包相关

        //[]获取运行时版本号，返回整数格式
        public int Kumo_Version()
        {
            return App.MainConfig.RuntimeVersion;
        }
        //[]获取当前包清单，返回JSON字符串格式
        public string Kumo_PkgManifest()
        {
            return JsonSerializer.Serialize(CurrentWindow.ParsedManifest);
        }
        //[]同步存储的选项，传入Json字符串则保存到本地，传入空白则不处理仅返回。返回Json字符串格式的存储的选项
        public string Kumo_SyncPreference(string? JsonText)
        {
            if (JsonText != null)
            {
                CurrentWindow.ParsedLocalData.PreferenceSaved = JsonText;
            }
            return CurrentWindow.ParsedLocalData.PreferenceSaved;
        }
        //[]打开选项设置窗口，无返回
        //设置窗口关闭后回调同步设置
        public void Kumo_OpenPreferenceWindow()
        {
            var PreferenceWindow = new Preference(CurrentWindow.ParsedManifest.Name, ref CurrentWindow.ParsedLocalData);
            PreferenceWindow.Closed += (o, e) =>
            {
                CurrentWindow.WebView.CoreWebView2.ExecuteScriptAsync("Callback.Preference_Change?Callback.Preference_Change():null;");
            };
            PreferenceWindow.Show();
        }


        //Package类方法，包管理相关

        //[]获取已安装的子包，返回子包字符串数组
        public string[] Package_GetInstalledChildPackages()
        {
            //TODO
            return [];
        }
    }
}
