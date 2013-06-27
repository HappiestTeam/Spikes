using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aquiles.Connection.Endpoint;
using Aquiles.Connection.Factory;
using Aquiles.Model.Internal;
using Aquiles.Logging;
using Aquiles.Configuration;
using System.Threading;
using System.Net.Sockets;

namespace Aquiles.Connection.Pooling
{
    internal class SizeControlledConnectionPool : IConnectionPool
    {
        private const int DEFAULTDUETIME = 20000; // 20 sec
        private const int DEFAULTPERIODICTTIME = 5000; // 5 sec
        private const string MINIMUM_CLIENTS_TO_KEEP_KEY = "minimumClientsToKeepInPool";
        private const string MAXIMUM_CLIENTS_TO_SUPPORT_KEY = "maximumClientsToSupportInPool";
        private const string MAGIC_NUMBER_KEY = "magicNumber";
        private const string MAXIMUM_RETRIES_TO_POLL_CLIENT = "maximumRetriesToPollClient";
        
        private const int DEFAULT_MINIMUM_CLIENTS_TO_KEEP = 10;
        private const int DEFAULT_MAXIMUM_CLIENTS_TO_SUPPORT = 1000;
        private const int DEFAULT_MAGIC_NUMBER = 7;
        private const int DEFAULT_MAXIMUM_RETRIES_TO_POLL_CLIENT = 0;

        private LoggerHelper logger;
        private Queue<CassandraClient> idleClients;
        private HashSet<CassandraClient> referencedClients;
        private IConnectionFactory connectionFactory;
        private IEndpointManager endpointManager;
        private Timer controlIdleClientSizeTimer;

        private int minimumClientsToKeep;
        private int maximumClientsToSupport;
        private int magicNumber;
        private int maximumRetriesToPollClient;
        private volatile int managedClientQuantity;

        public SizeControlledConnectionPool()
        {
            this.managedClientQuantity = 0;
            this.logger = LoggerHelper.CreateLogger(this.GetType());
            this.referencedClients = new HashSet<CassandraClient>();
            this.idleClients = new Queue<CassandraClient>();
            this.controlIdleClientSizeTimer = new Timer(new TimerCallback(this.controlIdleClientSizeMethod), null, DEFAULTDUETIME, Timeout.Infinite);
        }

        #region IConnectionPool Members

        public string Name
        {
            get;
            set;
        }

        public IEndpointManager EndpointManager
        {
            set { this.endpointManager = value; }
        }

        public IConnectionFactory ConnectionFactory
        {
            set { this.connectionFactory = value; }
        }

        public void Initialize()
        {
            this.maximumRetriesToPollClient = DEFAULT_MAXIMUM_RETRIES_TO_POLL_CLIENT;


            this.minimumClientsToKeep = this.ParseIntegerProperty(this.SpecialConnectionParameters, MINIMUM_CLIENTS_TO_KEEP_KEY, DEFAULT_MINIMUM_CLIENTS_TO_KEEP);
            this.maximumClientsToSupport = this.ParseIntegerProperty(this.SpecialConnectionParameters, MAXIMUM_CLIENTS_TO_SUPPORT_KEY, DEFAULT_MAXIMUM_CLIENTS_TO_SUPPORT);
            this.magicNumber = this.ParseIntegerProperty(this.SpecialConnectionParameters, MAGIC_NUMBER_KEY, DEFAULT_MAGIC_NUMBER);
            this.maximumRetriesToPollClient = this.ParseIntegerProperty(this.SpecialConnectionParameters, MAXIMUM_RETRIES_TO_POLL_CLIENT, DEFAULT_MAXIMUM_RETRIES_TO_POLL_CLIENT);

            this.ValidateProperties();

            logger.Info(logger.StringFormatInvariantCulture("Starting pool '{0}' with {1}: '{2}', {3}: '{4}', {5}: '{6}', {7}: '{8}'.", 
                this.Name, 
                MINIMUM_CLIENTS_TO_KEEP_KEY, 
                this.minimumClientsToKeep, 
                MAXIMUM_CLIENTS_TO_SUPPORT_KEY, 
                this.maximumClientsToSupport,
                MAGIC_NUMBER_KEY,
                this.magicNumber,
                MAXIMUM_RETRIES_TO_POLL_CLIENT,
                this.maximumRetriesToPollClient));

            this.CreateMinimumClients();

            logger.Info(logger.StringFormatInvariantCulture("Pool '{0}' started with clients: '{1}'.", this.Name, this.idleClients.Count));
        }

