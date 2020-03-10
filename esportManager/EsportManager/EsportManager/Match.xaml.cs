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
    /// Interaction logic for Match.xaml
    /// </summary>
    public partial class Match : Window
    {
        string databaseName;
        string curDate;
        int idTeam;
        int idSection;
        int homeTeam;
        int awayTeam;
        bool matchFinished;
        int nOfPlayers;
        bool positionRequired;
        int homeScore;
        int awayScore;
        int bestOf;
        int idMatch;
        List<Player> homePlayers = new List<Player>();
        List<Player> awayPlayers = new List<Player>();
        public Match(string database, int idT, int idS)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            matchFinished = false;
            databaseName = database;
            idSection = idS;
            idTeam = idT;
            GetDate();
            SetTeams();
            SetPlayers();
        }

        private void SetPlayers()
        {
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select position_required, n_o_players from section where id_section=" + idSection + ";", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                reader.Read();
                if (reader.GetInt32(0) == 1) {
                    positionRequired = true;
                }
                else
                {
                    positionRequired = false;
                }
                nOfPlayers = reader.GetInt32(1);
                reader.Close();
            }
            for (int i = 1; i <= nOfPlayers*2; i++)
            {
                ComboBox c = new ComboBox();
                c.Width = 100;
                if (i % 2 == 0)
                {
                    c.Name = "H" + (i/2);
                    c.Margin = new Thickness(265, 40+(40*(i/2+i%2)), 0, 0);
                }
                else
                {
                    c.Name = "A" + (1 + i/2);
                    c.Margin = new Thickness(425, 40+(40*(i/2+i%2)), 0, 0);
                }
                c.SelectionChanged += PlayerChange;
                c.VerticalAlignment = VerticalAlignment.Top;
                c.HorizontalAlignment = HorizontalAlignment.Left;
                this.Panel.Children.Add(c);
            }
            // naplnění domácího týmu
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select id_player, nick, position, playerCoop, individualSkill, teamplaySkill, energy, p.name from player join position_type p on p.id_section=player.game and p.id_position_in_game=player.position where team_fk=" + homeTeam + ";", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    homePlayers.Add(new Player(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2), reader.GetInt32(3), reader.GetInt32(4), reader.GetInt32(5), reader.GetInt32(6), reader.GetString(7), homeTeam));  
                }
                reader.Close();
            }
            // naplnění venkovního týmu
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select id_player, nick, position, playerCoop, individualSkill, teamplaySkill, energy, p.name from player join position_type p on p.id_section=player.game and p.id_position_in_game=player.position where team_fk=" + awayTeam + ";", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    awayPlayers.Add(new Player(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2), reader.GetInt32(3), reader.GetInt32(4), reader.GetInt32(5), reader.GetInt32(6), reader.GetString(7), awayTeam));
                }
                reader.Close();
            }
            if (positionRequired)
            {
                int counter = 1;
                for (int i = 0; i < Panel.Children.Count; i++)
                {
                    var child = Panel.Children[i];
                    if (child is ComboBox)
                    {
                        ComboBox c = (ComboBox)child;
                        if (c.Name.StartsWith("H"))
                        {

                            string position = "";
                            for (int j = 0; j < homePlayers.Count; j++)
                            {
                                if (homePlayers.ElementAt(j).Position == int.Parse(c.Name.Substring(1, 1)))
                                {
                                    c.Items.Add(homePlayers.ElementAt(j).Nick);
                                    position = homePlayers.ElementAt(j).PositionName;
                                    if (homeTeam != idTeam)
                                    {
                                        c.IsEnabled = false;
                                    }
                                }
                            }
                            Label l = new Label();
                            l.Margin = new Thickness(365, 40 + (40 * counter), 0, 0);
                            counter++;
                            l.Width = 60;
                            l.VerticalAlignment = VerticalAlignment.Top;
                            l.HorizontalAlignment = HorizontalAlignment.Left;
                            l.HorizontalContentAlignment = HorizontalAlignment.Center;
                            l.Content = position;
                            this.Panel.Children.Add(l);
                        }
                        else
                        {
                            for (int j = 0; j < awayPlayers.Count; j++)
                            {
                                if (awayPlayers.ElementAt(j).Position == int.Parse(c.Name.Substring(1, 1)))
                                {
                                    c.Items.Add(awayPlayers.ElementAt(j).Nick);
                                    if (awayTeam != idTeam)
                                    {
                                        c.IsEnabled = false;
                                    }
                                }
                            }
                        }
                        c.SelectedIndex = 0;
                    }
                }
            }
            else
            {
                for (int i = 0; i < Panel.Children.Count; i++)
                {
                    var child = Panel.Children[i];
                    if (child is ComboBox)
                    {
                        ComboBox c = (ComboBox)child;
                        if (c.Name.StartsWith("H"))
                        {
                            for (int j = 0; j < homePlayers.Count; j++)
                            {
                                c.Items.Add(homePlayers.ElementAt(j).Nick);
                                if (homeTeam != idTeam)
                                {
                                    c.IsEnabled = false;
                                }
                            }
                        }
                        else
                        {
                            for (int j = 0; j < awayPlayers.Count; j++)
                            {
                                c.Items.Add(awayPlayers.ElementAt(j).Nick);
                                if (awayTeam != idTeam)
                                {
                                    c.IsEnabled = false;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void PlayerChange(object sender, SelectionChangedEventArgs e)
        {
            //ComboBox c = (ComboBox)sender;
            //c.Items.Clear();
            //c.ItemsSource = homePlayers;
        }

        private void GetDate()
        {
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select id_team, date from info;", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                reader.Read();
                idTeam = reader.GetInt32(0);
                curDate = reader.GetString(1);
                reader.Close();
            }
        }

        private void SetTeams()
        {
            int year = int.Parse(curDate.Substring(0, 4));
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select ta.name,tb.name,ta.logo,tb.logo,m.id_teamxsection_home,m.id_teamxsection_away,tour.games_best_of,tour.name,m.id_match from '" + year + "match" + idSection +"' m join teamxsection a on a.id_teamxsection=m.id_teamxsection_home join teamxsection b on b.id_teamxsection=m.id_teamxsection_away join team ta on ta.id_team=a.id_team join team tb on tb.id_team=b.id_team join tournament tour on tour.id_tournament=m.id_tournament where match_date='" + curDate + "' and (id_teamxsection_home="+idTeam+" or id_teamxsection_away=" + idTeam + ")", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                reader.Read();
                HomeName.Content = reader.GetString(0);
                AwayName.Content = reader.GetString(1);
                byte[] data = (byte[])reader[2];
                BitmapImage imageSource = new BitmapImage();
                using (MemoryStream ms = new MemoryStream(data))
                {
                    imageSource.BeginInit();
                    imageSource.StreamSource = ms;
                    imageSource.CacheOption = BitmapCacheOption.OnLoad;
                    imageSource.EndInit();
                }
                HomeLogo.Source = imageSource;
                data = (byte[])reader[3];
                imageSource = new BitmapImage();
                using (MemoryStream ms = new MemoryStream(data))
                {
                    imageSource.BeginInit();
                    imageSource.StreamSource = ms;
                    imageSource.CacheOption = BitmapCacheOption.OnLoad;
                    imageSource.EndInit();
                }
                AwayLogo.Source = imageSource;
                homeTeam = reader.GetInt32(4);
                awayTeam = reader.GetInt32(5);
                bestOf = reader.GetInt32(6);
                TournamentName.Content = reader.GetString(7);
                idMatch = reader.GetInt32(8);
                reader.Close();
            }
        }

        private void PlayMatchClick(object sender, RoutedEventArgs e)
        {
            int homeStrength = 0;
            int awayStrength = 0;
            if (matchFinished)
            {
                using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
                {
                    int year = int.Parse(curDate.Substring(0, 4));
                    conn.Open();
                    SQLiteCommand command = new SQLiteCommand("update '" + year + "match" + idSection + "' set home_score=" + homeScore + ", away_score=" + awayScore + " where id_match=" + idMatch + ";", conn);
                    command.ExecuteReader();
                }
                this.Close();
                } else
            {
                
                for (int i = 0; i < Panel.Children.Count; i++)
                {
                    if (Panel.Children[i] is ComboBox)
                    {
                        ComboBox c = (ComboBox)Panel.Children[i];
                        if (c.Name.StartsWith("H"))
                        {
                            int counter = 0;
                            for (int j = 0; j < homePlayers.Count; j++)
                            {
                                if (homePlayers.ElementAt(j).Position == int.Parse(c.Name.Substring(1, 1)))
                                {
                                    if (counter == c.SelectedIndex)
                                    {
                                        homeStrength += homePlayers.ElementAt(j).IndiSkill + homePlayers.ElementAt(j).TeamSkill + homePlayers.ElementAt(j).PlayerCoop;
                                        break;
                                    }
                                    counter++;
                                }
                            }
                        } else if (c.Name.StartsWith("A"))
                        {
                            int counter = 0;
                            for (int j = 0; j < awayPlayers.Count; j++)
                            {
                                if (awayPlayers.ElementAt(j).Position == int.Parse(c.Name.Substring(1, 1)))
                                {
                                    if (counter == c.SelectedIndex)
                                    {
                                        awayStrength += awayPlayers.ElementAt(j).IndiSkill + awayPlayers.ElementAt(j).TeamSkill + awayPlayers.ElementAt(j).PlayerCoop;
                                        break;
                                    }
                                    counter++;
                                }
                            }
                        }
                    }
                }
                Score.Visibility = Visibility.Visible;
                if (awayStrength > homeStrength)
                {
                    awayScore++;
                    
                } else
                {
                    homeScore++;
                }
                Score.Content = homeScore + "-" + awayScore;
                if (homeScore + awayScore == bestOf)
                {
                    matchFinished = true;
                    PlayMatch.Content = "Pokračovat";
                    if (awayScore > homeScore)
                    {
                        HomeLogo.Opacity = 0.1;
                        AwayBorder.Visibility = Visibility.Visible;
                    } else
                    {
                        AwayLogo.Opacity = 0.1;
                        HomeBorder.Visibility = Visibility.Visible;
                    }
                }
                
            }
        }

        
    }
}
