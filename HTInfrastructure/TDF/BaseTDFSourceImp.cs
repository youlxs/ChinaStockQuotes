using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using TDFAPI;

namespace HTInfrastructure.TDF
{
    public abstract class BaseTDFSourceImp : TDFDataSource
    {
        private static readonly ILog logger = TinyIoC.TinyIoCContainer.Current.Resolve<ILog>();

        protected BaseTDFSourceImp(TDFOpenSetting openSetting)
            : base(openSetting)
        {
            this.ShowAllData = false;
        }

        ~BaseTDFSourceImp()
        {
            base.Dispose(false);
        }

        public bool ShowAllData { get; set; }

        //重载 OnRecvSysMsg 方法，接收系统消息通知
        public override void OnRecvSysMsg(TDFMSG msg)
        {
            if (msg.MsgID == TDFMSGID.MSG_SYS_CONNECT_RESULT)
            {
                //连接结果
                TDFConnectResult connectResult = msg.Data as TDFConnectResult;
                string strPrefix = connectResult.ConnResult ? "连接成功" : "连接失败";

                string output = string.Format("{0}！server:{1}:{2},{3},{4}, connect id:{5}", strPrefix, connectResult.Ip, connectResult.Port, connectResult.Username, connectResult.Password, connectResult.ConnectID);
                Console.WriteLine(output);
                logger.Debug(output);
            }
            else if (msg.MsgID == TDFMSGID.MSG_SYS_LOGIN_RESULT)
            {
                TDFLoginResult loginResult = msg.Data as TDFLoginResult;
                if (loginResult.LoginResult)
                {
                    //登陆结果
                    Console.WriteLine("登陆成功，市场个数:{0}:", loginResult.Markets.Length);
                    logger.Debug(string.Format("登陆成功，市场个数:{0}:", loginResult.Markets.Length));
                    for (int i = 0; i < loginResult.Markets.Length; i++)
                    {
                        Console.WriteLine("market:{0}, dyn-date:{1}", loginResult.Markets[i], loginResult.DynDate[i]);
                        logger.Debug(string.Format("market:{0}, dyn-date:{1}", loginResult.Markets[i], loginResult.DynDate[i]));
                    }
                }
                else
                {
                    Console.WriteLine("登陆失败！info:{0}", loginResult.Info);
                    logger.Debug(string.Format("登陆失败！info:{0}", loginResult.Info));
                }
            }
            else if (msg.MsgID == TDFMSGID.MSG_SYS_CODETABLE_RESULT)
            {
                //接收代码表结果
                TDFCodeResult codeResult = msg.Data as TDFCodeResult;
                Console.WriteLine("获取到代码表, info:{0}，市场个数:{1}", codeResult.Info, codeResult.Markets.Length);
                logger.Debug(string.Format("获取到代码表, info:{0}，市场个数:{1}", codeResult.Info, codeResult.Markets.Length));
                for (int i = 0; i < codeResult.Markets.Length; i++)
                {
                    Console.WriteLine("market:{0}, date:{1}, code count:{2}", codeResult.Markets[i], codeResult.CodeDate[i], codeResult.CodeCount[i]);
                    logger.Debug(string.Format("market:{0}, date:{1}, code count:{2}", codeResult.Markets[i], codeResult.CodeDate[i], codeResult.CodeCount[i]));
                }

                TDFCode[] codeArr;
                GetCodeTable("", out codeArr);
                Console.WriteLine("接收到{0}项代码!, 输出前100项", codeArr.Length);
                logger.Debug(string.Format("接收到{0}项代码!, 输出前100项", codeArr.Length));
                for (int i = 0; i < 100 && i < codeArr.Length; i++)
                {
                    if (codeArr[i].Type >= 0x90 && codeArr[i].Type <= 0x95)
                    {
                        // 期权数据
                        TDFOptionCode code = new TDFOptionCode();
                        var ret = GetOptionCodeInfo(codeArr[i].WindCode, ref code);
                        PrintHelper.PrintObject(code);
                    }
                    else
                    {
                        PrintHelper.PrintObject(codeArr[i]);
                    }
                }
            }
            else if (msg.MsgID == TDFMSGID.MSG_SYS_QUOTATIONDATE_CHANGE)
            {
                //行情日期变更
                TDFQuotationDateChange quotationChange = msg.Data as TDFQuotationDateChange;
                Console.WriteLine("接收到行情日期变更通知消息，market:{0}, old date:{1}, new date:{2}", quotationChange.Market, quotationChange.OldDate, quotationChange.NewDate);
                logger.Debug(string.Format("接收到行情日期变更通知消息，market:{0}, old date:{1}, new date:{2}", quotationChange.Market, quotationChange.OldDate, quotationChange.NewDate));
            }
            else if (msg.MsgID == TDFMSGID.MSG_SYS_MARKET_CLOSE)
            {
                //闭市消息
                TDFMarketClose marketClose = msg.Data as TDFMarketClose;
                Console.WriteLine("接收到闭市消息, 交易所:{0}, 时间:{1}, 信息:{2}", marketClose.Market, marketClose.Time, marketClose.Info);
                logger.Debug(string.Format("接收到闭市消息, 交易所:{0}, 时间:{1}, 信息:{2}", marketClose.Market, marketClose.Time, marketClose.Info));
            }
            else if (msg.MsgID == TDFMSGID.MSG_SYS_HEART_BEAT)
            {
                //心跳消息
                logger.Debug(string.Format("接收到心跳消息!"));
                Console.WriteLine(string.Format("接收到心跳消息!"));

                try
                {
                    ProcessHeartBeatData();
                }
                catch (Exception exp)
                {
                    logger.Error(exp);
                    throw;
                }
            }
        }

