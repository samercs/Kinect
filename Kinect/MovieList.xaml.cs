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
    /// Interaction logic for MovieList.xaml
    /// </summary>
    public partial class MovieList : Window
    {
        public int MovieType { get; set; }
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        public MovieList(int movieType)
        {
            MovieType = movieType;
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            Label1.Content = String.Format(Label1.Content.ToString(), Enum.Parse(typeof(Kinect.DB.MovieType), this.MovieType.ToString()).ToString());
            Database db = new Database();
            db.AddParameter("@type", MovieType);
            DataTable data = db.ExecuteDataTable("select * from movie where Type=@type");
            var items = new List<Movie>();
            foreach (DataRow row in data.Rows)
            {
                items.Add(new Movie
                {
                    Id = Int32.Parse(row["id"].ToString()),
                    Name = row["name"].ToString(),
                    Img = @"~\..\Images\"+ row["image"]
                });
            }
            ItemsControl.ItemsSource = items;
        }

        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            Image image = e.Source as Image;
            var movieId = int.Parse(image.ToolTip.ToString());
            var form = new MovieDetail(movieId);
            form.Show();
            this.Hide();
        }

        private void ImgBack_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void ImgClose_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MovieList_OnLoaded(object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }
    }
}
