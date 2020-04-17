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
    public partial class TournamentShow : Window
    {
        

        List<TeamSection> sectionsList = new List<TeamSection>();

        string databaseName;
        int teamId;
        int sectionId;
        string date;
        //1=zobrazení zůčastněných turnajů
        //2=zobrazení volných turnajů
        int type;
        List<Tournament> tournamentsForSection = new List<Tournament>();
        List<Tournament> openTournaments = new List<Tournament>();
        public TournamentShow(string database, int typeI)
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
            type = typeI;
            InitializeComponent();
            SetComboBox();
            if (type == 2)
            {
                TournamentAction.Content = "Příhlásit";
            }
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
                        sectionsList.Add(new TeamSection(reader.GetInt32(0), reader.GetInt32(1), reader.GetString(2) + " B"));
                    }
                    else
                    {
                        //je to A tým
                        sectionsList.Add(new TeamSection(reader.GetInt32(0), reader.GetInt32(1), reader.GetString(2)));
                    }

                    sectionBefore = reader.GetInt32(1);
                }
                reader.Close();
            }
            if (sectionsList.Count > 0)
            {
                for (int i = 0; i < sectionsList.Count; i++)
                {
                    SectionList.Items.Add(sectionsList.ElementAt(i).SectionName); 
                }
                SectionList.SelectedIndex = 0;
                SetParticipatingList();
            }
        }

        private void SetParticipatingList()
        {
            tournamentsForSection.Clear();
            sectionId = sectionsList.ElementAt(SectionList.SelectedIndex).ID;
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select id_tournament_to, tournament.name, tournament.start_date, tournament.end_date, prize_pool, city.name, token_value, tournament.drawn from tournament_token join tournament on tournament.id_tournament=tournament_token.id_tournament_to join city on tournament.id_city=city.id_city where id_teamxsection=" + sectionId + ";", conn);
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
            if (type == 1)
            {
                AddListToListBox();
            } else
            {
                SetOpenTournamentList();
            }
            
        }

        private void SetOpenTournamentList()
        {
            openTournaments.Clear();
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select distinct id_tournament_to, tournament.name, tournament.start_date, tournament.end_date, prize_pool, city.name, token_value, tournament.drawn from tournament_token join tournament on tournament.id_tournament=tournament_token.id_tournament_to join city on tournament.id_city=city.id_city where start_date>'" + date + "' and id_teamxsection is null and open_reg=1 order by start_date;", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    bool possible = true;
                    for (int i = 0; i < tournamentsForSection.Count; i++)
                    {
                        if (!(reader.GetString(2).CompareTo(tournamentsForSection.ElementAt(i).DateTo)>=0 || reader.GetString(3).CompareTo(tournamentsForSection.ElementAt(i).DateFrom)<=0))
                        {
                            possible = false;
                            break;
                        }
                    }
                    if (possible)
                    {
                        openTournaments.Add(new Tournament(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetInt32(4), reader.GetString(5), reader.GetInt32(6), false));
                    }
                }
                reader.Close();
            }
            AddListToListBox();
        }

        private void AddListToListBox()
        {
            
            TournamentList.Items.Clear();
            if (type == 1)
            {
                for (int i = 0; i < tournamentsForSection.Count; i++)
                {
                    TournamentList.Items.Add(tournamentsForSection.ElementAt(i).Name + ", " + tournamentsForSection.ElementAt(i).DateFrom + " - " + tournamentsForSection.ElementAt(i).DateTo + ", " + tournamentsForSection.ElementAt(i).PrizePool + "$, " + tournamentsForSection.ElementAt(i).City);
                }
            } else
            {
                for (int i = 0; i < openTournaments.Count; i++)
                {
                    TournamentList.Items.Add(openTournaments.ElementAt(i).Name + ", " + openTournaments.ElementAt(i).DateFrom + " - " + openTournaments.ElementAt(i).DateTo + ", " + openTournaments.ElementAt(i).PrizePool + "$, " + openTournaments.ElementAt(i).City);
                }
            }
            
        }

        private void BuutonAction(object sender, RoutedEventArgs e)
        {
            if (type == 1)
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
                        command = new SQLiteCommand("delete from tournament_token where id_tournament_to=" + tournamentsForSection.ElementAt(TournamentList.SelectedIndex).ID + " and id_teamxsection=" + sectionId + ";", conn);
                    }
                    else
                    {
                        command = new SQLiteCommand("update tournament_token set id_teamxsection=21 where id_teamxsection=" + sectionId + ";", conn);
                    }
                    command.ExecuteReader();
                }
                SetParticipatingList();
            } else
            {
                if (TournamentList.SelectedIndex < 0)
                {
                    return;
                }
                MessageBoxResult result = MessageBox.Show("Vážně se chcete příhlásit do turnaje?", "Chystáte se přihlásit do turnaje", MessageBoxButton.YesNo);
                if (result != MessageBoxResult.Yes)
                {
                    return;
                }
                using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
                {
                    conn.Open();
                    SQLiteCommand command = new SQLiteCommand("select id_token from tournament_token where id_tournament_to=" + openTournaments.ElementAt(TournamentList.SelectedIndex).ID + " and id_teamxsection is null", conn);
                    SQLiteDataReader reader = command.ExecuteReader();
                    if (!reader.Read())
                    {
                        return;
                    }
                    int token = reader.GetInt32(0);
                    {
                        command = new SQLiteCommand("update tournament_token set id_teamxsection=" + sectionsList.ElementAt(SectionList.SelectedIndex).ID +" where id_token=" + token + ";", conn);
                    }
                    command.ExecuteReader();
                }
            }
            
        }

        private void ShowTournament(object sender, MouseButtonEventArgs e)
        {
            if (type == 1)
            {
                if (TournamentList.SelectedIndex == -1)
                {
                    return;
                }
                TournamentInfoGroup win2 = new TournamentInfoGroup(databaseName, tournamentsForSection.ElementAt(TournamentList.SelectedIndex).ID, true);
                win2.ShowDialog();
            } else
            {

            }
            
        }

        private void SectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetParticipatingList();
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
