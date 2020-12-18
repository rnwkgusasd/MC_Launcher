using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private int ram = 4096;

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

        public bool Start(string path, string version)
        {
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
                StartVersion = new MVersion(),
                Session = se,
                MaximumRamMb = ram,
                ServerIp = ip,
                ServerPort = port,
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
    }
}
