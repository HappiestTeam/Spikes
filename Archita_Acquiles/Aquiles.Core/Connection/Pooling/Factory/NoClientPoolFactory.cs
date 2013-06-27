using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aquiles.Core.Connection.Pooling.Impl;
using Aquiles.Core.Connection.EndpointManager;
using Aquiles.Core.Connection.Factory;
using Aquiles.Core.Diagnostics;

namespace Aquiles.Core.Connection.Pooling.Factory
{
    public class NoClientPoolFactory : IFactory<NoClientPool>
    {
        public string Name
        {
            get;
            set;
        }

        public IEndpointManager EndpointManager
        {
            get;
            set;
        }

        public IConnectionFactory ClientFactory
        {
            get;
            set;
        }

        public ILogger Logger
        {
            get;
            set;
        }

        #region IFactory<NoConnectionPool> Members

        public NoClientPool Create()
        {
            NoClientPool pool = new NoClientPool();
            pool.Name = this.Name;
            pool.ClientFactory = this.ClientFactory;
            pool.EndpointManager = this.EndpointManager;
            if (this.Logger != null)
            {
                pool.Logger = this.Logger;
            }
            return pool;
        }

        #endregion
    }
}
