using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamEdit
{
    class TeamBasic
    {
        public int IdNation { get; set; }
        public string Name { get; set; }

        public TeamBasic(int id_nation, String name)
        {
            IdNation = id_nation;
            Name = name;
        }
    }
}
