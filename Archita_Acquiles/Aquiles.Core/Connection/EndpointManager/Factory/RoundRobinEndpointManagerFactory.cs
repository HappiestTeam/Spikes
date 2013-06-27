using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aquiles.Core.Connection.EndpointManager.Impl;
using Aquiles.Core.Connection.Factory;
using Aquiles.Core.Model;
using Aquiles.Core.Diagnostics;

namespace Aquiles.Core.Connection.EndpointManager.Factory
{
    public class RoundRobinEndpointManagerFactory : IFactory<RoundRobinEndpointManager>
    {
        private const int DEFAULTDUETIME = 5000; // 5 sec
        private const int DEFAULTPERIODICTTIME = 60000; // 1 min

        public string Name
        {
            get;
            set;
        }

        public int DueTime
        {
            get;
            set;
        }

        public int PeriodicTime
        {
            get;
            set;
        }

        public ILogger Logger
        {
            get;
            set;
        }

        public IConnectionFactory ClientFactory
        {
            set;
            get;
        }

        public List<IEndpoint> Endpoints
        {
            get;
            set;
        }

        public RoundRobinEndpointManagerFactory()
        {
            this.PeriodicTime = DEFAULTPERIODICTTIME;
            this.DueTime = DEFAULTDUETIME;
        }

        #region IFactory<RoundRobinEndpointManager> Members

        public RoundRobinEndpointManager Create()
        {
            RoundRobinEndpointManager endpointManager = new RoundRobinEndpointManager();
            endpointManager.Name = this.Name;
            endpointManager.ClientFactory = this.ClientFactory;
            endpointManager.DueTime = this.DueTime;
            endpointManager.PeriodicTime = this.PeriodicTime;
            endpointManager.Endpoints = this.Endpoints;
            if (this.Logger != null)
            {
                endpointManager.Logger = this.Logger;
            }
            return endpointManager;
        }

        #endregion
    }
}
