using System;
using System.Collections.Generic;
using System.Text;

namespace Redirection.Data
{
    [Serializable]
    public class RServer
    {
        public string key;
        public string name;
    }
    [Serializable]
    public class ServerInfo
    {
        public int ip;
        public int port;
        public string name;
    }
    public class ServerTable
    {
        public static ServerInfo[] servers = new ServerInfo[256];
        public static void AddServer(int ip,int port,string name)
        {
            int max = 0;
            for(int i=0;i<256;i++)
            {
                var s = servers[i];
                if(s!=null)
                {
                    if(s.ip==ip)
                    {
                        s.port = port;
                        s.name = name;
                        return;
                    }
                    if (i == max)
                        max++;
                }
            }
            ServerInfo si = new ServerInfo();
            si.ip = ip;
            si.port = port;
            si.name = name;
            servers[max] = si;
        }
        public static List<ServerInfo> GetAllServer()
        {
            List<ServerInfo> list = new List<ServerInfo>();
            for(int i=0;i<256;i++)
            {
                if(servers[i].port>0)
                {
                    list.Add(servers[i]);
                }
            }
            return list;
        }
        public static void RemoveServer(int index)
        {
            if (index < 0)
                return;
            if (index >= 256)
                return;
            servers[index].port=0;
        }
        public static ServerInfo GetServer(int index)
        {
            if (index < 0)
                return new ServerInfo();
            if (index >= 256)
                return new ServerInfo();
            return servers[index];
        }
    }
}
