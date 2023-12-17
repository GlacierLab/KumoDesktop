using KumoNEXT.AppCore;
using KumoNEXT.Scheme;
using KumoNEXT.Utils;
using Microsoft.Web.WebView2.Core;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text.Json;

namespace KumoNEXT
{
    class PackageManager
    {
        public async static Task<int> InstallFromOfficial(string PkgName, Action<Scheme.PackageManagerInstallCallback>? Callback = null)
        {
            Console.WriteLine("Install package:" + PkgName);
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
                            CallbackValue.Progress = 10 + (int)(e * 0.8f);
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
                        }
                        catch (Exception)
                        {
                            //-102扩展包下载失败
                            if (Callback != null)
                            {
                                CallbackValue.Progress = -102;
                                Callback(CallbackValue);
                            }
                            return -102;
                        }
                        await file.DisposeAsync();
                        return await InstallFromFile(FileName);
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
        public async static Task<int> InstallFromFile(string Path, Action<Scheme.PackageManagerInstallCallback>? Callback = null, bool DeletePkg = true)
        {
            Scheme.PackageManagerInstallCallback CallbackValue = new();
            //读取包信息
            ZipArchive PackageFile;
            Scheme.PkgManifest ParsedManifest;
            try
            {
                PackageFile = ZipFile.OpenRead(Path);
                ParsedManifest = await JsonSerializer.DeserializeAsync<Scheme.PkgManifest>(PackageFile.GetEntry("manifest.json").Open());
            }
            catch (Exception)
            {
                //-400扩展包无法解析
                if (Callback != null)
                {
                    CallbackValue.Progress = -400;
                    Callback(CallbackValue);
                }
                return -400;
            }
            Directory.CreateDirectory("PackageData");
            //安装依赖包
            if (ParsedManifest.Dependency.Length > 0)
            {
                foreach (var pkg in ParsedManifest.Dependency)
                {
                    if (!await EnsureInstall(pkg))
                    {
                        //-401依赖包无法安装
                        if (Callback != null)
                        {
                            CallbackValue.Progress = -401;
                            Callback(CallbackValue);
                        }
                        return -401;
                    };
                }
            }
            //建立存档
            if (!File.Exists("PackageData\\" + ParsedManifest.Name + ".json"))
            {
                using FileStream createStream = File.Create("PackageData\\" + ParsedManifest.Name + ".json");
                await JsonSerializer.SerializeAsync(createStream, new Scheme.PkgLocalData());
                await createStream.DisposeAsync();
            }
            //升级配置文件
            if (File.Exists("Package\\" + ParsedManifest.Name.Replace(".", "\\") + "\\config.json"))
            {
                await UpgradeConfig(ParsedManifest.Name);
            }
            //解压到目录
            Directory.CreateDirectory("Package\\" + ParsedManifest.Path);
            await Task.Run(() => PackageFile.ExtractToDirectory("Package\\" + ParsedManifest.Path, true));
            PackageFile.Dispose();
            if (DeletePkg)
            {
                File.Delete(Path);
            }
            //安装成功
            if (Callback != null)
            {
                CallbackValue.Progress = 100;
                Callback(CallbackValue);
            }
            return 100;
        }

        //根据config文件变化升级现有的设置
        //对于现有设置中不存在但config文件中存在的选项，统一添加默认值
        public async static Task UpgradeConfig(string name)
        {
            //在C#里写不定类型的JSON解析太麻烦了，WebView套壳走起！
            CoreWebView2Controller browserController;
            IntPtr HWND_MESSAGE = new IntPtr(-3);
            await App.InitAppWebView();
            browserController = await App.WebView2Environment.CreateCoreWebView2ControllerAsync(HWND_MESSAGE);
            var Bridge = new PreferenceBridge(name);
            browserController.CoreWebView2.AddHostObjectToScript("PreferenceBridge", Bridge);
            //browserController.CoreWebView2.OpenDevToolsWindow();
            var tcs = new TaskCompletionSource<bool>();
            Bridge.Callback = () =>
            {
                tcs.TrySetResult(true);
                Console.WriteLine("WebView Job Done");
            };
            browserController.CoreWebView2.NavigationCompleted += (o, e) =>
            {
            };
            browserController.CoreWebView2.NavigateToString(Properties.Resources.PreferenceUpgradeHeadless);
            await tcs.Task;
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
            bool Installed = File.Exists("Package\\" + PkgName.Replace(".", "\\") + "\\manifest.json");
            Console.WriteLine("Check package:" + PkgName + " - " + Installed.ToString());
            return Installed;
        }
        public async static Task<bool> EnsureInstall(string PkgName)
        {
            if (!CheckInstall(PkgName))
            {
                int ReturnValue = await InstallFromOfficial(PkgName);
                if (!(ReturnValue == 100))
                {
                    return false;
                }
            }
            return true;
        }
        public static int GetInstalledVersion(string PkgName)
        {
            return 0;
        }
    }
}
