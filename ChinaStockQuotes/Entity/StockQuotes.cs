using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChinaStockQuotes.Entity
{
    public class StockQuotes
    {
        private float averageWeight;
        private float average;

        public string Name { get; set; }

        public string Code { get; set; }

        public float Opening { get; set; }

        public float Closing { get; set; }

        public float High { get; set; }

        public float Low { get; set; }

        public float Average
        {
            get { return this.average; }
            set { this.average = (float)Math.Round(value, 2); }
        }

        public float AverageWeight
        {
            get
            {
                return this.averageWeight;
            }
            set
            {
                this.averageWeight = (float)Math.Round(value, 2);
            }
        }

        public long Volume { get; set; }

        public long TotalVolume { get; set; }

        public float BestBid { get; set; }

        public uint BidSize { get; set; }

        public float BestAsk { get; set; }

        public uint AskSize { get; set; }

        public string Market { get; set; }

        public DateTime Time { get; set; }

        public bool IsBesBidOrAskChanged { get; set; }

        public StockQuotes CopyStockQuotes()
        {
            return new StockQuotes
            {
                Code = this.Code,
                Name = this.Name,
                Opening = this.Opening,
                Closing = this.Closing,
                High = this.High,
                Low = this.Low,
                Average = this.Average,
                AverageWeight = this.AverageWeight,
                Volume = this.Volume,
                TotalVolume = this.TotalVolume,
                BestBid = this.BestBid,
                BidSize = this.BidSize,
                BestAsk = this.BestAsk,
                AskSize = this.AskSize,
                Market = this.Market,
                Time = this.Time
            };
        }
    }
}
