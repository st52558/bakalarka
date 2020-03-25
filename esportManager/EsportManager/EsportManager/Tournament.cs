using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EsportManager
{
    class Tournament
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Shortcut { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public int CancelFee { get; set; }
        public int PrizePool { get; set; }
        public string City { get; set; }
        public int TokenValue { get; set; }
        public bool Drawn { get; set; }
        public int IdSection { get; set; }
        public string SectionName { get; set; }

        public Tournament(int iD, string shortcut, string dateFrom, string dateTo, int cancelFee)
        {
            ID = iD;
            Shortcut = shortcut;
            DateFrom = dateFrom;
            DateTo = dateTo;
            CancelFee = cancelFee;
        }

        public Tournament(int iD, string name, string dateFrom, string dateTo, int prizePool, string city, int tokenValue, bool drawn)
        {
            ID = iD;
            Name = name;
            DateFrom = dateFrom;
            DateTo = dateTo;
            PrizePool = prizePool;
            City = city;
            TokenValue = tokenValue;
            Drawn = drawn;
        }

        public Tournament()
        {
                
        }
    }
}
