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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;

namespace TeamEdit
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Nation> nationList = new List<Nation>();
        List<TeamBasic> teamList = new List<TeamBasic>();
        public MainWindow()
        {
            InitializeComponent();
            GetAllNationsToComboBox();
        }

        private void GetAllNationsToComboBox()
        {
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\test.db;"))
            {
                conn.Open();

                SQLiteCommand command = new SQLiteCommand("select stat.id_stat,stat.jmeno from tym_basic inner join stat on stat.id_stat=tym_basic.id_stat_fk group by id_stat_fk", conn);
                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    nationList.Add(new Nation(reader.GetInt32(0), reader.GetString(1)));
                }
                for (int i = 0; i < nationList.Count; i++)
                {
                    NationsCB.Items.Add(nationList.ElementAt(i).Name);
                }
                reader.Close();
            }
        }

        private void NationChangeComboBox(object sender, SelectionChangedEventArgs e)
        {
            teamList.Clear();
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\test.db;"))
            {
                conn.Open();
                int teamID = nationList.ElementAt(NationsCB.SelectedIndex).IdNation;
                Console.WriteLine(teamID);
                SQLiteCommand command = new SQLiteCommand("select id_tym,nazev from tym_basic where id_stat_fk=" + teamID, conn);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    teamList.Add(new TeamBasic(reader.GetInt32(0), reader.GetString(1)));
                }
                for (int i = 0; i < teamList.Count; i++)
                {
                    TeamListLB.Items.Add(teamList.ElementAt(i).Name);
                }
            }
        }

        private void NationChangeTextView(object sender, TextChangedEventArgs e)
        {
            teamList.Clear();
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\test.db;"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select nazev from tym_basic where nazev like'%" + TeamNameTW.Text + "%'", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    teamList.Add(new TeamBasic(reader.GetInt32(0), reader.GetString(1)));
                }
                for (int i = 0; i < teamList.Count; i++)
                {
                    TeamListLB.Items.Add(teamList.ElementAt(i).Name);
                }
            }
        }

        private void EditButtonClick(object sender, RoutedEventArgs e)
        {
                TeamEditor win2 = new TeamEditor(TeamListLB.SelectedItem.ToString());
                win2.Show();
        }

        private void NewButtonClick(object sender, RoutedEventArgs e)
        {
            TeamEditor win2 = new TeamEditor();
            win2.Show();
        }
    }
}
