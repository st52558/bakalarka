using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamEdit
{
    class Nation
    {
        public int IdNation { get; set; }
        public string Name { get; set; }

        public Nation(int id_nation, String name)
        {
            IdNation = id_nation;
            Name = name;
        }
        
    }
}
