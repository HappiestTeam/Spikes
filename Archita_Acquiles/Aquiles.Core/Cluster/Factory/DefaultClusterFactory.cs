using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aquiles.Core.Cluster.Impl;
using Aquiles.Core.Connection.Pooling;
using Aquiles.Core.Diagnostics;

namespace Aquiles.Core.Cluster.Factory
{
    public class DefaultClusterFactory : IFactory<ICluster>
    {
        public IClientPool PoolManager
        {
            get;
            set;
        }

        public string FriendlyName
        {
            get;
            set;
        }

        public ILogger Logger
        {
            get;
            set;
        }

        #region IFactory<ICluster> Members

        public ICluster Create()
        {
            DefaultCluster cluster = new DefaultCluster();
            cluster.PoolManager = this.PoolManager;
            cluster.Name = FriendlyName;
            if (this.Logger != null)
            {
                cluster.Logger = this.Logger;
            }
            return cluster;
        }

        #endregion
    }
}
