using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aquiles.Core.Model;
using System.Threading;
using Aquiles.Core.Exceptions;
using Aquiles.Core.Client;
using Aquiles.Core.Connection.Factory;
using Aquiles.Core.Diagnostics;

namespace Aquiles.Core.Connection.EndpointManager.Impl
{
    public class RoundRobinEndpointManager : IEndpointManager, IDisposable
    {
        //private MonitorHelper monitor = null;
        private LoggerManager LOGGER = LoggerManager.CreateLoggerManager(typeof(RoundRobinEndpointManager));
        private const int DEFAULT_PERIODIC = 1000;
        private const int DEFAULT_DUE = 5000;
        private bool endpointRecoveryTimerStarted = false;

        public string Name
        {
            get { return LOGGER.Identifier; }
            set { LOGGER.Identifier = value; }
        }

        public ILogger Logger
        {
            get { return LOGGER.Instance; }
            set { LOGGER.Instance = value; }
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

        private List<IEndpoint> endpoints;
        private Queue<IEndpoint> availableEndpoints;
        private HashSet<IEndpoint> blackListedEndpoints;
        private Timer endpointRecoveryTimer;

        public RoundRobinEndpointManager()
        {
            this.blackListedEndpoints = new HashSet<IEndpoint>();
            this.endpointRecoveryTimer = new Timer(new TimerCallback(this.EndpointRecoveryMethod), null, Timeout.Infinite, Timeout.Infinite);
            this.PeriodicTime = DEFAULT_PERIODIC;
            this.DueTime = DEFAULT_DUE;
        }

        public IConnectionFactory ClientFactory
        {
            set;
            private get;
        }

        #region IEndpointManager Members

        public List<IEndpoint> Endpoints
        {
            get { return endpoints; }
            set
            {
                endpoints = value;
                this.availableEndpoints = new Queue<IEndpoint>(this.endpoints);
            }
        }

        public Aquiles.Core.Model.IEndpoint Pick()
        {
            IEndpoint borrowedEndpoint = null;
            do
            {
                borrowedEndpoint = null;
                lock (this.availableEndpoints)
                {
                    if (this.availableEndpoints.Count > 0)
                    {
                        LOGGER.Debug("Picking up an endpoint.");
                        borrowedEndpoint = this.availableEndpoints.Dequeue();
                    }
                    else
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (IEndpoint endpoint in this.Endpoints)
                        {
                            if (sb.Length > 0)
                            {
                                sb.Append("','");
                            }
                            sb.Append(endpoint.ToString());
                        }
                        string error = String.Format("All endpoints ['{0}'] are blacklisted, is cluster down?", sb.ToString());
                        LOGGER.Fatal(error);
                        throw new AquilesException(error);
                    }
                    if (this.IsBlackListed(borrowedEndpoint))
                    {
                        LOGGER.Debug("Picked endpoint is blacklisted.");
                        borrowedEndpoint = null;
                        LOGGER.Info("Starting endpoint health checker.");
                        this.startTimer(1);
                    }
                    else
                    {
                        LOGGER.Debug("Sending back the endpoint to be selectable again.");
                        this.availableEndpoints.Enqueue(borrowedEndpoint);
                    }
                }
            } while ((borrowedEndpoint == null));

            return borrowedEndpoint;
        }

        public void Ban(Aquiles.Core.Model.IEndpoint endpoint, DateTime connectionCreationTime)
        {
            if (endpoint.UpDateTime <= connectionCreationTime)
            {
                LOGGER.Warn(String.Format("Banning endpoint {0}", (endpoint != null) ? endpoint.ToString() : "'null'"));
                lock (this.blackListedEndpoints)
                {
                    if (!this.blackListedEndpoints.Contains(endpoint))
                    {
                        this.blackListedEndpoints.Add(endpoint);
                    }
                    else
                    {
                        LOGGER.Debug(String.Format("Endpoint {0} already banned.",
                                                   (endpoint != null) ? endpoint.ToString() : "'null'"));
                    }
                }
                this.startTimer(this.DueTime);
                //this.monitor.Decrement(AquilesMonitorFeature.NumberOfActiveEndpoints);
                //this.monitor.Increment(AquilesMonitorFeature.NumberOfBannedEndpoints);
                LOGGER.Warn(LOGGER.StringFormatInvariantCulture("Endpoint '{0}' was banned.", endpoint.ToString()));
            } else
            {
                LOGGER.Debug(LOGGER.StringFormatInvariantCulture("Endpoint '{0}' was not banned.", endpoint.ToString()));
            }
        }

