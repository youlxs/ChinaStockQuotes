using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChinaStockQuotes.Core;
using log4net;
using System;
using System.Configuration;
using System.IO;

namespace ChinaStockQuotes.Configuration
{
    public class ConfigurationData
    {
        private static readonly ILog logger = TinyIoC.TinyIoCContainer.Current.Resolve<ILog>();

        private static readonly Dictionary<string, ChinaStock> chinastockMap = new Dictionary<string, ChinaStock>();

        static ConfigurationData()
        {
            var all = File.ReadAllLines(ChinaStockPath);

            var codes = from x in all
                        let items = x.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries)
                        select new ChinaStock { Name = items[0], Code = items[1] };

            chinastockMap = codes.ToDictionary(c => c.Code);

        }

        public static string ChinaStockPath
        {
            get
            {
                try
                {
                    return AppDomain.CurrentDomain.BaseDirectory + "\\ChinaStock.txt";
                }
                catch (Exception exp)
                {
                    throw exp;
                }
            }
        }


        public static string ServerIp
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["ServerIp"];
                }
                catch (Exception exception)
                {
                   logger.Error(exception);

                    return "127.0.0.1";
                }
            }
        }

        public static string ServerPort
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["ServerPort"];
                }
                catch (Exception exception)
                {
                    logger.Error(exception);

                    return "10001";
                }
            }
        }

        public static int MaxWorkingThreads
        {
            get
            {
                try
                {
                    return Int32.Parse(ConfigurationManager.AppSettings["MaxWorkingThreads"]);
                }
                catch (Exception exp)
                {
                    logger.Error(exp);

                    return 10;
                }
            }
        }

        public static int TradeInterval
        {
            get
            {
                try
                {
                    return Int32.Parse(ConfigurationManager.AppSettings["TradeInterval"]);
                }
                catch (Exception exp)
                {
                    logger.Error(exp);

                    return 10;
                }
            }
        }

        public static string TradeTableTick
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["TradeTickTableName"];
                }
                catch (Exception exp)
                {
                    logger.Error(exp);

                    return "dbo.stocktick";
                }
            }
        }

        public static string TradeTable
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["TradeTableName"];
                }
                catch (Exception exp)
                {
                    logger.Error(exp);

                    return "dbo.stock1m";
                }
            }
        }

        public static DateTime MarketOpenTime
        {
            get
            {
                try
                {
                    return DateTime.Parse(string.Format("{0} {1}", DateTime.Now.ToShortDateString(), ConfigurationManager.AppSettings["MarketOpenTime"]));
                }
                catch (Exception exp)
                {
                    logger.Error(exp);

                    return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 09, 0, 0);
                }
            }
        }

        public static DateTime MarketCloseTime
        {
            get
            {
                try
                {
                    return DateTime.Parse(string.Format("{0} {1}", DateTime.Now.ToShortDateString(), ConfigurationManager.AppSettings["MarketCloseTime"]));
                }
                catch (Exception exp)
                {
                    logger.Error(exp);

                    return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 16, 01, 0);
                }
            }
        }

        public static string GetChinaCodeName(string code)
        {
            if (chinastockMap.ContainsKey(code))
            {
                return chinastockMap[code].Name;
            }

            ////logger.Error(string.Format("Code {0} is missed.", code));

            return code;
        }

        public static Encoding GetDefaultEncoding
        {
            get { return Encoding.UTF8; }
        }


        public static readonly float Denominator = 10000f;
    }
}
