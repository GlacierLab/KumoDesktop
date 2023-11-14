namespace KumoNEXT.Scheme
{
    //包清单文件，描述包体信息
    //该文件与包版本绑定，不面向普通用户开放修改
    public class PkgManifest
    {
        //清单版本，用于向下兼容
        public int ManifestVersion { get; set; } = 1;
        //包名
        public string Name { get; set; } = "CorePkg.TestPkg";
        //可读名
        public string DisplayName { get; set; } = "测试包";
        //描述
        public string Description { get; set; } = "一个测试包体";
        //作者
        public string Author { get; set; } = "琴梨梨";
        //是否为原生包
        public bool NativeExt { get; set; } = false;
        //是否为简单PWA包，简单PWA包只含有配置文件和图标
        public bool PWA { get; set; } = false;

        //当前包数字版本
        public int PkgVersion { get; set; } = 0;
        //当前包所需最低的主程序版本，若主程序版本低于所需版本，会请求更新主程序
        public int RequireVersion { get; set; } = 0;
        //包管理下载地址，该包今后都从此地址检索更新
        public string Server { get; set; } = "https://github.com/GlacierLab/KumoDesktopPackages/releases/tag/";
        //包安全级别，只有内置在主配置文件内的可信包允许使用0级别
        //0-无安全措施，适用于可信任的包，无进程/目录隔离，无沙盒，可跨域，无限制调用本地API
        //1-标准安全措施，适用于大部分扩展包，无进程/目录隔离，启用沙盒，禁止跨域，调用受限API需请求权限
        //2-严格安全措施，适用于对安全要求较高的扩展包，进程/目录隔离，启用沙盒，禁止跨域，禁止调用本地API
        public int SecurityLevel { get; set; } = 2;
        //权限列表，请求此列表外的权限会被忽略
        public string[] Permissions { get; set; } = { };
        //依赖包名，安装依赖包后再安装此包
        public string[] Dependency { get; set; } = { };
        //扩展子包，仅本名单内的扩展包允许安装文件到目录下
        public string[] ChildPkg { get; set; } = { };
        //安装路径
        public string Path { get; set; } = "CorePkg/TestPkg";
        //映射域名，对于纯本地扩展包避免使用重复域名，对于在线应用本地化预载资源可以使用重复域名
        //[PWA包]下该域名即为目标在线域名
        public string Domain { get; set; } = "test.kumo-desktop.qinlili.bid";
        //启动页面
        public string Entry { get; set; } = "index.html";
        //信任域名，除映射域名外允许在窗体内加载的域名，若尝试跳转非信任域名则使用浏览器打开
        public string[] TrustedDomain { get; set; } = {};
        //主题颜色，决定部分窗体边框和背景色，暂不支持透明色
        public string ThemeColor { get; set; } = "#FFFFFF";

        //安全校验值，目前暂未实施
        public string Signature { get; set; } = "";
    }
}
