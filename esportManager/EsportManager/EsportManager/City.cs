using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EsportManager
{
    class City
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public City (int id, string name)
        {
            ID = id;
            Name = name;
        }
    }
}
