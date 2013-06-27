using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aquiles.Core.Model;
using Aquiles.Core.Client;

namespace Aquiles.Core.Cluster
{
    public interface ICluster
    {
        string Name
        {
            get;
        }

        IClient Borrow();
        IClient Borrow(string keyspaceName);
        void Release(IClient client);
        void Invalidate(IClient client);
        object Execute(Delegate executionBlock);
        object Execute(Delegate executionBlock, ConnectionConfig overrideConnectionConfig);
        object Execute(Delegate executionBlock, string keyspaceName);
        object Execute(Delegate executionBlock, string keyspaceName, ConnectionConfig overrideConnectionConfig);
    }
}
