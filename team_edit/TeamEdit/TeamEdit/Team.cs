using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamEdit
{
    class Team
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Shortcut { get; set; }
        public double Money { get; set; }
        public string Nation { get; set; }
        public CityBasic Residency { get; set; }
        public string Manager { get; set; }
        public string Owner { get; set; }
        public int Reputation { get; set; }
        //public List<GameTeam> GameTeams { get; set; }
       // public List<Sponsor> Sponsors { get; set; }
        //public List<Employee> Employees { get; set; }
    }
}
