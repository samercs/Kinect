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
            if (ComboBoxPopCornType.Text.Length != 0)
            {
                var quntity = int.Parse(ComboBoxPopCornQ.Text);
                db.AddParameter("@ReservationId", this.ReservationId);
                db.AddParameter("@SnakName", "Popcorn");
                db.AddParameter("@Quntity", quntity);
                db.AddParameter("@Type", ComboBoxPopCornType.Text);
                db.ExecuteNonQuery(
                    "insert into ReservationSnak(ReservationId,SnakName,Quntity,Type) values(@ReservationId,@SnakName,@Quntity,@Type)");
            }

            if (ComboBoxPipsiType.Text.Length != 0)
            {
                var quntity = int.Parse(ComboBoxPipsiQ.Text);
                db.AddParameter("@ReservationId", this.ReservationId);
                db.AddParameter("@SnakName", "Pepsi");
                db.AddParameter("@Quntity", quntity);
                db.AddParameter("@Type", ComboBoxPipsiType.Text);
                db.ExecuteNonQuery(
                    "insert into ReservationSnak(ReservationId,SnakName,Quntity,Type) values(@ReservationId,@SnakName,@Quntity,@Type)");
            }

            if (ComboBoxWaterType.Text.Length != 0)
            {
                var quntity = int.Parse(ComboBoxWaterQ.Text);
                db.AddParameter("@ReservationId", this.ReservationId);
                db.AddParameter("@SnakName", "Water");
                db.AddParameter("@Quntity", quntity);
                db.AddParameter("@Type", ComboBoxWaterType.Text);
                db.ExecuteNonQuery(
                    "insert into ReservationSnak(ReservationId,SnakName,Quntity,Type) values(@ReservationId,@SnakName,@Quntity,@Type)");
            }

            if (ComboBoxNashosType.Text.Length != 0)
            {
                var quntity = int.Parse(ComboBoxNashosQ.Text);
                db.AddParameter("@ReservationId", this.ReservationId);
                db.AddParameter("@SnakName", "Nashos");
                db.AddParameter("@Quntity", quntity);
                db.AddParameter("@Type", ComboBoxNashosType.Text);
                db.ExecuteNonQuery(
                    "insert into ReservationSnak(ReservationId,SnakName,Quntity,Type) values(@ReservationId,@SnakName,@Quntity,@Type)");
            }

            MessageBox.Show("Snak has been added to your order successfully. Click Ok to see your order summary page.");
            var form = new ReservationSummary(this.ReservationId);
            form.Show();
            this.Hide();
        }
    }
}
