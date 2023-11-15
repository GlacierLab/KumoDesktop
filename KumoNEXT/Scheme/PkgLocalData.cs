using System.Text.Json;
using System.Text.Json.Serialization;

namespace KumoNEXT.Scheme
{

    public class PkgLocalData
    {
        //长期允许的权限
        public string[] AcceptPermissions { get; set; } = { };
        //长期拒绝的权限
        public string[] DenyPermissions { get; set; } = { };
        //保存的窗口大小
        public int Height { get; set; } = 0;
        public int Width { get; set; } = 0;
        //JSON格式的偏好设置，C#层不做进一步解析
        public string PreferenceSaved { get; set; } = "{}";
        [JsonExtensionData]
        public Dictionary<string, JsonElement>? ExtensionData { get; set; }
    }
}
