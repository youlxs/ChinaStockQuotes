using ChinaStockQuotes.Core.Handler;
using ChinaStockQuotes.Entity;
using ChinaStockQuotes.Persistance;
using log4net;
using System;
using System.Windows.Forms;
using TinyIoC;

namespace ChinaStockQuotes
{
    static class Program
    {
        public static readonly TinyIoCContainer Container = TinyIoCContainer.Current;

        static Program()
        {
            Initialize();
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (var mainform = new MainForm())
            {
                mainform.Start();
            }

            Console.WriteLine("The application is end, press enter to quit ...");

            Console.ReadLine();
        }

        static void Initialize()
        {
            Container.AutoRegister();

            LoggerRegistration();

            RepositoryRegistration();
        }

        static void LoggerRegistration()
        {
            log4net.Config.XmlConfigurator.Configure();
            Container.Register<ILog>(LogManager.GetLogger("DefaultLogFile"));
        }

        static void RepositoryRegistration()
        {
            Container.Register<IRepository<MarketStockQuotes, string>, MarketStockQuotesRepository>();
            Container.Register<IDataHandler<TDFData>, MarketDataHandler>();
            Container.Register<INetClientHandler, TDFNetClientHandler>();
        }
    }
}
