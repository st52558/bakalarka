using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EsportManager
{
    class MatchDetail
    {
        public int IdMatch { get; set; }
        public string Date { get; set; }
        public int IdTxSHome { get; set; }
        public int IdTxSAway { get; set; }
        public string HomeShortcut { get; set; }
        public string AwayShortcut { get; set; }
        public int IdCity { get; set; }
        public string City { get; set; }
        public int IdTournament { get; set; }
        public string Tournament { get; set; }
        public string OpponentShortcut { get; set; }
        public int Won { get; set; } //0-NotPlayed 1-Won, 2-Lost
        public int IdGame { get; set; }
        string databaseName;
        int idWinner;
        string curDate;
        public int HomeScore { get; set; }
        public int AwayScore { get; set; }
        public int HomePowerRanking { get; set; }
        public int AwayPowerRanking { get; set; }
        Random random;

        public MatchDetail(string databaseNameI, int idSectionI, string date)
        {
            Won = 0;
            IdGame = idSectionI;
            databaseName = databaseNameI;
            curDate = date;
            random = new Random();
        }

        public void UpdateMatches()
        {
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
            {
                int idLoser;
                if (HomeScore > AwayScore)
                {
                    idWinner = IdTxSHome;
                    idLoser = IdTxSAway;
                    HomePowerRanking++;
                    AwayPowerRanking--;
                    if (HomePowerRanking < AwayPowerRanking)
                    {
                        HomePowerRanking++;
                        AwayPowerRanking--;
                    }
                } 
                else
                {
                    idWinner = IdTxSAway;
                    idLoser = IdTxSHome;
                    HomePowerRanking--;
                    AwayPowerRanking++;
                    if (HomePowerRanking > AwayPowerRanking)
                    {
                        HomePowerRanking--;
                        AwayPowerRanking++;
                    }
                }
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("update player set playerCoop=playerCoop+" + random.Next(-1,2) + ", energy=energy-2 where team_fk=" + idLoser + "; update player set playerCoop=playerCoop+" + random.Next(0, 3) + ", energy=energy-2 where team_fk=" + idWinner + ";", conn);
                command.ExecuteReader();
                command = new SQLiteCommand("update teamxsection set power_ranking=" + HomePowerRanking + " where id_teamxsection=" + IdTxSHome + "; update teamxsection set power_ranking=" + AwayPowerRanking + " where id_teamxsection=" + IdTxSAway + ";", conn);
                command.ExecuteReader();
                int year = int.Parse(curDate.Substring(0, 4));
                command = new SQLiteCommand("update '" + year + "match" + IdGame + "' set home_score=" + HomeScore + ", away_score=" + AwayScore + " where id_match=" + IdMatch + ";", conn);
                command.ExecuteReader();
                command = new SQLiteCommand("select id_home,id_away,type_home,type_away from '" + year + "future_match" + IdGame + "' where (id_home=" + IdMatch + " and type_home=1) or (id_away=" + IdMatch + " and type_away=1);", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    SQLiteCommand command2;
                    if (reader.GetInt32(0) == IdMatch && reader.GetInt32(2) == 1)
                    {
                        command2 = new SQLiteCommand("update '" + year + "future_match" + IdGame + "' set id_home=" + idWinner + ", type_home=2 where id_home=" + IdMatch + ";", conn);
                        command2.ExecuteReader();
                    }
                    else if (reader.GetInt32(1) == IdMatch && reader.GetInt32(3) == 1)
                    {
                        command2 = new SQLiteCommand("update '" + year + "future_match" + IdGame + "' set id_away=" + idWinner + ", type_away=2 where id_away=" + IdMatch + ";", conn);
                        command2.ExecuteReader();
                    }
                }
                reader.Close();
                command = new SQLiteCommand("select id_home,id_away,date,id_tournament,id_match from '" + year + "future_match" + IdGame + "' where type_home=2 and type_away=2;", conn);
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    SQLiteCommand command2 = new SQLiteCommand("insert into '" + year + "match" + IdGame + "' ('id_teamxsection_home', 'id_teamxsection_away', 'match_date', 'id_tournament') values (" + reader.GetInt32(0) + ", " + reader.GetInt32(1) + ", '" + reader.GetString(2) + "', " + reader.GetInt32(3) + ");", conn);
                    command2.ExecuteReader();
                    command2 = new SQLiteCommand("select last_insert_rowid()", conn);
                    SQLiteDataReader reader2 = command2.ExecuteReader();
                    reader2.Read();
                    int idNewMatch = reader2.GetInt32(0);
                    reader2.Close();
                    command2 = new SQLiteCommand("update '" + year + "future_match" + IdGame + "' set id_home=" + idNewMatch + ", type_home=1 where id_home=" + reader.GetInt32(4) + " and type_home=3;", conn);
                    command2.ExecuteReader();
                    command2 = new SQLiteCommand("update '" + year + "future_match" + IdGame + "' set id_away=" + idNewMatch + ", type_away=1 where id_away=" + reader.GetInt32(4) + " and type_away=3;", conn);
                    command2.ExecuteReader();
                    command2 = new SQLiteCommand("delete from '" + year + "future_match" + IdGame + "' where id_match=" + reader.GetInt32(4), conn);
                    command2.ExecuteReader();
                }
            }
        }
    }
}
