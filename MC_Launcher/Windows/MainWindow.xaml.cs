using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Threading;
using System.IO;
using System.Diagnostics;
using System.Timers;
using System.Management;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Runtime.InteropServices;
using CmlLib.Core.Downloader;

namespace MC_Launcher
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        #region [Member Variable]

        private List<string> cfg_val = new List<string>();
        private ServerManager sm = new ServerManager();

        public Source.AccessAPI api = new Source.AccessAPI();
        public Source.Minecraft mine = new Source.Minecraft();

        public Server selectedServer;

        public int MAX_RAM = 0;

        #endregion

        #region [Initialize]

        public MainWindow()
        {
            InitializeComponent();
            // https://www.wpf-tutorial.com/tabcontrol/styling-the-tabitems/ custom grid
        }

        #endregion

        #region [Contol Event]

        private void SvrCbBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SvrCbBox.Items.Count == 0)
            {
                selectedServer = null;
                return;
            }

            ComboBoxItem item = (ComboBoxItem)SvrCbBox.SelectedItem;

            StackPanel panel = (StackPanel)item.Content;

            if(panel.Children.Count == 1)
            {
                selectedServer = null;
                return;
            }

            string name = ((TextBlock)panel.Children[0]).Text;

            Server findServer = sm.GetServers().Find(x => x.NAME == name);

            selectedServer = findServer;
        }

        private void mcRam_TextChanged(object sender, TextChangedEventArgs e)
        {
            int txtRam = 0;
            int.TryParse(mcRam.Text, out txtRam);

            if (txtRam > MAX_RAM)
            {
                mcRam.Text = MAX_RAM.ToString();
            }
        }

        #endregion

        #region [Button Click Event]

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ID.Text != "" && PWD.Password != "")
            {
                loadingLogin.Visibility = Visibility.Visible;

                //if (!mine.Login(ID.Text, PWD.Password))
                //{
                //    loadingLogin.Visibility = Visibility.Hidden;

                //    MessageBox.Show("ID와 Password를 확인해주세요..", "LOGIN", MessageBoxButton.OK);

                //    return;
                //}
                //else imgSkin.Source = api.GetSkinFromAPI(mine.UUID);

                Storyboard sb = Resources["LoginBtn"] as Storyboard;
                sb.Begin(SlidePanel);
                loadingLogin.Visibility = Visibility.Hidden;
            }
            else
            {
                MessageBox.Show("ID와 Password를 입력해주세요.", "LOGIN", MessageBoxButton.OK);

                return;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (IDsave.IsChecked == null || IDsave.IsChecked == false)
            {
                ID.Text = "";
            }
            PWD.Password = "";

            Storyboard sb = Resources["BackLogin"] as Storyboard;
            sb.Begin(SlidePanel);
        }

        private void BackMenu_Click(object sender, RoutedEventArgs e)
        {
            Storyboard sb = Resources["BackMenu"] as Storyboard;
            sb.Begin(SlidePanel);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        Process p = null;
        System.Timers.Timer t = new System.Timers.Timer();

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            //if(selectedServer == null)
            //{
            //    //MessageBox.Show("서버를 선택 해주세요.", "GAME", MessageBoxButton.OK);
            //    p = mine.Start("");
            //}
            //else
            //{
            //    p = mine.Start(selectedServer);
            //}
            
            //startBtn.IsEnabled = false;

            //t = new System.Timers.Timer(500);
            //t.Elapsed += ProcessFunction;
            //t.Start();
        }

        private void serverBtn_Click(object sender, RoutedEventArgs e)
        {
            serverVersionBackground.Visibility = Visibility.Visible;
            serverPopup.Visibility = Visibility.Visible;
        }

        private void modsBtn_Click(object sender, RoutedEventArgs e)
        {
            serverVersionBackground.Visibility = Visibility.Visible;
            modsPopup.Visibility = Visibility.Visible;
        }

        private void versionBtn_Click(object sender, RoutedEventArgs e)
        {
            serverVersionBackground.Visibility = Visibility.Visible;
            versionPopup.Visibility = Visibility.Visible;
        }

        private void optionBtn_Click(object sender, RoutedEventArgs e)
        {
            Storyboard sb = Resources["Setup"] as Storyboard;
            sb.Begin(SlidePanel);
        }

        private void serverOKBtn_Click(object sender, RoutedEventArgs e)
        {
            serverVersionBackground.Visibility = Visibility.Hidden;
            serverPopup.Visibility = Visibility.Hidden;
        }

        private void versionOKBtn_Click(object sender, RoutedEventArgs e)
        {
            serverVersionBackground.Visibility = Visibility.Hidden;
            versionPopup.Visibility = Visibility.Hidden;
        }

        private void fileDialogBtn_Click(object sender, RoutedEventArgs e)
        {
            using (System.Windows.Forms.FolderBrowserDialog fd = new System.Windows.Forms.FolderBrowserDialog())
            {
                if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    mcPath.Text = fd.SelectedPath; 
                }
            }
        }

        private void modsOKBtn_Click(object sender, RoutedEventArgs e)
        {
            serverVersionBackground.Visibility = Visibility.Hidden;
            modsPopup.Visibility = Visibility.Hidden;
        }

        private void serverAddBtn_Click(object sender, RoutedEventArgs e)
        {
            serverAddPopup.Visibility = Visibility.Visible;
        }

        private void serverAddBtnCancel_Click(object sender, RoutedEventArgs e)
        {
            serverAddIP.Text = "";
            serverAddPort.Text = "";
            serverAddName.Text = "";
            serverAddVersion.Text = "";
            serverAddType.IsChecked = false;

            serverAddPopup.Visibility = Visibility.Hidden;
        }

        private void serverAddBtnAdd_Click(object sender, RoutedEventArgs e)
        {
            string ip = serverAddIP.Text;
            string port = serverAddPort.Text;
            string name = serverAddName.Text;
            string version = serverAddVersion.Text;
            string type = serverAddType.IsChecked == true ? "forge" : "vanila";

            if (ip == "")
            {
                return;
            }

            int int_port = 0;
            int.TryParse(port, out int_port);

            if (name == "")
            {
                return;
            }

            if (version == "")
            {
                return;
            }

            if(sm.GetServers().Find(x => x.NAME == name) != null)
            {
                return;
            }

            sm.AddServer(new Server(ip, int_port, version, name, type));

            UpdateServerList();

            serverAddIP.Text = "";
            serverAddPort.Text = "";
            serverAddName.Text = "";
            serverAddVersion.Text = "";
            serverAddType.IsChecked = false;

            serverAddPopup.Visibility = Visibility.Hidden;
        }

        #endregion

        #region [Button Animation]

        private void startBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            startImg.Margin = new Thickness(0, 0, 0, 50);
            startTxt.Visibility = Visibility.Visible;
        }

        private void startBtn_MouseLeave(object sender, MouseEventArgs e)
        {
            startImg.Margin = new Thickness(0, 0, 0, 0);
            startTxt.Visibility = Visibility.Hidden;
        }

        private void serverBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            serverImg.Margin = new Thickness(0, 0, 0, 50);
            serverTxt.Visibility = Visibility.Visible;
        }

        private void serverBtn_MouseLeave(object sender, MouseEventArgs e)
        {
            serverImg.Margin = new Thickness(0, 0, 0, 0);
            serverTxt.Visibility = Visibility.Hidden;
        }

        private void versionBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            versionImg.Margin = new Thickness(0, 0, 0, 50);
            versionTxt.Visibility = Visibility.Visible;
        }

        private void versionBtn_MouseLeave(object sender, MouseEventArgs e)
        {
            versionImg.Margin = new Thickness(0, 0, 0, 0);
            versionTxt.Visibility = Visibility.Hidden;
        }

        private void optionBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            optionImg.Margin = new Thickness(0, 0, 0, 50);
            optionTxt.Visibility = Visibility.Visible;
        }

        private void optionBtn_MouseLeave(object sender, MouseEventArgs e)
        {
            optionImg.Margin = new Thickness(0, 0, 0, 0);
            optionTxt.Visibility = Visibility.Hidden;
        }

        private void modsBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            modsImg.Margin = new Thickness(0, 0, 0, 50);
            modsTxt.Visibility = Visibility.Visible;
        }

        private void modsBtn_MouseLeave(object sender, MouseEventArgs e)
        {
            modsImg.Margin = new Thickness(0, 0, 0, 0);
            modsTxt.Visibility = Visibility.Hidden;
        }

        private void startTxt_MouseEnter(object sender, MouseEventArgs e)
        {
            startImg.Margin = new Thickness(0, 0, 0, 50);
            startTxt.Visibility = Visibility.Visible;
        }

        private void startTxt_MouseLeave(object sender, MouseEventArgs e)
        {
            startImg.Margin = new Thickness(0, 0, 0, 0);
            startTxt.Visibility = Visibility.Hidden;
        }

        private void serverTxt_MouseEnter(object sender, MouseEventArgs e)
        {
            serverImg.Margin = new Thickness(0, 0, 0, 50);
            serverTxt.Visibility = Visibility.Visible;
        }

        private void serverTxt_MouseLeave(object sender, MouseEventArgs e)
        {
            serverImg.Margin = new Thickness(0, 0, 0, 0);
            serverTxt.Visibility = Visibility.Hidden;
        }

        private void versionTxt_MouseEnter(object sender, MouseEventArgs e)
        {
            versionImg.Margin = new Thickness(0, 0, 0, 50);
            versionTxt.Visibility = Visibility.Visible;
        }

        private void versionTxt_MouseLeave(object sender, MouseEventArgs e)
        {
            versionImg.Margin = new Thickness(0, 0, 0, 0);
            versionTxt.Visibility = Visibility.Hidden;
        }

        private void optionTxt_MouseEnter(object sender, MouseEventArgs e)
        {
            optionImg.Margin = new Thickness(0, 0, 0, 50);
            optionTxt.Visibility = Visibility.Visible;
        }

        private void optionTxt_MouseLeave(object sender, MouseEventArgs e)
        {
            optionImg.Margin = new Thickness(0, 0, 0, 0);
            optionTxt.Visibility = Visibility.Hidden;
        }

        private void modsTxt_MouseEnter(object sender, MouseEventArgs e)
        {
            modsImg.Margin = new Thickness(0, 0, 0, 50);
            modsTxt.Visibility = Visibility.Visible;
        }

        private void modsTxt_MouseLeave(object sender, MouseEventArgs e)
        {
            modsImg.Margin = new Thickness(0, 0, 0, 0);
            modsTxt.Visibility = Visibility.Hidden;
        }

        private void startTxtClick(object sender, MouseButtonEventArgs e)
        {
            ButtonAutomationPeer peer = new ButtonAutomationPeer(startBtn);
            IInvokeProvider invokeProv = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
            invokeProv.Invoke();
        }

        private void serverTxt_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ButtonAutomationPeer peer = new ButtonAutomationPeer(serverBtn);
            IInvokeProvider invokeProv = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
            invokeProv.Invoke();
        }

        private void versionTxt_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ButtonAutomationPeer peer = new ButtonAutomationPeer(versionBtn);
            IInvokeProvider invokeProv = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
            invokeProv.Invoke();
        }

        private void optionTxt_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ButtonAutomationPeer peer = new ButtonAutomationPeer(optionBtn);
            IInvokeProvider invokeProv = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
            invokeProv.Invoke();
        }

        private void modsTxt_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ButtonAutomationPeer peer = new ButtonAutomationPeer(modsBtn);
            IInvokeProvider invokeProv = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
            invokeProv.Invoke();
        }

        #endregion

        #region [Window Event]

        private void DragPoint_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void HyperLink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.AbsoluteUri);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Init();

            LoadConfig();
            sm.LoadServers();

            UpdateServerList();
            UpdateVersionList();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            SaveConfig();
            sm.SaveServers();
        }


        #endregion

        #region [Function]

        public void Init()
        {
            ulong _ram = 0;

            SafeNativeMethods.GetPhysicallyInstalledSystemMemory(out _ram);

            MAX_RAM = (int)_ram / (1024 * 1024);

            mine.download_file_change += Download_File_Change;
            mine.download_progress_change += Download_Progress_Change;
        }

        public void ProcessFunction(object sender, ElapsedEventArgs e)
        {
            try
            {
                Process.GetProcessById(p.Id);
            }
            catch (InvalidOperationException)
            {

                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                {
                    startBtn.IsEnabled = true;
                }));

                t.Stop();
                return;
            }
            catch (ArgumentException)
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                {
                    startBtn.IsEnabled = true;
                }));

                t.Stop();
                return;
            }
        }

        public enum PropertySettings
        {
            ID, PWD, ID_SAVE, MC_PATH, MC_RAM, MC_USE_OPTIFINE
        }

        public void SaveConfig()
        {
            List<string> config = new List<string>();

            string _id, _idSave, _mcPath, _mcRam, _mcOptifine, _pwd;

            _id = ID.Text;
            _idSave = IDsave.IsChecked.ToString();
            _mcPath = mcPath.Text;
            _mcRam = mcRam.Text;
            _mcOptifine = mcOptifine.IsChecked.ToString();
            _pwd = PWD.Password;

            if (_mcRam == "") _mcRam = "2";

            ManagementObjectSearcher sys = new ManagementObjectSearcher("select * from Win32_ComputerSystem");

            string maxRamSize = "";

            foreach (ManagementObject obj in sys.Get())
            {
                maxRamSize = obj["totalphysicalmemory"].ToString();
            }

            int maxRamSizeINT = (int)(double.Parse(maxRamSize) / 1000 / 1000 / 1000);

            if (int.Parse(_mcRam) > maxRamSizeINT)
            {
                _mcRam = maxRamSizeINT.ToString();
            }

            if (IDsave.IsChecked == true)
            {
                config.Add($"{PropertySettings.ID}={_id}");
                config.Add($"{PropertySettings.PWD}={_pwd}");
                config.Add($"{PropertySettings.ID_SAVE}={_idSave}");
            }
            else
            {
                config.Add($"{PropertySettings.ID}=");
                config.Add($"{PropertySettings.PWD}=");
                config.Add($"{PropertySettings.ID_SAVE}=False");
            }

            config.Add($"{PropertySettings.MC_PATH}={_mcPath}");
            config.Add($"{PropertySettings.MC_RAM}={_mcRam}");
            config.Add($"{PropertySettings.MC_USE_OPTIFINE}={_mcOptifine}");

            File.WriteAllLines($"{AppDomain.CurrentDomain.BaseDirectory}\\config.txt", config);
        }

        public void LoadConfig()
        {
            List<string> config = new List<string>();

            if (!File.Exists($"{AppDomain.CurrentDomain.BaseDirectory}\\config.txt"))
            {
                File.Create($"{AppDomain.CurrentDomain.BaseDirectory}\\config.txt");

                return;
            }

            config = File.ReadAllLines($"{AppDomain.CurrentDomain.BaseDirectory}\\config.txt").ToList();

            if (config.Count == 0) return;

            for (int i = 0; i < config.Count; i++)
            {
                config[i] = config[i].Split(new string[] { "=" }, StringSplitOptions.None)[1];
            }

            cfg_val = config;

            int load_ram = int.Parse(config[(int)PropertySettings.MC_RAM]);

            if(load_ram > MAX_RAM)
            {
                mcRam.Text = MAX_RAM.ToString();
            }
            else
            {
                mcRam.Text = load_ram.ToString();
            }

            mcPath.Text = config[(int)PropertySettings.MC_PATH];
            
            mcOptifine.IsChecked = bool.Parse(config[(int)PropertySettings.MC_USE_OPTIFINE]);

            ManagementObjectSearcher sys = new ManagementObjectSearcher("select * from Win32_VideoController");

            string graphicName = "";

            foreach (ManagementObject obj in sys.Get())
            {
                graphicName = obj["Name"].ToString();
            }

            textBlockGraphic.Text = graphicName;

            if(mcPath.Text == "")
            {
                mcPath.Text = mine.GetDefaultPath();
            }

            mine.PATH = mcPath.Text;

            if (bool.Parse(config[(int)PropertySettings.ID_SAVE]))
            {
                IDsave.IsChecked = true;
                ID.Text = config[(int)PropertySettings.ID];
                PWD.Password = config[(int)PropertySettings.PWD];

                ButtonAutomationPeer peer = new ButtonAutomationPeer(LoginBtn);
                IInvokeProvider invokeProv = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                invokeProv.Invoke();
            }
        }

        public class SafeNativeMethods
        {
            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool GetPhysicallyInstalledSystemMemory(out ulong MemoryInKilobytes);
        }

        public bool ImageSizeCheck(Image _img, int _width, int _height)
        {
            if(_img.Source.Width == _width && _img.Source.Height == _height)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void ChangeSkin()
        {
            Image img = new Image();

            if(ImageSizeCheck(img, 64, 64))
            {
                api.SetSkinFromAPI(mine.ACCESS_TOKEN, "");
            }
        }

        public void UpdateServerList()
        {
            SvrCbBox.Items.Clear();

            StackPanel noServerPanel = new StackPanel();
            noServerPanel.Orientation = Orientation.Vertical;

            TextBlock noServer = new TextBlock();
            noServer.Text = "No Server";
            noServer.FontSize = 15;
            noServer.FontWeight = FontWeights.Bold;

            noServerPanel.Children.Add(noServer);

            ComboBoxItem defaultItem = new ComboBoxItem();

            BrushConverter tBc = new BrushConverter();

            defaultItem.Background = (Brush)tBc.ConvertFrom("#FF897FA2");
            defaultItem.Content = noServerPanel;

            SvrCbBox.Items.Add(defaultItem);

            foreach (Server tServer in sm.GetServers())
            {
                //SvrCbBox.Items.Add(tServer.NAME);

                StackPanel tPanel = new StackPanel();
                tPanel.Orientation = Orientation.Vertical;

                TextBlock tName = new TextBlock();
                tName.Text = tServer.NAME;
                tName.FontSize = 15;
                tName.FontWeight = FontWeights.Bold;

                string ip = tServer.IP;

                if(tServer.PORT != 0)
                {
                    ip.Concat($":{tServer.PORT}");
                }

                TextBlock tIp = new TextBlock();
                tIp.Text = tServer.IP;

                TextBlock tVersion = new TextBlock();
                tVersion.Text = tServer.VERSION;

                Button tRemove = new Button();
                tRemove.Click += TRemove_Click;
                tRemove.Width = 20;
                tRemove.Height = 20;
                tRemove.HorizontalAlignment = HorizontalAlignment.Left;
                tRemove.Background = (Brush)tBc.ConvertFrom("#FF897FA2");
                tRemove.Content = "🗑";

                tPanel.Children.Add(tName);
                tPanel.Children.Add(tIp);
                tPanel.Children.Add(tVersion);
                tPanel.Children.Add(tRemove);

                ComboBoxItem tCbi = new ComboBoxItem();

                tCbi.Background = (Brush)tBc.ConvertFrom("#FF897FA2");
                tCbi.Content = tPanel;

                SvrCbBox.Items.Add(tCbi);

                if(selectedServer == null)
                {
                    SvrCbBox.SelectedIndex = 0;
                }
            }
        }

        private void TRemove_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            StackPanel pa = (StackPanel)btn.Parent;

            string name = ((TextBlock)pa.Children[0]).Text;

            Server findServer = sm.GetServers().Find(x => x.NAME == name);
            sm.GetServers().Remove(findServer);

            if(selectedServer.NAME == findServer.NAME)
            {
                selectedServer = null;
            }

            UpdateServerList();
        }

        public void UpdateVersionList()
        {
            foreach(string version in mine.GetAllReleaseVersion())
            {
                serverAddVersion.Items.Add(version);
                VerCbBox.Items.Add(version);
            }
        }

        public void Download_Progress_Change(object sender, ProgressChangedEventArgs e)
        {
            int percent = e.ProgressPercentage;
        }

        public void Download_File_Change(DownloadFileChangedEventArgs e)
        {
            int total = e.TotalFileCount;
            int progress = e.ProgressedFileCount;

            string name = e.FileName;

            string msg = $"Download({progress}/{total})... {{{name}}}";
        }

        #endregion
    }
}
