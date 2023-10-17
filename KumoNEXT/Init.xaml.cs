using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KumoNEXT
{
    public partial class Init : Window
    {
        public Init()
        {
            InitializeComponent();
            InitAsync();
        }
        private async void InitAsync()
        {
            //检查是否存在必要的包
            string[] RequirePkg = {"CorePkg.Update","CorePkg.Main.UI", "CorePkg.Main.Data" };
            if (PackageManager.CheckInstall("CorePkg.Update") == 0)
            {

            }
        }
    }
}