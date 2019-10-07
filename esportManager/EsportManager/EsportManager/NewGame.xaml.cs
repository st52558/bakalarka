﻿using System;
using System.Collections.Generic;
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
    /// Interakční logika pro Window1.xaml
    /// </summary>
    public partial class NewGame : Window
    {
        public NewGame()
        {
            InitializeComponent();
        }

        private void CheckIfEnableUpdateButton()
        {
            if (!TeamEditButton.IsEnabled && TeamListLB.SelectedIndex != -1 || !PlyerEditButton.IsEnabled && PlayersListLB.SelectedIndex != -1)
            {
                EditButton.IsEnabled = true;
            }
            else
            {
                EditButton.IsEnabled = false;
            }
        }

        private void GetAllNationsToComboBox()
        {
            nationList.Add(new Nation(0, "Všechny národnosti"));
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\test.db;"))
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

        private void NationChangeComboBox(object sender, SelectionChangedEventArgs e)
        {
            AddTeamsToListBox();
        }

        private void TeamNameChangeTextView(object sender, TextChangedEventArgs e)
        {
            AddTeamsToListBox();
        }

        private void AddTeamsToListBox()
        {
            teamList.Clear();
            TeamListLB.Items.Clear();
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\test.db;"))
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
            CheckingAfterEveryClick();
        }
    }
}
