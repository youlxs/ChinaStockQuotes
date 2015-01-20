using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTInfrastructure.TDF;
using HTTransfer.Core;
using TDFAPI;

namespace HTTransfer.Extensions
{
    public static class TDFExtension
    {
        public static string ToNetworkString(this TDFMarketData marketData)
        {
            var sb = new StringBuilder();

            sb.Append("1009|");
            sb.Append(marketData.Code); //code
            sb.Append("\t");
            sb.Append(Configuration.ConfigurationData.GetChinaCodeName(marketData.Code)); //name
            sb.Append("\t");
            sb.Append(marketData.Match); //match
            sb.Append("\t");
            sb.Append(marketData.Open); //open
            sb.Append("\t");
            sb.Append(marketData.PreClose); //preclose
            sb.Append("\t");
            sb.Append(marketData.High); //high
            sb.Append("\t");
            sb.Append(marketData.Low); //low
            sb.Append("\t");
            sb.Append(marketData.NumTrades);
            sb.Append("\t");
            sb.Append(marketData.Volume);
            sb.Append("\t");
            sb.Append(marketData.Turnover);
            sb.Append("\t");
            sb.Append(marketData.HighLimited);
            sb.Append("\t");
            sb.Append(marketData.LowLimited);
            sb.Append("\t");

            foreach (var ask in marketData.AskPrice)
            {
                sb.Append(ask);
                sb.Append("\t");
            }

            foreach (var bid in marketData.BidPrice)
            {
                sb.Append(bid);
                sb.Append("\t");
            }

            foreach (var ask in marketData.AskVol)
            {
                sb.Append(ask);
                sb.Append("\t");
            }

            foreach (var bid in marketData.BidVol)
            {
                sb.Append(bid);
                sb.Append("\t");
            }

            sb.Append(marketData.Time);
            sb.Append("\t");
            sb.Append(marketData.WindCode.Substring(7));
            sb.Append("\t");
            sb.Append("^");

            return sb.ToString();
        }

        public static string ToNetworkString(this TDFTransaction transactionData)
        {
            const string template = "2001|{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t^";

            return string.Format(template, transactionData.Code, Configuration.ConfigurationData.GetChinaCodeName(transactionData.Code), transactionData.Price, transactionData.Time, transactionData.Volume, transactionData.WindCode.Substring(7));

        }

        public static string ToNetworkString(this TDFIndexData indexData)
        {
            const string template = "1100|{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t^";

            return string.Format(template, indexData.Code, indexData.HighIndex, indexData.LowIndex, indexData.OpenIndex, indexData.PreCloseIndex, indexData.LastIndex, indexData.TotalVolume, indexData.Turnover, indexData.Time, indexData.WindCode.Substring(7));
        }

        public static string ToNetworkString(this List<TDFMarketData> list)
        {
            var sb = new StringBuilder();

            list.ForEach(d => sb.Append(d.ToNetworkString()));

            return sb.ToString();
        }

        public static string ToNetworkString(this List<TDFTransaction> list)
        {
            var sb = new StringBuilder();

            list.ForEach(d => sb.Append(d.ToNetworkString()));

            return sb.ToString();
        }

        public static string ToNetworkString(this List<TDFIndexData> list)
        {
            var sb = new StringBuilder();

            list.ForEach(d => sb.Append(d.ToNetworkString()));

            return sb.ToString();
        }
    }
}
