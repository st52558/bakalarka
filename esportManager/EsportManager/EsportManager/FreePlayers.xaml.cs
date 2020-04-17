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
        List<Player> players;
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
                    command = new SQLiteCommand("select section.shortcut, position_type.name, nick, player.name, surname, id_player from player join section on player.id_section=section.id_section join position_type on position_type.id_section=section.id_section and id_position_in_game=player.id_position where player.id_teamxsection is null;", conn);
                }
                else
                {
                    int game = sections.ElementAt(GamesComboBox.SelectedIndex - 1).ID;
                    command = new SQLiteCommand("select section.shortcut, position_type.name, nick, player.name, surname, id_player from player join section on player.id_section=section.id_section join position_type on position_type.id_section=section.id_section and id_position_in_game=player.id_position where player.id_teamxsection is null and player.id_section=" + game + ";", conn);
                }
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
                    players.Add(p);
                }
                reader.Close();
                FreePlayersGrid.ItemsSource = players;
            }
            

        }

        private void GameChanged(object sender, SelectionChangedEventArgs e)
        {
            AddPlayersToGrid();
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
                    AddPlayersToGrid();
                }
            }
        }
    }
}
