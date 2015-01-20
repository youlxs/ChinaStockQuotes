using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HTInfrastructure.TDF;
using HTTransfer.Configuration;
using log4net;
using HTTransfer.Extensions;
using System.Threading;
using Beetle.Express;
using TDFAPI;

namespace HTTransfer.Core
{
    public class TDFNetServerHandler : ITDFNetServerHandler
    {
        private readonly SocketWrapper socketWrapper;
        private readonly ILog logger;
        private readonly Dictionary<int, Dictionary<string, Symbol>> clientRequestMap;
        private static readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        public TDFNetServerHandler(ILog logger)
        {
            this.clientRequestMap = new Dictionary<int, Dictionary<string, Symbol>>();
            var mFactory = new ServerFactory("serverSection");
            foreach (IServer item in mFactory.Servers)
            {
                Console.WriteLine("{0} start @{1}", item.Name, item.Port);
            }

            this.socketWrapper = mFactory.Servers[0].Handler as SocketWrapper;
            this.socketWrapper.ServerHander = this;
            this.socketWrapper.SocketServer = mFactory.Servers[0];
            this.logger = logger;
        }

        public void ServerReceive(byte[] buffer, int receivedBytes, int clientNumber)
        {
            var data = Encoding.ASCII.GetString(buffer, 0, receivedBytes);

            var items = data.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            if (items[0] == "1008")
            {
                _lock.EnterWriteLock();

                try
                {
                    var symbols = new Dictionary<string, Symbol>();

                    if (items.Length > 1)
                    {
                        var subItems = items[1].Split(new char[] { '^' }, StringSplitOptions.RemoveEmptyEntries);

                        if (subItems.Length == 1 && subItems[0] == "888888")
                        {
                            var stockItems = File.ReadAllLines(ConfigurationData.RzrqStockPath);

                            symbols = (from s in stockItems
                                       let codeItems = s.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries)
                                       select new Symbol { Code = codeItems[0], Market = codeItems[1] == "1" ? "sh" : "sz" }).ToDictionary(p=>p.Code);

                        }
                        else
                        {
                            symbols = (from s in subItems
                                          let codeItems = s.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries)
                                          select new Symbol { Code = codeItems[0], Market = codeItems[1] == "1" ? "sh" : "sz" }).ToDictionary(p=>p.Code);
                        }

                        this.clientRequestMap[clientNumber] = symbols;
                    }
                    else
                        this.clientRequestMap[clientNumber] = symbols;
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
            else if(items[0] == "8888")
            {
                this.socketWrapper.SendData(data);
            }
        }

        public void ServerReceive(string content)
        {
            var items = content.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            if (items[0] == "8888")
            {
                this.socketWrapper.SendData(content);
            }
        }

        public void ServerError(SocketWrapper wrapper, Exception e)
        {
           
        }

        public void ServerClientDisposed(int clientId)
        {
            _lock.EnterWriteLock();

            try
            {
                if (this.clientRequestMap.ContainsKey(clientId))
                {
                    this.clientRequestMap.Remove(clientId);
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
            
        }


        public void ServerClientConnected(int id)
        {
        }


        public void SendData(IEnumerable<object> dataList)
        {
            var dataType = dataList.GetType();

            if (dataType.IsAssignableFrom(typeof(List<TDFMarketData>)))
            {
                var md = (List<TDFMarketData>)dataList;
                var output = md.ToNetworkString();

                this.socketWrapper.SendData(output);

            }
            else if (dataType.IsAssignableFrom(typeof(List<TDFTransaction>)))
            {
                var trans = (List<TDFTransaction>)dataList;
                var output = trans.ToNetworkString();

                this.socketWrapper.SendData(output);

            }
            else if (dataType.IsAssignableFrom(typeof(List<TDFIndexData>)))
            {
                var index = (List<TDFIndexData>)dataList;
               
                var output = index.ToNetworkString();

                this.socketWrapper.SendData(output);


            }
        }

        public void SendDataInner(IEnumerable<object> dataList)
        {
            var dataType = dataList.GetType();

            if (dataType.IsAssignableFrom(typeof(List<TDFMarketData>)))
            {
                var md = (List<TDFMarketData>)dataList;
                var map = new Dictionary<int, string>();
                var output = md.ToNetworkString();

                _lock.EnterReadLock();

                try
                {
                    foreach (var pair in this.clientRequestMap)
                    {
                        if (pair.Value.Count == 0)
                        {
                            map[pair.Key] = output;
                        }
                        else
                        {
                            var tmp = md.Where(v => pair.Value.ContainsKey(v.Code) && pair.Value[v.Code].Market == v.WindCode.Substring(7).ToLower()).ToList();
                            map[pair.Key] = tmp.ToNetworkString();
                        }
                    }
                }
                finally
                {
                    _lock.ExitReadLock();
                }

                this.socketWrapper.SendData(map);

            }
            else if (dataType.IsAssignableFrom(typeof(List<TDFTransaction>)))
            {
                var trans = (List<TDFTransaction>)dataList;
                var map = new Dictionary<int, string>();
                var output = trans.ToNetworkString();

                _lock.EnterReadLock();

                try
                {
                    foreach (var pair in this.clientRequestMap)
                    {
                        if (pair.Value.Count == 0)
                        {
                            map[pair.Key] = output;
                        }
                        else
                        {
                            var tmp = trans.Where(v => pair.Value.ContainsKey(v.Code) && pair.Value[v.Code].Market == v.WindCode.Substring(7).ToLower()).ToList();
                            map[pair.Key] = tmp.ToNetworkString();
                        }
                    }
                }
                finally
                {
                    _lock.ExitReadLock();
                }

                this.socketWrapper.SendData(map);

            }
            else if (dataType.IsAssignableFrom(typeof(List<TDFIndexData>)))
            {
                var index = (List<TDFIndexData>)dataList;
                var map = new Dictionary<int, string>();
                var output = index.ToNetworkString();

                _lock.EnterReadLock();

                try
                {
                    foreach (var pair in this.clientRequestMap)
                    {
                        if (pair.Value.Count == 0)
                        {
                            map[pair.Key] = output;
                        }
                        else
                        {
                            var tmp = index.Where(v => pair.Value.ContainsKey(v.Code) && pair.Value[v.Code].Market == v.WindCode.Substring(7).ToLower()).ToList();
                            map[pair.Key] = tmp.ToNetworkString();
                        }
                    }
                }
                finally
                {
                    _lock.ExitReadLock();
                }

                this.socketWrapper.SendData(map);


            }
        }

        #region INetServerHandler Members


        #endregion
    }
}
