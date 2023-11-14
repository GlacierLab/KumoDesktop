namespace KumoNEXT.Scheme
{
    //调用程序的参数清单，不在清单中的参数会被忽略
    public class LaunchArgu
    {
        //进程类型
        //ui-标准的界面进程，可以有多个
        //pwa-PWA应用进程，每个进程默认只承载一个PWA应用
        //service-后台服务进程，负责任务分配及长时间任务处理，有且仅能有一个
        public string type { get; set; } = "ui";
        //启动目标包名，默认为主界面
        public string package { get; set; } = "PWA.545WebPlayer";
        //对目标包名传递的信息
        public string msg { get; set; } = "";
    }
}
