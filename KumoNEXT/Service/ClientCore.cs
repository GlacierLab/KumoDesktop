using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace KumoNEXT.Service
{
    internal class ClientCore
    {
        public static void Main()
        {
            Console.WriteLine("Launch Client...");
            new Thread(ClientThread).Start();
        }
        private static void ClientThread()
        {
            var pipeClient = new NamedPipeClientStream(".", "KumoDesktop", PipeDirection.InOut, PipeOptions.Asynchronous, TokenImpersonationLevel.Impersonation);
            Console.WriteLine("Connect to Service...\n");
            pipeClient.Connect();
            var ss = new StreamString(pipeClient);
            // Validate the server's signature string.
            if (ss.ReadString() == "KumoService")
            {
                Console.WriteLine("Connected to Service!");
                ss.WriteString("Test");
            }
            else
            {
            }
            pipeClient.Close();
        }
    }
}
