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
    /// Interakční logika pro AddNewSection.xaml
    /// </summary>
    public partial class AddNewSection : Window
    {
        
        List<Game> sectionList = new List<Game>();
        string databaseName;
        int teamId;
        int teamHomeCity;
        public AddNewSection(string database)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            databaseName = database;
            AddAvailableSectionsToComboBox(GetNotAvailableSections());
        }

        private List<int> GetNotAvailableSections()
        {
            List<int> notAvailableSections = new List<int>();
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select info.id_team, date, id_city from info join team on info.id_team=team.id_team;", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                reader.Read();
                teamId = reader.GetInt32(0);
                teamHomeCity = reader.GetInt32(2);
                reader.Close();

                //zjistit počet nepoužitelných sekcí
                command = new SQLiteCommand("select teamxsection.id_section,count(teamxsection.id_section) from teamxsection where teamxsection.id_team=" + teamId + " group by teamxsection.id_section", conn);
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (reader.GetInt32(1) >= 2)
                    {
                        notAvailableSections.Add(reader.GetInt32(0));
                    }
                }
                
                reader.Close();
            }

            return notAvailableSections;
        }

        private void AddAvailableSectionsToComboBox(List<int> sections)
        {
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select id_section, name from section;", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                bool availableSection;
                while (reader.Read())
                {
                    availableSection = true;
                    for (int i = 0; i < sections.Count; i++)
                    {
                        if (sections.ElementAt(i) == reader.GetInt32(0))
                        {
                            availableSection = false;
                            break;
                        }
                    }
                    if (availableSection)
                    {
                        sectionList.Add(new Game(reader.GetInt32(0), reader.GetString(1)));
                    }
                }
            }
            for (int i = 0; i < sectionList.Count; i++)
            {
                SectionList.Items.Add(sectionList.ElementAt(i).Name);
            }
        }

        private void AddSection(object sender, RoutedEventArgs e)
        {
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("insert into teamxsection (id_team,id_section,power_ranking,id_city) values (" + teamId + "," + sectionList.ElementAt(SectionList.SelectedIndex).ID + ",50 ," + teamHomeCity + ");", conn);
                command.ExecuteReader();
            }
            this.Close();
        }
    }
}
