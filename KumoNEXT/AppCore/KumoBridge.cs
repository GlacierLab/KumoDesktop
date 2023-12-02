using System.Windows;

namespace KumoNEXT.AppCore
{
    //云酱原生功能调用
    //每个方法前面的[]内注明了需要请求的权限
    public class KumoBridge
    {
        Window CurrentWindow;
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



        //Kumo类方法，云酱运行时相关
        public int Kumo_Version()
        {
            return App.MainConfig.RuntimeVersion;
        }
    }
}
