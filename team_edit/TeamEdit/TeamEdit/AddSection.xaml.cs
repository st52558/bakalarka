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

namespace TeamEdit
{
    /// <summary>
    /// Interakční logika pro AddSection.xaml
    /// </summary>
    public partial class AddSection : Window
    {
        List<Section> sectionList = new List<Section>();
        int selectedTeamId;
        int[] blacklistedSections;
        TeamEditor parent;
        public AddSection(int chosenTeam, int[] SectionsIDs)
        {
            blacklistedSections = SectionsIDs;
            selectedTeamId = chosenTeam;
            InitializeComponent();
            GetAllAvailableGamesToCB();
        }

        private void GetAllAvailableGamesToCB()
        {
            SectionsCB.Items.Clear();
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\test.db;"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select ID_SECTION,NAME from SECTION", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    bool isBlacklisted = false;
                    for (int i = 0; i < blacklistedSections.Length; i++)
                    {
                        
                        if (reader.GetInt32(0)==blacklistedSections[i])
                        {
                            isBlacklisted = true;
                            break;
                        }
                    }
                    if (!isBlacklisted)
                    {
                        sectionList.Add(new Section(reader.GetInt32(0), reader.GetString(1)));
                    }
                    
                }
                for (int i = 0; i < sectionList.Count; i++)
                {
                    SectionsCB.Items.Add(sectionList.ElementAt(i).Name);
                }
                reader.Close();
            }
        }

        private void AddSectionClick(object sender, RoutedEventArgs e)
        {
            parent.sqlStatements.Add("insert into teamxsection (id_team,id_section) values (" + selectedTeamId + "," + sectionList.ElementAt(SectionsCB.SelectedIndex).IdSection + ")");
            //parent.GetAllSectionsToListBox();
            this.Close();
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        internal void Show(TeamEditor teamEditor)
        {
            parent = teamEditor;
            this.Show();
        }
    }
}
