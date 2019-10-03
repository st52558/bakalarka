using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamEdit
{
    class Section
    {
        public int IdSection { get; set; }
        public string Name { get; set; }

        public Section(int id, string name)
        {
            Name = name;
            IdSection = id;
        }
    }
}
