using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aquiles.Core.Connection.Pooling;
using Aquiles.Core.Connection.Factory;
using Aquiles.Core.Connection.EndpointManager;
using Aquiles.Core.Exceptions;
using Aquiles.Core.Model;
using Aquiles.Core.Client;
using Aquiles.Core.Diagnostics;
using Aquiles.Core.Diagnostics.Impl;

namespace Aquiles.Core.Cluster.Impl
{
    public class DefaultCluster : ICluster
    {
        private LoggerManager LOGGER = LoggerManager.CreateLoggerManager(typeof(DefaultCluster));
        
        public ILogger Logger
        {
            get { return LOGGER.Instance; }
            set { LOGGER.Instance = value; }
        }

        public IClientPool PoolManager
        {
            get;
            set;
        }

        public int MaximumRetries
        {
            get;
            set;
        }

        public DefaultCluster()
        {
            this.MaximumRetries = 0;
            this.Logger = new TraceLogger();
        }

        #region ICluster Members

        public string Name
        {
            get { return LOGGER.Identifier; }
            set { LOGGER.Identifier = value; }
        }

        public IClient Borrow()
        {
            return this.Borrow(null);
        }

        public IClient Borrow(string keyspaceName)
        {
            LOGGER.Trace("Borrowing client.");
            IClient client = this.AcquireClient(keyspaceName);
            if (client != null)
            {
                LOGGER.Trace("Detaching client from pool");
                this.PoolManager.Detach(client);
                LOGGER.Trace(LOGGER.StringFormatInvariantCulture("Setting client keyspace: {0}.", keyspaceName));
                client.KeyspaceName = keyspaceName;
            }
            return client;
        }

        public void Release(IClient client)
        {
            this.Release(client, true);
        }

        /// <summary>
        /// Release a client after attaching it again back to the pool only if @attachFirst is true.
        /// </summary>
        /// <param name="client">the client to be released.</param>
        /// <param name="attachFirst">true if it was detached previously and now it needs to be attached back to the owner pool.</param>
        private void Release(IClient client, bool attachFirst)
        {
            if (attachFirst)
            {
                LOGGER.Trace("Attaching client from pool");
                this.PoolManager.Attach(client);
            }
            LOGGER.Trace("Releasing client.");
            this.PoolManager.Release(client);
        }

        public void Invalidate(IClient client)
        {
            this.Invalidate(client, true);
        }

        /// <summary>
        /// Invalidates a client after attaching it again back to the pool only if @attachFirst is true.
        /// </summary>
        /// <param name="client">the client to be invalidated.</param>
        /// <param name="attachFirst">true if it was detached previously and now it needs to be attached back to the owner pool.</param>
        private void Invalidate(IClient client, bool attachFirst)
        {
            if (attachFirst)
            {
                LOGGER.Trace("Attaching client from pool");
                this.PoolManager.Attach(client);
            }
            LOGGER.Trace("Invalidating client.");
            this.PoolManager.Invalidate(client);
        }

        public object Execute(Delegate executionBlock, string keyspaceName)
        {
            return this.Execute(executionBlock, keyspaceName, null);
        }

        public object Execute(Delegate executionBlock)
        {
            return this.Execute(executionBlock, (string) null);
        }

        public object Execute(Delegate executionBlock, ConnectionConfig overrideConnectionConfig)
        {
            return this.Execute(executionBlock, null, overrideConnectionConfig);
        }

        public object Execute(Delegate executionBlock, string keyspaceName, ConnectionConfig overrideConnectionConfig)
        {
            object rtnObject = null;
            int executionCounter = 0;
            IClient client = null;
            bool noException = false;
            bool isClientHealthy = true;
            Exception exception = null;
            bool existsConnectionConfig = overrideConnectionConfig != null;
            do
            {
                LOGGER.Trace(LOGGER.StringFormatInvariantCulture("Execution try #{0} from #{1}.", executionCounter, this.MaximumRetries));
                exception = null;
                noException = false;
                isClientHealthy = false;
                client = this.AcquireClient(keyspaceName);

                if (client != null)
                {
                    LOGGER.Trace("Client not null.");
                    if (existsConnectionConfig)
                    {
                        LOGGER.Trace("Overriding client connection config.");
                        client.OverrideConnectionConfig(overrideConnectionConfig);
                    }
                    try
                    {
                        LOGGER.Trace("Executing executionBlock.");
                        rtnObject = client.Execute(executionBlock);
                        noException = true;
                        isClientHealthy = true;
                    }
                    catch (ExecutionBlockException ex)
                    {
                        LOGGER.Error("ExecutionBlockException found.", ex);
                        exception = ex;
                        isClientHealthy = ex.IsClientHealthy;
                        if (!ex.ShouldRetry)
                        {
                            executionCounter = 0;
                        }
                    }
                    finally
                    {
                        if (noException || isClientHealthy)
                        {
                            if (existsConnectionConfig)
                            {
                                LOGGER.Debug("Resetting client connection parameters to its defaults.");
                                client.ResetToDefaultConnectionConfig();
                            }
                            LOGGER.Debug("Releasing client back to pool.");
                            this.Release(client, false);
                        }
                        else
                        {
                            LOGGER.Debug("Invalidating client.");
                            this.Invalidate(client, false);
                        }
                    }
                }
                else
                {
                    throw new AquilesException("No client could be borrowed.");
                }
                executionCounter++;
            } while (executionCounter < this.MaximumRetries && !noException);

            if (exception != null)
            {
                throw exception;
            }

            return rtnObject;
        }

        #endregion

        private IClient AcquireClient(string keyspaceName)
        {
            LOGGER.Debug("Acquiring client from pool.");
            IClient client = this.PoolManager.Borrow();
            if (client != null && keyspaceName != null)
            {
                LOGGER.Debug("Setting keyspace to client.");
                client.KeyspaceName = keyspaceName;
            }
            return client;
        }
    }
}
