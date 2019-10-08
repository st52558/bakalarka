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

                SQLiteCommand command = new SQLiteCommand("select id_stat,jmeno from (select mesto.id_mesto,mesto.nazev,mesto.id_stat_fk from tym inner join mesto on mesto.id_mesto=tym.id_mesto_fk group by id_mesto_fk) inner join stat on id_stat_fk=stat.id_stat group by id_stat", conn);
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
                int nationID = nationList.ElementAt(NationsCB.SelectedIndex).IdNation;
                if (nationID != 0)
                {
                    command = new SQLiteCommand("select id_mesto from mesto where id_stat_fk=" + nationID, conn);
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        citiesList.Add(reader.GetInt32(0));
                    }
                    reader.Close();
                    for (int i = 0; i < citiesList.Count; i++)
                    {
                        command = new SQLiteCommand("select id_tym,nazev from tym where nazev like '%" + TeamNameTW.Text + "%' and id_mesto_fk=" + citiesList.ElementAt(i), conn);
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
                    command = new SQLiteCommand("select id_tym,nazev from tym where nazev like '%" + TeamNameTW.Text + "%'", conn);
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
            MainGame win2 = new MainGame();
            win2.DatabaseName = "./games/" + GameNameTB.Text + ".db";
            win2.Show();
            Mainwindow.Close();
            this.Close();
        }

        private void DatabaseChanged(object sender, SelectionChangedEventArgs e)
        {
            GetAllNationsToComboBox();
            
        }
    }
}
