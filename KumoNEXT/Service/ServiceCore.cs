using System.IO;
using System.IO.Pipes;

namespace KumoNEXT.Service
{
    internal class ServiceCore
    {
        public static int ActiveThreads = 0;
        public static void Main()
        {
            Console.WriteLine("Launch Service...");
            new Thread(ServerThread).Start();
        }
        private static void ServerThread(object? data)
        {
            NamedPipeServerStream pipeServer = new NamedPipeServerStream("KumoDesktop", PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances);
            int threadId = Thread.CurrentThread.ManagedThreadId;
            pipeServer.WaitForConnection();
            ActiveThreads++;
            Console.WriteLine("Connected to Client thread[{0}].", threadId);
            try
            {
                StreamString ss = new StreamString(pipeServer);
                ss.WriteString("KumoService");
                string filename = ss.ReadString();
            }
            catch (IOException e)
            {
                Console.WriteLine("Error: {0}", e.Message);
            }
            //新建线程等待新连接
            new Thread(ServerThread).Start();
            pipeServer.Close();
            ActiveThreads--;
            new Thread(CheckAutoExit).Start();
        }

        private static void CheckAutoExit()
        {
            Thread.Sleep(1000);
            if (ActiveThreads == 0)
            {
                Console.WriteLine("Exit Service");
                App.Current.Dispatcher.Invoke(() =>
                {
                    App.Current.Shutdown();
                });
            }
        }
    }


}
