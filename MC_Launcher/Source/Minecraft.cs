using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.Downloader;
using CmlLib.Core.Version;

namespace MC_Launcher.Source
{
    public class Minecraft
    {
        private MSession se;

        private string ip = "";
        private int port = 0;
        private int ram = 2048;

        public string USERNAME { get { return se.Username; } }
        public string UUID { get { return se.UUID; } }

        public Minecraft()
        {

        }

        private MLogin login = new MLogin();

        public bool Login(string email, string pwd)
        {
            //var rsp = login.TryAutoLogin();
            var rsp = login.Authenticate(email, pwd);

            if(!rsp.IsSuccess)
            {
                return false;
            }

            se = rsp.Session;
            
            return true;
        }

        public Process Start(string version)
        {
            System.Net.ServicePointManager.DefaultConnectionLimit = 256;

            var path = MinecraftPath.GetOSDefaultPath();
            var game = new MinecraftPath(path);

            var launcher = new CMLauncher(game);

            launcher.ProgressChanged += Download_Progress;
            launcher.FileChanged += Download_ChangeFile;

            var versionMeta = MVersionLoader.GetVersionMetadatas(game);
            var versionLocal = MVersionLoader.GetVersionMetadatasFromLocal(game);

            var findMVersion = versionMeta.GetVersion(version);

            if (findMVersion == null)
            {
                return null;
            }

            var findLVersion = versionLocal.GetVersion(version);

            if (findLVersion == null)
            {
                MDownloader downloader = new MDownloader(game, findMVersion);

                downloader.ChangeFile += Download_ChangeFile;
                downloader.ChangeProgress += Download_Progress;

                downloader.DownloadAll();
            }

            var launchOption = new MLaunchOption
            {
                MaximumRamMb = 2048,
                Session = se,
                Path = game,
                StartVersion = findLVersion,
                GameLauncherName = "JML",
                GameLauncherVersion = "1.0"
            };

            var process = launcher.CreateProcess(launchOption);
            process.Start();

            return process;
        }

        public bool Start(string path, string version, string _ip, int _port, int _ram)
        {
            System.Net.ServicePointManager.DefaultConnectionLimit = 256;

            ip = _ip;
            port = _port;
            ram = _ram;

            var game = new MinecraftPath(path);
            //var launcher = new CMLauncher(game);

            var versionMeta = MVersionLoader.GetVersionMetadatas(game);
            var versionLocal = MVersionLoader.GetVersionMetadatasFromLocal(game);

            var findMVersion = versionMeta.GetVersion(version);

            if (findMVersion == null)
            {
                return false;
            }

            var findLVersion = versionLocal.GetVersion(version);

            if(findLVersion == null)
            {
                MDownloader downloader = new MDownloader(game, findMVersion);

                downloader.ChangeFile += Download_ChangeFile;
                downloader.ChangeProgress += Download_Progress;

                downloader.DownloadAll();
            }

            var launchOption = new MLaunchOption
            {
                Path = game,
                StartVersion = findLVersion,
                Session = se,
                MaximumRamMb = ram,
                //ServerIp = ip,
                //ServerPort = port,
                GameLauncherName = "JML"
            };
            
            //var process = launcher.CreateProcess(version, launchOption);
            var launch = new MLaunch(launchOption);
            var process = launch.GetProcess();

            return process.Start();
        }

        private void Download_ChangeFile(DownloadFileChangedEventArgs e)
        {
            
        }

        private void Download_Progress(object sender, ProgressChangedEventArgs e)
        {
            
        }

        public string GetDefaultPath()
        {
            return MinecraftPath.GetOSDefaultPath();
        }
    }
}
