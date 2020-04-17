using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramEdit
{
    class Coach
    {
        public int IdCoach { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Nick { get; set; }
        public string ContractEnd { get; set; }
        public int Training { get; set; }
        public int IdSection { get; set; }
        public string SectionName { get; set; }
        public int IdTeam { get; set; }
        public string TeamName { get; set; }

        public Coach(int id, string name, string surname, string nick, string contarct, int training, int idSection, string secName, int idTeam, string teamName)
        {
            IdCoach = id;
            Name = name;
            Surname = surname;
            Nick = nick;
            ContractEnd = contarct;
            Training = training;
            IdSection = idSection;
            SectionName = secName;
            IdTeam = idTeam;
            TeamName = teamName;
        }
    }
}
