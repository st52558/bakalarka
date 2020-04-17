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
    /// Interaction logic for TeamDetail.xaml
    /// </summary>
    public partial class TeamDetail : Window
    {
        int team;
        string databaseName;
        public TeamDetail(string databaseNameI, int teamI)
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            team = teamI;
            databaseName = databaseNameI;
            InitializeComponent();
            SetTeamDetail();
            SetPlayersToGrid();
            SetTournamentsToGrid();
            
        }

        private void SetTournamentsToGrid()
        {
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
            {
                List<Tournament> tournaments = new List<Tournament>();
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select tournament.id_tournament, tournament.name, section.shortcut from team join teamxsection on teamxsection.id_team=team.id_team join tournament_token on tournament_token.id_teamxsection=teamxsection.id_teamxsection join tournament on tournament.id_tournament=tournament_token.id_tournament_to join section on section.id_section=tournament.id_section where team.id_team=" + team + ";", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Tournament t = new Tournament();
                    t.ID = reader.GetInt32(0);
                    t.Name = reader.GetString(1);
                    t.SectionName = reader.GetString(2);
                    tournaments.Add(t);
                }
                reader.Close();
                TournamentsGrid.ItemsSource = tournaments;
            }
        }

        private void SetPlayersToGrid()
        {
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
            {
                List<Player> players = new List<Player>();
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select nick, player.name, surname, position_type.name, section.shortcut, id_player from player join position_type on position_type.id_position_in_game=player.id_position and position_type.id_section=player.id_section join section on player.id_section=section.id_section join teamxsection on teamxsection.id_teamxsection=player.id_teamxsection join team on teamxsection.id_team=team.id_team where team.id_team=" + team + ";", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Player p = new Player();
                    p.PositionName = reader.GetString(3);
                    p.Nick = reader.GetString(0);
                    p.Name = reader.GetString(1);
                    p.Surname = reader.GetString(2);
                    p.SectionName = reader.GetString(4);
                    p.IdPlayer = reader.GetInt32(5);
                    players.Add(p);
                }

                reader.Close();
                command = new SQLiteCommand("select nick, coach.name, surname, section.shortcut from coach join section on coach.id_section=section.id_section where coach.id_team=" + team + ";", conn);
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Player p = new Player();
                    p.PositionName = "Trenér";
                    p.Nick = reader.GetString(0);
                    p.Name = reader.GetString(1);
                    p.Surname = reader.GetString(2);
                    p.SectionName = reader.GetString(3);
                    players.Add(p);
                }
                reader.Close();
                PlayersGrid.ItemsSource = players;
            }
        }

        private void SetTeamDetail()
        {
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select logo, name from team where id_team=" + team + ";", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    TeamNameLabel.Content = reader.GetString(1);
                    byte[] data = (byte[])reader[0];
                    BitmapImage imageSource = new BitmapImage();
                    using (MemoryStream ms = new MemoryStream(data))
                    {
                        imageSource.BeginInit();
                        imageSource.StreamSource = ms;
                        imageSource.CacheOption = BitmapCacheOption.OnLoad;
                        imageSource.EndInit();
                    }
                    TeamLogo.Source = imageSource;
                }
                reader.Close();
            }
        }

        private void ShowTournamentDetail(object sender, MouseButtonEventArgs e)
        {
            DataGrid d = (DataGrid)sender;
            if (d.SelectedIndex > -1)
            {
                List<Tournament> l = (List<Tournament>)d.ItemsSource;
                TournamentInfoGroup win2 = new TournamentInfoGroup(databaseName, l.ElementAt(d.SelectedIndex).ID, false);
                win2.ShowDialog();
            }
        }

        private void ShowPlayerDetail(object sender, MouseButtonEventArgs e)
        {
            DataGrid d = (DataGrid)sender;
            if (d.SelectedIndex > -1)
            {
                List<Player> l = (List<Player>)d.ItemsSource;
                if (l.ElementAt(d.SelectedIndex).IdPlayer != 0)
                {
                    PlayerDetail win2 = new PlayerDetail(databaseName, l.ElementAt(d.SelectedIndex).IdPlayer);
                    win2.ShowDialog();
                }
            }
        }
    }
}
