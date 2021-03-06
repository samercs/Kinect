﻿using System;
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
            KinectRegion.SetKinectRegion(this, kinectRegion);
            App app = ((App)Application.Current);
            app.KinectRegion = kinectRegion;
            this.kinectRegion.KinectSensor = KinectSensor.GetDefault();
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
        
        private void MovieList_OnLoaded(object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var btn = e.Source as Button;
            var movieId = int.Parse(btn.CommandParameter.ToString());
            var form = new MovieDetail(movieId);
            form.Show();
            this.Hide();
        }
        private void ButtonClose_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ButtonBack_OnClick(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
