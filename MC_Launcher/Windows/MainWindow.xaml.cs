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
        private List<string> cfg_val = new List<string>();
        private ServerManager sm = new ServerManager();

        public MainWindow()
        {
            InitializeComponent();
            // https://www.wpf-tutorial.com/tabcontrol/styling-the-tabitems/ custom grid
        }

        private void DragPoint_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void HyperLink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.AbsoluteUri);
        }

        public Source.AccessAPI api = new Source.AccessAPI();
        public Source.Minecraft mine = new Source.Minecraft();

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
            if(IDsave.IsChecked == null || IDsave.IsChecked == false)
            {
                ID.Text = "";
            }
            PWD.Password = "";

            Storyboard sb = Resources["BackLogin"] as Storyboard;
            sb.Begin(SlidePanel);
        }

        private void Setup_Click(object sender, RoutedEventArgs e)
        {
            Storyboard sb = Resources["Setup"] as Storyboard;
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

            for(int i = 0; i < config.Count; i++)
            {
                config[i] = config[i].Split(new string[] { "=" }, StringSplitOptions.None)[1];
            }

            cfg_val = config;

            if(bool.Parse(config[(int)PropertySettings.ID_SAVE]))
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
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadConfig();
            sm.LoadServers();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            SaveConfig();
            sm.SaveServers();
        }
    }
}
