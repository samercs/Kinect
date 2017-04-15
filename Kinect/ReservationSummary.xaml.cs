using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Shapes;
using Kinect.DB;
using Microsoft.Kinect;
using Microsoft.Kinect.Wpf.Controls;

namespace Kinect
{
    
    public partial class ReservationSummary : Window
    {

        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        private int ReservationId { get; set; }

        private void MovieList_OnLoaded(object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }

        public ReservationSummary(int reservationId)
        {
            this.ReservationId = reservationId;
            InitializeComponent();
            KinectRegion.SetKinectRegion(this, kinectRegion);
            App app = ((App)Application.Current);
            app.KinectRegion = kinectRegion;
            this.kinectRegion.KinectSensor = KinectSensor.GetDefault();
            LoadData();
        }

        private void LoadData()
        {
            var db = new Database();
            var tool = new Tool(db);
            LblReservationId.Content = this.ReservationId.ToString();
            db.AddParameter("@id", this.ReservationId);
            var reservationSeats = db.ExecuteDataTable("select * from reservationSeat where reservationId=@id");
            var reservationSeatsList = new List<string>();
            foreach (DataRow row in reservationSeats.Rows)
            {
                reservationSeatsList.Add(row["SeatNo"].ToString());
            }
            LblReservationSeats.Content = string.Join(",", reservationSeatsList);

            db.AddParameter("@id", this.ReservationId);
            var reservationSnaks = db.ExecuteDataTable("select * from reservationSnak where reservationId=@id");
            var reservationSnaksList = new List<string>();
            foreach (DataRow row in reservationSnaks.Rows)
            {
                reservationSnaksList.Add($"{row["Quntity"]} {row["Type"]} {row["SnakName"]}");
            }
            LblReservationSnaks.Text = string.Join(",", reservationSnaksList);

            LblReservationTotal.Content = tool.GetOrderTotal(this.ReservationId);
        }

        private void ButtonOk_OnClick(object sender, RoutedEventArgs e)
        {
            var main = new MainWindow();
            main.Show();
            this.Hide();
        }
    }
}
