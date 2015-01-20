using System;
using System.Configuration;
using System.IO;
using System.Text;
using log4net;
using System.Collections.Generic;
using HTTransfer.Core;
using System.Linq;

namespace HTTransfer.Configuration
{
    public static class ConfigurationData
    {
        private static readonly ILog logger = TinyIoC.TinyIoCContainer.Current.Resolve<ILog>();
        private static readonly Dictionary<string, ChinaStock> chinastockMap = new Dictionary<string, ChinaStock>();

        static ConfigurationData()
        {
           var all = File.ReadAllLines(ChinaStockPath);

            var codes = from x in all 
                        let items = x.Split(new char[] { '\t'}, StringSplitOptions.RemoveEmptyEntries)
                        select new ChinaStock { Name = items[0], Code = items[1]};

            chinastockMap = codes.ToDictionary(c => c.Code);
                        
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

                    return "192.168.2.4";
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

                    return "10000";
                }
            }
        }

        public static string UserId
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["UserId"];
                }
                catch (Exception exception)
                {
                    logger.Error(exception);

                    return "htzq_tdf45";
                }
            }
        }

        public static string Password
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["Password"];
                }
                catch (Exception exception)
                {
                    logger.Error(exception);

                    return "htzq_tdf45";
                }
            }
        }

        /// <summary>
        ///  //市场列表，以 ; 分割，例如: sh;sz;cf;shf;czc;dce
        /// </summary>
        public static string SubMarket
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["SubMarket"];
                }
                catch (Exception exception)
                {
                    logger.Error(exception);

                    return "sh;sz";
                }
            }
        }

        /// <summary>
        /// 连接ID，标识某个Open调用，跟回调消息中TDFMSG结构nConnectionID字段相同
        /// </summary>
        public static uint ConnectionId
        {
            get
            {
                return 10000;
            }
        }

        /// <summary>
        /// 请求的日期，格式YYMMDD，为0则请求今天
        /// </summary>
        public static uint RequestDate
        {
            get
            {
                try
                {
                    return unchecked((uint)Convert.ToInt32(ConfigurationManager.AppSettings["RequestDate"]));
                }
                catch (Exception exception)
                {
                    logger.Error(exception);

                    return 0;
                }
                
            }
        }

        /// <summary>
        /// 请求的时间，格式HHMMSS，为0则请求实时行情，为(uint)-1从头请求
        /// </summary>
        public static uint RequestTime
        {
            get
            {
                try
                {
                    return unchecked((uint)Convert.ToInt32(ConfigurationManager.AppSettings["RequestTime"]));
                }
                catch (Exception exception)
                {
                    logger.Error(exception);

                    return 0;
                }

            }
        }

        public static string SubCodes
        {
            get
            {
                try
                {
                    var codes = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "\\Code.txt");

                    return string.Join(";", codes);
                }
                catch (Exception exp)
                {
                    throw exp;
                }
            }
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

        public static string RzrqStockPath
        {
            get
            {
                try
                {
                    return AppDomain.CurrentDomain.BaseDirectory + "\\RzrqCode.txt";
                }
                catch (Exception exp)
                {
                    throw exp;
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

        public static int ListenPort
        {
            get
            {
                try
                {
                    return Int32.Parse(ConfigurationManager.AppSettings["ListenPort"]);
                }
                catch (Exception exp)
                {
                    logger.Error(exp);

                    return 10001;
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
            get { return Encoding.Default; }
        }

        public static int NumOfMaxConnections
        {
            get { return 20; }
        }

        public static readonly float Denominator = 10000f;
    }
}
