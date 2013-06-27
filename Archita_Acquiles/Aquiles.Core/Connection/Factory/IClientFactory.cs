using System;
using System.Collections.Generic;
using System.Text;
using Aquiles.Core.Model;
using Aquiles.Core.Connection.Pooling;
using Aquiles.Core.Client;

namespace Aquiles.Core.Connection.Factory
{
    public interface IConnectionFactory
    {
        IClient Create(IEndpoint endpoint, IClientPool ownerPool);
    }
}
