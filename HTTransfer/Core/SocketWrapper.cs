using System.Runtime.Remoting;
using Beetle.Express;
using HTTransfer.Configuration;
using log4net;
using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Collections.Generic;

namespace HTTransfer.Core
{
    public class SocketWrapper : IServerHandler
    {
        
        private readonly ILog logger = TinyIoC.TinyIoCContainer.Current.Resolve<ILog>();

        private Int32 m_clientCount = 0;
        private readonly ArrayList workSocketList = ArrayList.Synchronized(new ArrayList());

        private INetServerHandler netServerHandler;
        private ServerFactory mFactory;
        private IServer mServer;

        public SocketWrapper()
        {
        }

        public INetServerHandler ServerHander
        {
            get { return this.netServerHandler; }
            set { this.netServerHandler = value; }
        }

        public IServer SocketServer
        {
            get { return this.mServer;  }
            set { this.mServer = value; }
        }
 

        public void SendData(Dictionary<int, string> outputMap)
        {
            if (Thread.CurrentThread.IsBackground)
                Thread.CurrentThread.IsBackground = false;

            if (this.mServer == null) return;

            Beetle.Express.IChannel workerSocket = null;

            try
            {
                for (Int32 i = 0; i < this.workSocketList.Count; i++)
                {
                    try
                    {
                        if (this.workSocketList[i] != null && outputMap.ContainsKey(i+1))
                        {
                            workerSocket = (Beetle.Express.IChannel)this.workSocketList[i];
                            if (workerSocket != null)
                            {
                                var output = ConfigurationData.GetDefaultEncoding.GetBytes(outputMap[i + 1]);
                                this.mServer.Send(new Data(output, output.Length), workerSocket);
                            }
                        }
                    }
                    catch (SocketException se)
                    {
                        this.logger.Error(se);
                        
                        continue;
                    }
                    catch (Exception ex)
                    {
                        this.logger.Error(ex);
                       
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                
                this.logger.Error(ex);
            }
        }

        public void SendData(string ouput)
        {
            if (Thread.CurrentThread.IsBackground)
                Thread.CurrentThread.IsBackground = false;

            if (this.mServer == null) return;

            Beetle.Express.IChannel workerSocket = null;

            try
            {
                var buffer = ConfigurationData.GetDefaultEncoding.GetBytes(ouput);

                for (Int32 i = 0; i < this.workSocketList.Count; i++)
                {
                    try
                    {
                        if (this.workSocketList[i] != null)
                        {
                            workerSocket = (Beetle.Express.IChannel)this.workSocketList[i];
                            if (workerSocket != null)
                            {
                                this.mServer.Send(new Data(buffer, buffer.Length), workerSocket);
                            }
                        }
                    }
                    catch (SocketException se)
                    {
                        this.logger.Error(se);
                      
                        continue;
                    }
                    catch (Exception ex)
                    {
                        this.logger.Error(ex);
                     
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
               
                this.logger.Error(ex);
            }
        }
 

        public void Connect(IServer server, ChannelConnectEventArgs e)
        {
            logger.Info("Client Connected from " + e.Channel.EndPoint);
            Console.WriteLine("Client Connected from " + e.Channel.EndPoint);

            Interlocked.Increment(ref m_clientCount);

            this.workSocketList.Add(e.Channel);
        }

        public void Disposed(IServer server, ChannelEventArgs e)
        {
            logger.Info(string.Format("{0} disposed", e.Channel.EndPoint));
            Console.WriteLine("{0} disposed", e.Channel.EndPoint);

            this.workSocketList.Remove(e.Channel);
        }

        public void Error(IServer server, ErrorEventArgs e)
        {
            logger.Info(string.Format("{0} error:{1}", e.Channel.EndPoint, e.Error.Message));
            Console.WriteLine("{0} error:{1}", e.Channel.EndPoint, e.Error.Message);

            try
            {
                this.netServerHandler.ServerError(this, e.Error);
            }
            catch
            {

            }
        }

        public void Receive(IServer server, ChannelReceiveEventArgs e)
        {
            try
            {
                this.netServerHandler.ServerReceive(e.Data.ToString(ConfigurationData.GetDefaultEncoding));
            }
            catch (Exception ex)
            {
                this.logger.Error(ex);
            }
        }

        public void SendCompleted(IServer server, ChannelSendEventArgs e)
        {
        }
    }
}
