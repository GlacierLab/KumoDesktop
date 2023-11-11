using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KumoNEXT.Scheme
{
    //调用程序的参数清单，不在清单中的参数会被忽略
    internal class LaunchArgu
    {
        //进程类型
        //ui-标准的界面进程，可以有多个
        //browser-浏览器进程，最多可存在一个，用于实现简单的网页浏览功能
        //service-后台服务进程，负责任务分配及长时间任务处理，有且仅能有一个
        public string type { get; set; } = "ui";
        //启动目标包名，默认为主界面
        public string package { get; set; } = "CorePkg.Main.UI";
        //对目标包名传递的信息
        public string msg { get; set; } = "";
    }
}
