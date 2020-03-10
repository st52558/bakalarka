using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EsportManager
{
    class Player
    {
        public int IdPlayer;
        public string Name;
        public string Surname;
        public string Nick;
        public int IndiSkill;
        public int TeamSkill;
        public int PlayerCoop;
        public int Position;
        public string PositionName;
        public int Energy;
        public int IdTeamSection;
        public bool chosen;

        public Player(int id_player, string nick, int position, int playerCoop, int individualSkill, int teamplaySkill, int energy, string posName ,int teamxsection)
        {
            IdPlayer = id_player;
            Nick = nick;
            Position = position;
            PlayerCoop = playerCoop;
            IndiSkill = individualSkill;
            TeamSkill = teamplaySkill;
            Energy = energy;
            PositionName = posName;
            IdTeamSection = teamxsection;
        }                
    }
}