        private void CreateMinimumClients()
        {
            for (int i = 0; i < this.minimumClientsToKeep; i++)
            {
                CassandraClient client = CreateNewClient();
                if (client != null)
                {
                    this.idleClients.Enqueue(client);
                }
                else
                {
                    logger.Error(logger.StringFormatInvariantCulture("Couldn't create a connection for '{0}'.", this.Name));
                    break;
                }
            }
        }

        private void ValidateProperties()
        {
            if (this.minimumClientsToKeep >= this.maximumClientsToSupport)
            {
                logger.Warn(logger.StringFormatInvariantCulture("Pool '{0}' wrongly configured ({1} >= {2}) using defaults values.", this.Name, MINIMUM_CLIENTS_TO_KEEP_KEY, MAXIMUM_CLIENTS_TO_SUPPORT_KEY));
                this.minimumClientsToKeep = DEFAULT_MINIMUM_CLIENTS_TO_KEEP;
                this.maximumClientsToSupport = DEFAULT_MAXIMUM_CLIENTS_TO_SUPPORT;
            }
        }

        private int ParseIntegerProperty(SpecialConnectionParameterCollection specialConnectionParameterCollection, string propertyKey, int propertyDefaultValue)
        {
            int returnValue = propertyDefaultValue;
            SpecialConnectionParameterElement element;
            element = specialConnectionParameterCollection[propertyKey];
            if (element != null)
            {
                if (!int.TryParse(element.Value, out returnValue))
                {
                    logger.Warn(logger.StringFormatInvariantCulture("Pool '{0}' - Value '{1}' for {2} not valid, using default.", this.Name, element.Value, propertyDefaultValue));
                    returnValue = propertyDefaultValue;
                }
            }

            return returnValue;
        }

        public SpecialConnectionParameterCollection SpecialConnectionParameters
        {
            set;
            private get;
        }

        public CassandraClient Borrow()
        {
            CassandraClient borrowedClient = null;
            int retryCount = 0;
            do
            {
                this.logger.Debug(String.Format("Clients referenced '{0}' - free {1} - total {2} ", this.referencedClients.Count,this.idleClients.Count,this.maximumClientsToSupport));
                borrowedClient = this.PollClient();
                if (borrowedClient == null)
                {
                    if (this.managedClientQuantity < this.maximumClientsToSupport)
                    {
                        // i got no connection, need to retrieve a new one
                        borrowedClient = CreateNewClient();
                        this.logger.Debug(logger.StringFormatInvariantCulture("Pool '{0}' - No client could be borrowed, retrieving an endpoint.", this.Name));

                    }
                    else
                    {
                        this.logger.Warn(logger.StringFormatInvariantCulture("Pool '{0}' - Referenced clients count is greater than '{1}'.", this.Name, this.maximumClientsToSupport));
                    }
                }
                else if (!borrowedClient.IsOpened)
                {
                    this.logger.Debug(logger.StringFormatInvariantCulture("Pool '{0}' - Destroying client '{1}' since it is not opened.", this.Name, borrowedClient));
                    this.destroy(borrowedClient);
                    borrowedClient = null;
                }

                if (borrowedClient != null)
                {
                    this.logger.Debug(logger.StringFormatInvariantCulture("Pool '{0}' - Marking client '{1}' as referenced.", this.Name, borrowedClient));
                    // i got it, gonna mark as referenced 
                    this.MarkAsReferenced(borrowedClient);
                    this.logger.Debug(logger.StringFormatInvariantCulture("Pool '{0}' - Opening '{1}' for use.", this.Name, borrowedClient));
                }
                retryCount++;
            } while ((retryCount < this.maximumRetriesToPollClient)
                && (borrowedClient == null));
            return borrowedClient;
        }

        public void Release(CassandraClient cassandraClient)
        {
            this.logger.Debug(logger.StringFormatInvariantCulture("Pool '{0}' - Unmarking client '{1}' as referenced.", this.Name, cassandraClient));
            this.UnmarkAsReferenced(cassandraClient);
            this.logger.Debug(logger.StringFormatInvariantCulture("Marking client '{0}' as idle.", cassandraClient));
            this.MarkAsIdle(cassandraClient);

        }

        private void destroy(CassandraClient cassandraClient)
        {
            this.logger.Debug(logger.StringFormatInvariantCulture("Pool '{0}' - Destroying '{1}'.", this.Name, cassandraClient));
            cassandraClient.CloseTransport();
            this.managedClientQuantity--;
        }

