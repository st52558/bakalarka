using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
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

namespace EsportManager
{
    /// <summary>
    /// Interakční logika pro TournamentInfo.xaml
    /// </summary>
    public partial class TournamentInfoGroup : Window
    {
        class MatchDataGrid
        {
            public BitmapImage homeTeam { get; set; }
            public BitmapImage awayTeam { get; set; }
            public string score { get; set; }
            public MatchDataGrid(BitmapImage h, BitmapImage a, string s)
            {
                homeTeam = h;
                awayTeam = a;
                score = s;
            }
        }
        List<MatchDataGrid> playedMatches = new List<MatchDataGrid>();
        string databaseName;
        int tournament;
        int year;
        int idSection;
        public TournamentInfoGroup(string databaseNameI, int tournamentI)
        {
            year = 2019;
            databaseName = databaseNameI;
            tournament = tournamentI;
            InitializeComponent();
            SetTournamentProperties();
            SetPlayedMatches();
            CreateStandings();
            SetIncomingMatches();
        }

        private void SetIncomingMatches()
        {
            
        }

        private void CreateStandings()
        {
            
        }

        private void SetPlayedMatches()
        {
            int counter = 0;
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select ta.name, tb.name, ta.logo, tb.logo, m.id_teamxsection_home, m.id_teamxsection_away, m.id_match, m.home_score, m.away_score from '" + year + "match" + idSection +"' m join teamxsection a on a.id_teamxsection = m.id_teamxsection_home join teamxsection b on b.id_teamxsection = m.id_teamxsection_away join team ta on ta.id_team = a.id_team join team tb on tb.id_team = b.id_team where id_tournament=" + tournament + " and home_score not null;", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    byte[] data = (byte[])reader[2];
                    BitmapImage imageSource1 = new BitmapImage();
                    using (MemoryStream ms = new MemoryStream(data))
                    {
                        imageSource1.BeginInit();
                        imageSource1.StreamSource = ms;
                        imageSource1.CacheOption = BitmapCacheOption.OnLoad;
                        imageSource1.EndInit();
                    }

                    data = (byte[])reader[3];
                    BitmapImage imageSource2 = new BitmapImage();
                    using (MemoryStream ms = new MemoryStream(data))
                    {
                        imageSource2.BeginInit();
                        imageSource2.StreamSource = ms;
                        imageSource2.CacheOption = BitmapCacheOption.OnLoad;
                        imageSource2.EndInit();
                    }
                    playedMatches.Add(new MatchDataGrid(imageSource1, imageSource2, "adsf"));
                    Image i = new Image();
                    i.Width = 30;
                    i.Height = 30;
                    i.Source = imageSource1;
                    i.HorizontalAlignment = HorizontalAlignment.Left;
                    i.VerticalAlignment = VerticalAlignment.Top;
                    i.Margin = new Thickness(10, 10+(counter*30), 0, 0);
                    MatchesPlayed.Children.Add(i);
                    Image i2 = new Image();
                    i2.Width = 30;
                    i2.Height = 30;
                    i2.HorizontalAlignment = HorizontalAlignment.Left;
                    i2.VerticalAlignment = VerticalAlignment.Top;
                    i2.Margin = new Thickness(90, 10 + (counter * 30), 0, 0);
                    i2.Source = imageSource2;
                    MatchesPlayed.Children.Add(i2);
                    Label sc = new Label();
                    sc.Content = reader.GetInt32(7) + " - " + reader.GetInt32(8);
                    sc.HorizontalAlignment = HorizontalAlignment.Center;
                    sc.VerticalAlignment = VerticalAlignment.Top;
                    sc.Width = 40;
                    sc.Margin = new Thickness(0, 20 + (counter * 30), 0, 0);
                    sc.HorizontalContentAlignment = HorizontalAlignment.Center;
                    MatchesPlayed.Children.Add(sc);
                    counter++;
                }
                reader.Close();
            }
        }

        private void SetTournamentProperties()
        {
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select name,game from tournament where id_tournament=" + tournament + ";", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                reader.Read();
                TournamentName.Content = reader.GetString(0);
                idSection = reader.GetInt32(1);
                reader.Close();
            }
        }
    }
}
