using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChinaStockQuotes.Entity
{
    public class StockIndex : TDFData
    {
        public float OpenIndex { get; set; }

        public float HightIndex { get; set; }

        public float LowIndex { get; set; }

        public float LastIndex { get; set; }

        public float PreCloseIndex { get; set; }

        public long TotalVolume { get; set; }

        public double Turnover { get; set; }

    }
}
