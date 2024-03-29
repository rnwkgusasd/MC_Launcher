﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_Launcher
{
    public class ServerManager
    {
        private List<Server> serverList = new List<Server>();

        private Source.Global G = new Source.Global();

        public ServerManager() { }

        public bool SaveServers()
        {
            List<string> list = new List<string>();

            foreach(Server item in serverList)
            {
                list.Add(item.ToString());
            }

            try
            {
                File.WriteAllLines($"{AppDomain.CurrentDomain.BaseDirectory}\\svlst.txt", list);
            }
            catch (Exception ex)
            {
                G.ErrorLog(ex.ToString());

                return false;
            }

            G.InfoLog("Save Server List Success");
            
            return true;
        }

        public bool LoadServers()
        {
            List<string> list = new List<string>();

            try
            {
                if(!File.Exists($"{AppDomain.CurrentDomain.BaseDirectory}\\svlst.txt")) 
                {
                    File.Create($"{AppDomain.CurrentDomain.BaseDirectory}\\svlst.txt");
                }
                
                list = File.ReadAllLines($"{AppDomain.CurrentDomain.BaseDirectory}\\svlst.txt").ToList();
            }
            catch (Exception ex)
            {
                G.ErrorLog(ex.ToString());

                return false;
            }

            if (list.Count == 0) return true;

            serverList = new List<Server>();

            foreach(string item in list)
            {
                serverList.Add(new Server(item));
            }

            G.InfoLog("Load Server List Success");

            return true;
        }

        public List<Server> GetServers()
        {
            return serverList;
        }

        public void AddServer(Server server)
        {
            serverList.Add(server);
        }

        public void DeleteServer(Server server)
        {
            serverList.Remove(server);
        }
    }

    public class Server
    {
        private string _ip = "";
        private int _port = 0;
        private string _name = "";
        private string _version = "";
        private string _type = "";

        public string IP
        {
            get { return _ip; }
            set {  _ip = value; }
        }

        public int PORT
        {
            get { return _port; }
            set { _port = value; }
        }

        public string NAME
        {
            get { return _name; }
            set { _name = value; }
        }

        public string VERSION
        {
            get { return _version; }
            set { _version = value; }
        }

        public string TYPE
        {
            get { return _type; }
            set { _type = value; }
        }

        public enum PropertyServer
        {
            IP, PORT, VERSION, NAME, TYPE
        }

        public Server() { }

        public Server(string format)
        {
            List<string> list = new List<string>();

            list = format.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();

            IP = list[(int)PropertyServer.IP];
            PORT = int.Parse(list[(int)PropertyServer.PORT]);
            VERSION = list[(int)PropertyServer.VERSION];
            NAME = list[(int)PropertyServer.NAME];
            TYPE = list[(int)PropertyServer.TYPE];
        }

        public Server(string _ip, int _port, string _version, string _name, string _type)
        {
            IP = _ip;
            PORT = _port;
            VERSION = _version;
            NAME = _name;
            TYPE = _type;
        }

        public override string ToString()
        {
            return $"{IP},{PORT},{VERSION},{NAME},{TYPE}";
        }
    }
}
