using System;
using System.Collections.Generic;
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

        public MatchDetail()
        {
            Won = 0;
        }
    }
}
