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
    /// Interaction logic for PowerRanking.xaml
    /// </summary>
    public partial class PowerRanking : Window
    {
        string databaseName;
        string date;
        int year;
        List<Game> games;
        List<TeamSection> teamSections;
        public PowerRanking(string databaseNameI)
        {
            databaseName = databaseNameI;
            games = new List<Game>();
            InitializeComponent();
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select date from info;", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                reader.Read();
                date = reader.GetString(0);
                reader.Close();
                year = int.Parse(date.Substring(0, 4));
            }
            AddSectionsToComboBox();
            
        }

        private void AddSectionsToComboBox()
        {
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select id_section, name from section;", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                games.Clear();
                SectionsCB.Items.Clear();
                while (reader.Read())
                {
                    games.Add(new Game(reader.GetInt32(0), reader.GetString(1)));
                }
                for (int i = 0; i < games.Count; i++)
                {
                    SectionsCB.Items.Add(games.ElementAt(i).Name);
                }
                SectionsCB.SelectedIndex = 0;
            }
        }

        private void CreateRanking()
        {
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select id_teamxsection, team.name, power_ranking, team.id_team from teamxsection join team on team.id_team=teamxsection.id_team where id_section=" + games.ElementAt(SectionsCB.SelectedIndex).ID + " order by power_ranking desc limit 20;", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                teamSections = new List<TeamSection>();
                int counter = 1;
                while (reader.Read())
                {
                    TeamSection t = new TeamSection(reader.GetInt32(0), games.ElementAt(SectionsCB.SelectedIndex).ID, games.ElementAt(SectionsCB.SelectedIndex).Name);
                    t.PowerPosition = counter;
                    counter++;
                    t.TeamName = reader.GetString(1);
                    t.PowerRanking = reader.GetInt32(2);
                    t.TeamID = reader.GetInt32(3);
                    teamSections.Add(t);
                }
                reader.Close();
                
                for (int i = 0; i < teamSections.Count; i++)
                {
                    command = new SQLiteCommand("select id_teamxsection_home, home_score, away_score from '" + year + "match" + games.ElementAt(SectionsCB.SelectedIndex).ID + "' where (id_teamxsection_home=" + teamSections.ElementAt(i).ID + " or id_teamxsection_away=" + teamSections.ElementAt(i).ID + ") and home_score is not null", conn);
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        if (reader.GetInt32(0) == teamSections.ElementAt(i).ID)
                        {
                            if (reader.GetInt32(1) > reader.GetInt32(2))
                            {
                                teamSections.ElementAt(i).Wins++;
                            } else
                            {
                                teamSections.ElementAt(i).Loses++;
                            }
                        } 
                        else
                        {
                            if (reader.GetInt32(1) > reader.GetInt32(2))
                            {
                                teamSections.ElementAt(i).Loses++;
                            }
                            else
                            {
                                teamSections.ElementAt(i).Wins++;
                            }
                        }
                    }
                    reader.Close();
                }
                PowerRankingGrid.ItemsSource = teamSections;
            }
        }

        private void SectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CreateRanking();
        }

        private void ShowTeam(object sender, MouseButtonEventArgs e)
        {
            if (PowerRankingGrid.SelectedIndex > -1)
            {
                TeamDetail win2 = new TeamDetail(databaseName, teamSections.ElementAt(PowerRankingGrid.SelectedIndex).TeamID);
                win2.ShowDialog();
            }
        }
    }
}
