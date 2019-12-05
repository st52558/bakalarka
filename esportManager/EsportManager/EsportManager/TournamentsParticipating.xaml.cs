﻿using System;
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
        class TournamentBasic
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public string StartDate { get; set; }
            public string EndDate { get; set; }
            public int PrizePool { get; set; }
            public string City { get; set; }
            public int TokenValue { get; set; }
        }

        string databaseName;
        int teamId;
        string date;

        List<TournamentBasic> tournamentsForSection1 = new List<TournamentBasic>();
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
            SetAllLists();
        }

        private void SetAllLists()
        {
            tournamentsForSection1.Clear();
            int index = 13;
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select id_tournament_to, tournament.name, tournament.start_date, tournament.end_date, prize_pool, mesto.nazev, token_value from tournament_token join tournament on tournament.id_tournament=tournament_token.id_tournament_to join mesto on tournament.city_fk=id_mesto where id_teamxsection=" + index + ";", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    tournamentsForSection1.Add(new TournamentBasic() { ID = reader.GetInt32(0), Name = reader.GetString(1), StartDate = reader.GetString(2), EndDate = reader.GetString(3), PrizePool = reader.GetInt32(4), City = reader.GetString(5), TokenValue= reader.GetInt32(6) });
                }
                reader.Close();
            }
            AddAllListsToTournamentListBoxes();
        }

        private void AddAllListsToTournamentListBoxes()
        {
            TournamentList.Items.Clear();
            for (int i = 0; i < tournamentsForSection1.Count; i++)
            {
                TournamentList.Items.Add(tournamentsForSection1.ElementAt(i).Name + ", " + tournamentsForSection1.ElementAt(i).StartDate + " - " + tournamentsForSection1.ElementAt(i).EndDate + ", " + tournamentsForSection1.ElementAt(i).PrizePool + "$, " + tournamentsForSection1.ElementAt(i).City);
            }
        }

        private void CancelChosenTournament(object sender, RoutedEventArgs e)
        {
            if (TournamentList.SelectedIndex < 0)
            {
                return;
            }
            MessageBoxResult result = MessageBox.Show("Vážně se chcete odhlásit z turnaje? Bude Vás to stát " + tournamentsForSection1.ElementAt(TournamentList.SelectedIndex).TokenValue + "$.", "Chystáte se odhlásit z turnaje", MessageBoxButton.YesNo);
            if (result != MessageBoxResult.Yes)
            {
                return;
            }
            
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("update team set budget=budget-" + tournamentsForSection1.ElementAt(TournamentList.SelectedIndex).TokenValue + " where id_team=" + teamId + ";", conn);
                command.ExecuteReader();
                command = new SQLiteCommand("delete from tournament_token where id_tournament_to=" + tournamentsForSection1.ElementAt(TournamentList.SelectedIndex).ID +" and id_teamxsection=14;", conn);
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
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select team.name, tournament.name from tournament_token join tournament on tournament.id_tournament=tournament_token.id_tournament_to join teamxsection on teamxsection.id_teamxsection=tournament_token.id_teamxsection join team on teamxsection.id_team=team.id_team where tournament_token.id_tournament_to=" + tournamentsForSection1.ElementAt(TournamentList.SelectedIndex).ID + ";", conn);
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
            }
        }
    }
}
