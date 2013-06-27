using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aquiles.Core.Model;
using Aquiles.Core.Connection.Factory;

namespace Aquiles.Core.Connection.EndpointManager
{
    public interface IEndpointManager
    {
        List<IEndpoint> Endpoints
        {
            get;
        }

        string Name
        {
            get;
        }

        IEndpoint Pick();
        void Ban(IEndpoint endpoint, DateTime connectionCreationTime);

    }
}
