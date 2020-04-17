using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramEdit
{
    class Team
    {
        public int IdTeam { get; set; }
        public string Name { get; set; }

        public Team(int id, string name)
        {
            IdTeam = id;
            Name = name;
        }
    }
}
