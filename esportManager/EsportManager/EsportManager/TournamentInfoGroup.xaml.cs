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
        List<TournamentStandings> standings = new List<TournamentStandings>();
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
            int counter = 0;
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select ta.logo, tb.logo, m.id_teamxsection_home, m.id_teamxsection_away, m.id_match from '" + year + "match" + idSection + "' m join teamxsection a on a.id_teamxsection = m.id_teamxsection_home join teamxsection b on b.id_teamxsection = m.id_teamxsection_away join team ta on ta.id_team = a.id_team join team tb on tb.id_team = b.id_team where id_tournament=" + tournament + " and home_score is null limit 7;", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    byte[] data = (byte[])reader[0];
                    BitmapImage imageSource1 = new BitmapImage();
                    using (MemoryStream ms = new MemoryStream(data))
                    {
                        imageSource1.BeginInit();
                        imageSource1.StreamSource = ms;
                        imageSource1.CacheOption = BitmapCacheOption.OnLoad;
                        imageSource1.EndInit();
                    }

                    data = (byte[])reader[1];
                    BitmapImage imageSource2 = new BitmapImage();
                    using (MemoryStream ms = new MemoryStream(data))
                    {
                        imageSource2.BeginInit();
                        imageSource2.StreamSource = ms;
                        imageSource2.CacheOption = BitmapCacheOption.OnLoad;
                        imageSource2.EndInit();
                    }
                    Image i = new Image();
                    i.Width = 30;
                    i.Height = 30;
                    i.SnapsToDevicePixels = true;
                    i.Source = imageSource1;
                    i.HorizontalAlignment = HorizontalAlignment.Left;
                    i.VerticalAlignment = VerticalAlignment.Top;
                    i.Margin = new Thickness(10, 10 + (counter * 50), 0, 0);
                    MatchesIncoming.Children.Add(i);
                    Image i2 = new Image();
                    i2.Width = 30;
                    i2.Height = 30;
                    i2.HorizontalAlignment = HorizontalAlignment.Left;
                    i2.VerticalAlignment = VerticalAlignment.Top;
                    i2.Margin = new Thickness(90, 10 + (counter * 50), 0, 0);
                    i2.Source = imageSource2;
                    MatchesIncoming.Children.Add(i2);
                    Label sc = new Label();
                    sc.Content =  "-";
                    sc.HorizontalAlignment = HorizontalAlignment.Center;
                    sc.VerticalAlignment = VerticalAlignment.Top;
                    sc.FontSize = 13;
                    sc.Width = 40;
                    sc.Margin = new Thickness(0, 20 + (counter * 50), 0, 0);
                    sc.HorizontalContentAlignment = HorizontalAlignment.Center;
                    MatchesIncoming.Children.Add(sc);

                    counter++;
                }
            }
        }

        private void CreateStandings()
        {
            standings = standings.OrderByDescending(o => o.MatchesWon).ToList();
            for (int i = 0; i < standings.Count; i++)
            {
                standings.ElementAt(i).Position = i + 1;
            }
            List<int> tournamentsColours = new List<int>();
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select tournament.shortcut, tournament_from_position, tournament.id_tournament from tournament_token join tournament on tournament_token.id_tournament_to=tournament.id_tournament where id_tournament_from=" + tournament + ";", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                
                while (reader.Read())
                {
                    standings.ElementAt(reader.GetInt32(1)-1).TournamentTo = reader.GetString(0);
                    tournamentsColours.Add(reader.GetInt32(2));
                }
            }
            Standings.ItemsSource = standings;

            /*foreach (TournamentStandings item in Standings.ItemsSource)
            {
                var row = Standings.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
                if (item.TournamentTo == "LEC2G")
                {
                    row.Background = Brushes.Pink;
                }
                else
                { 
                    row.Background = Brushes.YellowGreen;
                }
            }*/
        }

        private void SetPlayedMatches()
        {
            int counter = 0;
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select ta.logo, tb.logo, m.id_teamxsection_home, m.id_teamxsection_away, m.id_match, m.home_score, m.away_score from '" + year + "match" + idSection +"' m join teamxsection a on a.id_teamxsection = m.id_teamxsection_home join teamxsection b on b.id_teamxsection = m.id_teamxsection_away join team ta on ta.id_team = a.id_team join team tb on tb.id_team = b.id_team where id_tournament=" + tournament + " and home_score not null order by m.match_date;", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    // výpočet pro tabulku
                    for (int j = 0; j < standings.Count; j++)
                    {
                        if (reader.GetInt32(2) == standings.ElementAt(j).IdTeamSection)
                        {
                            if (reader.GetInt32(5) > reader.GetInt32(6))
                            {
                                standings.ElementAt(j).MatchesWon++;
                            } else
                            {
                                standings.ElementAt(j).MatchesLost++;
                            }
                            standings.ElementAt(j).MatchesPlayed++;
                            standings.ElementAt(j).MapsWon += reader.GetInt32(5);
                            standings.ElementAt(j).MapsLost += reader.GetInt32(6);
                        }
                        else if (reader.GetInt32(3) == standings.ElementAt(j).IdTeamSection)
                        {
                            if (reader.GetInt32(6) > reader.GetInt32(5))
                            {
                                standings.ElementAt(j).MatchesWon++;
                            }
                            else
                            {
                                standings.ElementAt(j).MatchesLost++;
                            }
                            standings.ElementAt(j).MatchesPlayed++;
                            standings.ElementAt(j).MapsWon += reader.GetInt32(6);
                            standings.ElementAt(j).MapsLost += reader.GetInt32(5);
                        }
                    }
                    // vizuální stránka
                    if (counter < 7)
                    {

                        byte[] data = (byte[])reader[0];
                        BitmapImage imageSource1 = new BitmapImage();
                        using (MemoryStream ms = new MemoryStream(data))
                        {
                            imageSource1.BeginInit();
                            imageSource1.StreamSource = ms;
                            imageSource1.CacheOption = BitmapCacheOption.OnLoad;
                            imageSource1.EndInit();
                        }   

                        data = (byte[])reader[1];
                        BitmapImage imageSource2 = new BitmapImage();
                        using (MemoryStream ms = new MemoryStream(data))
                        {
                            imageSource2.BeginInit();
                            imageSource2.StreamSource = ms;
                            imageSource2.CacheOption = BitmapCacheOption.OnLoad;
                            imageSource2.EndInit();
                        }
                        Image i = new Image();
                        i.Width = 30;
                        i.Height = 30;
                        i.SnapsToDevicePixels = true;
                        i.Source = imageSource1;
                        i.HorizontalAlignment = HorizontalAlignment.Left;
                        i.VerticalAlignment = VerticalAlignment.Top;
                        i.Margin = new Thickness(10, 10+(counter*50), 0, 0);
                        MatchesPlayed.Children.Add(i);
                        Image i2 = new Image();
                        i2.Width = 30;
                        i2.Height = 30;
                        i2.HorizontalAlignment = HorizontalAlignment.Left;
                        i2.VerticalAlignment = VerticalAlignment.Top;
                        i2.Margin = new Thickness(90, 10 + (counter * 50), 0, 0);
                        i2.Source = imageSource2;
                        MatchesPlayed.Children.Add(i2);
                        Label sc = new Label();
                        sc.Content = reader.GetInt32(5) + " - " + reader.GetInt32(6);
                        sc.HorizontalAlignment = HorizontalAlignment.Center;
                        sc.VerticalAlignment = VerticalAlignment.Top;
                        sc.FontSize = 13;
                        sc.Width = 40;
                        sc.Margin = new Thickness(0, 20 + (counter * 50), 0, 0);
                        sc.HorizontalContentAlignment = HorizontalAlignment.Center;
                        MatchesPlayed.Children.Add(sc);
                    }
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
                command = new SQLiteCommand("select tournament_token.id_teamxsection,team.name from tournament_token join teamxsection on teamxsection.id_teamxsection=tournament_token.id_teamxsection join team on team.id_team=teamxsection.id_team where id_tournament_to=" + tournament + ";", conn);
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    standings.Add(new TournamentStandings(reader.GetInt32(0), reader.GetString(1)));
                }
                reader.Close();
            }
        }
    }
}
