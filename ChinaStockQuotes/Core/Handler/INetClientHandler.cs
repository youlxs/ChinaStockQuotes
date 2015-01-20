using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChinaStockQuotes.Core.Handler
{
    public interface INetClientHandler
    {
        void ClientReceive(byte[] buffer, int receivedBytes);

        void ClientError(SocketWrapper client, Exception e);

        void ClientDisposed(SocketWrapper client);

        void Connected(SocketWrapper client);
    }
}
