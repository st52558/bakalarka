using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramEdit
{
    class Section
    {
        public int IdSection { get; set; }
        public string Name { get; set; }

        public Section(int id, string name)
        {
            IdSection = id;
            Name = name;
        }
    }
}
