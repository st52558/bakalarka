using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EsportManager
{
    class Player
    {
        public int IdPlayer { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Nick { get; set; }
        public int IndiSkill { get; set; }
        public int TeamSkill { get; set; }
        public int IndiPotencial { get; set; }
        public int TeamPotencial { get; set; }
        public int PlayerCoop { get; set; }
        public int Position { get; set; }
        public string PositionName { get; set; }
        public int Energy { get; set; }
        public int IdTeamSection { get; set; }
        public bool Chosen { get; set; }
        public string SectionName { get; set; }
        public int IdSection { get; set; }
        public string TeamName { get; set; }
        public int IdTeam { get; set; }
        public int Value { get; set; }
        public int Salary { get; set; }
        public string ContractEnd { get; set; }


        // pro zápas
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
        public Player()
        {

        }
        // pro detail hráče
        public Player(int id_player, string name, string nick, string surname, string teamName, string sectionName, string positionName, int salary, string contractEnd, int value, int idTeam, int individualSkill, int teamplaySkill, int teamSection, int section, int playercoop)
        {
            IdPlayer = id_player;
            Nick = nick;
            Name = name;
            Nick = nick;
            Surname = surname;
            TeamName = teamName;
            SectionName = sectionName;
            PositionName = positionName;
            Salary = salary;
            ContractEnd = contractEnd;
            Value = value;
            IdTeam = idTeam;
            IndiSkill = individualSkill;
            TeamSkill = teamplaySkill;
            IdTeamSection = teamSection;
            IdSection = section;
            PlayerCoop = playercoop;
        }
    }
}
