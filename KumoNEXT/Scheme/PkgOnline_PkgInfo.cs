namespace KumoNEXT.Scheme
{
    //托管库清单中包体信息格式
    public class PkgOnline_PkgInfo
    {
        //当前包数字版本
        public int PkgVersion { get; set; } = 0;
        //当前包所需最低的主程序版本，若主程序版本低于所需版本，会请求更新主程序
        public int RequireVersion { get; set; } = 0;
        //依赖包名，安装依赖包后再安装此包
        public string[] Dependency { get; set; } = { };
        //包下载地址
        public string Link { get; set; } = "";
    }
}
