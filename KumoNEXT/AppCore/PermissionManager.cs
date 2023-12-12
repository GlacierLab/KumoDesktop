namespace KumoNEXT.AppCore
{
    public static class PermissionManager
    {
        public static bool CheckPackageManagePermission(string ContextPkg, string TargetPkg)
        {
            return TargetPkg.StartsWith(ContextPkg);
        }

        public async static Task<bool> CheckAndRequestPermission(string ContextPkg, string Permission)
        {
            return false;
        }
    }
}
