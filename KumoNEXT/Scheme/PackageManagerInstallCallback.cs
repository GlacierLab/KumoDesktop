namespace KumoNEXT.Scheme
{
    //安装扩展包过程的回调格式
    public class PackageManagerInstallCallback
    {
        //进度，默认0-100整数格式，出错则为负数的错误码
        //错误码：
        //-100网络连接失败
        //-101清单下载成功，但解析失败
        //-102扩展包下载失败
        //-200没有兼容的版本
        //-201本地版本比在线版本高
        //-202扩展包依赖安装失败
        //-300扩展包正在被使用
        public int Progress { get; set; } = 0;
    }
}
