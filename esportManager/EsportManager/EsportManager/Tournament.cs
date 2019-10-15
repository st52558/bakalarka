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

        public Tournament(int iD, string shortcut, string dateFrom, string dateTo, int cancelFee)
        {
            ID = iD;
            Shortcut = shortcut;
            DateFrom = dateFrom;
            DateTo = dateTo;
            CancelFee = cancelFee;
        }
    }
}
