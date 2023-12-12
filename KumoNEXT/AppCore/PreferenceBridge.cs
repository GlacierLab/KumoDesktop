using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace KumoNEXT.AppCore
{
    public class PreferenceBridge
    {
        Preference? Window = null;
        string PkgName = "";
        Scheme.PkgLocalData? ParsedLocalData = null;
        bool Headless = false;

        public Action? Callback = null;

        //GUI模式需要传入窗口
        public PreferenceBridge(Preference Window)
        {
            this.Window = Window;
            PkgName = Window.ParsedManifest.Name;
        }
        //无头模式只需包名
        public PreferenceBridge(string Name)
        {
            PkgName = Name;
            Headless = true;
        }

        //关闭窗口，无头模式下不可用
        public void Close()
        {
            if (Window != null)
            {
                Window.ReadyToClose = true;
                Window.Close();
            }
        }
        //收工信号，无头模式下触发即视为处理完成
        public void JobDone()
        {
            if (Callback != null)
            {
                Callback();
            }
        }

        //输出到C#命令行 
        public void WriteConsole(string message)
        {
            Console.WriteLine(message);
        }

        //浏览器内打开，无头模式下不可用
        public void OpenLink(string url)
        {
            if (Window != null)
            {
                ProcessStartInfo startInfo = new(url);
                startInfo.UseShellExecute = true;
                Process.Start(startInfo);
            }
        }

        //获取显示名称，无头模式下不可用
        public string GetDisplayName()
        {
            if (Window != null)
            {
                return Window.ParsedManifest.DisplayName;
            }
            else
            {
                return "";
            }
        }

        //读取配置文件
        public string ReadConfig()
        {
            if (File.Exists("Package\\" + PkgName.Replace(".", "\\") + "\\config.json"))
            {
                return File.ReadAllText("Package\\" + PkgName.Replace(".", "\\") + "\\config.json");
            }
            else
            {
                return "NOTHING";
            }
        }

        //读取用户设置
        public string ReadPreference()
        {
            var FileStream = File.OpenRead("PackageData\\" + PkgName + ".json");
            ParsedLocalData = JsonSerializer.Deserialize<Scheme.PkgLocalData>(FileStream);
            FileStream.Dispose();
            return ParsedLocalData.PreferenceSaved;
        }
        //写入用户设置
        public void WritePreference(string message)
        {
            if (ParsedLocalData == null)
            {
                ReadPreference();
            }
            ParsedLocalData.PreferenceSaved = message;
            FileStream createStream = File.Create("PackageData\\" + PkgName + ".json");
            JsonSerializer.Serialize(createStream, ParsedLocalData);
            createStream.Dispose();
            if (Window != null)
            {
                Window.ParsedLocalData[0].PreferenceSaved = message;
            }
        }


    }
}