        //重载OnRecvDataMsg方法，接收行情数据
        public override void OnRecvDataMsg(TDFMSG msg)
        {

            if (msg.MsgID == TDFMSGID.MSG_DATA_MARKET)
            {
                //行情消息
                TDFMarketData[] marketDataArr = msg.Data as TDFMarketData[];

                ////foreach (TDFMarketData data in marketDataArr)
                ////{
                ////    if (!ShowAllData)
                ////        Console.WriteLine(data.WindCode);
                ////    else
                ////        PrintHelper.PrintObject(data);
                ////    return; //Let's only show the first element.
                ////}

                try
                {
                    this.ProcessMarketData(new List<TDFMarketData>(marketDataArr));
                }
                catch (Exception exception)
                {
                    logger.Error(exception);
                }
            }
            else if (msg.MsgID == TDFMSGID.MSG_DATA_FUTURE)
            {
                //期货行情消息
                TDFFutureData[] futureDataArr = msg.Data as TDFFutureData[];
                foreach (TDFFutureData data in futureDataArr)
                {
                    if (!ShowAllData)
                        Console.WriteLine(data.WindCode);
                    else
                        PrintHelper.PrintObject(data);
                    return; //Let's only show the first element.
                }
            }
            else if (msg.MsgID == TDFMSGID.MSG_DATA_INDEX)
            {
                //指数消息
                TDFIndexData[] indexDataArr = msg.Data as TDFIndexData[];
                ////foreach (TDFIndexData data in indexDataArr)
                ////{
                ////    if (!ShowAllData)
                ////        Console.WriteLine(data.WindCode);
                ////    else
                ////        PrintHelper.PrintObject(data);
                ////    return; //Let's only show the first element.
                ////}

                try
                {
                    this.ProcessStockIndex(new List<TDFIndexData>(indexDataArr));
                }
                catch (Exception exception)
                {
                    logger.Error(exception);
                }

            }
            else if (msg.MsgID == TDFMSGID.MSG_DATA_TRANSACTION)
            {
                //逐笔成交

                TDFTransaction[] transactionDataArr = msg.Data as TDFTransaction[];
                ////foreach (TDFTransaction data in transactionDataArr)
                ////{
                ////    if (!ShowAllData)
                ////        Console.WriteLine(data.WindCode);
                ////    else
                ////        PrintHelper.PrintObject(data);
                ////    return; //Let's only show the first element.
                ////}

                try
                {
                    this.ProcessTransactionData(new List<TDFTransaction>(transactionDataArr));
                }
                catch (Exception exception)
                {
                    logger.Error(exception);
                }
            }
            else if (msg.MsgID == TDFMSGID.MSG_DATA_ORDER)
            {
                //逐笔委托
                TDFOrder[] orderDataArr = msg.Data as TDFOrder[];
                foreach (TDFOrder data in orderDataArr)
                {
                    if (!ShowAllData)
                        Console.WriteLine(data.WindCode);
                    else
                        PrintHelper.PrintObject(data);
                    return; //Let's only show the first element.
                }
            }
            else if (msg.MsgID == TDFMSGID.MSG_DATA_ORDERQUEUE)
            {
                //委托队列
                TDFOrderQueue[] orderQueueArr = msg.Data as TDFOrderQueue[];
                foreach (TDFOrderQueue data in orderQueueArr)
                {
                    if (!ShowAllData)
                        Console.WriteLine(data.WindCode);
                    else
                        PrintHelper.PrintObject(data);
                    return; //Let's only show the first element.
                }
            }
        }

        protected abstract void ProcessMarketData(IEnumerable<TDFMarketData> marketData);

        protected abstract void ProcessTransactionData(IEnumerable<TDFTransaction> marketData);

        protected abstract void ProcessStockIndex(IEnumerable<TDFIndexData> indexData);

        protected abstract void ProcessHeartBeatData();
    }
}
