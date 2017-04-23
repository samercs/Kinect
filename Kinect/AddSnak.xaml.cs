using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Kinect.DB;
using Microsoft.Kinect;
using Microsoft.Kinect.Wpf.Controls;

namespace Kinect
{

    public partial class AddSnak : Window
    {
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);


        public int ReservationId { get; set; }
        private string PopCornType { get; set; }
        private string PipiType { get; set; }
        private string WaterType { get; set; }
        private string NachosType { get; set; }


        private void MovieList_OnLoaded(object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }
        public AddSnak(int id)
        {
            this.ReservationId = id;
            InitializeComponent();
            KinectRegion.SetKinectRegion(this, kinectRegion);
            App app = ((App)Application.Current);
            app.KinectRegion = kinectRegion;
            this.kinectRegion.KinectSensor = KinectSensor.GetDefault();

        }

        private void ButtonClose_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var db = new Database();

            //Popcorn has been selected
            if (!string.IsNullOrEmpty(PopCornType))
            {
                var quntity = ComboBoxPopCornQ.Value;
                db.AddParameter("@ReservationId", this.ReservationId);
                db.AddParameter("@SnakName", "Popcorn");
                db.AddParameter("@Quntity", quntity);
                db.AddParameter("@Type", PopCornType);
                db.ExecuteNonQuery(
                    "insert into ReservationSnak(ReservationId,SnakName,Quntity,Type) values(@ReservationId,@SnakName,@Quntity,@Type)");
            }

            if (!string.IsNullOrEmpty(PipiType))
            {
                var quntity = ComboBoxPipsiQ.Value;
                db.AddParameter("@ReservationId", this.ReservationId);
                db.AddParameter("@SnakName", "Pepsi");
                db.AddParameter("@Quntity", quntity);
                db.AddParameter("@Type", PipiType);
                db.ExecuteNonQuery(
                    "insert into ReservationSnak(ReservationId,SnakName,Quntity,Type) values(@ReservationId,@SnakName,@Quntity,@Type)");
            }

            if (!string.IsNullOrEmpty(WaterType))
            {
                var quntity = ComboBoxWaterQ.Value;
                db.AddParameter("@ReservationId", this.ReservationId);
                db.AddParameter("@SnakName", "Water");
                db.AddParameter("@Quntity", quntity);
                db.AddParameter("@Type", WaterType);
                db.ExecuteNonQuery(
                    "insert into ReservationSnak(ReservationId,SnakName,Quntity,Type) values(@ReservationId,@SnakName,@Quntity,@Type)");
            }

            if (!string.IsNullOrEmpty(NachosType))
            {
                var quntity = ComboBoxNachosQ.Value;
                db.AddParameter("@ReservationId", this.ReservationId);
                db.AddParameter("@SnakName", "Nashos");
                db.AddParameter("@Quntity", quntity);
                db.AddParameter("@Type", NachosType);
                db.ExecuteNonQuery(
                    "insert into ReservationSnak(ReservationId,SnakName,Quntity,Type) values(@ReservationId,@SnakName,@Quntity,@Type)");
            }

            var form = new ReservationSummary(this.ReservationId);
            form.Show();
            this.Hide();
        }

        private void ComboBoxPopCornTypeL_OnClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            PopCornType = button.Content.ToString();
            ComboBoxPopCornTypeL.Background = Brushes.LightGray;
            ComboBoxPopCornTypeM.Background = Brushes.LightGray;
            ComboBoxPopCornTypeS.Background = Brushes.LightGray;
            button.Background = Brushes.Red;

        }

        private void ComboBoxPopCornQ_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (PopCornQ != null)
            {
                PopCornQ.Content = e.NewValue;
            }

        }

        private void ComboBoxPipsiTypeL_OnClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            PipiType = button.Content.ToString();
            ComboBoxPipsiTypeL.Background = Brushes.LightGray;
            ComboBoxPipsiTypeM.Background = Brushes.LightGray;
            ComboBoxPipsiTypeS.Background = Brushes.LightGray;
            button.Background = Brushes.Red;
        }

        private void ComboBoxPipsiQ_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (PipsiQ != null)
            {
                PipsiQ.Content = e.NewValue;
            }
        }

        private void ComboBoxWaterTypeL_OnClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            WaterType = button.Content.ToString();
            ComboBoxWaterTypeL.Background = Brushes.LightGray;
            ComboBoxWaterTypeM.Background = Brushes.LightGray;
            ComboBoxWaterTypeS.Background = Brushes.LightGray;
            button.Background = Brushes.Red;
        }

        private void ComboBoxWaterQ_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (WaterQ != null)
            {
                WaterQ.Content = e.NewValue;
            }
        }

        private void ComboBoxNachosTypeL_OnClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            NachosType = button.Content.ToString();
            ComboBoxNachosTypeL.Background = Brushes.LightGray;
            ComboBoxNachosTypeM.Background = Brushes.LightGray;
            ComboBoxNachosTypeS.Background = Brushes.LightGray;
            button.Background = Brushes.Red;
        }

        private void ComboBoxNachosQ_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (NachosQ != null)
            {
                NachosQ.Content = e.NewValue;
            }
        }
    }
}
