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

namespace Kinect
{

    public partial class Reservation : Window
    {

        public ReservationData ReservationData { get; set; }
        private IList<string> Seats { get;set; }

        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        public Reservation(ReservationData reservationData)
        {
            this.ReservationData = reservationData;

            Seats = new List<string>();
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            var db = new Database();
            db.AddParameter("@id", ReservationData.MovieId);
            db.AddParameter("@time", ReservationData.ReservationTime);
            DataTable data = db.ExecuteDataTable("select ReservationSeat.SeatNo from ReservationSeat inner join reservation on (ReservationSeat.ReservationId = reservation.id) where reservation.MovieId=@id and ReservationTime=@time");
            foreach (DataRow row in data.Rows)
            {
                var btn = UIHelper.FindChild<Button>(Grid1, $"Btn{row["seatNo"]}");
                if (btn != null)
                {
                    btn.IsEnabled = false;
                }
                
            }
        }

        private void MovieList_OnLoaded(object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }

        private void ButtonBack_OnClick(object sender, RoutedEventArgs e)
        {
            var movieDetail = new MovieDetail(this.ReservationData.MovieId);
            movieDetail.Show();
            this.Hide();
            
        }

        private void ButtonClose_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ButtonSeat_OnClick(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var seat = btn.Content;
            if (btn.CommandParameter == null)
            {
                btn.CommandParameter = "true";
                btn.Background = Brushes.Red;
                Seats.Add(seat.ToString());
            }
            else
            {
                btn.CommandParameter = null;
                btn.Background = Brushes.LightGray;
                Seats.Remove(seat.ToString());
            }
        }

        private void ButtonProceed_OnClick(object sender, RoutedEventArgs e)
        {
            var db = new Database();
            db.AddParameter("@ReservationTime", ReservationData.ReservationTime);
            db.AddParameter("@MovieId", ReservationData.MovieId);
            db.AddParameter("@Status", "0");

            var reservationId = db.ExecuteNonQuery_id(
                "insert into reservation(ReservationTime, MovieId, Status) values(@ReservationTime, @MovieId, @Status)");

            foreach (var seat in Seats)
            {
                db.AddParameter("@ReservationId", reservationId);
                db.AddParameter("@SeatNo", seat);
                db.ExecuteNonQuery_id(
                "insert into reservationSeat(ReservationId, SeatNo) values(@ReservationId, @SeatNo)");
            }

            var confirmation = new ReservationConfirmation((int)reservationId);
            confirmation.Show();
            this.Hide();

        }
    }
}
