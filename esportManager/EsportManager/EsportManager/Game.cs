using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EsportManager
{
    class Game
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public Game(int id, string name)
        {
            ID = id;
            Name = name;
        }
    }
}
