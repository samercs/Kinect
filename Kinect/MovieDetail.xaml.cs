using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Shapes;
using Kinect.DB;

namespace Kinect
{
    /// <summary>
    /// Interaction logic for MovieDetail.xaml
    /// </summary>
    
    public partial class MovieDetail : Window
    {
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
        public MovieDetail(int movieId)
        {
            this.MovieId = movieId;
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            Database db = new Database();
            db.AddParameter("@id", MovieId);
            DataTable dt = db.ExecuteDataTable("select * from movie where id=@id");
            Movie = new Movie
            {
                Id = int.Parse(dt.Rows[0]["id"].ToString()),
                Name = dt.Rows[0]["name"].ToString(),
                Img = @"~/../Images/" + dt.Rows[0]["image"],
                Type = int.Parse(dt.Rows[0]["type"].ToString()),
                Description = dt.Rows[0]["description"].ToString(),
                Duration = dt.Rows[0]["duration"].ToString(),
                Rate = int.Parse(dt.Rows[0]["rate"].ToString()),
                VideoUrl = dt.Rows[0]["VideoUrl"].ToString(),
                Writer = dt.Rows[0]["Author"].ToString(),
                Director = dt.Rows[0]["Director"].ToString(),
                Year = dt.Rows[0]["Year"].ToString(),
                Language = dt.Rows[0]["language"].ToString()
            };

            name.Content = Movie.Name;
            WebBrowser1.Source = new Uri(Movie.VideoUrl);
            descriptionLbl.Text = $"Description:\n{Movie.Description}";
            durationLbl.Content = $"Duration: {Movie.Duration}";
            lblRate.Content = $"({Movie.Rate}/5)";
            lblYear.Content = $"Year :{Movie.Year}";
            lblLanguage.Content = $"Language: {Movie.Language}";
            lblWriter.Content = $"Writer: {Movie.Writer}";
            lblDirector.Content = $"Director: {Movie.Director}";
            if (Movie.Rate >= 1)
            {
                rateImg1.Source = new BitmapImage(new Uri(@"pack://application:,,,/Kinect;component/Images/star.png"));
            }
            if (Movie.Rate >= 2)
            {
                rateImg2.Source = new BitmapImage(new Uri(@"pack://application:,,,/Kinect;component/Images/star.png"));
            }
            if (Movie.Rate >= 3)
            {
                rateImg3.Source = new BitmapImage(new Uri(@"pack://application:,,,/Kinect;component/Images/star.png"));
            }
            if (Movie.Rate >= 4)
            {
                rateImg4.Source = new BitmapImage(new Uri(@"pack://application:,,,/Kinect;component/Images/star.png"));
            }
            if (Movie.Rate >= 5)
            {
                rateImg5.Source = new BitmapImage(new Uri(@"pack://application:,,,/Kinect;component/Images/star.png"));
            }


        }

        private void BackImage_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var form = new MovieList(Movie.Type);
            form.Show();
            this.Hide();
        }

        private void CloseBtn_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
