using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KumoNEXT.AppCore.BridgeModel
{
    //对标准Process对象的二次封装，避免暴露太多不必要的属性，简化使用
    public class ProcessModel
    {
        private Process CurrentProcess { get; set; }

        public readonly string ProcessName;
        public readonly int Pid;

        public ProcessModel(Process Proc) {
            CurrentProcess = Proc;
            ProcessName = Proc.ProcessName;
            Pid = Proc.Id;
        }

        //获取进程的文件路径，可能返回null
        public string? GetExecutablePath()
        {
            if (CurrentProcess.HasExited)
            {
                return null;
            }
            return CurrentProcess.MainModule?.FileName;
        }
    }
}
