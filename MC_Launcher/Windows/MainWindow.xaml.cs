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

        #endregion

        #region [Initialize]

        public MainWindow()
        {
            InitializeComponent();
            // https://www.wpf-tutorial.com/tabcontrol/styling-the-tabitems/ custom grid
        }

        #endregion

        #region [Button Click Event]

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ID.Text != "" && PWD.Password != "")
            {
                loadingLogin.Visibility = Visibility.Visible;

                if (!mine.Login(ID.Text, PWD.Password))
                {
                    loadingLogin.Visibility = Visibility.Hidden;
                    return;
                }
                else imgSkin.Source = api.GetSkinFromAPI(mine.UUID);

                Storyboard sb = Resources["LoginBtn"] as Storyboard;
                sb.Begin(SlidePanel);
                loadingLogin.Visibility = Visibility.Hidden;
            }
            else
            {
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
            p = mine.Start("1.16.5");

            startBtn.IsEnabled = false;

            t = new System.Timers.Timer(500);
            t.Elapsed += ProcessFunction;
            t.Start();
        }

        private void serverBtn_Click(object sender, RoutedEventArgs e)
        {
            serverVersionBackground.Visibility = Visibility.Visible;
            serverPopup.Visibility = Visibility.Visible;
        }

        private void modsBtn_Click(object sender, RoutedEventArgs e)
        {
            serverVersionBackground.Visibility = Visibility.Visible;
            modsBtn.Visibility = Visibility.Visible;
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
            serverVersionBackground.Visibility = Visibility.Visible;
            modsBtn.Visibility = Visibility.Visible;
        }

        private void serverAddBtn_Click(object sender, RoutedEventArgs e)
        {
            sm.AddServer(new Server("192.168.0.1", 9100, "1.16.5", "TEST", "T"));
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
            LoadConfig();
            sm.LoadServers();

            foreach (Server tServer in sm.GetServers())
            {
                //SvrCbBox.Items.Add(tServer.NAME);

                StackPanel tPanel = new StackPanel();

                TextBlock tName = new TextBlock();
                tName.Text = tServer.NAME;

                tPanel.Children.Add(tName);

                SvrCbBox.Items.Add(tPanel);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            SaveConfig();
            sm.SaveServers();
        }


        #endregion

        #region [Function]

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
            ID, ID_SAVE, MC_PATH, MC_RAM, MC_USE_OPTIFINE
        }

        public void SaveConfig()
        {
            List<string> config = new List<string>();

            string _id, _idSave, _mcPath, _mcRam, _mcUseOptifine;

            _id = ID.Text;
            _idSave = IDsave.IsChecked.ToString();
            _mcPath = mcPath.Text;
            _mcRam = mcRam.Text;
            _mcUseOptifine = "False";

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
                config.Add($"{PropertySettings.ID_SAVE}={_idSave}");
            }
            else
            {
                config.Add($"{PropertySettings.ID}=");
                config.Add($"{PropertySettings.ID_SAVE}=False");
            }

            config.Add($"{PropertySettings.MC_PATH}={_mcPath}");
            config.Add($"{PropertySettings.MC_RAM}={_mcRam}");
            config.Add($"{PropertySettings.MC_USE_OPTIFINE}={_mcUseOptifine}");

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

            if (bool.Parse(config[(int)PropertySettings.ID_SAVE]))
            {
                IDsave.IsChecked = true;
                ID.Text = config[(int)PropertySettings.ID];
            }

            mcPath.Text = config[(int)PropertySettings.MC_PATH];
            mcRam.Text = config[(int)PropertySettings.MC_RAM];
            // not already optifine checkbox
            //mcUseOptifine.IsChecked = bool.Parse(settings[(int)PropertySettings.MC_USE_OPTIFINE]);

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
                //api.SetSkinFromAPI(mine.ACCESS_TOKEN, "");
            }
        }

        #endregion

        
    }
}
