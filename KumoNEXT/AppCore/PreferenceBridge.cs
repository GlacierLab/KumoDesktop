using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KumoNEXT.AppCore
{
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
}
