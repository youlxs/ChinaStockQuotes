using System;
using ChinaStockQuotes.Core.Handler;
using ChinaStockQuotes.Entity;
using log4net;
using NUnit.Framework;
using System.Collections.Generic;
using TinyIoC;

namespace ChinaStockQuotes.UnitTest
{
    [TestFixture]
    public class MarketStockQuotesTest : TestsBase
    {
        public static readonly TinyIoCContainer Container = TinyIoCContainer.Current;

        [Test]
        public void SaveDataIntoDataBase()
        {
            Initialize();

            ////var marketStockQuotes = new ChinaStockQuotes.Entity.MarketStockQuotes
            ////{
            ////    High = 1.0f,
            ////    AskVol = 100,
            ////    BestAsk = 2.0f,
            ////    BestBid = 2.0f,
            ////    BidVol = 100,
            ////    Code = "sh",
            ////    HighLimited = 3.0f,
            ////    Low = 0.5f,
            ////    LowLimited = 0.4f,
            ////    Market = "sz",
            ////    Time    = DateTime.Now
            ////};

            ////var marketStockQuotes1 = new ChinaStockQuotes.Entity.MarketStockQuotes
            ////{
            ////    High = 1.0f,
            ////    AskVol = 100,
            ////    BestAsk = 2.0f,
            ////    BestBid = 2.0f,
            ////    BidVol = 100,
            ////    Code = "sh",
            ////    HighLimited = 3.0f,
            ////    Low = 0.5f,
            ////    LowLimited = 0.4f,
            ////    Market = "sz",
            ////    Time = DateTime.Now
            ////};

            ////var data = new List<MarketStockQuotes> {marketStockQuotes, marketStockQuotes1};

            ////var handler = new MarketDataHandler(new ChinaStockQuotes.Persistance.MarketStockQuotesRepository(), Mocks.StrictMock<ILog>());

            ////handler.Handle(marketStockQuotes);
            ////handler.Handle(marketStockQuotes1);
        }

        private void Initialize()
        { 
            LoggerRegistration();
        }

        private void LoggerRegistration()
        {
            log4net.Config.XmlConfigurator.Configure();
            Container.Register<ILog>(LogManager.GetLogger("DefaultLogFile"));
        }
    }
}
