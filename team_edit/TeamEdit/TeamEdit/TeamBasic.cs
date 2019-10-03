using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamEdit
{
    class TeamBasic
    {
        public int IdTeam { get; set; }
        public string Name { get; set; }

        public TeamBasic(int id_team,  String name)
        {
            IdTeam = id_team;
            Name = name;
        }
    }
}
