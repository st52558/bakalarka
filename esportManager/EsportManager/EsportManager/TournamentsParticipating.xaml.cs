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
    /// Interakční logika pro TournamentsParticipating.xaml
    /// </summary>
    public partial class TournamentsParticipating : Window
    {
        

        List<TeamSectionBasic> sectionsList = new List<TeamSectionBasic>();
        string databaseName;
        int teamId;
        int sectionId;
        string date;

        List<Tournament> tournamentsForSection = new List<Tournament>();
        public TournamentsParticipating(string database)
        {
            databaseName = database;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select id_team, date from info;", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                reader.Read();
                teamId = reader.GetInt32(0);
                date = reader.GetString(1);
                reader.Close();
            }
            InitializeComponent();
            SetComboBox();
            
        }

        private void SetComboBox()
        {
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select teamxsection.id_teamxsection, section.id_section, name from section join teamxsection on teamxsection.id_section=section.id_section where id_team=" + teamId + " order by section.id_section;", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                int sectionBefore = -1;
                while (reader.Read())
                {
                    if (sectionBefore == reader.GetInt32(1))
                    {
                        //je to B tým
                        sectionsList.Add(new TeamSectionBasic(reader.GetInt32(0), reader.GetInt32(1), reader.GetString(2) + " B"));
                    }
                    else
                    {
                        //je to A tým
                        sectionsList.Add(new TeamSectionBasic(reader.GetInt32(0), reader.GetInt32(1), reader.GetString(2)));
                    }

                    sectionBefore = reader.GetInt32(1);
                }
                reader.Close();
            }
            if (sectionsList.Count > 0)
            {
                for (int i = 0; i < sectionsList.Count; i++)
                {
                    SectionList.Items.Add(sectionsList.ElementAt(i).sectionName); 
                }
                SectionList.SelectedIndex = 0;
                SetAllLists();
            }
        }

        private void SetAllLists()
        {
            tournamentsForSection.Clear();
            sectionId = sectionsList.ElementAt(SectionList.SelectedIndex).ID;
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select id_tournament_to, tournament.name, tournament.start_date, tournament.end_date, prize_pool, mesto.nazev, token_value, tournament.drawn from tournament_token join tournament on tournament.id_tournament=tournament_token.id_tournament_to join mesto on tournament.city_fk=id_mesto where id_teamxsection=" + sectionId + ";", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                bool drawn = false;
                while (reader.Read())
                {
                    if (reader.GetInt32(7) == 1)
                    {
                        drawn = true;
                    }
                    tournamentsForSection.Add(new Tournament(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetInt32(4), reader.GetString(5), reader.GetInt32(6),drawn));
                }
                reader.Close();
            }
            AddAllListsToTournamentListBoxes();
        }

        private void AddAllListsToTournamentListBoxes()
        {
            int index = SectionList.SelectedIndex;
            TournamentList.Items.Clear();
            {
                for (int i = 0; i < tournamentsForSection.Count; i++)
                {
                    TournamentList.Items.Add(tournamentsForSection.ElementAt(i).Name + ", " + tournamentsForSection.ElementAt(i).DateFrom + " - " + tournamentsForSection.ElementAt(i).DateTo + ", " + tournamentsForSection.ElementAt(i).PrizePool + "$, " + tournamentsForSection.ElementAt(i).City);
                }
            }
        }

        private void CancelChosenTournament(object sender, RoutedEventArgs e)
        {
            if (TournamentList.SelectedIndex < 0)
            {
                return;
            }
            if (tournamentsForSection.ElementAt(TournamentList.SelectedIndex).Drawn)
            {
                MessageBox.Show("Z tohoto turnaje se nelze odhlásit. Turnaj je již nalosován", "Chystáte se odhlásit z turnaje", MessageBoxButton.OK);
                return;
            }
            if (IsMoreThanTwoMonths(date, tournamentsForSection.ElementAt(TournamentList.SelectedIndex).DateFrom))
            {
                tournamentsForSection.ElementAt(TournamentList.SelectedIndex).TokenValue = 0;
            }
            MessageBoxResult result = MessageBox.Show("Vážně se chcete odhlásit z turnaje? Bude Vás to stát " + tournamentsForSection.ElementAt(TournamentList.SelectedIndex).TokenValue + "$.", "Chystáte se odhlásit z turnaje", MessageBoxButton.YesNo);
            if (result != MessageBoxResult.Yes)
            {
                return;
            }
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("update team set budget=budget-" + tournamentsForSection.ElementAt(TournamentList.SelectedIndex).TokenValue + " where id_team=" + teamId + ";", conn);
                command.ExecuteReader();
                if (IsMoreThanTwoMonths(date, tournamentsForSection.ElementAt(TournamentList.SelectedIndex).DateFrom))
                {
                    command = new SQLiteCommand("delete from tournament_token where id_tournament_to=" + tournamentsForSection.ElementAt(TournamentList.SelectedIndex).ID + " and id_teamxsection=" +  sectionId + ";", conn);
                } else
                {
                    command = new SQLiteCommand("update tournament_token set id_teamxsection=21 where id_teamxsection=" + sectionId + ";", conn);
                }
                command.ExecuteReader();
            }
            SetAllLists();
            /*tournamentsForSection1.RemoveAt(TournamentList.SelectedIndex);
            AddAllListsToTournamentListBoxes();*/
        }

        private void ShowTournament(object sender, MouseButtonEventArgs e)
        {
            if (TournamentList.SelectedIndex == -1)
            {
                return;
            }
            /* using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select team.name, tournament.name from tournament_token join tournament on tournament.id_tournament=tournament_token.id_tournament_to join teamxsection on teamxsection.id_teamxsection=tournament_token.id_teamxsection join team on teamxsection.id_team=team.id_team where tournament_token.id_tournament_to=" + tournamentsForSection.ElementAt(TournamentList.SelectedIndex).ID + ";", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                int counter = 1;
                string allTeams = "";
                while (reader.Read())
                {
                    if (counter == 1)
                    {
                        allTeams += reader.GetString(1) + '\n' + '\n';
                    }
                    allTeams += counter + ": " + reader.GetString(0) + '\n';
                    counter++;
                }
                MessageBox.Show(allTeams, "Registrovaní do turnaje", MessageBoxButton.OK);
                reader.Close();
            }*/
            TournamentInfoGroup win2 = new TournamentInfoGroup(databaseName,tournamentsForSection.ElementAt(TournamentList.SelectedIndex).ID);
            win2.Show();
        }

        private void SectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetAllLists();
        }

        private string TransformDate(string v)
        {
            string year = v.Remove(4, 6);
            string month = v.Remove(7, 3).Remove(0, 5);
            string day = v.Remove(0, 8);
            return day + ". " + month + ". " + year;
        }

        private bool IsMoreThanTwoMonths(string first, string second)
        {
            int fyear = int.Parse(first.Remove(4, 6));
            int fmonth = int.Parse(first.Remove(7, 3).Remove(0, 5));
            int fday = int.Parse(first.Remove(0, 8));
            int syear = int.Parse(second.Remove(4, 6));
            int smonth = int.Parse(second.Remove(7, 3).Remove(0, 5));
            int sday = int.Parse(second.Remove(0, 8));
            DateTime firstDate = new DateTime(fyear, fmonth, fday);
            DateTime secondDate = new DateTime(syear, smonth, sday);
            firstDate = firstDate.AddMonths(2);
            return firstDate.CompareTo(secondDate) < 0;
        }
    }
}
