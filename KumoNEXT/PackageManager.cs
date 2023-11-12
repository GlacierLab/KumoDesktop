using KumoNEXT.Scheme;
using System.Net.Http;
using System.Text.Json;

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
            return 0;
        }
        public static int InstallFromFile(string Path, Action<Scheme.PackageManagerInstallCallback>? Callback = null)
        {
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
