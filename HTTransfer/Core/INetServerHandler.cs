using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTInfrastructure.TDF;
using TDFAPI;

namespace HTTransfer.Core
{
    public interface INetServerHandler
    {
        void ServerReceive(string content);

        void ServerError(SocketWrapper wrapper, Exception e);

        void ServerClientDisposed(int clientId);

        void ServerClientConnected(int clientId);
    }

    public interface ITDFNetServerHandler : INetServerHandler
    {
        void SendData(IEnumerable<object> dataList);
    }
}
