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
            db.AddParameter("@id", this.ReservationId);
            db.ExecuteNonQuery("update reservation set status=1 where id=@id");
            MessageBox.Show("Your reservation has been confirmed successfully. thank you");
            var main = new MainWindow();
            main.Show();
            this.Hide();
        }
    }
}
