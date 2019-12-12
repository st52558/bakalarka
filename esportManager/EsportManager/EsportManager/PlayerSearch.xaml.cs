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
        List<int> playerIds = new List<int>();
        List<PlayersDataGrid> players = new List<PlayersDataGrid>();
        List<GameBasic> games = new List<GameBasic>();
        List<TournamentBasic> tournaments = new List<TournamentBasic>();
        List<TeamBasic> teams = new List<TeamBasic>();
        string databaseName;
        class PlayersDataGrid
        {
            public string Hra { get; set; }
            public string Tym { get; set; }
            public string Pozice { get; set; }
            public string Nick { get; set; }
            public string Jmeno { get; set; }
            public string Prijmeni { get; set; }
        }

        class TournamentBasic
        {
            public int ID { get; set; }
            public string Name { get; set; }
        }

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
            AddTournamentsToComboBox();
            AddGamesToComboBox();
        }

        private void AddTournamentsToComboBox()
        {
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select id_tournament, name from tournament;", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    tournaments.Add(new TournamentBasic() { ID = reader.GetInt32(0), Name = reader.GetString(1) });
                }
                reader.Close();
            }
            TournamentsComboBox.Items.Clear();
            TournamentsComboBox.Items.Add("Všechny turnaje");
            for (int i = 0; i < tournaments.Count; i++)
            {
                TournamentsComboBox.Items.Add(tournaments.ElementAt(i).Name);
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
        }

        private void AddPlayersToGrid()
        {
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command;
                command = new SQLiteCommand("select section.shortcut, position_type.name, nick, player.name, surname, id_player, team.shortcut from player join section on player.game=section.id_section join position_type on position_type.id_section=section.id_section and id_position_in_game=player.position join teamxsection on player.team_fk=teamxsection.id_teamxsection join team on team.id_team=teamxsection.id_team;", conn);

                if (TeamsComboBox.SelectedIndex <= 0 && GameComboBox.SelectedIndex <= 0 && TournamentsComboBox.SelectedIndex <= 0) //není vybráno nic
                {
                    command = new SQLiteCommand("select section.shortcut, position_type.name, nick, player.name, surname, id_player, team.shortcut from player join section on player.game=section.id_section join position_type on position_type.id_section=section.id_section and id_position_in_game=player.position join teamxsection on player.team_fk=teamxsection.id_teamxsection join team on team.id_team=teamxsection.id_team;", conn);
                }
                else if (GameComboBox.SelectedIndex <= 0 && TournamentsComboBox.SelectedIndex <= 0) //je vybrán tým
                {
                    command = new SQLiteCommand("select section.shortcut, position_type.name, nick, player.name, surname, id_player, team.shortcut from player join section on player.game=section.id_section join position_type on position_type.id_section=section.id_section and id_position_in_game=player.position join teamxsection on player.team_fk=teamxsection.id_teamxsection join team on team.id_team=teamxsection.id_team where teamxsection.id_team=" + teams.ElementAt(TeamsComboBox.SelectedIndex - 1).IdTeam + ";", conn);
                }
                else if (TeamsComboBox.SelectedIndex <= 0 && TournamentsComboBox.SelectedIndex <= 0) //je vybraná hra
                {
                    command = new SQLiteCommand("select section.shortcut, position_type.name, nick, player.name, surname, id_player, team.shortcut from player join section on player.game=section.id_section join position_type on position_type.id_section=section.id_section and id_position_in_game=player.position join teamxsection on player.team_fk=teamxsection.id_teamxsection join team on team.id_team=teamxsection.id_team where player.game=" + games.ElementAt(GameComboBox.SelectedIndex - 1).ID + ";", conn);
                }
                else if (TeamsComboBox.SelectedIndex <= 0 && GameComboBox.SelectedIndex <= 0) //je vybrán turnaj
                {

                    //command s tím že se vyberou hráči hrající ten turnaj
                    //command = new SQLiteCommand("select section.shortcut, position_type.name, nick, player.name, surname, id_player, team.shortcut from player join section on player.game=section.id_section join position_type on position_type.id_section=section.id_section and id_position_in_game=player.position join teamxsection on player.team_fk=teamxsection.id_teamxsection join team on team.id_team=teamxsection.id_team where player.game = " + games.ElementAt(GameComboBox.SelectedIndex - 1).ID + ";", conn);
                }
                else if (GameComboBox.SelectedIndex <= 0) //je vybrán tým a turnaj
                {

                    //command = new SQLiteCommand("select section.shortcut, position_type.name, nick, player.name, surname, id_player, team.shortcut from player join section on player.game=section.id_section join position_type on position_type.id_section=section.id_section and id_position_in_game=player.position join teamxsection on player.team_fk=teamxsection.id_teamxsection join team on team.id_team=teamxsection.id_team where player.game = " + games.ElementAt(GameComboBox.SelectedIndex - 1).ID + ";", conn);

                }
                else if (TournamentsComboBox.SelectedIndex <= 0) //je vybrán tým a hra
                {
                    command = new SQLiteCommand("select section.shortcut, position_type.name, nick, player.name, surname, id_player, team.shortcut from player join section on player.game=section.id_section join position_type on position_type.id_section=section.id_section and id_position_in_game=player.position join teamxsection on player.team_fk=teamxsection.id_teamxsection join team on team.id_team=teamxsection.id_team where player.game=" + games.ElementAt(GameComboBox.SelectedIndex - 1).ID + " and teamxsection.id_team = " + teams.ElementAt(TeamsComboBox.SelectedIndex - 1).IdTeam +";", conn);

                }
                else if (TeamsComboBox.SelectedIndex <= 0) //je vybrán turnaj a hra
                {
                    // command = new SQLiteCommand("select section.shortcut, position_type.name, nick, player.name, surname, id_player, team.shortcut from player join section on player.game=section.id_ssection join position_type on position_type.id_section=section.id_section and id_position_in_game=player.position join teamxsection on player.team_fk=teamxsection.id_teamxsection join team on team.id_team=teamxsection.id_team where player.game = " + games.ElementAt(GameComboBox.SelectedIndex - 1).ID + ";", conn);

                }
                else //je vybráno všechno
                {
                    //command = new SQLiteCommand("select section.shortcut, position_type.name, nick, player.name, surname, id_player, team.shortcut from player join section on player.game=section.id_section join position_type on position_type.id_section=section.id_section and id_position_in_game=player.position join teamxsection on player.team_fk=teamxsection.id_teamxsection join team on team.id_team=teamxsection.id_team where player.game = " + games.ElementAt(GameComboBox.SelectedIndex - 1).ID + ";", conn);
                }
                players = new List<PlayersDataGrid>();
                playerIds = new List<int>();
                string jmeno, prijmeni;
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (reader.IsDBNull(3))
                    {
                        jmeno = "";
                    }
                    else
                    {
                        jmeno = reader.GetString(3);
                    }
                    if (reader.IsDBNull(4))
                    {
                        prijmeni = "";
                    }
                    else
                    {
                        prijmeni = reader.GetString(4);
                    }
                    players.Add(new PlayersDataGrid() { Hra = reader.GetString(0), Pozice = reader.GetString(1), Nick = reader.GetString(2), Jmeno = jmeno, Prijmeni = prijmeni, Tym = reader.GetString(6) });
                    playerIds.Add(reader.GetInt32(5));
                }
                reader.Close();
            }
            FreePlayersGrid.ItemsSource = players;

        }

        private void SignPlayer(object sender, MouseButtonEventArgs e)
        {
            int individualSkill, teamplaySkill, teamId, reputation, salary, budget, value, playerTeamId;
            string nick, date;
            if (FreePlayersGrid.SelectedIndex >= 0)
            {
                using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
                {
                    conn.Open();
                    SQLiteCommand command = new SQLiteCommand("select nick, individualSkill, teamplaySkill, value, team_fk from player where id_player=" + playerIds.ElementAt(FreePlayersGrid.SelectedIndex) + ";", conn);
                    SQLiteDataReader reader = command.ExecuteReader();
                    reader.Read();
                    nick = reader.GetString(0);
                    individualSkill = reader.GetInt32(1);
                    teamplaySkill = reader.GetInt32(2);
                    value = reader.GetInt32(3);
                    playerTeamId = 0;
                    if (!reader.IsDBNull(4))
                    {
                        playerTeamId = reader.GetInt32(4);
                    }
                    reader.Close();
                    command = new SQLiteCommand("select id_team, date from info;", conn);
                    reader = command.ExecuteReader();
                    reader.Read();
                    teamId = reader.GetInt32(0);
                    date = reader.GetString(1);
                    reader.Close();
                    command = new SQLiteCommand("select reputation, budget from team where id_team=" + teamId + ";", conn);
                    reader = command.ExecuteReader();
                    reader.Read();
                    reputation = reader.GetInt32(0);
                    budget = reader.GetInt32(1);
                    reader.Close();
                    command = new SQLiteCommand("select id_teamxsection from teamxsection where id_team=" + teamId + ";", conn);
                    reader = command.ExecuteReader();
                    bool isAlreadyPlayer = false;
                    while (reader.Read())
                    {
                        if (playerTeamId == reader.GetInt32(0)){
                            isAlreadyPlayer = true;
                            break;
                        }
                    }
                    reader.Close();
                    if (isAlreadyPlayer)
                    {
                        MessageBox.Show("Tento hráč už je členem týmu.", "Podpis smlouvy", MessageBoxButton.OK);
                    } else if ((individualSkill+teamplaySkill)/2 <= reputation-10 || (individualSkill + teamplaySkill) / 2 >= reputation + 10)
                    {
                        MessageBox.Show("Hráč nemá zájem hrát ve vašem týmu", "Podpis smlouvy", MessageBoxButton.OK);
                    } else if (budget < value){
                        MessageBox.Show("Nemáte dostatek financí na podpis hráče.", "Podpis smlouvy", MessageBoxButton.OK);
                    } else
                    {
                        salary = 1000 + (int)(((reputation * (individualSkill + teamplaySkill) / 2) - 3600) * 1.02);
                        MessageBoxResult result = MessageBox.Show("Chystáte se podepsat smlouvu s hráčem " + nick + ".\nPoplatek činí " + value + "$.\nJeho smlouva je na rok za " + salary + "$ měsíčně. Chcete smlouvu podepsat?", "Podpis smlouvy", MessageBoxButton.YesNo);
                        if (result == MessageBoxResult.Yes)
                        {
                            int year = int.Parse(date.Remove(4, 6));
                            year++;
                            date = year.ToString() + date.Remove(0, 4);
                            int playerValue = (salary * 100 / 3);
                            playerValue /= playerValue;
                            playerValue *= playerValue;
                            command = new SQLiteCommand("update player set team_fk=" + teamId + ", contractEnd='" + date +"', value=" + playerValue + " where id_player=" + playerIds.ElementAt(FreePlayersGrid.SelectedIndex) + ";", conn);
                            command.ExecuteReader();
                            command = new SQLiteCommand("update team set budget=budget-" + value + " where id_team=" + teamId + ";", conn);
                            command.ExecuteReader();
                        }
                    }
                }
                
            }
            AddPlayersToGrid();
        }

        private void TeamChanged(object sender, SelectionChangedEventArgs e)
        {
            AddPlayersToGrid();
            
        }

        private void GameChanged(object sender, SelectionChangedEventArgs e)
        {
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
                    sqlCommand += " join teamxsection on teamxsection.id_team=team.id_team join tournament_token on teamxsection.id_teamxsection=tournament_token.id_teamxsection where tournament_token.id_tournament_from=" + tournaments.ElementAt(TournamentsComboBox.SelectedIndex-1).ID;
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