        #endregion

        #region IDisposable Members
        private bool disposed = false;
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(Boolean forceDispose)
        {
            if (!this.disposed)
            {
                this.disposed = true;
                if (forceDispose)
                {
                    if (this.endpointRecoveryTimer != null)
                    {
                        this.endpointRecoveryTimer.Dispose();
                        this.endpointRecoveryTimer = null;
                    }
                }
            }
        }

        ~RoundRobinEndpointManager()
        {
            this.Dispose(false);
        }
        #endregion

        private bool IsBlackListed(IEndpoint borrowedEndpoint)
        {
            LOGGER.Debug(String.Format("Checking if endpoint {0} is blacklisted.", (borrowedEndpoint != null) ? borrowedEndpoint.ToString(): "'null'"));
            bool isBlackListed = false;
            if (this.blackListedEndpoints.Count > 0)
            {
                lock (this.blackListedEndpoints)
                {
                    isBlackListed = this.blackListedEndpoints.Contains(borrowedEndpoint);
                }
            }
            LOGGER.Debug(LOGGER.StringFormatInvariantCulture("Endpoint: '{0}' blacklisted: {1}", borrowedEndpoint, isBlackListed));
            return isBlackListed;
        }

        private void EndpointRecoveryMethod(object state)
        {
            LOGGER.Info("Stoping EndpointRecovery Timer.");
            bool wasProperlyFinished = false;
            bool areThereLeftBehind = false;
            // need to stop the time to avoid 2 methods run concurrent
            this.stopTimer();
            try
            {
                LOGGER.Debug("Getting blacklisted endpoints.");
                HashSet<IEndpoint> clonedBlackList = null;
                lock (this.blackListedEndpoints)
                {
                    clonedBlackList = new HashSet<IEndpoint>(this.blackListedEndpoints);
                }

                bool isUp = false;
                //now i can work without problems
                foreach (IEndpoint endpoint in clonedBlackList)
                {
                    LOGGER.Debug(LOGGER.StringFormatInvariantCulture("Checking on '{0}'", endpoint));
                    IClient cassandraClient = this.ClientFactory.Create(endpoint, null);
                    try
                    {
                        cassandraClient.Open();
                        string clusterName = cassandraClient.getClusterName();
                        isUp = true;
                        LOGGER.Debug(LOGGER.StringFormatInvariantCulture("Endpoint '{0}' s up again", endpoint));
                    }
                    catch
                    {
                        LOGGER.Debug(LOGGER.StringFormatInvariantCulture("Endpoint '{0}' still down", endpoint));
                        // this endpoint is still down :(
                        isUp = false;
                    }
                    finally
                    {
                        cassandraClient.Close();
                    }

                    if (isUp)
                    {
                        lock (this.blackListedEndpoints)
                        {
                            this.blackListedEndpoints.Remove(endpoint);
                        }
                        endpoint.UpDateTime = DateTime.Now;
                        lock (this.availableEndpoints)
                        {
                            this.availableEndpoints.Enqueue(endpoint);
                        }
                        LOGGER.Debug(LOGGER.StringFormatInvariantCulture("Endpoint '{0}' moved to available queue.", endpoint));
                    }
                    else
                    {
                        areThereLeftBehind = true;
                    }
                }
                wasProperlyFinished = true;
            }
            catch (Exception ex)
            {
                LOGGER.Error(ex);
            }
            finally
            {
                if (wasProperlyFinished && areThereLeftBehind)
                {
                    LOGGER.Debug("There are endpoints still blacklisted.");
                    // i should reactive myself again 
                    this.startTimer(this.PeriodicTime);
                }
                else if (!wasProperlyFinished)
                {
                    LOGGER.Debug("Endpoint recovery crashed. It will restart sooner.");
                    // damn, i want my revenge!
                    this.startTimer(this.DueTime);
                }
                else
                {
                    // i get to here, im done... im gonna go to the beach! :)
                    LOGGER.Debug("Endpoint recovery is done!.");
                }
            }
        }

        private void stopTimer()
        {
            this.endpointRecoveryTimerStarted = false;
            this.endpointRecoveryTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void startTimer(int dueTime)
        {
            if (!this.endpointRecoveryTimerStarted)
            {
                this.endpointRecoveryTimerStarted = true;
                this.endpointRecoveryTimer.Change(dueTime, Timeout.Infinite);
            }
        }
    }
}
