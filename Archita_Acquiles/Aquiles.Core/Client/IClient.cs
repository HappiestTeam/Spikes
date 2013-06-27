using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aquiles.Core.Model;
using Aquiles.Core.Connection.Pooling;

namespace Aquiles.Core.Client
{
    public interface IClient : IDisposable
    {
        IClientPool OwnerPool
        {
            get;
        }

        string KeyspaceName
        {
            get;
            set;
        }

        IEndpoint Endpoint
        {
            get;
        }

        DateTime Created
        {
            get;
        }

        void Open();
        void Close();
        bool IsOpen();
        Object Execute(Delegate executionBlock);
        void OverrideConnectionConfig(ConnectionConfig config);
        void ResetToDefaultConnectionConfig();
        string getClusterName();
    }
}
