using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Aquiles.Core.Connection.EndpointManager;
using Aquiles.Core.Connection.Factory;
using Aquiles.Core.Model;
using System.Diagnostics;
using System.Net.Sockets;
using Aquiles.Core.Exceptions;
using Aquiles.Core.Client;
using Aquiles.Core.Diagnostics;

namespace Aquiles.Core.Connection.Pooling.Impl
{
    public class SizeControlledClientPool : IClientPool
    {
        private LoggerManager LOGGER = LoggerManager.CreateLoggerManager(typeof(SizeControlledClientPool));

        public ILogger Logger
        {
            get { return LOGGER.Instance; }
            set { LOGGER.Instance = value; }
        }

        private long managedClientQuantity;
        private Queue<IClient> idleClients;
        private HashSet<IClient> referencedClients;
        private int dueTime = Timeout.Infinite;
        private int periodicTime = Timeout.Infinite;
        private Timer controlIdleClientSizeTimer;

        public int MinimumClientsToKeep
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
        public int MaximumRetriesToPollClient
        {
            get;
            set;
        }
        public int DueTime
        {
            get { return this.dueTime; }
            set
            {
                this.dueTime = value;
                this.controlIdleClientSizeTimer.Change(this.dueTime, this.periodicTime);
            }
        }
        public int PeriodicTime
        {
            get { return this.periodicTime; }
            set
            {
                this.periodicTime = value;
                this.controlIdleClientSizeTimer.Change(this.dueTime, this.periodicTime);
            }
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

        public SizeControlledClientPool()
        {
            this.managedClientQuantity = 0;
            this.referencedClients = new HashSet<IClient>();
            this.idleClients = new Queue<IClient>();
            this.controlIdleClientSizeTimer = new Timer(new TimerCallback(this.controlIdleClientSizeMethod), null, Timeout.Infinite, Timeout.Infinite);
        }

        public void Initialize()
        {
            LOGGER.Info("Initializing pool.");
            this.CreateMinimumClients();
        }

        #region IConnectionPool Members

        public string Name
        {
            get { return LOGGER.Identifier; }
            set { LOGGER.Identifier = value; }
        }

        public IClient Borrow()
        {
            LOGGER.Debug("Borrowing a client.");
            Stopwatch timeMeasure = Stopwatch.StartNew();
            IClient borrowedClient = null;
            int retryCount = 0;
            do
            {
                LOGGER.Info(LOGGER.StringFormatInvariantCulture("Borrowing client attempt #{0} of #{1}", retryCount, this.MaximumRetriesToPollClient));
                borrowedClient = this.PollClient();
                if (borrowedClient == null)
                {
                    long managedClientQuantityRead = Interlocked.Read(ref this.managedClientQuantity);
                    LOGGER.Debug(LOGGER.StringFormatInvariantCulture("We need a new client; managed clients size is #{0} and Maximum is #{1}.", managedClientQuantityRead, MaximumClientsToSupport));
                    if (managedClientQuantityRead < this.MaximumClientsToSupport)
                    {
                        // i got no connection, need to retrieve a new one
                        borrowedClient = CreateNewClient();
                    }
                }
                else
                {
                    LOGGER.Debug("We got a client");
                    if (!borrowedClient.IsOpen())
                    {
                        LOGGER.Debug("Client port is closed, Cassandra node on the other side might have crashed. Destroying client");
                        this.destroy(borrowedClient);
                        borrowedClient = null;
                    }
                }
                if (borrowedClient != null)
                {
                    // i got it, gonna mark as referenced 
                    this.MarkAsReferenced(borrowedClient);
                }
                retryCount++;
            } while ((retryCount < this.MaximumRetriesToPollClient)
                && (borrowedClient == null));

            timeMeasure.Stop();

            LOGGER.Info(LOGGER.StringFormatInvariantCulture("Time taken to borrow a client (ms): #{0}", timeMeasure.ElapsedMilliseconds));

            return borrowedClient;
        }

        public void Release(IClient client)
        {
            LOGGER.Info(LOGGER.StringFormatInvariantCulture("Releasing client {0}", client));
            Stopwatch timeMeasure = Stopwatch.StartNew();

            this.UnmarkAsReferenced(client);
            this.MarkAsIdle(client);

            timeMeasure.Stop();

            LOGGER.Info(LOGGER.StringFormatInvariantCulture("Time taken to release a client (ms): #{0}", timeMeasure.ElapsedMilliseconds));
        }

        public void Invalidate(IClient client)
        {
            LOGGER.Info(LOGGER.StringFormatInvariantCulture("Invalidating client {0}", client));
            Stopwatch timeMeasure = Stopwatch.StartNew();

            this.UnmarkAsReferenced(client);
            this.EndpointManager.Ban(client.Endpoint, client.Created);
            this.destroy(client);

            timeMeasure.Stop();
            LOGGER.Info(LOGGER.StringFormatInvariantCulture("Time taken to invalidate a client (ms): #{0}", timeMeasure.ElapsedMilliseconds));
        }

        public void Attach(IClient client)
        {
            if (client.OwnerPool == this)
            {
                LOGGER.Info(LOGGER.StringFormatInvariantCulture("Attaching client {0} into this pool", client));
                Interlocked.Increment(ref this.managedClientQuantity);
                this.MarkAsReferenced(client);
            }
            else
            {
                LOGGER.Error(LOGGER.StringFormatInvariantCulture("Given client {0} does not belong to this pool.", client));
                throw new AquilesException("Given client does not belong to this pool.");
            }
        }

        public void Detach(IClient client)
        {
            if (client.OwnerPool == this)
            {
                LOGGER.Info(LOGGER.StringFormatInvariantCulture("Detaching client {0} from this pool", client));
                Interlocked.Decrement(ref this.managedClientQuantity);
                this.UnmarkAsReferenced(client);
            }
            else
            {
                LOGGER.Error(LOGGER.StringFormatInvariantCulture("Given client {0} does not belong to this pool.", client));
                throw new AquilesException("Given client does not belong to this pool.");
            }
        }
        #endregion

        private void destroy(IClient client)
        {
            LOGGER.Trace(LOGGER.StringFormatInvariantCulture("Destroying client {0}", client));
            client.Close();
            Interlocked.Decrement(ref this.managedClientQuantity);
        }

        private void MarkAsReferenced(IClient client)
        {
            LOGGER.Trace(LOGGER.StringFormatInvariantCulture("Marking as referenced client {0}", client));
            lock (this.referencedClients)
            {
                this.referencedClients.Add(client);
            }
        }

        private void MarkAsIdle(IClient client)
        {
            LOGGER.Trace(LOGGER.StringFormatInvariantCulture("Marking as idle client {0}", client));
            lock (this.idleClients)
            {
                this.idleClients.Enqueue(client);
            }
        }

        private void UnmarkAsReferenced(IClient client)
        {
            LOGGER.Trace(LOGGER.StringFormatInvariantCulture("Unmarking as referenced client {0}", client));
            lock (this.referencedClients)
            {
                this.referencedClients.Remove(client);
            }
        }

        private IClient PollClient()
        {
            LOGGER.Trace("Polling a client");
            IClient borrowedClient = null;
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
            LOGGER.Trace(LOGGER.StringFormatInvariantCulture("Client polled: {0}", borrowedClient));
            return borrowedClient;
        }

        private IClient CreateNewClient()
        {
            long managedClientQuantityRead = Interlocked.Read(ref this.managedClientQuantity);
            LOGGER.Trace(LOGGER.StringFormatInvariantCulture("Creating new client. Client Quantity: {0}", managedClientQuantityRead));
            IClient borrowedClient = null;
            IEndpoint endpoint = null;
            do
            {
                LOGGER.Trace("Picking endpoint");
                endpoint = this.EndpointManager.Pick();
                LOGGER.Trace(LOGGER.StringFormatInvariantCulture("Endpoint chosen: {0}", endpoint));
                if (endpoint != null)
                {
                    LOGGER.Trace(LOGGER.StringFormatInvariantCulture("Creating client for endpoint: {0}", endpoint));
                    borrowedClient = this.ClientFactory.Create(endpoint, this);
                    try
                    {
                        LOGGER.Trace(LOGGER.StringFormatInvariantCulture("Opening client port for endpoint: {0}", endpoint));
                        borrowedClient.Open();
                    }
                    catch (SocketException)
                    {
                        LOGGER.Error(LOGGER.StringFormatInvariantCulture("Socket exception when opening client to endpoint: {0}", endpoint));
                        // ok this endpoint is not good
                        this.EndpointManager.Ban(endpoint, borrowedClient.Created);
                        borrowedClient = null;
                    }
                }
                else
                {
                    string message = String.Format("No endpoints available.");
                    LOGGER.Error(message);
                    throw new AquilesException(message);
                }
            } while (endpoint != null && borrowedClient == null);

            if (borrowedClient != null)
            {
                Interlocked.Increment(ref this.managedClientQuantity);
            }

            return borrowedClient;
        }

        private void controlIdleClientSizeMethod(object state)
        {
            // need to stop the time to avoid 2 methods run concurrent
            this.stopTimer();
            bool wasProperlyFinished = false;
            // i took half of the difference in order not to block for a long time the queue
            int difference = (this.MaximumClientsToSupport - this.MinimumClientsToKeep) / this.MagicNumber;
            LOGGER.Info(LOGGER.StringFormatInvariantCulture("Starting control over idle clients. Clients to clean: {0}", (difference > 0) ? difference : 0));
            try
            {
                if (this.idleClients.Count > this.MinimumClientsToKeep)
                {
                    LOGGER.Trace("Gathering clients to destroy.");
                    HashSet<IClient> clientsToDestroy = new HashSet<IClient>();
                    lock (this.idleClients)
                    {
                        for (int i = 0; (i < difference) && (this.idleClients.Count > this.MinimumClientsToKeep); i++)
                        {
                            clientsToDestroy.Add(this.idleClients.Dequeue());
                        }
                    }
                    LOGGER.Trace("Destroying idle unnecesary clients.");
                    HashSet<IClient>.Enumerator destroyerIterator = clientsToDestroy.GetEnumerator();
                    while (destroyerIterator.MoveNext())
                    {
                        this.destroy(destroyerIterator.Current);
                    }
                }
                wasProperlyFinished = true;
            }
            catch (Exception ex)
            {
                LOGGER.Error(ex);
                throw;
            }
            finally
            {
                if (wasProperlyFinished)
                {
                    LOGGER.Info(LOGGER.StringFormatInvariantCulture("Clean up finished. Restarting timer."));
                    this.startTimer(this.periodicTime);
                }
                else
                {
                    LOGGER.Warn(LOGGER.StringFormatInvariantCulture("Clean up crashed. Restarting timer sooner."));
                    this.startTimer(this.periodicTime/2);
                }

            }
        }

        private void CreateMinimumClients()
        {
            LOGGER.Info(LOGGER.StringFormatInvariantCulture("Creating minimum clients: {0}", this.MinimumClientsToKeep));
            for (int i = 0; i < this.MinimumClientsToKeep; i++)
            {
                IClient client = CreateNewClient();
                if (client != null)
                {
                    LOGGER.Info(LOGGER.StringFormatInvariantCulture("Client #{0} from #{1}", i, this.MinimumClientsToKeep));
                    this.idleClients.Enqueue(client);
                }
                else
                {
                    throw new AquilesException("No client could be created during startup phase.");
                }
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
