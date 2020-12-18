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

        private BackgroundWorker bw = new BackgroundWorker();

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ID.Text != "" && PWD.Password != "")
            {
                bw.WorkerReportsProgress = true;
                bw.WorkerSupportsCancellation = true;
                bw.DoWork += new DoWorkEventHandler(DoLogin);
                bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(LoginSuccess);

                bw.RunWorkerAsync();
                loadingLogin.Visibility = Visibility.Visible;
            }
            else
            {
                return;
            }
        }

        private void DoLogin(object sender, DoWorkEventArgs e)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
            {
                if (!mine.Login(ID.Text, PWD.Password)) e.Cancel = true;
                else imgSkin.Source = api.GetSkinFromAPI(mine.UUID);
            }));
        }

        private void LoginSuccess(object sender, RunWorkerCompletedEventArgs e)
        {
            if(e.Cancelled)
            {
                loadingLogin.Visibility = Visibility.Hidden;
            }
            else
            {
                this.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                {
                    Storyboard sb = Resources["LoginBtn"] as Storyboard;
                    sb.Begin(SlidePanel);
                    loadingLogin.Visibility = Visibility.Hidden;
                }));
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
    }
}
