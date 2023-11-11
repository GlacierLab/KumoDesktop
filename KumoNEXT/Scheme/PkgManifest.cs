﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KumoNEXT.Scheme
{
    //包清单文件，描述包体信息
    //该文件与包版本绑定，不面向普通用户开放修改
    internal class PkgManifest
    {
        //清单版本，用于向下兼容
        public int ManifestVersion { get; set; } = 1;
        //包名
        public string Name { get; set; } = "CorePkg.TestPkg";
        //作者
        public string Author { get; set; } = "琴梨梨";
        //当前包数字版本
        public int PkgVersion { get; set; } = 0;
        //当前包所需最低的主程序版本，若主程序版本低于所需版本，会请求更新主程序
        public int RequireVersion { get; set; } = 0;
        //包管理下载地址，该包今后都从此地址检索更新
        public string Server { get; set; } = "https://github.com/GlacierLab/KumoDesktop/releases/tag/";
        //包安全级别，只有内置在主配置文件内的可信包允许使用0级别
        //0-无安全措施，适用于可信任的包，无进程/目录隔离，无沙盒，可跨域，无限制调用本地API
        //1-标准安全措施，适用于大部分扩展包，无进程/目录隔离，启用沙盒，禁止跨域，调用受限API需请求权限
        //2-严格安全措施，适用于对安全要求较高的扩展包，进程/目录隔离，启用沙盒，禁止跨域，禁止调用本地API
        public int SecurityLevel { get; set; } = 2;
    }
}