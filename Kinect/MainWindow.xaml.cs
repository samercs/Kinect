using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Kinect.DB;
using Microsoft.Kinect;
using Microsoft.Kinect.Wpf.Controls;

namespace Kinect
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        public MainWindow()
        {
            

            InitializeComponent();
            KinectRegion.SetKinectRegion(this, kinectRegion);
            App app = ((App)Application.Current);
            app.KinectRegion = kinectRegion;
            this.kinectRegion.KinectSensor = KinectSensor.GetDefault();
            LoadData();
        }

        private void LoadData()
        {
            var data = Enum.GetValues(typeof(MovieType)).Cast<MovieType>().Select(i => new
            {
                Name = i.ToString(),
                Value = (int)i
            }).ToList();

            ItemsControl.ItemsSource = data;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Button btn = e.Source as Button;
            MovieList form = new MovieList((int)btn.CommandParameter);
            form.Show();
            this.Close();
        }

        

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }

        private void ButtonBack_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
