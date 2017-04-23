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
    /// <summary>
    /// Interaction logic for ReservationConfirmation.xaml
    /// </summary>
    public partial class ReservationConfirmation : Window
    {
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        public int ReservationId { get; set; }


        public ReservationConfirmation(int id)
        {
            ReservationId = id;
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
            db.AddParameter("@id", this.ReservationId);
            DataTable reservation = db.ExecuteDataTable("select * from Reservation where id=@id");
            db.AddParameter("@id", this.ReservationId);
            DataTable reservationSeat = db.ExecuteDataTable("select * from ReservationSeat where ReservationId=@id");
            var seatList = new List<string>();
            foreach (DataRow reservationSeatRow in reservationSeat.Rows)
            {
                seatList.Add(reservationSeatRow["seatNo"].ToString());
            }

            var txt = Label1.Content.ToString();

            Label1.Content = txt.Replace("{0}", reservation.Rows[0]["id"] + " at " + reservation.Rows[0]["ReservationTime"]).Replace("{1}", string.Join(",", seatList));

        }

        private void MovieList_OnLoaded(object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }

        private void ButtonConfirm_OnClick(object sender, RoutedEventArgs e)
        {
            var db = new Database();
            var tool = new Tool(db);
            db.AddParameter("@id", this.ReservationId);
            db.ExecuteNonQuery("update reservation set status=1 where id=@id");
            var total = tool.GetOrderTotal(this.ReservationId);
            Label1.Content =
                $"Your reservation has been confirmed successfully.{Environment.NewLine} Your order total is KD {total}. {Environment.NewLine} Do you need to add snaks to your order?";
            ButtonConfirm.Visibility = Visibility.Hidden;
            ButtonYes.Visibility = Visibility.Visible;
            ButtonNo.Visibility = Visibility.Visible;

        }

        private void ButtonClose_OnClick(object sender, RoutedEventArgs e)
        {
            DeleteReservation();
            Application.Current.Shutdown();
        }

        private void DeleteReservation()
        {
            var db = new Database();
            db.AddParameter("@id", this.ReservationId);
            db.ExecuteNonQuery("delete from ReservationSeat where ReservationId=@id");
            db.AddParameter("@id", this.ReservationId);
            db.ExecuteNonQuery("delete from Reservation where Id=@id");
        }

        private void ButtonBack_OnClick(object sender, RoutedEventArgs e)
        {

            var db = new Database();
            db.AddParameter("@id", this.ReservationId);
            DataTable reservation = db.ExecuteDataTable("select * from reservation where id=@id");
            var movieId = reservation.Rows[0]["movieId"].ToString();
            DeleteReservation();
            var movieDetail = new MovieDetail(int.Parse(movieId));
            movieDetail.Show();
            this.Hide();
        }

        private void ButtonYes_OnClick(object sender, RoutedEventArgs e)
        {
            var addSnak = new AddSnak(this.ReservationId);
            addSnak.Show();
            this.Hide();
        }

        private void ButtonNo_OnClick(object sender, RoutedEventArgs e)
        {
            var main = new MainWindow();
            main.Show();
            this.Hide();
        }
    }
}
