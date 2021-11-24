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
using CmlLib.Core.Installer;
using CmlLib.Core.Installer.LiteLoader;
using CmlLib.Core.Installer.FabricMC;
using CmlLib.Core.Version;
using CmlLib.Core.VersionLoader;

namespace MC_Launcher.Source
{
    public class Minecraft
    {
        private MSession se;

        private string ip = "";
        private int port = 0;
        private int ram = 2048;
        private string path = "";

        public delegate void DownloadFileChangeEvent(DownloadFileChangedEventArgs e);
        public delegate void DownloadProgressChangeEvent(object sender, ProgressChangedEventArgs e);

        public DownloadFileChangeEvent download_file_change;
        public DownloadProgressChangeEvent download_progress_change;

        public string USERNAME { get { return se.Username; } }
        public string UUID { get { return se.UUID; } }
        public string ACCESS_TOKEN { get { return se.AccessToken; } }
        public string PATH { get { return path; } set { path = value; } }
        public int RAM { get { return ram; } set { ram = value; } }

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

        public Process Start(Server server)
        {
            System.Net.ServicePointManager.DefaultConnectionLimit = 256;

            var game = new MinecraftPath(path);
            var launcher = new CMLauncher(game);

            launcher.ProgressChanged += Download_Progress;
            launcher.FileChanged += Download_ChangeFile;

            string version = "";

            if(server.TYPE == "forge")
            {
                version = FindForgeVersion(server.VERSION);

                if(version == "")
                {
                    return null;
                }
            }
            else { version = server.VERSION; }

            var lv = new LocalVersionLoader(game).GetVersionMetadatas();

            var findVersion = lv.GetVersion(version);
            
            if(findVersion == null)
            {
                var findMVersion = lv.GetVersionMetadata(version);

                if(findMVersion != null)
                {
                    findMVersion.Save(game);
                }
                else
                {
                    var findWVersion = new MojangVersionLoader().GetVersionMetadatas().GetVersionMetadata(version);

                    if(findWVersion != null)
                    {
                        findWVersion.Save(game);
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            var launchOption = new MLaunchOption
            {
                ServerIp = server.IP,
                ServerPort = server.PORT,
                MaximumRamMb = ram,
                Session = se,
                Path = game,
                StartVersion = findVersion,
                GameLauncherName = "JML",
                GameLauncherVersion = "1.0"
            };

            var process = launcher.CreateProcess(launchOption);
            process.Start();

            return process;
        }

        public Process Start(string version)
        {
            System.Net.ServicePointManager.DefaultConnectionLimit = 256;

            var _path = path;
            var game = new MinecraftPath(_path);

            var launcher = new CMLauncher(game);

            launcher.ProgressChanged += Download_Progress;
            launcher.FileChanged += Download_ChangeFile;

            //var versionMeta = MVersionLoader.GetVersionMetadatas(game);
            //var versionLocal = MVersionLoader.GetVersionMetadatasFromLocal(game);

            //var findMVersion = versionMeta.GetVersion(version);

            //if (findMVersion == null)
            {
                return null;
            }

            //var findLVersion = versionLocal.GetVersion(version);

            //if (findLVersion == null)
            {
                //MDownloader downloader = new MDownloader(game, findMVersion);

                //downloader.ChangeFile += Download_ChangeFile;
                //downloader.ChangeProgress += Download_Progress;

                //downloader.DownloadAll();
            }

            var launchOption = new MLaunchOption
            {
                MaximumRamMb = 2048,
                Session = se,
                Path = game,
                //StartVersion = findLVersion,
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

            //var versionMeta = MVersionLoader.GetVersionMetadatas(game);
            //var versionLocal = MVersionLoader.GetVersionMetadatasFromLocal(game);

            //var findMVersion = versionMeta.GetVersion(version);

            //if (findMVersion == null)
            {
                return false;
            }

            //var findLVersion = versionLocal.GetVersion(version);

            //if(findLVersion == null)
            //{
            //    MDownloader downloader = new MDownloader(game, findMVersion);

            //    downloader.ChangeFile += Download_ChangeFile;
            //    downloader.ChangeProgress += Download_Progress;

            //    downloader.DownloadAll();
            //}

            var launchOption = new MLaunchOption
            {
                Path = game,
                //StartVersion = findLVersion,
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
            if(download_file_change != null)
            {
                download_file_change(e);
            }
        }

        private void Download_Progress(object sender, ProgressChangedEventArgs e)
        {
            if(download_progress_change != null)
            {
                download_progress_change(sender, e);
            }
        }

        public string GetDefaultPath()
        {
            return MinecraftPath.GetOSDefaultPath();
        }

        public string FindForgeVersion(string version)
        {
            string forgeVersion = "";

            var vm = new LocalVersionLoader(new MinecraftPath(path)).GetVersionMetadatas();

            List<string> extractVersion = new List<string>();

            vm.Select(x =>
            {
                string _name = x.Name;

                if (_name.Contains(version) && _name.Contains("forge"))
                {
                    extractVersion.Add(x.Name);

                    return true;
                }
                else return false;
            });

            return forgeVersion;
        }

        private void Forge_InstallerOutput(object sender, string e)
        {
            //throw new NotImplementedException();
        }

        private void Forge_FileChanged(DownloadFileChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }
    }
}
