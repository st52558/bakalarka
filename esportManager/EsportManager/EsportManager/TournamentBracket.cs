using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace EsportManager
{
    class TournamentBracket
    {
        internal class BracketObject
        {
            public int Id { get; set; }
            public TournamentTeam HomeTeam { get; set; }
            public TournamentTeam AwayTeam { get; set; }
            public BracketObject NextMatch { get; set; }
            public int Round { get; set; }

            public BracketObject(int idI, TournamentTeam home, TournamentTeam away, int round)
            {
                Id = idI;
                HomeTeam = home;
                AwayTeam = away;
                NextMatch = null;
                Round = round;
            }
        }
        string databaseName;
        int tournament;
        List<TournamentTeam> teams;
        public List<BracketObject> firstRound { get; set; }
        public List<BracketObject> allMatches { get; set; }

        public TournamentBracket(string databasenameI, int tournamentI)
        {
            teams = new List<TournamentTeam>();
            firstRound = new List<BracketObject>();
            allMatches = new List<BracketObject>();
            databaseName = databasenameI;
            tournament = tournamentI;
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select tournament_token.id_teamxsection,team.name,team.id_team from tournament_token join teamxsection on teamxsection.id_teamxsection=tournament_token.id_teamxsection join team on team.id_team=teamxsection.id_team where id_tournament_to=" + tournament + ";", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    teams.Add(new TournamentTeam(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2)));
                }
                reader.Close();
            }
            CreateFirstRound();
            CreateOtherMatches();
        }

        private void CreateOtherMatches()
        {
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select id_match, id_home, id_away, type_home, type_away from '2019future_match1' where id_tournament=" + tournament + ";", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                TournamentTeam home;
                TournamentTeam away;
                while (reader.Read())
                {
                    home = null;
                    away = null;
                    if (reader.GetInt32(3) == 1)
                    {
                        for (int i = 0; i < teams.Count; i++)
                        {
                            if (teams.ElementAt(i).IdTeamSection == reader.GetInt32(1))
                            {
                                home = teams.ElementAt(i);
                            }
                        }
                    }
                    if (reader.GetInt32(4) == 1)
                    {
                        for (int i = 0; i < teams.Count; i++)
                        {
                            if (teams.ElementAt(i).IdTeamSection == reader.GetInt32(2))
                            {
                                away = teams.ElementAt(i);
                            }
                        }
                    }
                    if (away != null && home != null)
                    {
                        allMatches.Add(new BracketObject(reader.GetInt32(0), home, away, 2));
                    }
                    else
                    {
                        BracketObject b = new BracketObject(reader.GetInt32(0), home, away, 0);
                        if (reader.GetInt32(3) >= 2)
                        {
                            for (int i = 0; i < allMatches.Count; i++)
                            {
                                if (allMatches.ElementAt(i).Id == reader.GetInt32(1))
                                {
                                    allMatches.ElementAt(i).NextMatch = b;
                                    b.Round = allMatches.ElementAt(i).Round + 1;
                                }
                            }
                        }
                        if (reader.GetInt32(4) >= 2)
                        {
                            for (int i = 0; i < allMatches.Count; i++)
                            {
                                if (allMatches.ElementAt(i).Id == reader.GetInt32(2))
                                {
                                    allMatches.ElementAt(i).NextMatch = b;
                                    b.Round = allMatches.ElementAt(i).Round + 1;
                                }
                            }
                        }
                        allMatches.Add(b);
                    }
                }
            }
        }

        private void CreateFirstRound()
        {
            // výpočet kolik západů je první kolo
            int a = 2;
            while (a < teams.Count)
            {
                a *= 2;
            }

            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select id_match,id_teamxsection_home, id_teamxsection_away from '2019match1' where id_tournament=" + tournament + ";", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                TournamentTeam home = null;
                TournamentTeam away = null;
                while (reader.Read())
                {
                    for (int i = 0; i < teams.Count; i++)
                    {
                        if (teams.ElementAt(i).IdTeamSection == reader.GetInt32(1))
                        {
                            home = teams.ElementAt(i);
                        }
                        if (teams.ElementAt(i).IdTeamSection == reader.GetInt32(2))
                        {
                            away = teams.ElementAt(i);
                        }
                    }
                    firstRound.Add(new BracketObject(reader.GetInt32(0), home, away, 1));
                    allMatches.Add(firstRound.Last());
                }
                reader.Close();
            }
        }

        /*int tournament;
        int prizePool;
        int ppTeams;
        int ppDividing;*/
    }
}