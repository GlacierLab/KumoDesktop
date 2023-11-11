using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KumoNEXT.Scheme
{
    //核心配置文件格式，可通过外部更新，默认使用内置值
    //该配置文件管理云酱运行时的参数，不面向普通用户开放，不可通过图形界面更改
    internal class MainConfig
    {
        //[不可覆盖]当前主程序数字版本
        public  int RuntimeVersion { get;  } = 100;
        //[不可覆盖]当前主程序字符串版本
        public string RuntimeVersionStr { get;  } = "1.0.0 Dev Channel 0";
        //[仅覆盖]当前配置文件所需最低和最高的主程序版本，该值仅适用于外部热更新配置文件，内置配置文件一定可被当前版本加载因此默认值永远为0，该值用于防止主程序加载版本不匹配的配置文件
        public int MaxRuntimeVersion { get; set; } = 0;
        public int MinRuntimeVersion { get; set; } = 0;

        //包管理下载地址，未指定包托管地址的包都从此地址检索，该地址可通过用户配置文件的镜像地址覆盖
        public string Server { get; set; } = "https://github.com/GlacierLab/KumoDesktop/releases/tag/";
        //默认启动包名，即直接运行时启动的包
        public string LaunchPkg { get; set; } = "CorePkg.Main.UI";
        //核心包依赖
        public string[] RequirePkg { get; set; } = { "CorePkg.Main.UI", "CorePkg.Main.Data", "CorePkg.Update" };

        //调试模式是否开启
        public Boolean EnableDebug { get; set; } = false;
        //调试模式组件包名，此包无需授权即可调用特权API
        public string DebugPkg { get; set; } = "CorePkg.Debug";
    }
}
