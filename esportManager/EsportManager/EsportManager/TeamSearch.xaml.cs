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
    /// Interakční logika pro TeamSearch.xaml
    /// </summary>
    public partial class TeamSearch : Window
    {
        class GameBasic
        {
            public int ID { get; set; }
            public string Name { get; set; }
        }

        List<Tournament> tournaments;
        List<GameBasic> games;
        List<Team> teams;
        string databaseName;

        public TeamSearch(string databasename)
        {
            databaseName = databasename;
            tournaments = new List<Tournament>();
            games = new List<GameBasic>();
            teams = new List<Team>();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            SetAllGamesToCB();
        }

        private void SetAllGamesToCB()
        {
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select id_section, name from section;", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                games.Clear();
                GamesCB.Items.Clear();
                GamesCB.Items.Add("Všechny hry");
                while (reader.Read())
                {
                    GameBasic g = new GameBasic();
                    g.ID = reader.GetInt32(0);
                    g.Name = reader.GetString(1);
                    games.Add(g);
                }
                for (int i = 0; i < games.Count; i++)
                {
                    GamesCB.Items.Add(games.ElementAt(i).Name);
                }
                GamesCB.SelectedIndex = 0;
            }
        }

        private void SetAllTournamentsToCB()
        {
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
            {
                conn.Open();
                string com = "select id_tournament, name from tournament";
                SQLiteCommand command;
                if (GamesCB.SelectedIndex > 0) 
                {
                    com += " where game=" + games.ElementAt(GamesCB.SelectedIndex - 1).ID;
                }
                com += ";";
                command = new SQLiteCommand(com, conn);
                SQLiteDataReader reader = command.ExecuteReader();
                tournaments.Clear();
                TournamentsCB.Items.Clear();
                TournamentsCB.Items.Add("Všechny turnaje");
                while (reader.Read())
                {
                    Tournament t = new Tournament();
                    t.ID = reader.GetInt32(0);
                    t.Name = reader.GetString(1);
                    tournaments.Add(t);
                }
                for (int i = 0; i < tournaments.Count; i++)
                {
                    TournamentsCB.Items.Add(tournaments.ElementAt(i).Name);
                }
                TournamentsCB.SelectedIndex = 0;
            }
        }

        private void TournamentChange(object sender, SelectionChangedEventArgs e)
        {
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
            {
                conn.Open();
                string com = "select distinct team.id_team, team.name from team";
                SQLiteCommand command;
                if (TournamentsCB.SelectedIndex > 0)
                {
                    com += " join teamxsection on team.id_team=teamxsection.id_team join tournament_token on tournament_token.id_teamxsection=teamxsection.id_teamxsection where tournament_token.id_tournament_to=" + tournaments.ElementAt(TournamentsCB.SelectedIndex - 1).ID;
                } 
                else if (GamesCB.SelectedIndex > 0)
                {
                    com += " join teamxsection on team.id_team=teamxsection.id_team where id_section=" + games.ElementAt(GamesCB.SelectedIndex - 1).ID;
                }
                
                com += ";";
                command = new SQLiteCommand(com, conn);
                SQLiteDataReader reader = command.ExecuteReader();
                teams.Clear();
                TeamsLB.Items.Clear();
                while (reader.Read())
                {
                    Team t = new Team();
                    t.ID = reader.GetInt32(0);
                    t.Name = reader.GetString(1);
                    teams.Add(t);
                }
                for (int i = 0; i < teams.Count; i++)
                {
                    TeamsLB.Items.Add(teams.ElementAt(i).Name);
                }
            }
        }

        private void GameChange(object sender, SelectionChangedEventArgs e)
        {
            SetAllTournamentsToCB();
        }

        private void ShowTeamDetail(object sender, MouseButtonEventArgs e)
        {
            if (TeamsLB.SelectedIndex > -1)
            {
                TeamDetail win2 = new TeamDetail(databaseName,teams.ElementAt(TeamsLB.SelectedIndex).ID);
                win2.ShowDialog();
            }
        }
    }
}
