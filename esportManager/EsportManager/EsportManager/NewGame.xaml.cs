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
using TeamEdit;

namespace EsportManager
{
    /// <summary>
    /// Interakční logika pro Window1.xaml
    /// </summary>
    public partial class NewGame : Window
    {
        public MainWindow Mainwindow { get; set; }
        List<Nation> nationList = new List<Nation>();
        List<TeamBasic> teamList = new List<TeamBasic>();

        public NewGame()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            GetAllDatabasesToComboBox();
            CheckIfEnableStartButton();
        }

        private void GetAllDatabasesToComboBox()
        {
            string[] files = System.IO.Directory.GetFiles("./", "*.db");
            for (int i = 0; i < files.Length; i++)
            {
                DatabaseComboBox.Items.Add(files[i].Substring(2)); //za to dopsat aktuální datum + tým, za který se hraje
            }
        }

        private void CheckIfEnableStartButton()
        {
            if (TeamListLB.SelectedIndex == -1 || !CheckGameName())
            {
                StartButton.IsEnabled = false;
            }
            else
            {
                StartButton.IsEnabled = true;
            }
        }

        private bool CheckGameName()
        {
            if (GameNameTB.Text != "" && GameNameTB.Text.Length <= 10)
            {
                if (!CheckIfCharIsAlphabetic(GameNameTB.Text.ElementAt(0)))
                {
                    return false;
                }
                for (int i = 1; i < GameNameTB.Text.Count(); i++)
                {
                    if (!CheckIfCharIsAlphabeticOrNumeric(GameNameTB.Text.ElementAt(i)))
                    {
                        return false;
                    }
                }
                //existuje název?
                return !File.Exists(@".\games\" + GameNameTB.Text + ".db" );
            }
            return false;
        }

        private bool CheckIfCharIsAlphabeticOrNumeric(char v)
        {
            return Char.IsLetterOrDigit(v);
        }

        private bool CheckIfCharIsAlphabetic(char v)
        {
            return Char.IsLetter(v);
        }

        private void GetAllNationsToComboBox()
        {
            nationList.Clear();
            teamList.Clear();
            nationList.Add(new Nation(0, "Všechny národnosti"));
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + DatabaseComboBox.SelectedItem  + ";"))
            {
                conn.Open();

                SQLiteCommand command = new SQLiteCommand("select nation.id_nation,nation.name from (select city.id_city,city.name,city.id_nation as nat from team inner join city on city.id_city=team.id_city group by city.id_city) inner join nation on nat=nation.id_nation group by nation.id_nation", conn);
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



        private void AddTeamsToListBox()
        {
            teamList.Clear();
            TeamListLB.Items.Clear();
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + DatabaseComboBox.SelectedItem + ";"))
            {
                List<int> citiesList = new List<int>();
                SQLiteCommand command;
                SQLiteDataReader reader;
                conn.Open();
                if (NationsCB.SelectedIndex > -1) 
                { 
                    int nationID = nationList.ElementAt(NationsCB.SelectedIndex).IdNation;
                    command = new SQLiteCommand("select id_city from city where id_nation=" + nationID, conn);
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        citiesList.Add(reader.GetInt32(0));
                    }
                    reader.Close();
                    for (int i = 0; i < citiesList.Count; i++)
                    {
                        command = new SQLiteCommand("select id_team,name from team where name like '%" + TeamNameTW.Text + "%' and id_city=" + citiesList.ElementAt(i), conn);
                        reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            teamList.Add(new TeamBasic(reader.GetInt32(0), reader.GetString(1)));
                        }
                        reader.Close();
                    }
                }
                else
                {
                    command = new SQLiteCommand("select id_team,name from team where name like '%" + TeamNameTW.Text + "%'", conn);
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        teamList.Add(new TeamBasic(reader.GetInt32(0), reader.GetString(1)));
                    }
                    reader.Close();
                }

                for (int i = 0; i < teamList.Count; i++)
                {
                    TeamListLB.Items.Add(teamList.ElementAt(i).Name);
                }
            }
            CheckIfEnableStartButton();
        }


        private void NationChangeComboBox(object sender, SelectionChangedEventArgs e)
        {
            AddTeamsToListBox();
        }

        private void TeamNameChangeTextView(object sender, TextChangedEventArgs e)
        {
            AddTeamsToListBox();
        }

        private void TeamListLBClickInto(object sender, SelectionChangedEventArgs e)
        {
            CheckIfEnableStartButton();
        }

        private void GameNameChange(object sender, TextChangedEventArgs e)
        {
            CheckIfEnableStartButton();
        }

        private void TeamListLB_MouseDown(object sender, MouseButtonEventArgs e)
        {
           // CheckIfEnableStartButton();
        }

        private void StartGame(object sender, RoutedEventArgs e)
        {
            File.Copy(@"./" + DatabaseComboBox.SelectedItem, @"./games/" + GameNameTB.Text + ".db");
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=./games/" + GameNameTB.Text + ".db;"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("update info set id_team=" + teamList.ElementAt(TeamListLB.SelectedIndex).IdTeam + ", date='2019-01-01'", conn);
                command.ExecuteReader();
                MainGame win2 = new MainGame("./games/" + GameNameTB.Text + ".db");
                Mainwindow.Close();
                this.Close();
                win2.ShowDialog();
            }
        }

        private void DatabaseChanged(object sender, SelectionChangedEventArgs e)
        {
            GetAllNationsToComboBox();
            ComboBox c = (ComboBox)sender;
            if (c.SelectedIndex > -1)
            {
                TeamNameTW.IsEnabled = true;
                NationsCB.IsEnabled = true;
                TeamNameTW.Text = "";
                NationsCB.SelectedIndex = -1;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            
            Mainwindow.IsEnabled = true;
        }
    }
}
