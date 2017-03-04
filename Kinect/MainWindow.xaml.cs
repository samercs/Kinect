using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Kinect.DB;

namespace Kinect
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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
    }
}
