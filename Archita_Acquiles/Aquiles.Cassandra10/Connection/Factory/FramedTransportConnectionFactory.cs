﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aquiles.Core.Connection.Factory;
using Aquiles.Core.Client;
using Aquiles.Core.Model;
using Aquiles.Core.Connection.Pooling;
using Thrift.Transport;
using Thrift.Protocol;
using CassandraClient = Apache.Cassandra.Cassandra.Client;
using Aquiles.Cassandra10.Client;
using System.Net.Sockets;

namespace Aquiles.Cassandra10.Connection.Factory
{
    public class FramedTransportConnectionFactory : IConnectionFactory
    {
        #region IConnectionFactory Members

        public IClient Create(IEndpoint endpoint, IClientPool ownerPool)
        {
            TSocket socket = null;
            TTransport transport = null;
            if (endpoint.Timeout == 0)
            {
                socket = new TSocket(endpoint.Address, endpoint.Port);
            }
            else
            {
                socket = new TSocket(endpoint.Address, endpoint.Port, endpoint.Timeout);
            }
            TcpClient tcpClient = socket.TcpClient;
            transport = new TFramedTransport(socket);
            TProtocol protocol = new TBinaryProtocol(transport);
            CassandraClient cassandraClient = new CassandraClient(protocol);
            IClient client = new DefaultClient()
            {
                CassandraClient = cassandraClient,
                Endpoint = endpoint,
                OwnerPool = ownerPool,
                TcpClient = tcpClient,
                Created = DateTime.Now
            };

            return client;
        }

        #endregion
    }
}
