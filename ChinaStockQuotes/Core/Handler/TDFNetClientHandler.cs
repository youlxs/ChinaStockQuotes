using ChinaStockQuotes.Configuration;
using ChinaStockQuotes.Entity;
using ChinaStockQuotes.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using log4net;

namespace ChinaStockQuotes.Core.Handler
{
    public class TDFNetClientHandler : INetClientHandler
    {
        private readonly IDataHandler<Entity.TDFData> dataHandler;
        private readonly ILog logger = TinyIoC.TinyIoCContainer.Current.Resolve<ILog>();

        public TDFNetClientHandler(IDataHandler<Entity.TDFData> handler)
        {
            this.dataHandler = handler;
        }

        public void ClientReceive(byte[] receive, int receivedBytes)
        {
            string rcvData = receive.DeCompress();

            var input = new List<string>(rcvData.Split(new[] { "^" }, StringSplitOptions.RemoveEmptyEntries));

            var packetCount = input.Count;

            for (var i = 0; i < packetCount; i++)
            {
                try
                {
                    var tmpStr = input[i];
                    var strData = tmpStr.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    var messageType = strData[0];

                    switch (messageType.ToUpper())
                    {
                        case "1009":
                            this.dataHandler.Handle(strData[1].ToTDFMarketData());
                            break;
                        case "2001":
                            this.dataHandler.Handle(strData[1].ToTDFTransaction());
                            break;
                        case "1100":
                            this.dataHandler.Handle(strData[1].ToTDFIndexData());
                            break;
                    }
                }
                catch (Exception exp)
                {
                    this.logger.Error(exp);
                }
            }

            try
            {
                this.dataHandler.Handle(new HeartBeatData());
            }
            catch (Exception exp)
            {
                this.logger.Error(exp);
            }
        }

        public void ClientError(SocketWrapper client, Exception e)
        {
        }

        public void ClientDisposed(SocketWrapper client)
        {
        }

        public void Connected(SocketWrapper client)
        {
        }
    }
}
