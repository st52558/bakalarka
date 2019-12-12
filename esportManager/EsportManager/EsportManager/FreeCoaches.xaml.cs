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
    public partial class FreeCoaches : Window
    {
        List<int> playerIds = new List<int>();
        List<CoachesDataGrid> players = new List<CoachesDataGrid>();
        List<GameBasic> sections = new List<GameBasic>();
        string databaseName;
        class CoachesDataGrid
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

        public FreeCoaches(string database)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
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
                SQLiteCommand command = new SQLiteCommand("select id_section, name from section;", conn);
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
                    command = new SQLiteCommand("select section.shortcut, nick, coach.name, surname, id_coach from coach join section on coach.game=section.id_section where coach.team_fk is null;", conn);
                }
                else
                {
                    int game = sections.ElementAt(GamesComboBox.SelectedIndex - 1).ID;
                    command = new SQLiteCommand("select section.shortcut, nick, coach.name, surname, id_coach from coach join section on coach.game=section.id_section where coach.team_fk is null and coach.game=" + game + ";", conn);
                }
                players = new List<CoachesDataGrid>();
                playerIds = new List<int>();
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    players.Add(new CoachesDataGrid() { Hra = reader.GetString(0), Pozice = "Trenér", Nick = reader.GetString(1), Jmeno = reader.GetString(2), Prijmeni = reader.GetString(3) });
                    playerIds.Add(reader.GetInt32(4));
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
            int training, teamId, reputation, salary, budget;
            string nick, date;
            if (FreePlayersGrid.SelectedIndex >= 0)
            {
                using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
                {
                    conn.Open();
                    SQLiteCommand command = new SQLiteCommand("select nick, training from coach where id_coach=" + playerIds.ElementAt(FreePlayersGrid.SelectedIndex) + ";", conn);
                    SQLiteDataReader reader = command.ExecuteReader();
                    reader.Read();
                    nick = reader.GetString(0);
                    training = reader.GetInt32(1);
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
                    if (training > reputation + 10)
                    {
                        MessageBox.Show("Trenér nemá zájem hrát ve vašem týmu", "Podpis smlouvy", MessageBoxButton.OK);
                    } else if (budget < 0){
                        MessageBox.Show("Nemáte dostatek financí na podpis hráče.", "Podpis smlouvy", MessageBoxButton.OK);
                    } else
                    {
                        salary = 1000 + (int)((reputation * training - 3600) * 0.765);
                        MessageBoxResult result = MessageBox.Show("Chystáte se podepsat smlouvu s trenérem " + nick + ". Jeho smlouva je na rok za " + salary + "$ měsíčně. Chcete smlouvu podepsat?", "Podpis smlouvy", MessageBoxButton.YesNo);
                        if (result == MessageBoxResult.Yes)
                        {
                            int year = int.Parse(date.Remove(4, 6));
                            year++;
                            date = year.ToString() + date.Remove(0, 4);
                            int playerValue = (salary * 100 / 3);
                            playerValue = playerValue / 100;
                            playerValue = playerValue * 100;
                            command = new SQLiteCommand("update coach set team_fk=" + teamId + ", contractEnd='" + date +"', value=" + playerValue + ", salary=" + salary + " where id_coach=" + playerIds.ElementAt(FreePlayersGrid.SelectedIndex) + ";", conn);
                            command.ExecuteReader();
                        }
                    }
                }
                
            }
            AddPlayersToGrid();
        }
    }
}
