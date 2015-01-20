using ChinaStockQuotes.Configuration;
using ChinaStockQuotes.Entity;
using System;

namespace ChinaStockQuotes.Extensions
{
    public static class StringExtension
    {
        public static MarketStockQuotes ToTDFMarketData(this string input)
        {
            var items = input.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
 
            return new MarketStockQuotes
            {
                Code = items[0],
                Name = items[1],
                Match = Convert.ToUInt32(items[2]) / ConfigurationData.Denominator,
                Open = Convert.ToUInt32(items[3]) / ConfigurationData.Denominator,
                PreClose = Convert.ToUInt32(items[4]) / ConfigurationData.Denominator,
                High = Convert.ToUInt32(items[5]) / ConfigurationData.Denominator,
                Low = Convert.ToUInt32(items[6]) / ConfigurationData.Denominator,
                TotalVolume = Convert.ToInt64(items[8]),
                HighLimited = Convert.ToUInt32(items[10]) / ConfigurationData.Denominator,
                LowLimited = Convert.ToUInt32(items[11]) / ConfigurationData.Denominator,
                BidVol = Convert.ToUInt32(items[42]),
                BestBid = Convert.ToUInt32(items[22]) / ConfigurationData.Denominator,
                AskVol =  Convert.ToUInt32(items[32]),
                BestAsk = Convert.ToUInt32(items[12]) / ConfigurationData.Denominator,
                Market = items[53],
                Time = DateTime.Now

            };
        }

        public static StockTransaction ToTDFTransaction(this string input)
        {
            var items = input.Split(new char[] {'\t'}, StringSplitOptions.RemoveEmptyEntries);

            return new StockTransaction
            {
                Code = items[0],
                Name = items[1],
                Price = Convert.ToInt32(items[2])/ConfigurationData.Denominator,
                Volume = Convert.ToInt32(items[4]),
                Market = items[5]

            };
        }

        public static StockIndex ToTDFIndexData(this string input)
        {
            var items = input.Split(new char[] {'\t'}, StringSplitOptions.RemoveEmptyEntries);


            return new StockIndex
            {
                Code = items[0],
                Name = ConfigurationData.GetChinaCodeName(items[0]),
                HightIndex = Convert.ToInt32(items[1])/ConfigurationData.Denominator,
                LastIndex = Convert.ToInt32(items[5])/ConfigurationData.Denominator,
                LowIndex = Convert.ToInt32(items[2])/ConfigurationData.Denominator,
                OpenIndex = Convert.ToInt32(items[3])/ConfigurationData.Denominator,
                PreCloseIndex = Convert.ToInt32(items[4])/ConfigurationData.Denominator,
                Market = items[9],
                TotalVolume = Convert.ToInt64(items[6]),
                Turnover = Convert.ToInt64(items[7]),
                Time = DateTime.Now
            };
        }
    }
}
