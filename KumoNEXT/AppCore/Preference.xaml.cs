using System.Windows;

namespace KumoNEXT.AppCore
{
    /// <summary>
    /// Interaction logic for Preference.xaml
    /// </summary>
    public partial class Preference : Window
    {
        string PkgName;
        Scheme.PkgLocalData[] ParsedLocalData = new Scheme.PkgLocalData[1];
        public Preference(string Name, ref Scheme.PkgLocalData LocalData)
        {
            InitializeComponent();
            PkgName = Name;
            ParsedLocalData[0] = LocalData;
        }



        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        public class PreferenceBridge
        {
            Preference Window;
            public PreferenceBridge(Preference Window)
            {
                this.Window = Window;
            }

            public void Close()
            {
                Window.Close();
            }
            public string ReadPreference()
            {
                return "";
            }
        }
        private async void WebView_Loaded(object sender, RoutedEventArgs e)
        {
            await WebView.EnsureCoreWebView2Async(App.WebView2Environment).ConfigureAwait(true);
            WebView.CoreWebView2.AddHostObjectToScript("PreferenceBridge", new PreferenceBridge(this));
            WebView.CoreWebView2.NavigateToString(Properties.Resources.Preference);
        }
    }
}
