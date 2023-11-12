using KumoNEXT.Scheme;
using KumoNEXT.Utils;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Windows.Threading;

namespace KumoNEXT
{
    class PackageManager
    {
        public async static Task<int> InstallFromOfficial(string PkgName, Action<Scheme.PackageManagerInstallCallback>? Callback = null)
        {
            return await InstallFromOnlineSource(App.MainConfig.Server + PkgName + "/Package.json", Callback);
        }
        public async static Task<int> InstallFromOnlineSource(string OnlineManifest, Action<Scheme.PackageManagerInstallCallback>? Callback = null)
        {
            Scheme.PackageManagerInstallCallback CallbackValue = new();
            string ManifestText = "{}";
            //下载在线版本信息
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    ManifestText = await client.GetStringAsync(OnlineManifest);
                }
                catch (Exception)
                {
                    //-100网络连接失败
                    if (Callback != null)
                    {
                        CallbackValue.Progress = -100;
                        Callback(CallbackValue);
                    }
                    return -100;
                }
            }
            //解析版本信息
            Scheme.PkgOnline ManifestParsed;
            try
            {
                ManifestParsed = JsonSerializer.Deserialize<Scheme.PkgOnline>(ManifestText);
            }
            catch (Exception)
            {
                //-101清单下载成功，但解析失败
                if (Callback != null)
                {
                    CallbackValue.Progress = -101;
                    Callback(CallbackValue);
                }
                return -101;
            }
            if (Callback != null)
            {
                CallbackValue.Progress = 10;
                Callback(CallbackValue);
            }
            //确定需要安装的版本
            PkgOnline_PkgInfo Current = new() { PkgVersion = CheckInstall(ManifestParsed.Name) ? GetInstalledVersion(ManifestParsed.Name) : 0 };
            bool Found = false;
            Array.ForEach(ManifestParsed.Versions, (PkgOnline_PkgInfo Version) =>
            {
                //检查运行时版本和已安装版本
                if (Version.RequireVersion <= App.MainConfig.RuntimeVersion)
                {
                    Found = true;
                    if (Version.PkgVersion > Current.PkgVersion)
                    {
                        Current = Version;
                    }
                }
            });
            if (Found)
            {
                if (Current.Link.Length > 0)
                {
                    //找到需要安装的版本，开始安装
                    var client = new HttpClient();
                    var progress = new Progress<float>();
                    progress.ProgressChanged += (_, e) =>
                    {
                        //回传下载进度
                        if (Callback != null)
                        {
                            CallbackValue.Progress = 10+ (int)(e*0.8f);
                            Callback(CallbackValue);
                        }
                    };

                    //下载扩展包
                    Directory.CreateDirectory("PackageCache");
                    string FileName = "PackageCache\\" + ManifestParsed.Name + "." + Current.PkgVersion.ToString() + ".kumopkg";
                    using (var file = new FileStream(FileName, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        try
                        {
                            await client.DownloadDataAsync(Current.Link, file, progress);
                        }catch (Exception)
                        {
                            //-102扩展包下载失败
                            if (Callback != null)
                            {
                                CallbackValue.Progress = 10 + (int)(e * 0.8f);
                                Callback(CallbackValue);
                            }
                            return -102;
                        }
                        return InstallFromFile(FileName);
                    }
                }
                else
                {
                    //-201本地版本比在线版本高
                    if (Callback != null)
                    {
                        CallbackValue.Progress = -201;
                        Callback(CallbackValue);
                    }
                    return -201;
                }
            }
            else
            {
                //-200没有兼容的版本
                if (Callback != null)
                {
                    CallbackValue.Progress = -200;
                    Callback(CallbackValue);
                }
                return -200;
            }
        }
        public static int InstallFromFile(string Path, Action<Scheme.PackageManagerInstallCallback>? Callback = null)
        {
            Scheme.PackageManagerInstallCallback CallbackValue = new();
            //读取包信息

            //解压到目录

            return 0;
        }
        public static int Update(string PkgName)
        {
            return 0;
        }
        public static int Uninstall(string PkgName, string Options = "{}")
        {
            return 0;
        }
        public static bool CheckInstall(string PkgName)
        {
            return false;
        }
        public static int GetInstalledVersion(string PkgName)
        {
            return 0;
        }
    }
}
