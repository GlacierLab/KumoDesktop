using KumoNEXT.Scheme;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KumoNEXT.AppCore
{
    public class PreferenceBridge
    {
        Preference? Window=null;
        string PkgName = "";
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
        public string ReadPreference()
        {
            return "";
        }
        //写入配置文件
        public void WritePreference(string message)
        {

        }


    }
}
