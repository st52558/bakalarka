using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EsportManager
{
    class DailyTraining
    {
        public int FreeTime { get; set; }
        public int Scrims { get; set; }
        public int IndiTraining { get; set; }
        public int Analysis { get; set; }

        public DailyTraining()
        {
            FreeTime = 0;
            Scrims = 0;
            IndiTraining = 0;
            Analysis = 0;
        }
    }
}
