using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EsportManager
{
    class TeamSectionBasic
    {
        public int ID { get; set; }
        public int sectionID { get; set; }
        public string sectionName { get; set; }

        public TeamSectionBasic(int iD, int sectionID, string sectionName)
        {
            ID = iD;
            this.sectionID = sectionID;
            this.sectionName = sectionName;
        }
    }
}