        public void Invalidate(CassandraClient cassandraClient)
        {
            this.logger.Debug(logger.StringFormatInvariantCulture("Pool '{0}' - Unmarking client '{1}' as referenced.", this.Name, cassandraClient));
            this.UnmarkAsReferenced(cassandraClient);
            this.logger.Debug(logger.StringFormatInvariantCulture("Pool '{0}' - Marking endpoint '{1}' as invalid.", this.Name, cassandraClient.Endpoint));
            this.endpointManager.Ban(cassandraClient.Endpoint);
            this.destroy(cassandraClient);
        }

        #endregion

        private void MarkAsReferenced(CassandraClient borrowedClient)
        {
            lock (this.referencedClients)
            {
                this.referencedClients.Add(borrowedClient);
            }
        }

        private void MarkAsIdle(CassandraClient client)
        {
            lock (this.idleClients)
            {
                this.idleClients.Enqueue(client);
            }
        }

        private void UnmarkAsReferenced(CassandraClient client)
        {
            lock (this.referencedClients)
            {
                this.referencedClients.Remove(client);
            }
        }

        private CassandraClient PollClient()
        {
            CassandraClient borrowedClient = null;
            if (this.idleClients.Count > 0)
            {
                    lock (idleClients)
                    {
                        if (this.idleClients.Count > 0)
                        {
                            borrowedClient = this.idleClients.Dequeue();
                        }
                    }
            }
            return borrowedClient;
        }

        private CassandraClient CreateNewClient()
        {
            this.managedClientQuantity++;
            CassandraClient borrowedClient = null;
            CassandraEndpoint endpoint = null;
            do {
                endpoint = this.endpointManager.Retrieve();
                if (endpoint != null)
                {
                    this.logger.Debug(logger.StringFormatInvariantCulture("Pool '{0}' - Creating client for endpoint '{1}'.", this.Name, endpoint));
                    borrowedClient = this.connectionFactory.Create(endpoint);
                    try
                    {
                        borrowedClient.OpenTransport();
                    }
                    catch (SocketException)
                    {
                        // ok this endpoint is not good
                        this.endpointManager.Ban(endpoint);
                        borrowedClient = null;
                    }
                }
                else
                {
                    this.logger.Error(logger.StringFormatInvariantCulture("Pool '{0}' - No endpoints available (Cluster crashed?).", this.Name));
                }
            } while (endpoint != null && borrowedClient == null);

            if (borrowedClient == null)
            {
                this.managedClientQuantity--;
            }

            return borrowedClient;
        }

        private void controlIdleClientSizeMethod(object state)
        {
            bool wasProperlyFinished = false;
            this.logger.Info(logger.StringFormatInvariantCulture("Pool: '{0}' - Stoping ControlIdleClientSize Timer.", this.Name));
            // i took half of the difference in order not to block for a long time the queue
            int difference = (this.maximumClientsToSupport - this.minimumClientsToKeep) / this.magicNumber;
            // need to stop the time to avoid 2 methods run concurrent
            this.stopTimer();
            try
            {
                if (this.idleClients.Count > this.minimumClientsToKeep)
                {
                    this.logger.Info(logger.StringFormatInvariantCulture("Pool: '{0}' - Need to clean up!!.", this.Name));
                    HashSet<CassandraClient> clientsToDestroy = new HashSet<CassandraClient>();
                    lock (this.idleClients)
                    {
                        for (int i = 0; (i < difference) && (this.idleClients.Count > this.minimumClientsToKeep); i++)
                        {
                            clientsToDestroy.Add(this.idleClients.Dequeue());
                        }
                    }
                    this.logger.Info(logger.StringFormatInvariantCulture("Pool: '{0}' - Going to free {1} clients.", this.Name, clientsToDestroy.Count));
                    HashSet<CassandraClient>.Enumerator destroyerIterator = clientsToDestroy.GetEnumerator();
                    while (destroyerIterator.MoveNext())
                    {
                        this.destroy(destroyerIterator.Current);
                    }
                }
                wasProperlyFinished = true;
            }
            catch (Exception ex)
            {
                logger.Error(this.Name, ex);
            }
            finally
            {
                if (wasProperlyFinished)
                {
                    this.logger.Info(logger.StringFormatInvariantCulture("Pool: '{0}' - Clean up finished.", this.Name));
                }
                else
                {
                    this.logger.Warn(logger.StringFormatInvariantCulture("Pool: '{0}' - Clean up crashed.", this.Name));
                }
                this.startTimer(DEFAULTPERIODICTTIME);
            }
        }

        

        private void stopTimer()
        {
            this.controlIdleClientSizeTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void startTimer(int dueTime)
        {
            this.controlIdleClientSizeTimer.Change(dueTime, Timeout.Infinite);
        }
    }
}
