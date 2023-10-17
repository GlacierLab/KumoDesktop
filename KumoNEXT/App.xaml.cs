using System.Configuration;
using System.Data;
using System.Windows;

namespace KumoNEXT
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent()
        {
            this.StartupUri = new System.Uri("Init.xaml", System.UriKind.Relative);
        }
        [System.STAThreadAttribute()]
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public static void Main()
        {
            KumoNEXT.App app = new KumoNEXT.App();
            app.InitializeComponent();
            app.Run();
        }
    }

}
