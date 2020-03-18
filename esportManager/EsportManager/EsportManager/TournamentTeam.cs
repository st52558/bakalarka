using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EsportManager
{
    class TournamentTeam
    {
        public int IdTeamSection { get; set; }
        public string TeamName { get; set; }
        public int IdTeam { get; set; }
        public string TournamentTo { get; set; }
        public int Position { get; set; }
        public int MatchesPlayed { get; set; }
        public int MatchesWon { get; set; }
        public int MatchesLost { get; set; }
        public int MapsWon { get; set; }
        public int MapsLost { get; set; }
        public int PrizePool { get; set; }

        public TournamentTeam(int id, string name, int idt)
        {
            IdTeamSection = id;
            TeamName = name;
            IdTeam = idt;
            Position = 0;
            MatchesPlayed = 0;
            MatchesWon = 0;
            MatchesLost = 0;
            MapsWon = 0;
            MapsLost = 0;
            PrizePool = 0;
        }
    }
}
