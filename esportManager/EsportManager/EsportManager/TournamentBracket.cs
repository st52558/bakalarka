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
            public bool HomeTeamWinner { get; set; }

            public BracketObject(int idI, TournamentTeam home, TournamentTeam away)
            {
                Id = idI;
                HomeTeam = home;
                AwayTeam = away;
                NextMatch = null;
            }
        }
        string databaseName;
        int tournament;
        public List<TournamentTeam> Teams;
        public List<BracketObject> firstRound { get; set; }
        public List<BracketObject> allMatches { get; set; }
        public int PrizePool { get; set; }
        public int PpTeams { get; set; }
        public int PpDividing { get; set; }
        public int System { get; set; }

        public TournamentBracket(string databasenameI, int tournamentI, bool drawn, int system)
        {
            Teams = new List<TournamentTeam>();
            firstRound = new List<BracketObject>();
            allMatches = new List<BracketObject>();
            databaseName = databasenameI;
            tournament = tournamentI;
            System = system;
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select tournament_token.id_teamxsection,team.name,team.id_team from tournament_token join teamxsection on teamxsection.id_teamxsection=tournament_token.id_teamxsection join team on team.id_team=teamxsection.id_team where id_tournament_to=" + tournament + ";", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Teams.Add(new TournamentTeam(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2)));
                }
                reader.Close();
            }
            if (drawn)
            {
                CreateFirstRound();
                CreateOtherMatches();
            }
        }

        public TournamentBracket(string databasenameI, int tournamentI, int prizePool, int ppTeams, int ppDividing, int system)
        {
            Teams = new List<TournamentTeam>();
            firstRound = new List<BracketObject>();
            allMatches = new List<BracketObject>();
            databaseName = databasenameI;
            tournament = tournamentI;
            PrizePool = prizePool;
            PpTeams = ppTeams;
            PpDividing = ppDividing;
            System = system;
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select tournament_token.id_teamxsection,team.name,team.id_team from tournament_token join teamxsection on teamxsection.id_teamxsection=tournament_token.id_teamxsection join team on team.id_team=teamxsection.id_team where id_tournament_to=" + tournament + ";", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Teams.Add(new TournamentTeam(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2)));
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
                        for (int i = 0; i < Teams.Count; i++)
                        {
                            if (Teams.ElementAt(i).IdTeamSection == reader.GetInt32(1))
                            {
                                home = Teams.ElementAt(i);
                            }
                        }
                    }
                    if (reader.GetInt32(4) == 1)
                    {
                        for (int i = 0; i < Teams.Count; i++)
                        {
                            if (Teams.ElementAt(i).IdTeamSection == reader.GetInt32(2))
                            {
                                away = Teams.ElementAt(i);
                            }
                        }
                    }
                    if (away != null && home != null)
                    {
                        allMatches.Add(new BracketObject(reader.GetInt32(0), home, away));
                    }
                    else
                    {
                        BracketObject b = new BracketObject(reader.GetInt32(0), home, away);
                        if (reader.GetInt32(3) >= 2)
                        {
                            for (int i = 0; i < allMatches.Count; i++)
                            {
                                if (allMatches.ElementAt(i).Id == reader.GetInt32(1))
                                {
                                    allMatches.ElementAt(i).NextMatch = b;
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
                                }
                            }
                        }
                        allMatches.Add(b);
                    }
                }
            }
            //zjištění kola
            if (System == 3)
            {
                int a = 2;
                while (a < Teams.Count)
                {
                    a *= 2;
                }
                int firstRoundGames = Teams.Count - (a / 2);
                int usedGames = firstRoundGames;
                // a = počet zbylých kol do konce turnaje
                int nextRoundGames = a / 4;
                a = a / 2 - 1;
                int currRound = 1;
                for (int i = 0; i < allMatches.Count; i++)
                {
                    if (i < firstRoundGames)
                    {
                    }
                    else if (i - usedGames < nextRoundGames)
                    {

                        if (usedGames == i)
                        {
                            currRound++;
                        }
                    }
                    else
                    {
                        // změna kola
                        currRound++;
                        usedGames += nextRoundGames;
                        nextRoundGames /= 2;
                    }
                    allMatches.ElementAt(i).Round = currRound;
                }
            }
            else if (System == 4)
            {
                for (int i = 0; i < allMatches.Count; i++)
                {
                    allMatches.ElementAt(i).Round = i + 1;
                }
            }
        }

        private void CreateFirstRound()
        {
            // výpočet kolik západů je první kolo
            

            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select id_match,id_teamxsection_home, id_teamxsection_away from '2019match1' where id_tournament=" + tournament + ";", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                TournamentTeam home = null;
                TournamentTeam away = null;
                while (reader.Read())
                {
                    for (int i = 0; i < Teams.Count; i++)
                    {
                        if (Teams.ElementAt(i).IdTeamSection == reader.GetInt32(1))
                        {
                            home = Teams.ElementAt(i);
                        }
                        if (Teams.ElementAt(i).IdTeamSection == reader.GetInt32(2))
                        {
                            away = Teams.ElementAt(i);
                        }
                    }
                    firstRound.Add(new BracketObject(reader.GetInt32(0), home, away));
                    allMatches.Add(firstRound.Last());
                }
                reader.Close();
            }
        }

        public void CountStandings()
        {
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select id_match, home_score, away_score from '2019match1' where id_tournament=" + tournament + ";", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    for (int i = 0; i < allMatches.Count; i++)
                    {
                        if (allMatches.ElementAt(i).Id == reader.GetInt32(0))
                        {
                            allMatches.ElementAt(i).HomeTeamWinner = reader.GetInt32(1) > reader.GetInt32(2);
                            break;
                        }
                    }
                }
            }
            int positionCounter = 3;
            for (int i = 0; i < allMatches.Count; i++)
            {
                
                if (i == 0)
                {
                    if (allMatches.ElementAt(allMatches.Count - 1).HomeTeamWinner)
                    {
                        allMatches.ElementAt(allMatches.Count - 1).HomeTeam.Position = 1;
                        allMatches.ElementAt(allMatches.Count - 1).AwayTeam.Position = 2;
                    } else
                    {
                        allMatches.ElementAt(allMatches.Count - 1).HomeTeam.Position = 2;
                        allMatches.ElementAt(allMatches.Count - 1).AwayTeam.Position = 1;
                    }
                } 
                else
                {
                    if (allMatches.ElementAt(allMatches.Count - 1 - i).HomeTeamWinner)
                    {
                        allMatches.ElementAt(allMatches.Count - 1 - i).AwayTeam.Position = positionCounter;
                    }
                    else
                    {
                        allMatches.ElementAt(allMatches.Count - 1 - i).HomeTeam.Position = positionCounter;
                    }
                    positionCounter++;
                }
            }
            Teams = Teams.OrderByDescending(o => o.Position).ToList();

        }

        /*int tournament;
        int prizePool;
        int ppTeams;
        int ppDividing;*/
    }
}