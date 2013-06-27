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
    public class SizeControlledClientPoolFactory : IFactory<SizeControlledClientPool>
    {
        private const int DEFAULTDUETIME = 20000; // 20 sec
        private const int DEFAULTPERIODICTTIME = 5000; // 5 sec

        private const int DEFAULT_MINIMUM_CLIENTS_TO_KEEP = 10;
        private const int DEFAULT_MAXIMUM_CLIENTS_TO_SUPPORT = 1000;
        private const int DEFAULT_MAGIC_NUMBER = 7;
        private const int DEFAULT_MAXIMUM_RETRIES_TO_POLL_CLIENT = 0;

        public SizeControlledClientPoolFactory()
        {
            this.MinimumClientsToKeep = DEFAULT_MINIMUM_CLIENTS_TO_KEEP;
            this.MaximumClientsToSupport = DEFAULT_MAXIMUM_CLIENTS_TO_SUPPORT;
            this.MagicNumber = DEFAULT_MAGIC_NUMBER;
            this.MaximumRetriesToPollClient = DEFAULT_MAXIMUM_RETRIES_TO_POLL_CLIENT;
            this.DueTime = DEFAULTDUETIME;
            this.PeriodicTime = DEFAULTPERIODICTTIME;
        }

        public string Name
        {
            get;
            set;
        }

        public int MinimumClientsToKeep
        {
            get;
            set;
        }

        public ILogger Logger
        {
            get;
            set;
        }

        public int MaximumClientsToSupport
        {
            get;
            set;
        }

        public int MagicNumber
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

        public int MaximumRetriesToPollClient
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

        #region IFactory<SizeControlledConnectionPool> Members

        public SizeControlledClientPool Create()
        {
            SizeControlledClientPool pool = new SizeControlledClientPool();
            pool.Name = this.Name;
            pool.ClientFactory = this.ClientFactory;
            pool.DueTime = this.DueTime;
            pool.EndpointManager = this.EndpointManager;
            pool.MagicNumber = this.MagicNumber;
            pool.MaximumClientsToSupport = this.MaximumClientsToSupport;
            pool.MaximumRetriesToPollClient = this.MaximumRetriesToPollClient;
            pool.MinimumClientsToKeep = this.MinimumClientsToKeep;
            pool.PeriodicTime = this.PeriodicTime;
            if (this.Logger != null)
            {
                pool.Logger = this.Logger;
            }
            pool.Initialize();

            return pool;
        }

        #endregion
    }
}
