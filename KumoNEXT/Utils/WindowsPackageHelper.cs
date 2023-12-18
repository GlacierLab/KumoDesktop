using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Management.Deployment;

namespace KumoNEXT.Utils
{
    public static class WindowsPackageHelper
    {
        public class PackageInfo
        {
            public string  Name { get; set; }
            public string DisplayName { get; set; }
            public string InstalledPath { get; set; }
            public string Architecture { get; set; }
            public int Version { get; set; }
        }


        public static void CacheWindowsPackages()
        {
            var InstalledUWP = new PackageManager().FindPackagesForUser(string.Empty).ToArray();
            var InstalledList=new List<PackageInfo>();
            foreach (Windows.ApplicationModel.Package item in InstalledUWP)
            {
                InstalledList.Add(new PackageInfo {
                    Name=item.Id.Name,
                    DisplayName= item.DisplayName,
                    InstalledPath=item.InstalledPath,
                    Architecture= item.Id.Architecture.ToString(),
                    Version = item.Id.Version.Build
                });
            };
            Directory.CreateDirectory("RuntimeCache");
            File.WriteAllText("RuntimeCache\\InstalledUWP.json", JsonSerializer.Serialize(InstalledList.ToArray()));
        }
    }
}
