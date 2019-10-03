using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamEdit
{
    class TeamSection
    {
        public int ID { get; set; }
        public string Section { get; set; }
        public int IdTeam { get; set; }

        public TeamSection(int id, string s, int idT)
        {
            ID = id;
            Section = s;
            IdTeam = idT;
        }
    }
}
