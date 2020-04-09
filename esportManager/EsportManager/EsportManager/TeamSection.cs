using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EsportManager
{
    class TeamSection
    {
        public int ID { get; set; }
        public int SectionID { get; set; }
        public string SectionName { get; set; }
        public string TeamName { get; set; }
        public int Wins { get; set; }
        public int Loses { get; set; }
        public int PowerRanking { get; set; }
        public int PowerPosition { get; set; }
        public int TeamID { get; set; }


        public TeamSection(int iD, int sectionID, string sectionName)
        {
            ID = iD;
            SectionID = sectionID;
            SectionName = sectionName;
        }
    }
}
