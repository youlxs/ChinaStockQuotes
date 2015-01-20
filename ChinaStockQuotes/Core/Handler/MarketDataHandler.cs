using Amib.Threading;
using ChinaStockQuotes.Configuration;
using ChinaStockQuotes.Entity;
using ChinaStockQuotes.Persistance;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace ChinaStockQuotes.Core.Handler
{
    public class MarketDataHandler : IDataHandler<TDFData>
    {
        private readonly Dictionary<string, StockQuotes> htData = new Dictionary<string, StockQuotes>();
        private readonly int tradeInterval = ConfigurationData.TradeInterval;
        private readonly IRepository<MarketStockQuotes, string> marketStockQuotesRepository;
        private readonly ILog logger;
        private readonly SmartThreadPool manSmartThreadPool;
        private static readonly System.Threading.ReaderWriterLockSlim RWLock = new System.Threading.ReaderWriterLockSlim();
        private DateTime dtStart = ConfigurationData.MarketOpenTime;
        private static readonly object locker = new object();

        public MarketDataHandler(IRepository<MarketStockQuotes, string> marketStockQuotesRepository, ILog logger)
        {
            this.marketStockQuotesRepository = marketStockQuotesRepository;
            this.logger = logger;
            // Create a STPStartInfo object
            var stpStartInfo = new STPStartInfo();
            // Change the defaults of the STPStartInfo object
            stpStartInfo.IdleTimeout = 1000;
            stpStartInfo.MinWorkerThreads = 0;
            stpStartInfo.MaxWorkerThreads = ConfigurationData.MaxWorkingThreads;
            stpStartInfo.DisposeOfStateObjects = true;
            // Start listening clients to connect
            // Set up mail service
            // Create the SmartThreadPool instance
            manSmartThreadPool = new SmartThreadPool(stpStartInfo);
            manSmartThreadPool.Start();
        }

        public void Handle(TDFData data)
        {
            try
            {
                if (data is MarketStockQuotes)
                {
                    this.HandleMarketData((MarketStockQuotes)data);
                }
                else if (data is StockTransaction)
                {
                    this.HanldeTransactionData((StockTransaction)data);
                }
                else if (data is StockIndex)
                {
                    this.HanldeIndexData((StockIndex)data);
                }
                else if (data is HeartBeatData)
                {
                    this.HanldeHeartBeatData();
                }

            }
            catch (Exception expException)
            {
                this.logger.Error(expException);
            }

        }

        private void HanldeHeartBeatData()
        {
            var dtNow = DateTime.Now;

            if (dtNow.Subtract(this.dtStart).TotalSeconds >= tradeInterval && htData.Count > 0)
            {
                IEnumerable<StockQuotes> list = null;

                var dataList = new List<StockQuotes>(htData.Values.Where(v => v.Closing > 0.0f && v.TotalVolume > 0 && !float.IsNaN(v.AverageWeight)));

                if (tradeInterval <= 1)
                {
                    dataList = dataList.Where(v => v.Volume > 0 || v.IsBesBidOrAskChanged).ToList();
                }
                else
                {
                    if (dtNow >= new DateTime(dtNow.Year, dtNow.Month, dtNow.Day, 11, 30, 0) &&
                        dtNow <= new DateTime(dtNow.Year, dtNow.Month, dtNow.Day, 13, 0, 0))
                    {
                        return;
                    }
                }

                if (dataList.Count > 0)
                {
                    dataList.ForEach(d => d.Time = DateTime.Now);
                    list = dataList.Select(d => d.CopyStockQuotes()).ToList();
                    dataList.ForEach(d =>
                    {
                        d.Volume = 0;
                        d.Opening = d.Closing;
                        d.High = d.Closing;
                        d.Low = d.Closing;
                        d.Average = d.Closing;
                        d.IsBesBidOrAskChanged = false;
                    });

                    this.manSmartThreadPool.QueueWorkItem(new WorkItemCallback(SaveData), list);

                    this.dtStart = DateTime.Now;
                }
            }
        }

        private void HandleMarketData(MarketStockQuotes marketData)
        {
            if (marketData.AskVol > uint.MaxValue || marketData.BidVol > uint.MaxValue)
            {
                return;
            }

            if (htData.ContainsKey(marketData.Code))
            {
                if (marketData.BestAsk > 0 && marketData.AskVol > 0)
                {
                    if (htData[marketData.Code].BestAsk != marketData.BestAsk)
                    {
                        htData[marketData.Code].IsBesBidOrAskChanged = true;
                    }

                    htData[marketData.Code].BestAsk = marketData.BestAsk;
                    htData[marketData.Code].AskSize = marketData.AskVol;
                }

                if (marketData.BestBid > 0 && marketData.BidVol > 0)
                {
                    if (htData[marketData.Code].BestBid != marketData.BestBid)
                    {
                        htData[marketData.Code].IsBesBidOrAskChanged = true;
                    }

                    htData[marketData.Code].BestBid = marketData.BestBid;
                    htData[marketData.Code].BidSize = marketData.BidVol;
                }

                htData[marketData.Code].TotalVolume = marketData.TotalVolume;
            }
            else
            {
                htData[marketData.Code] = new StockQuotes
                {
                    Average = 0,
                    AverageWeight = 0,
                    Opening = 0.0f,
                    Closing = 0.0f,
                    High = 0.0f,
                    Low = 0.0f,
                    Volume = 0,
                    TotalVolume = marketData.TotalVolume,
                    AskSize = marketData.AskVol,
                    BestAsk = marketData.BestAsk,
                    BestBid = marketData.BestBid,
                    BidSize = marketData.BidVol,
                    Code = marketData.Code,
                    Name = marketData.Name,
                    Market = marketData.Market
                };
            }
        }

        private void HanldeTransactionData(StockTransaction transactionData)
        {
            var price = transactionData.Price;
            var lastVolume = transactionData.Volume;
            var code = transactionData.Code;
            var market = transactionData.Market;

            if (price == 0.0f)
                return;

            if (htData.ContainsKey(code))
            {
                var stockQuotes = htData[code];
                var high = stockQuotes.High;
                var low = stockQuotes.Low;
                var opening = stockQuotes.Opening;
                var volume = stockQuotes.Volume;
                var totalVolume = stockQuotes.TotalVolume;
                var average = stockQuotes.Average;
                var average_wt = stockQuotes.Average;

                if (opening < 0.01)
                    stockQuotes.Opening = price;

                stockQuotes.Closing = price;
                stockQuotes.High = (price >= high) ? price : high;
                stockQuotes.Low = (price <= low || low == 0.0f) ? price : low;
                stockQuotes.Average = (average > 0) ? (average + price) / 2.0f : price;
                stockQuotes.AverageWeight = (float)Math.Round((average_wt * totalVolume + price * lastVolume) / (totalVolume + lastVolume), 2);
                stockQuotes.Volume = volume + lastVolume;
                stockQuotes.Market = transactionData.Market;
                ////stockQuotes.TotalVolume = stockQuotes.TotalVolume + stockQuotes.Volume;
                if (volume + lastVolume > totalVolume)
                    stockQuotes.Volume = totalVolume;

            }
            else
            {
                htData[code] = new StockQuotes
                {
                    Average = price,
                    AverageWeight = price,
                    Opening = price,
                    Closing = price,
                    High = price,
                    Low = price,
                    Volume = lastVolume,
                    TotalVolume = lastVolume,
                    AskSize = 1,
                    BestAsk = price,
                    BestBid = price,
                    BidSize = 1,
                    Code = code,
                    Market = market,
                    Name = transactionData.Name
                };
            }
        }

        private void HanldeIndexData(StockIndex stockIndex)
        {
            var price = stockIndex.LastIndex;
            var code = stockIndex.Code;
            var market = stockIndex.Market;
            var totalVolume = stockIndex.TotalVolume;

            if (price == 0.0f)
                return;

            if (htData.ContainsKey(code))
            {
                var stockQuotes = htData[code];
                var high = stockQuotes.High;
                var low = stockQuotes.Low;
                var opening = stockQuotes.Opening;
                var volume = stockQuotes.Volume;
                var average = stockQuotes.Average; ;
                var lastVolume = totalVolume - stockQuotes.TotalVolume;

                if (opening < 0.01)
                    stockQuotes.Opening = price;

                stockQuotes.Closing = price;
                stockQuotes.High = (price >= high) ? price : high;
                stockQuotes.Low = (price <= low || low == 0.0f) ? price : low;
                stockQuotes.Average = (average > 0) ? (average + price) / 2.0f : price;
                stockQuotes.AverageWeight = stockQuotes.Average;
                stockQuotes.Volume = stockQuotes.Volume + lastVolume;
                stockQuotes.Market = market;
                stockQuotes.TotalVolume = totalVolume;
                if (volume + lastVolume > totalVolume)
                    stockQuotes.Volume = totalVolume;

            }
            else
            {
                htData[code] = new StockQuotes
                {
                    Average = price,
                    AverageWeight = price,
                    Opening = price,
                    Closing = price,
                    High = price,
                    Low = price,
                    Volume = 0,
                    TotalVolume = totalVolume,
                    AskSize = 1,
                    BestAsk = price,
                    BestBid = price,
                    BidSize = 1,
                    Code = code,
                    Market = market,
                    Name = stockIndex.Name
                };
            }

        }

        private object SaveData(object list)
        {
            try
            {
                lock (locker)
                {
                    this.marketStockQuotesRepository.GetDatabese().BulkInsertRecords((List<StockQuotes>)list, ConfigurationData.TradeTable, false);
                }
            }
            catch (Exception exp)
            {
                this.logger.Error(exp);
            }

            return true;
        }
    }
}
