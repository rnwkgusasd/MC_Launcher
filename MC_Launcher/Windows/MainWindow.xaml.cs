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

namespace MC_Launcher
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
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

        public void SaveSettings()
        {
            List<string> settings = new List<string>();

            if (IDsave.IsChecked == true)
            {

            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string strFolder = mine.GetDefaultPath();//Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\.minecraft";

            if(Directory.Exists(strFolder))
            {
                mcPath.Text = strFolder;
            }

            mcRam.Text = "2048";
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            
        }
    }
}
