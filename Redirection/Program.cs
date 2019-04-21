using System;
using huqiang;

namespace Redirection
{
    class Program
    {
        static void Main(string[] args)
        {
            //new SocServer("192.168.0.196", 6666);
            KcpServer.CreateLink = (o) => { return new KcpUser(o); };
            new KcpServer(6666);
            while (true)
            {
                var cmd = Console.ReadLine();
                if (cmd == "close" | cmd == "Close")
                    break;
            }
        }
    }
}
