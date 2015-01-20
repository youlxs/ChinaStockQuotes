using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChinaStockQuotes.Entity
{
    [Serializable]
    public class MarketStockQuotes : TDFData
    {
        public float PreClose { get; set; } 			//前收盘价
        public float Open { get; set; } 				//开盘价
        public float High { get; set; } 				//最高价
        public float Low { get; set; } 				//最低价
        public float Match { get; set; } 				//最新价
        public float BestAsk { get; set; } 			//申卖价

        public uint AskVol { get; set; } 			//申卖量

        public float BestBid { get; set; } 			//申买价

        public uint BidVol { get; set; } 			//申买量

        
        public long TotalVolume { get; set; } 				//成交总量


        public long Volume { get; set; } 				//当前成交总量
    
        public long TotalBidVol { get; set; } 			//委托买入总量
        public long TotalAskVol { get; set; } 			//委托卖出总量
        public float WeightedAvgBidPrice { get; set; } 	//加权平均委买价格
        public float WeightedAvgAskPrice { get; set; }   //加权平均委卖价格
         
      
        public float HighLimited { get; set; } 			//涨停价
        public float LowLimited { get; set; } 			//跌停价
 
        public float TodayClose { get; set; }

        public override string ToString()
        {
            var properties = this.GetType().GetProperties();

            var sb = new StringBuilder();

            foreach (var property in properties)
            {
                sb.Append(string.Format("{0}: {1}, ", property.Name, property.GetValue(this, null)));
            }

            return sb.ToString();
        }
    }
}
