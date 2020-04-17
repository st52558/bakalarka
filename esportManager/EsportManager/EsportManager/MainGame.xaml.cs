using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace EsportManager
{
    /// <summary>
    /// Interakční logika pro MainGame.xaml
    /// </summary>
    public partial class MainGame : Window
    {
        public string DatabaseName { get; set; }
        List<TeamSection> sectionsList = new List<TeamSection>();
        string date;
        int idTeam;
        int todaysMatchTeam;
        int matchTeamSection;
        string teamName;
        int coach;
        string coachName;
        int year;
        Random random;
        public MainGame(string database)
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            random = new Random();
            DatabaseName = database;
            InitializeComponent();
            SetLabels();
            AddSectionsToList();
            SetTabs();
            AddAllMatches();
            AddAllPlayers();
            ChangePropertiesOfNextActionButton();
            NonPlayerTeamsActions();
        }

        private void NonPlayerTeamsActions()
        {
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + DatabaseName + ";"))
            {
                /*conn.Open();
                SQLiteCommand command = new SQLiteCommand("select date from info;", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                string curDate = reader.GetString(0);
                //(u hráčů, kteří nepatří do týmu se na 50 % prodlouží smlouva, zbytku ne)
                command = new SQLiteCommand("select player.id_player, player.contractEnd, player.individualSkill, player.teamplaySkill, team.id_team from player join teamxsection on player.team_fk=teamxsection.id_teamxsection join team on team.id_team=teamxsection.id_team where team.id_team=" + idTeam + ";", conn);
            */
            }
        }

        private void SetLabels()
        {
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + DatabaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select id_team, date from info;", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                reader.Read();
                idTeam = reader.GetInt32(0);
                date = reader.GetString(1);
                DateLabel.Content = TransformDate(date);
                reader.Close();
                command = new SQLiteCommand("select name, budget, logo from team where id_team=" + idTeam + ";", conn);
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    teamName = reader.GetString(0);
                    TeamNameLabel.Content = teamName;
                    BudgetLabel.Content = reader.GetInt32(1) + "$";
                    byte[] data = (byte[])reader[2];
                    BitmapImage imageSource = new BitmapImage();
                    using (MemoryStream ms = new MemoryStream(data))
                    {
                        imageSource.BeginInit();
                        imageSource.StreamSource = ms;
                        imageSource.CacheOption = BitmapCacheOption.OnLoad;
                        imageSource.EndInit();
                    }


                    Logo.Source = imageSource;
                }
                reader.Close();
            }
        }

        private string TransformDate(string v)
        {
            string year = v.Remove(4, 6);
            string month = v.Remove(7, 3).Remove(0, 5);
            string day = v.Remove(0, 8);
            return day + ". " + month + ". " + year;
        }

        private string NextDay(string v)
        {
            int day = int.Parse(v.Remove(0, 8));
            int month = int.Parse(v.Remove(7, 3).Remove(0, 5));
            if ((day == 30 && (month == 4 || month == 6 || month == 9 || month == 11)) || day == 31 || (day == 28 && month == 2))
            {
                day = 1;
                month++;
            }
            else
            {
                day++;
            }

            string dayString = day.ToString();
            if (dayString.Length == 1)
            {
                dayString = "0" + dayString;
            }
            string monthString = month.ToString();
            if (monthString.Length == 1)
            {
                monthString = "0" + monthString;
            }
            return v.Remove(5, 5) + monthString + "-" + dayString;
        }

        private void ChangePropertiesOfNextActionButton()
        {
            NextActionButton.Click -= SponsorContractExpired;
            NextActionButton.Click -= NextMonthClick;
            NextActionButton.Click -= PlayerContractExpired;
            NextActionButton.Click -= CoachContractExpired;
            NextActionButton.Click -= NextDayClick;
            NextActionButton.Click -= EndYearClick;
            NextActionButton.Click -= PlayMatch;
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + DatabaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select date from info;", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                reader.Read();
                string curDate = reader.GetString(0);
                reader.Close();
                // hledání hráčů, kterým končí smlouvy v týmu!!!!  
                command = new SQLiteCommand("select player.contractEnd from player join teamxsection on player.id_teamxsection=teamxsection.id_teamxsection join team on team.id_team=teamxsection.id_team where team.id_team=" + idTeam + " and contractEnd='" + curDate + "';", conn);
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    NextActionButton.Content = "hráči končí smlouva";
                    NextActionButton.Click += PlayerContractExpired;
                    return;
                }
                reader.Close();
                // končí zaměstnanci smlouva
                command = new SQLiteCommand("select coach.id_coach, coach.nick from coach where coach.id_team=" + idTeam + " and contractEnd='" + curDate + "';", conn);
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    coach = reader.GetInt32(0);
                    coachName = reader.GetString(1);
                    NextActionButton.Content = "trenérovi končí smlouva";
                    NextActionButton.Click += CoachContractExpired;
                    return;
                }
                reader.Close();
                // končí sponzorská smlouva
                int yearCurrent = int.Parse(date.Remove(4, 6));
                int monthCurrent = int.Parse(date.Remove(7, 3).Remove(0, 5));
                int dayCurrent = int.Parse(date.Remove(0, 8));
                command = new SQLiteCommand("select teamxsponsor.expiration_date from sponsor join teamxsponsor on sponsor.id_sponsor=teamxsponsor.id_sponsor where teamxsponsor.id_team=" + idTeam + ";", conn);
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int yearExpire = int.Parse(reader.GetString(0).Remove(4, 6));
                    int monthExpire = int.Parse(reader.GetString(0).Remove(7, 3).Remove(0, 5));
                    int dayExpire = int.Parse(reader.GetString(0).Remove(0, 8));
                    if (yearExpire == yearCurrent && monthCurrent == monthExpire && dayExpire == dayCurrent)
                    {
                        NextActionButton.Content = "končí sponzorská smlouva";
                        NextActionButton.Click += SponsorContractExpired;
                        return;
                    }
                }
                reader.Close();
                // je na řadě zápas
                for (int i = 0; i < sectionsList.Count; i++)
                {
                    
                    command = new SQLiteCommand("select count(*) from '" + yearCurrent +"match" + sectionsList.ElementAt(i).SectionID + "' where match_date='" + curDate + "' and (id_teamxsection_home=" + sectionsList.ElementAt(i).ID + " or id_teamxsection_away=" + sectionsList.ElementAt(i).ID + ") and home_score is null", conn);
                    reader = command.ExecuteReader();
                    reader.Read();
                    if (reader.GetInt32(0) > 0)
                    {
                        // otevřít zápas
                        NextActionButton.Content = "Zápas";
                        NextActionButton.Click += PlayMatch;
                        todaysMatchTeam = sectionsList.ElementAt(i).ID;
                        matchTeamSection = sectionsList.ElementAt(i).SectionID;
                        return;
                    }
                    reader.Close();
                }


                // nic se neděje - konec roku
                if (date.Remove(0, 5) == "31-12")
                {
                    NextActionButton.Content = "ukončit rok";
                    NextActionButton.Click += EndYearClick;
                    return;
                }
                //nic se neděje - konec měsíce
                int month = int.Parse(date.Remove(7, 3).Remove(0, 5));
                int day = int.Parse(date.Remove(0, 8));
                if ((day == 30 && (month == 4 || month == 6 || month == 9 || month == 11)) || day == 31 || (day == 28 && month == 2))
                {
                    NextActionButton.Content = "další měsíc";
                    NextActionButton.Click += NextMonthClick;
                    return;
                }
                // nic se neděje - další den
                NextActionButton.Content = "další den";
                NextActionButton.Click += NextDayClick;
                return;

            }
            /* hledání, co se vlastně bude dít.. možnosti:
               1 - nic se neděje - a - další den
                                 - b - konec roku
               2 - je na řadě zápas, takže button posune na okno se zápasem
               3 - končí sponzorská smlouva, vyjede okno, jestli chceme přesunout na smlouvy
               4 - končí hráčovi smlouva, vyjede okno, jestli chceme přesunout na přehled hráčů     DONE
               5 - končí zaměstnanci smlouva, vyjede okno, jestli chceme přesunout na 
            */
        }

        private void CoachContractExpired(object sender, RoutedEventArgs e)
        {
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + DatabaseName + ";"))
            {
                conn.Open();
                MessageBoxResult result = MessageBox.Show("Trenérovi " + coachName + " končí smlouva. Chcete jeho smlouvu o rok prodloužit?", "Trenérům končí smlouva", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.No)
                {
                    SQLiteCommand command = new SQLiteCommand("update coach set value = 0, salary = 0, id_team = NULL, contractEnd = '' where id_coach = " + coach + "; ", conn);
                    command.ExecuteReader();
                    AddAllPlayers();
                    //trenéři jsou volní
                    ChangePropertiesOfNextActionButton();
                }
                else if (result == MessageBoxResult.Yes)
                {
                    SQLiteCommand command = new SQLiteCommand("select contractEnd from coach where id_coach=" + coach + ";", conn);
                    SQLiteDataReader reader = command.ExecuteReader();
                    string contractEnd = "";
                    if (reader.Read())
                    {
                        int year = int.Parse(reader.GetString(0).Substring(0, 4));
                        year++;
                        contractEnd = year.ToString() + reader.GetString(0).Remove(0, 4);
                    }
                    reader.Close();
                    command = new SQLiteCommand("update coach set contractEnd='" + contractEnd + "' where id_coach=" + coach + ";", conn);
                    command.ExecuteReader();
                }
            }
        }

        private void PlayMatch(object sender, RoutedEventArgs e)
        {
            Match win2 = new Match(DatabaseName, todaysMatchTeam, matchTeamSection);
            win2.ShowDialog();
        }

        private void SponsorContractExpired(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Nejméně jedna smlouva se sponzorem dnes končí, kliknutím na Ano skončí se sponzorem/sponzory spolupráce.", "Končí smlouva se sponzorem", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + DatabaseName + ";"))
                {
                    conn.Open();
                    SQLiteCommand command = new SQLiteCommand("delete from teamxsponsor where id_team=" + idTeam + " and expiration_date='" + date + "';", conn);
                    command.ExecuteReader();
                    ChangePropertiesOfNextActionButton();
                }
            } 
            else if (result == MessageBoxResult.No)
            {
                ViewSponsors win2 = new ViewSponsors(1, DatabaseName);
                win2.ShowDialog();
                ChangePropertiesOfNextActionButton();
            }
        }

        private void NextMonthClick(object sender, RoutedEventArgs e)
        {
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + DatabaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select sum(monthly_payment) from sponsor join teamxsponsor on teamxsponsor.id_sponsor=sponsor.id_sponsor and teamxsponsor.id_team=" + idTeam + ";", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                reader.Read();
                int income = 0;
                if (reader[0].GetType() != typeof(DBNull))
                {
                     income = reader.GetInt32(0);
                }
                reader.Close();
                command = new SQLiteCommand("select sum(salary) from player join teamxsection on teamxsection.id_team=player.id_teamxsection and teamxsection.id_team=" + idTeam + ";", conn);
                reader = command.ExecuteReader();
                reader.Read();
                income -= reader.GetInt32(0);
                Console.WriteLine(income);
                reader.Close();
                command = new SQLiteCommand("update team set budget=budget+" + income + " where id_team=" + idTeam + ";", conn);
                command.ExecuteReader();
            }
            FillOpenTournaments();
            NextDayClick(sender, e);
            SetLabels();
        }

        private void FillOpenTournaments()
        {
            string year = date.Remove(4, 6);
            string month = date.Remove(7, 3).Remove(0, 5);
            int m = int.Parse(month) + 2;
            month = m.ToString();
            if (month.Length == 1)
            {
                month = "0" + month;
            }
            string date2 = year + "-" + month + "-01";
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + DatabaseName + ";"))
            {
                List<int> tokens = new List<int>();
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select id_token from tournament_token join tournament on id_tournament_to=id_tournament where tournament.start_date > '" + date + "' and tournament.start_date < '" + date2 + "' and id_teamxsection is null and id_tournament_from is null;", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    tokens.Add(reader.GetInt32(0));
                }
                reader.Close();
                command = new SQLiteCommand("select DISTINCT teamxsection.id_teamxsection from teamxsection left join tournament_token on teamxsection.id_teamxsection=tournament_token.id_teamxsection where tournament_token.id_teamxsection is null and teamxsection.id_team <>" + idTeam + " order by power_ranking desc;", conn);
                reader = command.ExecuteReader();
                int counter = 0;
                while (reader.Read())
                {
                    if (tokens.Count == counter)
                    {
                        break;
                    }
                    SQLiteCommand command2 = new SQLiteCommand("update tournament_token set id_teamxsection=" + reader.GetInt32(0) + " where id_token=" + tokens.ElementAt(counter) + ";", conn);
                    command2.ExecuteReader();
                    counter++;
                }
                reader.Close();
            }
        }

        private void NextDayClick(object sender, RoutedEventArgs e)
        {
            // každodenní rutina týmů
            EveryDayTeamActions();
            // odehrávání zápasů
            PlayAllOtherMatches();
            // vyhodnocení ukončených turnajů
            FinishTournaments();
            // losování turnajů
            int curYear = int.Parse(date.Remove(4, 6));
            int curMonth = int.Parse(date.Remove(7, 3).Remove(0, 5));
            int curDay = int.Parse(date.Remove(0, 8));
            List<int> tournamentsToDraw = new List<int>();
            int tournamentYear;
            int tournamentMonth;
            int tournamentDay;
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + DatabaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select id_tournament, start_date, n_of_teams from tournament where start_date>'" + date + "' and drawn=0;", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    tournamentYear = int.Parse(reader.GetString(1).Remove(4, 6));
                    tournamentMonth = int.Parse(reader.GetString(1).Remove(7, 3).Remove(0, 5));
                    tournamentDay = int.Parse(reader.GetString(1).Remove(0, 8));
                    if (tournamentMonth <= curMonth + 1)
                    {
                        SQLiteCommand command2 = new SQLiteCommand("select count(id_teamxsection) from tournament_token where id_tournament_to=" + reader.GetInt32(0) + ";", conn);
                        SQLiteDataReader reader2 = command2.ExecuteReader();
                        reader2.Read();
                        if (reader.GetInt32(2) == reader2.GetInt32(0))
                        {
                            tournamentsToDraw.Add(reader.GetInt32(0));
                        }
                        reader2.Close();
                    }
                }
                reader.Close();
            }
            // už vím turnaje, které jsou potřeba nalosovat
            DrawTournaments(tournamentsToDraw);

            // tréninky
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + DatabaseName + ";"))
            {
                conn.Open();
                for (int i = 0; i < sectionsList.Count; i++)
                {
                    
                    SQLiteCommand command = new SQLiteCommand("select count(*) from '" + curYear + "match" + sectionsList.ElementAt(i).SectionID +"' where match_date='" + date + "' and id_teamxsection_home=" + sectionsList.ElementAt(i).ID + " or id_teamxsection_away=" + sectionsList.ElementAt(i).ID + ";", conn);
                    SQLiteDataReader reader = command.ExecuteReader();
                    reader.Read();
                    int matches = reader.GetInt32(0);
                    reader.Close();
                    if (matches == 0)
                    {
                       DailyTraining(sectionsList.ElementAt(i).ID);
                    }
                    
                }
            }
            FinishTournament(1, 1, 1, 1, 1, 2);
            // platba za ubytování
            PayingForAccommodations();

            // další den
            date = NextDay(date);
            DateLabel.Content = TransformDate(date);
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + DatabaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("update info set date='" + date + "';", conn);
                command.ExecuteReader();
            }
            ChangePropertiesOfNextActionButton();
        }

        private void EveryDayTeamActions()
        {
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + DatabaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select teamxsection.id_teamxsection, section.n_o_players-count(player.id_teamxsection), team.reputation from player join teamxsection on player.id_teamxsection=teamxsection.id_teamxsection join section on section.id_section=teamxsection.id_section join team on team.id_team=teamxsection.id_team where teamxsection.id_team<>" + idTeam + " group by player.id_teamxsection HAVING count(player.id_teamxsection)<section.n_o_players;", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                SQLiteCommand command2 = new SQLiteCommand("select id_player, individualSkill, teamplaySkill from player where id_teamxsection is null", conn);
                SQLiteDataReader reader2 = command2.ExecuteReader();
                string newDate = year + random.Next(1, 3) + date.Substring(4, 6);
                while (reader.Read())
                {
                    for (int i = 0; i < reader.GetInt32(1); i++)
                    {
                        while (reader2.Read())
                        {
                            int newSalary = 1000 + (int)(((reader.GetInt32(2) * (reader2.GetInt32(1) + reader2.GetInt32(2)) / 2) - 3600) * 1.02);
                            int playerValue = (newSalary * 100 / 3);
                            playerValue = playerValue / 100;
                            playerValue = playerValue * 100;
                            SQLiteCommand command3 = new SQLiteCommand("update player set contractEnd = '" + newDate + "', value = " + playerValue + ", salary = " + newSalary + ", playerCoop=70 where id_player = " + reader2.GetInt32(0) + "; ",conn);
                            command3.ExecuteReader();
                        }
                    }
                }
                reader.Close();
                reader2.Close();
            }
        }

        private void PayingForAccommodations()
        {
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + DatabaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select teamxsection.id_city, team.id_city from teamxsection join team on teamxsection.id_team=team.id_team where teamxsection.id_team=" + idTeam + ";", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                int payment = 0;
                while (reader.Read()) 
                {
                    if (reader.GetInt32(0) != reader.GetInt32(1))
                    {
                        payment += 1000;
                    }
                }
                reader.Close();
                command = new SQLiteCommand("update team set budget=budget-" + payment + " where id_team=" + idTeam + ";", conn);
                command.ExecuteReader();
            }
            SetLabels();
        }

        private void DailyTraining(int section)
        {
            int year = int.Parse(date.Remove(4, 6));
            int month = int.Parse(date.Remove(7, 3).Remove(0, 5));
            int day = int.Parse(date.Remove(0, 8));
            DateTime dateDate = new DateTime(year, month, day);
            int dayOfWeek;
            switch (dateDate.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    dayOfWeek = 7;
                    break;
                case DayOfWeek.Monday:
                    dayOfWeek = 1;
                    break;
                case DayOfWeek.Tuesday:
                    dayOfWeek = 2;
                    break;
                case DayOfWeek.Wednesday:
                    dayOfWeek = 3;
                    break;
                case DayOfWeek.Thursday:
                    dayOfWeek = 4;
                    break;
                case DayOfWeek.Friday:
                    dayOfWeek = 5;
                    break;
                case DayOfWeek.Saturday:
                    dayOfWeek = 6;
                    break;
                default:
                    dayOfWeek = 0;
                    break;
            }
            Random random = new Random();
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + DatabaseName + ";"))
            {
                List<Player> players = new List<Player>();
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select id_player, playerCoop, individualSkill, teamplaySkill, individualPotencial, teamplayPotencial, energy, id_section from player where id_teamxsection=" + section + ";", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                int idSection = 0;
                while (reader.Read())
                {
                    Player p = new Player();
                    p.IdPlayer = reader.GetInt32(0);
                    p.PlayerCoop = reader.GetInt32(1);
                    p.IndiSkill = reader.GetInt32(2);
                    p.TeamSkill = reader.GetInt32(3);
                    p.IndiPotencial = reader.GetInt32(4);
                    p.TeamPotencial = reader.GetInt32(5);
                    p.Energy = reader.GetInt32(6);
                    idSection = reader.GetInt32(7);
                    players.Add(p);
                }
                reader.Close();
                command = new SQLiteCommand("select id_coach, training from coach where id_team=" + idTeam + " and id_section=" + idSection + ";", conn);
                reader = command.ExecuteReader();
                Coach coach = new Coach();
                coach.Training = 20;
                if (reader.Read())
                {
                    coach.IdCoach = reader.GetInt32(0);
                    coach.Training = reader.GetInt32(1);
                }
                reader.Close();
                command = new SQLiteCommand("select type from training where day=" + dayOfWeek + ";", conn);
                reader = command.ExecuteReader();
                DailyTraining training = new DailyTraining();
                while (reader.Read())
                {
                    /*
                    0 - "Volno",
                    1 - "Individuální",
                    2 - "Analýza",
                    3 - "Cvičný zápas"*/
                    switch (reader.GetInt32(0))
                    {
                        case 0:
                            training.FreeTime++;
                            break;
                        case 1:
                            training.IndiTraining++;
                            break;
                        case 2:
                            training.Analysis++;
                            break;
                        case 3:
                            training.Scrims++;
                            break;
                        default:
                            break;
                    }
                }
                reader.Close();
                int energyDown = training.Analysis + training.IndiTraining + training.Scrims * 3 - training.FreeTime - 3;
                string com = "";
                for (int i = 0; i < players.Count; i++)
                {
                    players.ElementAt(i).Energy = Math.Min(100, players.ElementAt(i).Energy - energyDown);
                    if (players.ElementAt(i).Energy < 0)
                    {
                        players.ElementAt(i).IndiSkill -= 3;
                        players.ElementAt(i).TeamSkill -= 3;
                        players.ElementAt(i).Energy = 0;
                    }
                    // trénink v pořádku
                    else
                    {
                        // volno
                        for (int j = 0; j < training.FreeTime; j++)
                        {
                            if (random.Next(40) == 1)
                            {
                                Math.Max(players.ElementAt(i).TeamSkill--, 0);
                            }
                            if (random.Next(40) == 1)
                            {
                                Math.Max(players.ElementAt(i).IndiSkill--, 0);
                            }
                            if (random.Next(40) == 1)
                            {
                                Math.Max(players.ElementAt(i).PlayerCoop--,0);
                            }
                        }
                        // scrimy
                        for (int j = 0; j < training.Scrims; j++)
                        {
                            if (random.Next(120 - coach.Training) == 1)
                            {
                                Math.Min(players.ElementAt(i).TeamSkill++, players.ElementAt(i).TeamPotencial);
                            }
                            if (random.Next(120 - coach.Training) == 1)
                            {
                                Math.Min(players.ElementAt(i).IndiSkill++, players.ElementAt(i).IndiPotencial);
                            }
                            if (random.Next(2) == 1)
                            {
                                Math.Min(players.ElementAt(i).PlayerCoop++,100);
                            }
                        }
                        // analýza
                        for (int j = 0; j < training.Analysis; j++)
                        {
                            if (random.Next(100 - coach.Training) == 1)
                            {
                                Math.Min(players.ElementAt(i).TeamSkill++, players.ElementAt(i).TeamPotencial);
                            }
                        }
                        // individuál
                        for (int j = 0; j < training.Analysis; j++)
                        {
                            if (random.Next(100 - coach.Training) == 1)
                            {
                                Math.Min(players.ElementAt(i).IndiSkill++, players.ElementAt(i).IndiPotencial);
                            }
                        }
                    }
                    com += "update player set playerCoop=" + players.ElementAt(i).PlayerCoop + ", individualSkill=" + players.ElementAt(i).IndiSkill + ", teamplaySkill=" + players.ElementAt(i).TeamSkill + ", energy=" + players.ElementAt(i).Energy + " where id_player=" + players.ElementAt(i).IdPlayer + ";";
                    
                }
                command = new SQLiteCommand(com, conn);
                command.ExecuteReader();
            }
        }

        private void FinishTournaments()
        {
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + DatabaseName + ";"))
            {
                string ex = "";
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select id_tournament, prize_pool, pp_teams, pp_dividing, id_section, system, from tournament where end_date='" + date + "';", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    ex += FinishTournament(reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2), reader.GetInt32(3), reader.GetInt32(4), reader.GetInt32(5));
                }
                reader.Close();
                command = new SQLiteCommand(ex, conn);
                command.ExecuteReader();
            }
        }

        private string FinishTournament(int idTournament, int prizePool, int ppTeams, int ppDividing, int game, int system )
        {
            string ret = "";
            if (system ==1 || system == 2 || system == 6)
            {
                TournamentStandings standings;
                standings = new TournamentStandings(DatabaseName, idTournament, prizePool, ppTeams, ppDividing, system, game);
                for (int i = 0; i < standings.standings.Count; i++)
                {
                    ret += "update tournament_token set id_teamxsection=" + standings.standings.ElementAt(i).IdTeamSection + " where id_tournament_from=" + idTournament + " and tournament_from_position=" + standings.standings.ElementAt(i).Position + ";";
                    ret += "update team set budget = budget + " + standings.standings.ElementAt(i).PrizePool + " where id_team=" + standings.standings.ElementAt(i).IdTeam + ";";
                }
                
            } 
            else
            {
                TournamentBracket bracket = new TournamentBracket(DatabaseName, idTournament, prizePool, ppTeams, ppDividing, system, game);
                bracket.CountStandings();
                for (int i = 0; i < bracket.Teams.Count; i++)
                {
                    ret += "update tournament_token set id_teamxsection=" + bracket.Teams.ElementAt(i).IdTeamSection + " where id_tournament_from=" + idTournament + " and tournament_from_position=" + bracket.Teams.ElementAt(i).Position + ";";
                    ret += "update team set budget = budget + " + bracket.Teams.ElementAt(i).PrizePool + " where id_team=" + bracket.Teams.ElementAt(i).IdTeam + ";";
                }
            }
            return ret;
        }

        private void PlayAllOtherMatches()
        {
            List<MatchDetail> updateMatch = new List<MatchDetail>();
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + DatabaseName + ";"))
            {
                int year = int.Parse(date.Substring(0, 4));
                conn.Open();
                int idSection = 1;
                // PŘEDĚLAT SEKCE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                SQLiteCommand command = new SQLiteCommand("select id_match, m.id_teamxsection_home, m.id_teamxsection_away, tour.games_best_of, a.power_ranking, b.power_ranking from '" + year + "match" + idSection + "' m join teamxsection a on a.id_teamxsection = m.id_teamxsection_home join teamxsection b on b.id_teamxsection = m.id_teamxsection_away join tournament tour on tour.id_tournament = m.id_tournament where match_date = '" + date + "'", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                
                while (reader.Read())
                {
                    MatchDetail m = new MatchDetail(DatabaseName, idSection, date);
                    m.IdMatch = reader.GetInt32(0);
                    m.IdTxSHome = reader.GetInt32(1);
                    m.IdTxSAway = reader.GetInt32(2);
                    m.HomePowerRanking = reader.GetInt32(4);
                    m.AwayPowerRanking = reader.GetInt32(5);
                    m.ScoreToWin = reader.GetInt32(3) / 2 + 1;
                    SQLiteCommand command2 = new SQLiteCommand("select sum(individualSkill + teamplaySkill + playerCoop) / count(*) as i from player where id_teamxsection=" + m.IdTxSHome + ";", conn);
                    SQLiteDataReader reader2 = command2.ExecuteReader();
                    reader2.Read();
                    m.HomeStrength = reader2.GetInt32(0);
                    reader2.Close();
                    command2 = new SQLiteCommand("select sum(individualSkill + teamplaySkill + playerCoop) / count(*) as i from player where id_teamxsection=" + m.IdTxSAway + ";", conn);
                    reader2 = command2.ExecuteReader();
                    reader2.Read();
                    m.AwayStrength = reader2.GetInt32(0);
                    reader2.Close();
                    m.HomeScore = 0;
                    m.AwayScore = 0;
                    m.PlayMatch();
                    updateMatch.Add(m);
                }
                reader.Close();
            }
            for (int i = 0; i < updateMatch.Count; i++)
            {
                updateMatch.ElementAt(i).UpdateMatches();
            }
        }

        private void DrawTournaments(List<int> tournamentsToDraw)
        {
            for (int tourToDraw = 0; tourToDraw < tournamentsToDraw.Count; tourToDraw++)
            {
                using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + DatabaseName + ";"))
                {
                    conn.Open();
                    SQLiteCommand command = new SQLiteCommand("select start_date, playing_days, n_of_teams, end_date, id_section, system from tournament where id_tournament=" + tournamentsToDraw.ElementAt(tourToDraw) + ";", conn);
                    SQLiteDataReader reader = command.ExecuteReader();
                    reader.Read();
                    int game = reader.GetInt32(4);
                    int startYear = int.Parse(reader.GetString(0).Remove(4, 6));
                    int startMonth = int.Parse(reader.GetString(0).Remove(7, 3).Remove(0, 5));
                    int startDay = int.Parse(reader.GetString(0).Remove(0, 8));
                    int endYear = int.Parse(reader.GetString(3).Remove(4, 6));
                    int endMonth = int.Parse(reader.GetString(3).Remove(7, 3).Remove(0, 5));
                    int endDay = int.Parse(reader.GetString(3).Remove(0, 8));
                    DateTime startDate = new DateTime(startYear, startMonth, startDay);
                    DateTime endDate = new DateTime(endYear, endMonth, endDay);
                    int weeks = (int)Math.Ceiling((endDate - startDate).TotalDays / 7);
                    string playingDays = reader.GetString(1);
                    int system = reader.GetInt32(5);
                    int teams = reader.GetInt32(2);
                    bool oddTeams = teams % 2 == 1;
                    int[,] finalDraw = null;
                    int[,] futureMatches = null;


                    reader.Close();
                    if (system==1 || system == 2 || system==6)
                    {
                        if (oddTeams)
                        {
                            teams++;
                        }

                        int games = (teams - 1) * teams / 2; //single round robin, double až po losu
                        int[,] drawTeams = new int[teams / 2, 2];
                        // single round robin
                        for (int i = 0; i < teams / 2; i++)
                        {
                            drawTeams[i, 0] = i;
                            drawTeams[i, 1] = i + teams / 2;
                        }

                        // [počet kol,počet zápasů v kole]
                        int[,] draw = new int[teams - 1, teams];
                        for (int i = 0; i < teams - 1; i++)
                        {
                            for (int j = 0; j < teams; j++)
                            {
                                if (j % 2 == 0)
                                {
                                    draw[i, j] = drawTeams[j / 2, 0];
                                }
                                else
                                {
                                    draw[i, j] = drawTeams[j / 2, 1];
                                }
                            }
                            int temp = drawTeams[0, 1];
                            for (int j = 0; j < teams / 2 - 1; j++)
                            {
                                drawTeams[j, 1] = drawTeams[j + 1, 1];
                            }
                            drawTeams[teams / 2 - 1, 1] = drawTeams[teams / 2 - 1, 0];
                            for (int j = teams / 2 - 1; j > 1; j--)
                            {
                                drawTeams[j, 0] = drawTeams[j - 1, 0];
                            }
                            drawTeams[1, 0] = temp;
                        }
                        for (int i = 0; i < teams - 1; i++)
                        {
                            if (i % 2 == 1)
                            {
                                for (int j = 0; j < teams / 2; j++)
                                {
                                    int temp = draw[i, j * 2];
                                    draw[i, j * 2] = draw[i, j * 2 + 1];
                                    draw[i, j * 2 + 1] = temp;
                                }
                            }
                        }
                        // je double round robin
                        if (system == 2)
                        {
                            if (oddTeams)
                            {
                                finalDraw = new int[(teams - 2) * (teams-1), 2];
                            } else
                            {
                                finalDraw = new int[(teams - 1) * teams, 2];
                            }
                            
                            int counter = 0;
                            for (int i = 0; i < teams - 1; i++)
                            {
                                for (int j = 0; j < teams; j=j+2)
                                {
                                    if (!oddTeams || (draw[i, j] != teams - 1 && draw[i, j + 1] != teams - 1))
                                    {
                                        finalDraw[counter, 0] = draw[i, j];
                                        finalDraw[counter, 1] = draw[i, j + 1];
                                        finalDraw[counter + finalDraw.GetLength(0)/2, 0] = draw[i, j + 1];
                                        finalDraw[counter + finalDraw.GetLength(0)/2, 1] = draw[i, j];
                                        counter++;
                                    }
                                }
                            }
                        }
                        // triple round robin
                        else if (system == 6)
                        {
                            if (oddTeams)
                            {
                                finalDraw = new int[(teams - 2) * (teams - 1) * 3 /2, 2];
                            }
                            else
                            {
                                finalDraw = new int[(teams - 1) * teams * 3 / 2, 2];
                            }

                            int counter = 0;
                            for (int i = 0; i < teams - 1; i++)
                            {
                                for (int j = 0; j < teams; j += 2)
                                {
                                    if (!oddTeams || (draw[i, j] != teams - 1 && draw[i, j + 1] != teams - 1))
                                    {
                                        finalDraw[counter, 0] = draw[i, j];
                                        finalDraw[counter, 1] = draw[i, j + 1];
                                        finalDraw[counter + finalDraw.GetLength(0)*2 / 3, 0] = draw[i, j];
                                        finalDraw[counter + finalDraw.GetLength(0)*2 / 3, 1] = draw[i, j + 1];
                                        finalDraw[counter + finalDraw.GetLength(0) / 3, 0] = draw[i, j + 1];
                                        finalDraw[counter + finalDraw.GetLength(0) / 3, 1] = draw[i, j];
                                        counter++;
                                    }
                                }
                            }
                        }
                        // single round robin
                        else
                        {
                            if (oddTeams)
                            {
                                finalDraw = new int[(teams - 2) * (teams - 1) / 2, 2];
                            }
                            else
                            {
                                finalDraw = new int[(teams - 1) * teams / 2, 2];
                            }
                            int counter = 0;
                            for (int i = 0; i < teams - 1; i++)
                            {
                                for (int j = 0; j < teams / 2; j++)
                                {
                                    if (!oddTeams || (draw[i, j] != teams - 1 && draw[i, j + 1] != teams - 1))
                                    {
                                        finalDraw[counter, 0] = draw[i, j];
                                        finalDraw[counter, 1] = draw[i, j + 1];
                                        counter++;
                                    }
                                }
                            }
                        }
                        if (oddTeams)
                        {
                            teams--;
                        }
                    }
                    if (system == 3)
                    {
                        int teamsInRound = 2;
                        while (teams > teamsInRound)
                        {
                            teamsInRound = teamsInRound * 2;
                        }
                        int numOfByeTeams = teamsInRound - teams;
                        // lost 1. kola play-off
                        finalDraw = new int[(teamsInRound / 2)-numOfByeTeams, 2];
                        int counter = 0;
                        for (int i = teamsInRound / 2; i > numOfByeTeams; i--)
                        {
                            finalDraw[counter, 0] = teamsInRound - i;
                            finalDraw[counter, 1] = i - 1;
                            counter++;
                        }
                        // při 6 týmech finaldraw=2,5;3,4
                        // los ostatních kol
                        futureMatches = new int[(teamsInRound / 2) - 1, 6];
                        // 0-zápas který určí domácí tým
                        // 1-zápas který určí hostující tým
                        // 2-tým který bude domácí
                        // 3-tým který bude hostující
                        int byeTeamsUsed = 0;
                        int finalDrawUsed = 0;
                        int lastByeTeam = numOfByeTeams - 1;
                        int lastFinalDraw = finalDraw.GetLength(0) - 1;
                        int futureMatchesCounter = 0;
                        for (int i = 0; i < (teamsInRound/2)-1; i++)
                        {
                            // bye tým bude hrát proti už hrajícímu týmu
                            if (byeTeamsUsed <= lastByeTeam && finalDrawUsed <= lastFinalDraw)
                            {
                                futureMatches[i, 0] = -1;
                                futureMatches[i, 1] = lastFinalDraw;
                                futureMatches[i, 2] = byeTeamsUsed;
                                futureMatches[i, 3] = -1;
                                futureMatches[i, 4] = -1;
                                futureMatches[i, 5] = -1;
                                byeTeamsUsed++;
                                lastFinalDraw--;
                            }
                            // bye tým bude hrát proti jinému bye týmu
                            else if (byeTeamsUsed < lastByeTeam)
                            {
                                futureMatches[i, 0] = -1;
                                futureMatches[i, 1] = -1;
                                futureMatches[i, 2] = byeTeamsUsed;
                                futureMatches[i, 3] = lastByeTeam;
                                futureMatches[i, 4] = -1;
                                futureMatches[i, 5] = -1;
                                byeTeamsUsed++;
                                lastByeTeam--;
                            } 
                            // doplnění zbytku hrajících týmů v 1. kole
                            else if (finalDrawUsed < lastFinalDraw)
                            {
                                futureMatches[i, 0] = finalDrawUsed;
                                futureMatches[i, 1] = lastFinalDraw;
                                futureMatches[i, 2] = -1;
                                futureMatches[i, 3] = -1;
                                futureMatches[i, 4] = -1;
                                futureMatches[i, 5] = -1;
                                finalDrawUsed++;
                                lastFinalDraw--;
                            }
                            else
                            {
                                futureMatches[i, 0] = -1;
                                futureMatches[i, 1] = -1;
                                futureMatches[i, 2] = -1;
                                futureMatches[i, 3] = -1;
                                futureMatches[i, 4] = futureMatchesCounter;
                                futureMatchesCounter++;
                                futureMatches[i, 5] = futureMatchesCounter;
                                futureMatchesCounter++;
                            }
                        }
                    }
                    if (system == 4)
                    {
                        finalDraw = new int[1, 2];
                        finalDraw[0, 0] = teams - 1;
                        finalDraw[0, 1] = teams - 2;
                        futureMatches = new int[teams-2,6];
                        for (int i = 0; i < futureMatches.GetLength(0); i++)
                        {
                            if (i==0)
                            {
                                futureMatches[i, 0] = -1;
                                futureMatches[i, 1] = 0;
                                futureMatches[i, 2] = teams - 3;
                                futureMatches[i, 3] = -1;
                                futureMatches[i, 4] = -1;
                                futureMatches[i, 5] = -1;
                            } else
                            {
                                futureMatches[i, 0] = -1;
                                futureMatches[i, 1] = -1;
                                futureMatches[i, 2] = teams - 3 - i;
                                futureMatches[i, 3] = -1;
                                futureMatches[i, 4] = -1;
                                futureMatches[i, 5] = i - 1;
                            }
                        }
                    }

                    command = new SQLiteCommand("select id_teamxsection from tournament_token where id_tournament_to=" + tournamentsToDraw.ElementAt(tourToDraw) + " order by seed", conn);
                    reader = command.ExecuteReader();

                    int[] realIds = new int[teams];
                    int[] realIdMatches = new int[finalDraw.GetLength(0)];
                    int[] realIdFuture = null;
                    if (futureMatches != null)
                    {
                        realIdFuture = new int[futureMatches.GetLength(0)];
                    }
                    
                    for (int i = 0; i < teams; i++)
                    {
                        reader.Read();
                        realIds[i] = reader.GetInt32(0);
                    }
                    reader.Close();
                    realIds = CheckIfTeamsAreUnique(realIds, tournamentsToDraw.ElementAt(tourToDraw));
                    DateTime date = startDate;
                    int[] daysPlaying = new int[7];
                    for (int i = 0; i < daysPlaying.Length; i++)
                    {
                        daysPlaying[i] = int.Parse(playingDays.Substring(i, 1));
                    }
                    int matchCounter = 0;
                    while (date.CompareTo(endDate) <= 0)
                    {
                        int dayOfWeek;
                        switch (date.DayOfWeek)
                        {
                            case DayOfWeek.Sunday:
                                dayOfWeek = 6;
                                break;
                            case DayOfWeek.Monday:
                                dayOfWeek = 0;
                                break;
                            case DayOfWeek.Tuesday:
                                dayOfWeek = 1;
                                break;
                            case DayOfWeek.Wednesday:
                                dayOfWeek = 2;
                                break;
                            case DayOfWeek.Thursday:
                                dayOfWeek = 3;
                                break;
                            case DayOfWeek.Friday:
                                dayOfWeek = 4;
                                break;
                            case DayOfWeek.Saturday:
                                dayOfWeek = 5;
                                break;
                            default:
                                dayOfWeek = -1;
                                break;
                        }
                        if (daysPlaying[dayOfWeek] != 0)
                        {
                            int matchesInDay = daysPlaying[dayOfWeek];
                            string separator = "-";
                            string separator2 = "-";
                            if (date.Month < 10)
                            {
                                separator += "0";
                            }
                            if (date.Day < 10)
                            {
                                separator2 += "0";
                            }
                            string stringDate = date.Year + separator + date.Month + separator2 + date.Day;
                            int typeH=0, typeA=0, idH = 0,idA = 0;
                            for (int i = 0; i < matchesInDay; i++)
                            {
                                if (matchCounter < finalDraw.GetLength(0))
                                {
                                    command = new SQLiteCommand("insert into '" + date.Year + "match" + game + "' ('id_teamxsection_home', 'id_teamxsection_away', 'match_date', 'id_tournament') values (" + realIds[finalDraw[matchCounter, 0]] + "," + realIds[finalDraw[matchCounter, 1]] + ",'" + stringDate + "'," + tournamentsToDraw.ElementAt(tourToDraw) + ");", conn);
                                    command.ExecuteReader();
                                    command = new SQLiteCommand("select last_insert_rowid()",conn);
                                    reader = command.ExecuteReader();
                                    reader.Read();
                                    realIdMatches[matchCounter] = reader.GetInt32(0);
                                    reader.Close();
                                    matchCounter++;
                                } else if (futureMatches !=null && matchCounter < finalDraw.GetLength(0) + futureMatches.GetLength(0))
                                {
                                    if (futureMatches[matchCounter - finalDraw.GetLength(0), 0] != -1)
                                    {
                                        typeH = 1;
                                        idH = realIdMatches[futureMatches[matchCounter - finalDraw.GetLength(0), 0]];
                                    } 
                                    else if (futureMatches[matchCounter - finalDraw.GetLength(0), 2] != -1)
                                    {
                                        typeH = 2;
                                        idH = realIds[futureMatches[matchCounter - finalDraw.GetLength(0), 2]];
                                    } 
                                    else if (futureMatches[matchCounter - finalDraw.GetLength(0), 4] != -1)
                                    {
                                        typeH = 3;
                                        idH = realIdFuture[futureMatches[matchCounter - finalDraw.GetLength(0), 4]];
                                    }
                                    if (futureMatches[matchCounter - finalDraw.GetLength(0), 1] != -1)
                                    {
                                        typeA = 1;
                                        idA = realIdMatches[futureMatches[matchCounter - finalDraw.GetLength(0), 1]];
                                    }
                                    else if (futureMatches[matchCounter - finalDraw.GetLength(0), 3] != -1)
                                    {
                                        typeA = 2;
                                        idA = realIds[futureMatches[matchCounter - finalDraw.GetLength(0), 3]];
                                    }
                                    else if (futureMatches[matchCounter - finalDraw.GetLength(0), 5] != -1)
                                    {
                                        typeA = 3;
                                        idA = realIdFuture[futureMatches[matchCounter - finalDraw.GetLength(0), 5]];
                                    }
                                    command = new SQLiteCommand("insert into '" + date.Year + "future_match" + game + "' ('id_home', 'id_away', 'type_home', 'type_away', 'date', 'id_tournament') values (" + idH + "," + idA + "," + typeH + "," + typeA + ",'" + stringDate + "'," + tournamentsToDraw.ElementAt(tourToDraw) + ");", conn);
                                    command.ExecuteReader();
                                    command = new SQLiteCommand("select last_insert_rowid()", conn);
                                    reader = command.ExecuteReader();
                                    reader.Read();
                                    realIdFuture[matchCounter - finalDraw.GetLength(0)] = reader.GetInt32(0);
                                    reader.Close();
                                    matchCounter++;
                                }
                                
                            }
                        }
                        date = date.AddDays(1);
                    }
                    command = new SQLiteCommand("update tournament set drawn=1 where id_tournament=" + tournamentsToDraw.ElementAt(tourToDraw) + ";", conn);
                    command.ExecuteReader();
                    Console.WriteLine("HOTOVO " + tourToDraw);
                }
            }
        }

        private int[] CheckIfTeamsAreUnique(int[] realIds, int tournament)
        {
            int invalidId = -1;
            for (int i = 0; i < realIds.Length - 1; i++)
            {
                for (int j = i + 1; j < realIds.Length; j++)
                {
                    if (realIds[i] == realIds[j])
                    {
                        invalidId = j;   
                    }
                }
            }
            if (invalidId != -1)
            {
                using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + DatabaseName + ";"))
                {
                    conn.Open();
                    SQLiteCommand command = new SQLiteCommand("select DISTINCT teamxsection.id_teamxsection from teamxsection left join tournament_token on teamxsection.id_teamxsection=tournament_token.id_teamxsection where tournament_token.id_teamxsection is null order by power_ranking;", conn);
                    SQLiteDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        realIds[invalidId] = reader.GetInt32(0);
                    }
                    reader.Close();
                    command = new SQLiteCommand("update tournament_token set id_teamxsection=" + realIds[invalidId] + " where id_tournament_to=" + tournament + " and seed=" + invalidId + ";", conn);
                    command.ExecuteReader();
                    conn.Close();
                }
                realIds = CheckIfTeamsAreUnique(realIds, tournament);
            }
            return realIds;
        }

        private void EndYearClick(object sender, RoutedEventArgs e)
        {
            date = NextDay(date);
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + DatabaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("CREATE TABLE '" + date.Substring(0, 4) + "future_match1' ('id_match'  INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,'id_home'  INTEGER NOT NULL,'id_away'   INTEGER NOT NULL,'type_home' INTEGER NOT NULL,'type_away' INTEGER NOT NULL,'date'  BLOB NOT NULL,'id_tournament' INTEGER NOT NULL,FOREIGN KEY('id_tournament') REFERENCES 'tournament'('id_tournament'));", conn);
                command.ExecuteReader();
                command = new SQLiteCommand("CREATE TABLE '" + date.Substring(0, 4) + "match1' ('id_match'  INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,    'id_teamxsection_home'  INTEGER NOT NULL,    'id_teamxsection_away'  INTEGER NOT NULL,    'home_score'    INTEGER,    'away_score'    INTEGER,    'match_date'    TEXT NOT NULL,    'id_tournament' INTEGER NOT NULL,    FOREIGN KEY('id_tournament') REFERENCES 'tournament'('id_tournament'),    FOREIGN KEY('id_teamxsection_home') REFERENCES 'teamxsection'('id_teamxsection'),    FOREIGN KEY('id_teamxsection_away') REFERENCES 'teamxsection'('id_teamxsection')); ", conn);
                command.ExecuteReader();
                command = new SQLiteCommand("select id_tournament, start_date, end_date from tournament", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    SQLiteCommand command2 = new SQLiteCommand("update tournament set start_date=" + date.Substring(0, 4) + reader.GetString(1).Substring(4,6) + ", end_date=" + date.Substring(0, 4) + reader.GetString(1).Substring(4, 6) + " where id_tournament=" + reader.GetInt32(0) + ";", conn);
                    command2.ExecuteReader();
                }
                reader.Close();
            }
            ChangePropertiesOfNextActionButton();
            SetLabels();
        }

        private void PlayerContractExpired(object sender, RoutedEventArgs e)
        {
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + DatabaseName + ";"))
            {
                conn.Open();
                MessageBoxResult result = MessageBox.Show("Nejméně jednomu vašemu hráči končí smlouva. Chcete jeho/jejich smlouvu prodloužit? Stisknutím 'Ne' všechny hráče propustíte.", "Hráčům končí smlouva", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.No)
                {

                    SQLiteCommand command = new SQLiteCommand("select player.id_player from player join teamxsection on player.id_teamxsection=teamxsection.id_teamxsection join team on team.id_team=teamxsection.id_team where team.id_team=" + idTeam + " and contractEnd<='" + date + "';", conn);
                    SQLiteDataReader reader = command.ExecuteReader();
                    string comm = "";
                    while (reader.Read())
                    {
                        comm += "update player set value=0, salary=0, id_teamxsection=NULL, contractEnd='' where id_player=" + reader.GetInt32(0) + ";";
                    }
                    reader.Close();
                    command = new SQLiteCommand(comm, conn);
                    command.ExecuteReader();
                    AddAllPlayers();
                    //hráči jsou volní
                    ChangePropertiesOfNextActionButton();
                }
                else if (result == MessageBoxResult.Yes)
                {
                    SQLiteCommand command = new SQLiteCommand("select player.id_player from player join teamxsection on player.id_teamxsection=teamxsection.id_teamxsection join team on team.id_team=teamxsection.id_team where team.id_team=" + idTeam + " and contractEnd<='" + date + "';", conn);
                    SQLiteDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        PlayerDetail win2 = new PlayerDetail(DatabaseName, reader.GetInt32(0));
                        win2.ShowDialog();
                    }
                    reader.Close();
                }
            }
        }

        private void AddAllPlayers()
        {

            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + DatabaseName + ";"))
            {
                conn.Open();
                for (int i = 0; i < sectionsList.Count; i++)
                {
                    List<Player> players = new List<Player>();
                    SQLiteCommand command = new SQLiteCommand("select nick, player.name, surname, position_type.name, energy, id_player from player join position_type on position_type.id_position_in_game=player.id_position and position_type.id_section=player.id_section where id_teamxsection=" + sectionsList.ElementAt(i).ID, conn);
                    SQLiteDataReader reader = command.ExecuteReader();
                    string nick, name, surname, position;
                    while (reader.Read())
                    {
                        if (reader.IsDBNull(0))
                        {
                            nick = "";
                        }
                        else
                        {
                            nick = reader.GetString(0);
                        }
                        if (reader.IsDBNull(1))
                        {
                            name = "";
                        }
                        else
                        {
                            name = reader.GetString(1);
                        }
                        if (reader.IsDBNull(2))
                        {
                            surname = "";
                        }
                        else
                        {
                            surname = reader.GetString(2);
                        }
                        if (reader.IsDBNull(3))
                        {
                            position = "";
                        }
                        else
                        {
                            position = reader.GetString(3);
                        }
                        //players.Add(new PlayersDataGrid() { Nick = reader.GetString(0), Jmeno = reader.GetString(1), Prijmeni = reader.GetString(2), Pozice = reader.GetString(3) });
                        Player p = new Player();
                        p.PositionName = position;
                        p.Nick = nick;
                        p.Name = name;
                        p.Surname = surname;
                        p.Energy = reader.GetInt32(4);
                        p.IdPlayer = reader.GetInt32(5);
                        players.Add(p);
                    }

                    reader.Close();
                    command = new SQLiteCommand("select nick, coach.name, surname, teamxsection.id_team from coach join teamxsection on teamxsection.id_team=coach.id_team where teamxsection.id_teamxsection=" + sectionsList.ElementAt(i).ID + " and teamxsection.id_section=" + sectionsList.ElementAt(i).SectionID + ";", conn);
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Player p = new Player();
                        p.PositionName = "Trenér";
                        p.Nick = reader.GetString(0);
                        p.Name = reader.GetString(1);
                        p.Surname = reader.GetString(2);
                        p.Energy = 100;
                        players.Add(p);
                    }
                    switch (i)
                    {
                        case 0:
                            Section1PlayersList.ItemsSource = players;
                            break;
                        case 1:
                            Section2PlayersList.ItemsSource = players;
                            break;
                        case 2:
                            Section3PlayersList.ItemsSource = players;
                            break;
                        case 3:
                            Section4PlayersList.ItemsSource = players;
                            break;
                        case 4:
                            Section5PlayersList.ItemsSource = players;
                            break;
                        case 5:
                            Section6PlayersList.ItemsSource = players;
                            break;
                        default:
                            break;
                    }
                }

            }
        }

        private void AddAllMatches()
        {

            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + DatabaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command;
                SQLiteDataReader reader;
                string year = date.Remove(4, 6);

                for (int i = 0; i < sectionsList.Count; i++)
                {
                    List<MatchDetail> tournaments = new List<MatchDetail>();
                    string matchTableName = year + "match" + sectionsList.ElementAt(i).SectionID;
                    command = new SQLiteCommand("select tournament.id_tournament, tournament.shortcut, match_date, id_teamxsection_home, id_teamxsection_away, tournament.id_city, city.name, home_score, away_score from '" + matchTableName + "' join tournament on tournament.id_tournament='" + matchTableName + "'.id_tournament join city on city.id_city=tournament.id_city where id_teamxsection_home=" + sectionsList.ElementAt(i).ID + " or id_teamxsection_away=" + sectionsList.ElementAt(i).ID + " order by match_date asc;", conn);
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        SQLiteCommand command2;
                        SQLiteDataReader reader2;
                        MatchDetail m = new MatchDetail(DatabaseName,sectionsList.ElementAt(i).SectionID, date);
                        if (reader.GetInt32(3) == sectionsList.ElementAt(i).ID)
                        {
                            m.IdTxSHome = sectionsList.ElementAt(i).ID;
                            m.HomeShortcut = teamName;
                            command2 = new SQLiteCommand("select id_teamxsection, shortcut from team join teamxsection on team.id_team=teamxsection.id_team where teamxsection.id_teamxsection=" + reader.GetInt32(4), conn);
                            reader2 = command2.ExecuteReader();
                            reader2.Read();
                            m.IdTxSAway = reader2.GetInt32(0);
                            m.AwayShortcut = reader2.GetString(1);
                            m.OpponentShortcut = reader2.GetString(1);
                            if (reader[7].GetType() == typeof(DBNull)) { m.Won = 0; }
                            else if (reader.GetInt32(7) > reader.GetInt32(8))
                            {
                                m.Won = 1;
                            } else
                            {
                                m.Won = 2;
                            }
                        }
                        else
                        {
                            m.IdTxSAway = sectionsList.ElementAt(i).ID;
                            m.AwayShortcut = teamName;
                            command2 = new SQLiteCommand("select id_teamxsection, shortcut from team join teamxsection on team.id_team=teamxsection.id_team where teamxsection.id_teamxsection=" + reader.GetInt32(3), conn);
                            reader2 = command2.ExecuteReader();
                            reader2.Read();
                            m.IdTxSHome = reader2.GetInt32(0);
                            m.HomeShortcut = reader2.GetString(1);
                            m.OpponentShortcut = reader2.GetString(1);
                            if (reader[7].GetType() == typeof(DBNull)) { m.Won = 0; }
                            else if (reader.GetInt32(8) > reader.GetInt32(7))
                            {
                                m.Won = 1;
                            }
                            else
                            {
                                m.Won = 2;
                            }
                        }
                        m.IdTournament = reader.GetInt32(0);
                        m.Tournament = reader.GetString(1);
                        m.Date = reader.GetString(2);
                        m.IdCity = reader.GetInt32(5);
                        m.City = reader.GetString(6);
                        tournaments.Add(m);
                    }
                    switch (i)
                    {
                        case 0:
                            Section1TournamentsList.ItemsSource = tournaments;
                            break;
                        case 1:
                            Section2TournamentsList.ItemsSource = tournaments;
                            break;
                        case 2:
                            Section3TournamentsList.ItemsSource = tournaments;
                            break;
                        case 3:
                            Section4TournamentsList.ItemsSource = tournaments;
                            break;
                        case 4:
                            Section5TournamentsList.ItemsSource = tournaments;
                            break;
                        case 5:
                            Section6TournamentsList.ItemsSource = tournaments;
                            break;
                        default:
                            break;
                    }
                }
            }


        }

        private void AddSectionsToList()
        {
            sectionsList.Clear();
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + DatabaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select teamxsection.id_teamxsection, section.id_section, name from section join teamxsection on teamxsection.id_section=section.id_section where id_team=" + idTeam + " order by section.id_section;", conn);
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
        }

        private void SetTabs()
        {
            int numberOfSections = GetNumberOfSections() + 1;
            SectionTabs.Items.Clear();
            SectionTabs.Items.Add(Section1);
            SectionTabs.Items.Add(Section2);
            SectionTabs.Items.Add(Section3);
            SectionTabs.Items.Add(Section4);
            SectionTabs.Items.Add(Section5);
            SectionTabs.Items.Add(Section6);
            while (numberOfSections < SectionTabs.Items.Count)
            {
                SectionTabs.Items.RemoveAt(numberOfSections);
            }
            switch (numberOfSections)
            {
                case 2:
                    SetTabsDesign(Section1, Section1Body, sectionsList.ElementAt(0), Section1PlayersList, Section1TournamentsList);
                    SetTabAsNewSection(Section2, Section2Body);
                    break;
                case 3:
                    SetTabsDesign(Section1, Section1Body, sectionsList.ElementAt(0), Section1PlayersList, Section1TournamentsList);
                    SetTabsDesign(Section2, Section2Body, sectionsList.ElementAt(1), Section2PlayersList, Section2TournamentsList);
                    SetTabAsNewSection(Section3, Section3Body);
                    break;
                case 4:
                    SetTabsDesign(Section1, Section1Body, sectionsList.ElementAt(0), Section1PlayersList, Section1TournamentsList);
                    SetTabsDesign(Section2, Section2Body, sectionsList.ElementAt(1), Section2PlayersList, Section2TournamentsList);
                    SetTabsDesign(Section3, Section3Body, sectionsList.ElementAt(2), Section3PlayersList, Section3TournamentsList);
                    SetTabAsNewSection(Section4, Section4Body);
                    break;
                case 5:
                    SetTabsDesign(Section1, Section1Body, sectionsList.ElementAt(0), Section1PlayersList, Section1TournamentsList);
                    SetTabsDesign(Section2, Section2Body, sectionsList.ElementAt(1), Section2PlayersList, Section2TournamentsList);
                    SetTabsDesign(Section3, Section3Body, sectionsList.ElementAt(2), Section3PlayersList, Section3TournamentsList);
                    SetTabsDesign(Section4, Section4Body, sectionsList.ElementAt(3), Section4PlayersList, Section4TournamentsList);
                    SetTabAsNewSection(Section5, Section5Body);
                    break;
                case 6:
                    SetTabsDesign(Section1, Section1Body, sectionsList.ElementAt(0), Section1PlayersList, Section1TournamentsList);
                    SetTabsDesign(Section2, Section2Body, sectionsList.ElementAt(1), Section2PlayersList, Section2TournamentsList);
                    SetTabsDesign(Section3, Section3Body, sectionsList.ElementAt(2), Section3PlayersList, Section3TournamentsList);
                    SetTabsDesign(Section4, Section4Body, sectionsList.ElementAt(3), Section4PlayersList, Section4TournamentsList);
                    SetTabsDesign(Section5, Section5Body, sectionsList.ElementAt(4), Section5PlayersList, Section5TournamentsList);
                    SetTabAsNewSection(Section6, Section6Body);
                    break;
                case 7:
                    SetTabsDesign(Section1, Section1Body, sectionsList.ElementAt(0), Section1PlayersList, Section1TournamentsList);
                    SetTabsDesign(Section2, Section2Body, sectionsList.ElementAt(1), Section2PlayersList, Section2TournamentsList);
                    SetTabsDesign(Section3, Section3Body, sectionsList.ElementAt(2), Section3PlayersList, Section3TournamentsList);
                    SetTabsDesign(Section4, Section4Body, sectionsList.ElementAt(3), Section4PlayersList, Section4TournamentsList);
                    SetTabsDesign(Section5, Section5Body, sectionsList.ElementAt(4), Section5PlayersList, Section5TournamentsList);
                    SetTabsDesign(Section6, Section6Body, sectionsList.ElementAt(5), Section6PlayersList, Section6TournamentsList);
                    break;
                default:
                    break;
            }

        }

        private void SetTabAsNewSection(TabItem tab, Grid body)
        {
            body.Children.Clear();
            tab.Header = "+";
            body.Children.Clear();
            Button addSectionButton = new Button();
            addSectionButton.Width = 150;
            addSectionButton.Height = 70;
            addSectionButton.Content = "Přidat novou sekci";
            addSectionButton.Name = "addSection";
            addSectionButton.Click += AddNewSection;
            body.Children.Add(addSectionButton);
        }

        private void AddNewSection(object sender, RoutedEventArgs e)
        {
            // vyjede form s přidáním sekce
            AddNewSection win2 = new AddNewSection(DatabaseName);
            win2.ShowDialog();
            Refresh(SectionTabs.SelectedIndex);
        }

        private void SetTabsDesign(TabItem tab, Grid body, TeamSection teamSectionBasic, DataGrid playersList, DataGrid tournamentsList)
        {
            body.Children.Clear();
            body.Children.Add(playersList);
            body.Children.Add(tournamentsList);
            Brush brush = Brushes.Black;
            if (teamSectionBasic.SectionID % 4 == 0) { brush = Brushes.Brown; }
            if (teamSectionBasic.SectionID % 4 == 1) { brush = Brushes.Cyan; }
            if (teamSectionBasic.SectionID % 4 == 2) { brush = Brushes.Salmon; }
            if (teamSectionBasic.SectionID % 4 == 3) { brush = Brushes.Snow; }
            tab.Background = brush;
            body.Background = brush;
            tab.Header = teamSectionBasic.SectionName;
        }

        private void GoToNextDay(object sender, RoutedEventArgs e)
        {
            Random rand = new Random();
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + DatabaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select player.id_player, player.individualSkill, player.teamplaySkill, team.reputation from player join teamxsection on player.id_teamxsection = teamxsection.id_teamxsection join team on team.id_team = teamxsection.id_team;", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                double salary, salary4;
                int salary2, salary3;
                while (reader.Read())
                {
                    /* teamplayS = Math.Min((int)(rand.Next(-4, 5) + reader.GetInt32(2) * 0.9), 100);
                     individualS = Math.Min((int)(rand.Next(-4, 5) + reader.GetInt32(2) * 0.9), 100);
                     teamplayP = Math.Min(teamplayS + rand.Next(2, 11), 100);
                     individualP = Math.Min(individualS + rand.Next(2, 11), 100);
                     SQLiteCommand command2 = new SQLiteCommand("update player set individualSkill=" + individualS + ", teamplaySkill=" + teamplayS + ", teamplayPotencial=" + teamplayP + ", individualPotencial=" + individualP + " where id_player=" + reader.GetInt32(0) + ";", conn);
                     SQLiteDataReader reader2 = command2.ExecuteReader();*/
                    /*salary = 1000 + (int)(((reader.GetInt32(3) * (reader.GetInt32(1) + reader.GetInt32(2)) / 2) - 3600) * 1.02);
                    salary4 = (salary * 100 / 3);
                    salary2 = (int)(salary4 / 100);
                    salary3 = salary2 * 100;
                    SQLiteCommand command2 = new SQLiteCommand("update player set salary=" + salary + ", value=" + salary3 +" where id_player=" + reader.GetInt32(0) + ";", conn);
                    //SQLiteCommand command2 = new SQLiteCommand("update player set value=" + salary3 + " where id_player=" + reader.GetInt32(0) +";", conn);
                    SQLiteDataReader reader2 = command2.ExecuteReader();*/
                }
                reader.Close();
            }

        }

        private int GetNumberOfSections()
        {
            return sectionsList.Count;
        }

        private void ShowRegisteredTournaments(object sender, RoutedEventArgs e)
        {
            TournamentShow win2 = new TournamentShow(DatabaseName, 1);
            win2.ShowDialog();
        }

        private void ShowSponsors(object sender, RoutedEventArgs e)
        {
            ViewSponsors win2 = new ViewSponsors(1, DatabaseName);
            win2.ShowDialog();
        }

        private void QuitGame(object sender, RoutedEventArgs e)
        {
            Environment.Exit(1);
        }

        private void ShowManual(object sender, RoutedEventArgs e)
        {
            Manual win2 = new Manual();
            win2.ShowDialog();
        }

        private void ShowFreeEmployees(object sender, RoutedEventArgs e)
        {
            FreeCoaches win2 = new FreeCoaches(DatabaseName);
            win2.ShowDialog();
        }

        private void ShowPowerRanking(object sender, RoutedEventArgs e)
        {
            PowerRanking win2 = new PowerRanking(DatabaseName);
            win2.ShowDialog();
        }

        private void ShowTeams(object sender, RoutedEventArgs e)
        {
            TeamSearch win2 = new TeamSearch(DatabaseName);
            win2.ShowDialog();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(1);
        }

        private void ShowFreePlyaers(object sender, RoutedEventArgs e)
        {
            FreePlayers win2 = new FreePlayers(DatabaseName);
            win2.ShowDialog();
        }

        private void ShowAllPlayers(object sender, RoutedEventArgs e)
        {
            PlayerSearch win2 = new PlayerSearch(DatabaseName);
            win2.ShowDialog();
        }

        private void Window_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            /*if (IsEnabled)
            {
                SetLabels();
                AddSectionsToList();
                SetTabs();
                AddAllTournaments();
                AddAllPlayers();
                ChangePropertiesOfNextActionButton();
                NonPlayerTeamsActions();
            }*/
        }

        private void Refresh(int tabOpened)
        {
            SetLabels();
            AddSectionsToList();
            SetTabs();
            AddAllMatches();
            AddAllPlayers();
            ChangePropertiesOfNextActionButton();
            NonPlayerTeamsActions();
            SectionTabs.SelectedIndex = tabOpened;
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            Refresh(SectionTabs.SelectedIndex);
        }

        private void ShowTraining(object sender, RoutedEventArgs e)
        {
            Training win2 = new Training(DatabaseName);
            win2.ShowDialog();
        }

        private void OpenTournament(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DataGrid d = (DataGrid)sender;
            if (d.SelectedIndex > -1)
            {
                List<MatchDetail> l = (List<MatchDetail>)d.ItemsSource;
                TournamentInfoGroup win2 = new TournamentInfoGroup(DatabaseName, l.ElementAt(d.SelectedIndex).IdTournament, true);
                win2.ShowDialog();
            }
                
        }

        private void OpenPlayer(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DataGrid d = (DataGrid)sender;
            if (d.SelectedIndex > -1)
            {
                List<Player> l = (List<Player>)d.ItemsSource;
                if (l.ElementAt(d.SelectedIndex).IdPlayer != 0)
                {
                    PlayerDetail win2 = new PlayerDetail(DatabaseName, l.ElementAt(d.SelectedIndex).IdPlayer);
                    win2.ShowDialog();
                }
            }
        }

        private void ShowOverview(object sender, RoutedEventArgs e)
        {
            TeamDetail win2 = new TeamDetail(DatabaseName, idTeam);
            win2.ShowDialog();
        }

        private void ShowOpenTournaments(object sender, RoutedEventArgs e)
        {
            TournamentShow win2 = new TournamentShow(DatabaseName, 2);
            win2.ShowDialog();
        }

        private void TravelingClick(object sender, RoutedEventArgs e)
        {
            Traveling win2 = new Traveling(DatabaseName, idTeam);
            win2.ShowDialog();
        }
    }
}
