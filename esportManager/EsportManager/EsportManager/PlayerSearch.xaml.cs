using System;
using System.Collections.Generic;
using System.Data.SQLite;
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
    /// Interakční logika pro FreePlayers.xaml
    /// </summary>
    public partial class PlayerSearch : Window
    {
        List<Player> players;
        List<GameBasic> games = new List<GameBasic>();
        List<Tournament> tournaments = new List<Tournament>();
        List<TeamBasic> teams = new List<TeamBasic>();
        string databaseName;


        class GameBasic
        {
            public int ID { get; set; }
            public string Name { get; set; }
        }

        public PlayerSearch(string database)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            databaseName = database;
            AddGamesToComboBox();
        }

        private void AddTournamentsToComboBox()
        {
            tournaments.Clear();
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select id_tournament, name, game from tournament;", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Tournament t = new Tournament();
                    t.ID = reader.GetInt32(0);
                    t.Name = reader.GetString(1);
                    t.IdSection = reader.GetInt32(2);
                    tournaments.Add(t);
                }
                reader.Close();
            }
            TournamentsComboBox.Items.Clear();
            TournamentsComboBox.Items.Add("Všechny turnaje");
            for (int i = 0; i < tournaments.Count; i++)
            {
                if (GameComboBox.SelectedIndex > 0)
                {
                    if (games.ElementAt(GameComboBox.SelectedIndex-1).ID == tournaments.ElementAt(i).IdSection)
                    {
                        TournamentsComboBox.Items.Add(tournaments.ElementAt(i).Name);
                    }
                } 
                else
                {
                    TournamentsComboBox.Items.Add(tournaments.ElementAt(i).Name);
                }
                
            }
            TournamentsComboBox.SelectedIndex = 0;
        }

        private void AddGamesToComboBox()
        {
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select id_section, shortcut from section;", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    games.Add(new GameBasic() { ID = reader.GetInt32(0), Name = reader.GetString(1)});
                }
                reader.Close();
            }
            GameComboBox.Items.Clear();
            GameComboBox.Items.Add("Všechny hry");
            for (int i = 0; i < games.Count; i++)
            {
                GameComboBox.Items.Add(games.ElementAt(i).Name);
            }
            GameComboBox.SelectedIndex = 0;
            AddTournamentsToComboBox();
        }

        private void AddPlayersToGrid()
        {
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
            {
                conn.Open();
                string com;
                SQLiteCommand command;
                com = "select section.shortcut, position_type.name, nick, player.name, surname, id_player, team.shortcut from player join section on player.game=section.id_section join position_type on position_type.id_section=section.id_section and id_position_in_game=player.position join teamxsection on player.team_fk=teamxsection.id_teamxsection join team on team.id_team=teamxsection.id_team ";
                if (TeamsComboBox.SelectedIndex > 0)
                {
                    com += "where team.id_team=" + teams.ElementAt(TeamsComboBox.SelectedIndex - 1).IdTeam + ";";
                } else if (GameComboBox.SelectedIndex > 0)
                {
                    com += "where section.id_section=" + games.ElementAt(GameComboBox.SelectedIndex - 1).ID + ";";
                }
                command = new SQLiteCommand(com, conn);
                players = new List<Player>();
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Player p = new Player();
                    p.SectionName = reader.GetString(0);
                    p.PositionName = reader.GetString(1);
                    p.Nick = reader.GetString(2);
                    p.Name = reader.GetString(3);
                    p.Surname = reader.GetString(4);
                    p.IdPlayer = reader.GetInt32(5);
                    p.TeamName = reader.GetString(6);
                    players.Add(p);
                }
                reader.Close();
            }
            FreePlayersGrid.ItemsSource = players;

        }

        private void SignPlayer(object sender, MouseButtonEventArgs e)
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

        private void TeamChanged(object sender, SelectionChangedEventArgs e)
        {
            AddPlayersToGrid();
            
        }

        private void GameChanged(object sender, SelectionChangedEventArgs e)
        {
            AddTournamentsToComboBox();
            AddPlayersToGrid();
        }

        private void TournamentChanged(object sender, SelectionChangedEventArgs e)
        {
            AddPlayersToGrid();
            AddTeamsToCB();
        }

        private void AddTeamsToCB()
        {
            string sqlCommand = "select team.id_team, name from team";
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
            {
                conn.Open();
                if (TournamentsComboBox.SelectedIndex > 0)
                {
                    sqlCommand += " join teamxsection on teamxsection.id_team=team.id_team join tournament_token on teamxsection.id_teamxsection=tournament_token.id_teamxsection where tournament_token.id_tournament_to=" + tournaments.ElementAt(TournamentsComboBox.SelectedIndex-1).ID;
                }
                
                sqlCommand += " order by name;";
                SQLiteCommand command = new SQLiteCommand(sqlCommand, conn);
                SQLiteDataReader reader = command.ExecuteReader();
                teams.Clear();
                while (reader.Read())
                {
                    teams.Add(new TeamBasic(reader.GetInt32(0), reader.GetString(1)));
                }
                reader.Close();
            }
            TeamsComboBox.Items.Clear();
            TeamsComboBox.Items.Add("Všechny týmy");
            for (int i = 0; i < teams.Count; i++)
            {
                TeamsComboBox.Items.Add(teams.ElementAt(i).Name);
            }
            TeamsComboBox.SelectedIndex = 0;
        }
        
    }
}
