using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aquiles.Core.Connection.Factory;
using Aquiles.Core.Connection.EndpointManager;
using Aquiles.Core.Model;
using System.Diagnostics;
using Aquiles.Core.Client;
using Aquiles.Core.Diagnostics;

namespace Aquiles.Core.Connection.Pooling.Impl
{
    public class NoClientPool : IClientPool
    {
        private LoggerManager LOGGER = LoggerManager.CreateLoggerManager(typeof(NoClientPool));
        public ILogger Logger
        {
            get { return LOGGER.Instance; }
            set { LOGGER.Instance = value; }
        }

        public IEndpointManager EndpointManager
        {
            set;
            get;
        }

        public IConnectionFactory ClientFactory
        {
            set;
            get;
        }

        #region IConnectionPool Members

        public string Name
        {
            get { return LOGGER.Identifier; }
            set { LOGGER.Identifier = value; }
        }

        public IClient Borrow()
        {
            IClient client = null;
            Stopwatch timeMeasure = Stopwatch.StartNew();

            IEndpoint endpoint = this.EndpointManager.Pick();
            if (endpoint != null)
            {
                client = this.ClientFactory.Create(endpoint, this);
                client.Open();
            }

            timeMeasure.Stop();

            return client;
        }

        public void Release(IClient client)
        {
            Stopwatch timeMeasure = Stopwatch.StartNew();

            client.Close();

            timeMeasure.Stop();
        }

        public void Invalidate(IClient client)
        {
            Stopwatch timeMeasure = Stopwatch.StartNew();

            client.Close();

            timeMeasure.Stop();
        }

        public void Attach(IClient client)
        {
            // DO NOTHING
        }

        public void Detach(IClient client)
        {
            // DO NOTHING
        }
        #endregion
    }
}
