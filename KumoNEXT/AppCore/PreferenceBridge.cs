using KumoNEXT.Scheme;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Media.TextFormatting;

namespace KumoNEXT.AppCore
{
    public class PreferenceBridge
    {
        Preference? Window=null;
        string PkgName = "";
        Scheme.PkgLocalData? ParsedLocalData=null;
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
            PkgName= Name;
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
            if(Callback != null)
            {
                Callback();
            }
        }

        //输出到C#命令行 
        public void WriteConsole(string message) 
        { 
            Console.WriteLine(message);
        }

        //读取配置文件
        public string ReadConfig()
        {
            if(File.Exists("Package\\" + PkgName.Replace(".", "\\")+"\\config.json"))
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
            FileStream createStream = File.OpenWrite("PackageData\\" + PkgName + ".json");
            JsonSerializer.Serialize(createStream, ParsedLocalData);
            createStream.Dispose();
            if (Window != null)
            {
                Window.ParsedLocalData[0].PreferenceSaved = message;
            }
        }


    }
}
