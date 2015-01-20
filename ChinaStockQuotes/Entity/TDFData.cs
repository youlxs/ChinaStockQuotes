using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChinaStockQuotes.Entity
{
    public abstract class TDFData
    {
        public string Code { get; set; }      //600001.SH 

        public DateTime Time { get; set; } 				//时间(HHMMSSmmm)

        public string Name { get; set; }

        public string Market { get; set; }
    }
}
