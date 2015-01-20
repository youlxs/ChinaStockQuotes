using ChinaStockQuotes.Configuration;
using ChinaStockQuotes.Core.Handler;
using log4net;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

namespace ChinaStockQuotes.Core
{
    public class SocketWrapper : IDisposable
    {
        private Socket clientSocket;
        private readonly ILog logger = TinyIoC.TinyIoCContainer.Current.Resolve<ILog>();
        private int reconnectTime = 0;
        private readonly INetClientHandler netClientHandler;
        private int revLen = 4;
        private bool readHeader = true;

        public SocketWrapper(INetClientHandler netServerHandler)
        {
            this.ConnectToServer();

            this.netClientHandler = netServerHandler;
        }

        public void ProcessData()
        {
            if (this.clientSocket == null)
            {
                this.ConnectToServer();

                Console.WriteLine("Connected to the server.");
            }

            if (this.clientSocket.Connected)
            {
               var receive = new Byte[1024 * 1024];

               while (true)
               {
                   #region Process

                   //Get the data from socket
                   int ret = 0;
                   var buffer = new byte[revLen];
                   try
                   {
                       if (this.clientSocket.Available > 0)
                       {
                           do
                           {
                               int tmp = this.clientSocket.Receive(receive, revLen - ret, SocketFlags.None);
                               Array.Copy(receive, 0, buffer, ret, tmp);
                               ret += tmp;
                           } while (revLen > ret);

                           if (readHeader)
                           {
                               revLen = BitConverter.ToInt32(buffer, 0);
                               readHeader = false;
                           }
                           else
                           {
                               this.netClientHandler.ClientReceive(buffer, ret);
                               revLen = 4;
                               readHeader = true;
                           }

                       }
                       else
                       {
                           Thread.Sleep(1);
                       }
                   }
                   catch (Exception ex)
                   {
                       logger.Error(ex);
                       revLen = 4;
                       readHeader = true;
                       ReconnectToServer();
                   }

                   #endregion
               }
 
            }
            else
            {
                ReconnectToServer();
            }
        }

        private void ConnectToServer()
        {
            while (true)
            {
                try
                {
                    if (reconnectTime > 5)
                    {
                        reconnectTime = 0;
                        break;
                    }
                    // Create the listening socket...
                    this.clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                    // The socket is in blocking mode. 
                    this.clientSocket.Blocking = true;
                    //Set the timeout for synchronous receive methods to 10 milliseconds
                    this.clientSocket.ReceiveTimeout = 61*1000;
                    //  m_Socket.SendTimeout = 10;
                    //Set the receive buffer size to 1M
                    this.clientSocket.ReceiveBufferSize = 1024*1024;
                    //Set the send buffer size to 8K
                    this.clientSocket.SendBufferSize = 8192;

                    var ipLocal = new IPEndPoint(IPAddress.Parse(ConfigurationData.ServerIp),
                        Convert.ToInt32(ConfigurationData.ServerPort));

                    // Connect to the remote host
                    this.clientSocket.Connect(ipLocal);
                    this.clientSocket.Send(Encoding.Default.GetBytes("1008|^"));

                    break;

                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    OnError(ex);
                    Thread.Sleep(1000);
                    Interlocked.Increment(ref reconnectTime);
                }
            }
        }

        private void ReconnectToServer()
        {
            if (this.clientSocket != null)
            {
                this.clientSocket.Close();
                this.clientSocket = null;
            }
            ConnectToServer();
        }

        private void OnError(Exception ex)
        {
            try
            {
                this.netClientHandler.ClientError(this, ex);
            }
            catch
            {

            }
        }

        public void Dispose()
        {
            if (this.clientSocket != null)
            {
                this.clientSocket.Close();
                this.clientSocket = null;
            }
        }
    }
}
