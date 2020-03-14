using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EsportManager
{
    class TournamentStandings
    {
        public List<TournamentTeam> standings { get; set; }
        string databaseName;
        int tournament;

        public TournamentStandings(string databasenameI, int tournamentI)
        {
            databaseName = databasenameI;
            tournament = tournamentI;
            standings = new List<TournamentTeam>();
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select tournament_token.id_teamxsection,team.name from tournament_token join teamxsection on teamxsection.id_teamxsection=tournament_token.id_teamxsection join team on team.id_team=teamxsection.id_team where id_tournament_to=" + tournament + ";", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    standings.Add(new TournamentTeam(reader.GetInt32(0), reader.GetString(1)));
                }
                reader.Close();
            }
        }

        public void CreateStandings()
        {
            
            standings = standings.OrderByDescending(o => o.MatchesWon).ToList();
            for (int i = 0; i < standings.Count; i++)
            {
                standings.ElementAt(i).Position = i + 1;
            }
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select tournament.shortcut, tournament_from_position, tournament.id_tournament from tournament_token join tournament on tournament_token.id_tournament_to=tournament.id_tournament where id_tournament_from=" + tournament + ";", conn);
                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    standings.ElementAt(reader.GetInt32(1) - 1).TournamentTo = reader.GetString(0);
                }
            }
        }

        public void SetPlayedMatch(int idHome, int idAway, int mapHome, int mapAway)
        {
            for (int j = 0; j < standings.Count; j++)
            {
                if (idHome == standings.ElementAt(j).IdTeamSection)
                {
                    if (mapHome > mapAway)
                    {
                        standings.ElementAt(j).MatchesWon++;
                    }
                    else
                    {
                        standings.ElementAt(j).MatchesLost++;
                    }
                    standings.ElementAt(j).MatchesPlayed++;
                    standings.ElementAt(j).MapsWon += mapHome;
                    standings.ElementAt(j).MapsLost += mapAway;
                }
                else if (idAway == standings.ElementAt(j).IdTeamSection)
                {
                    if (mapAway > mapHome)
                    {
                        standings.ElementAt(j).MatchesWon++;
                    }
                    else
                    {
                        standings.ElementAt(j).MatchesLost++;
                    }
                    standings.ElementAt(j).MatchesPlayed++;
                    standings.ElementAt(j).MapsWon += mapAway;
                    standings.ElementAt(j).MapsLost += mapHome;
                }
            }
        }
    }
}
