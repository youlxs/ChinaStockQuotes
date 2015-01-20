using HTInfrastructure.TDF;
using HTTransfer.Configuration;
using System;
using System.Collections.Generic;
using TDFAPI;

namespace HTTransfer.Core
{
    public class TDFTransfer : BaseTDFSourceImp
    {
        private readonly ITDFNetServerHandler tdfNetServerHandler;

        private TDFTransfer(TDFOpenSetting openSetting, ITDFNetServerHandler tdfNetServerHandler)
            : base(openSetting)
        {
            this.tdfNetServerHandler = tdfNetServerHandler;
        }
        protected override void ProcessMarketData(IEnumerable<TDFMarketData> marketData)
        {
            this.tdfNetServerHandler.SendData((IEnumerable<object>)marketData);
        }

        protected override void ProcessTransactionData(IEnumerable<TDFTransaction> transactionData)
        {
            this.tdfNetServerHandler.SendData((IEnumerable<object>)transactionData);
        }

        protected override void ProcessStockIndex(IEnumerable<TDFIndexData> indexData)
        {
            this.tdfNetServerHandler.SendData((IEnumerable<object>)indexData);
        }

        protected override void ProcessHeartBeatData()
        {
        }

        public static readonly TDFTransfer TDFImpInstance = new TDFTransfer(new TDFOpenSetting
        {
            ConnectionID = ConfigurationData.ConnectionId, //连接ID，标识某个Open调用，跟回调消息中TDFMSG结构nConnectionID字段相同
            Date = ConfigurationData.RequestDate,          //请求的日期，格式YYMMDD，为0则请求今天
            Ip = ConfigurationData.ServerIp,             //服务器Ip
            Port = ConfigurationData.ServerPort,         //服务器端口
            Username = ConfigurationData.UserId,             //服务器用户名
            Password = ConfigurationData.Password,            //服务器密码
            ReconnectCount = 9999999,                      //当连接断开时重连次数，断开重连在TDFDataSource.Connect成功之后才有效
            ReconnectGap = 5,                              //重连间隔秒数
            Time = ConfigurationData.RequestTime,          //请求的时间，格式HHMMSS，为0则请求实时行情，为(uint)-1从头请求
            TypeFlags = unchecked((uint)(DataTypeFlag.DATA_TYPE_INDEX | DataTypeFlag.DATA_TYPE_TRANSACTION)),   //为0请求所有品种，或者取值为DataTypeFlag中多种类别，比如DATA_TYPE_MARKET | DATA_TYPE_TRANSACTION
            Subscriptions = ConfigurationData.SubCodes,                //订阅列表，以 ; 分割的代码列表，例如:if1406.cf;if1403.cf；如果置为空，则全市场订阅
            Markets = ConfigurationData.SubMarket,       //市场列表，以 ; 分割，例如: sh;sz;cf;shf;czc;dce
        }, TinyIoC.TinyIoCContainer.Current.Resolve<ITDFNetServerHandler>());

    }
}
