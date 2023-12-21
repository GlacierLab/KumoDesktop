namespace KumoNEXT.AppCore
{
    public  class PermissionManager
    {

        public PermissionManager(ref Scheme.PkgLocalData ParsedLocalData)
        {
        }

        public static bool CheckPackageManagePermission(string ContextPkg, string TargetPkg)
        {
            return TargetPkg.StartsWith(ContextPkg);
        }

        public async  Task<bool> CheckAndRequestPermission(string ContextPkg, string Permission)
        {
            //先检查白名单
            string[] Permissions;
            if (PermissionWhitelist.TryGetValue(ContextPkg, out Permissions))
            {
                if (Permissions.Contains(Permission))
                {
                    return true;
                }
            }
            //检查是否已经同意或拒绝过权限

                return false;
        }

        public static Dictionary<string, string[]> PermissionWhitelist=new Dictionary<string, string[]>()
        {
            { "CorePkg.Main",[ "ReadFile","WriteFile","Execute"]},
            { "CorePkg.Update",[ "Package"]},
        };
    }
}
