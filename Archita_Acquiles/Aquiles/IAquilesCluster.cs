using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aquiles.Connection.Pooling;
using Aquiles.Model;

namespace Aquiles
{
    internal interface IAquilesCluster
    {
        string FriendlyName
        {
            get;
        }

        string ClusterName
        {
            get;
        }

        IConnectionPool ConnectionPool
        {
            get;
        }

        Dictionary<String,AquilesKeyspace> KeySpaces
        {
            get;
        }

        void Initialize();
    }
}
