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
    public partial class FreePlayers : Window
    {
        List<int> playerIds = new List<int>();
        List<PlayersDataGrid> players = new List<PlayersDataGrid>();
        List<GameBasic> sections = new List<GameBasic>();
        string databaseName;
        class PlayersDataGrid
        {
            public string Hra { get; set; }
            public string Pozice { get; set; }
            public string Nick { get; set; }
            public string Jmeno { get; set; }
            public string Prijmeni { get; set; }
        }

        class GameBasic
        {
            public int ID { get; set; }
            public string Name { get; set; }
        }

        public FreePlayers(string database)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            databaseName = database;
            AddGamesToComboBox();
            AddPlayersToGrid();
        }

        private void AddGamesToComboBox()
        {
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select id_sekce, nazev from sekce;", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    sections.Add(new GameBasic() { ID = reader.GetInt32(0), Name = reader.GetString(1)});
                }
                reader.Close();
            }
            GamesComboBox.Items.Clear();
            GamesComboBox.Items.Add("Všechny hry");
            for (int i = 0; i < sections.Count; i++)
            {
                GamesComboBox.Items.Add(sections.ElementAt(i).Name);
            }
            GamesComboBox.SelectedIndex = 0;
        }

        private void AddPlayersToGrid()
        {
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command;
                if (GamesComboBox.SelectedIndex <= 0)
                {
                    command = new SQLiteCommand("select sekce.zkratka, position_type.name, nick, player.name, surname, id_player from player join sekce on player.game=sekce.id_sekce join position_type on id_section=sekce.id_sekce and id_position_in_game=player.position where player.team_fk is null;", conn);
                }
                else
                {
                    int game = sections.ElementAt(GamesComboBox.SelectedIndex - 1).ID;
                    command = new SQLiteCommand("select sekce.zkratka, position_type.name, nick, player.name, surname, id_player from player join sekce on player.game=sekce.id_sekce join position_type on id_section=sekce.id_sekce and id_position_in_game=player.position where player.team_fk is null and player.game=" + game + ";", conn);
                }
                players = new List<PlayersDataGrid>();
                playerIds = new List<int>();
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine(reader.GetString(2));
                    players.Add(new PlayersDataGrid() { Hra = reader.GetString(0), Pozice = reader.GetString(1), Nick = reader.GetString(2), Jmeno = reader.GetString(3), Prijmeni = reader.GetString(4) });
                    playerIds.Add(reader.GetInt32(5));
                }
                reader.Close();
            }
            FreePlayersGrid.ItemsSource = players;

        }

        private void GameChanged(object sender, SelectionChangedEventArgs e)
        {
            AddPlayersToGrid();
        }

        private void SignPlayer(object sender, MouseButtonEventArgs e)
        {
            int individualSkill, teamplaySkill, teamId, reputation, salary, budget;
            string nick, date;
            if (FreePlayersGrid.SelectedIndex >= 0)
            {
                using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
                {
                    conn.Open();
                    SQLiteCommand command = new SQLiteCommand("select nick, individualSkill, teamplaySkill from player where id_player=" + playerIds.ElementAt(FreePlayersGrid.SelectedIndex) + ";", conn);
                    SQLiteDataReader reader = command.ExecuteReader();
                    reader.Read();
                    nick = reader.GetString(0);
                    individualSkill = reader.GetInt32(1);
                    teamplaySkill = reader.GetInt32(2);
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
                    if ((individualSkill+teamplaySkill)/2 < reputation-10 || (individualSkill + teamplaySkill) / 2 > reputation + 10)
                    {
                        MessageBox.Show("Hráč nemá zájem hrát ve vašem týmu", "Podpis smlouvy", MessageBoxButton.OK);
                    } else if (budget < 0){
                        MessageBox.Show("Nemáte dostatek financí na podpis hráče.", "Podpis smlouvy", MessageBoxButton.OK);
                    } else
                    {
                        salary = 1000 + (int)(((reputation * (individualSkill + teamplaySkill) / 2) - 3600) * 1.02);
                        MessageBoxResult result = MessageBox.Show("Chystáte se podepsat smlouvu s hráčem " + nick + ". Jeho smlouva je na rok za " + salary + "$ měsíčně. Chcete smlouvu podepsat?", "Podpis smlouvy", MessageBoxButton.YesNo);
                        if (result == MessageBoxResult.Yes)
                        {
                            int year = int.Parse(date.Remove(4, 6));
                            year++;
                            date = year.ToString() + date.Remove(0, 4);
                            int playerValue = (salary * 100 / 3);
                            playerValue = playerValue / 100;
                            playerValue = playerValue * 100;
                            command = new SQLiteCommand("update player set team_fk=" + teamId + ", contractEnd='" + date +"', value=" + playerValue + " where id_player=" + playerIds.ElementAt(FreePlayersGrid.SelectedIndex) + ";", conn);
                            command.ExecuteReader();
                        }
                    }
                }
                
            }
            AddPlayersToGrid();
        }
    }
}
