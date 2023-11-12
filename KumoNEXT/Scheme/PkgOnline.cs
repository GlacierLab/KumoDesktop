using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KumoNEXT.Scheme
{
    //托管库中保存库基本信息的文件，该文件永远只会动态拉取，不会保存到本地
    public class PkgOnline
    {
        //清单版本，用于向下兼容
        public int ManifestVersion { get; set; } = 1;
        //包名
        public string Name { get; set; } = "CorePkg.TestPkg";
        //作者
        public string Author { get; set; } = "琴梨梨";
        //是否为原生包
        public bool NativeExt { get; set; } = false;

        //版本信息
        public PkgOnline_PkgInfo[] Versions { get; set; } = { };
    }
}
