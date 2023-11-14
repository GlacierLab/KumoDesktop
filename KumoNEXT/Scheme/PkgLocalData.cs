using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace KumoNEXT.Scheme
{

    public class PkgLocalData
    {
        //长期允许的权限
        public string[] AcceptPermissions { get; set; } = { };
        //长期拒绝的权限
        public string[] DenyPermissions { get; set; } = { };
        //JSON格式的偏好设置，C#层不做进一步解析
        public string PreferenceSaved { get; set; } = "{}";
        [JsonExtensionData]
        public Dictionary<string, JsonElement>? ExtensionData { get; set; }
    }
}
