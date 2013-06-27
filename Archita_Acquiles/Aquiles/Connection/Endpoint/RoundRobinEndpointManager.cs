using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aquiles.Model.Internal;
using Aquiles.Logging;
using System.Threading;
using Aquiles.Connection.Factory;

namespace Aquiles.Connection.Endpoint
{
    internal class RoundRobinEndpointManager : IEndpointManager
    {
        private LoggerHelper logger;
        private const int DEFAULTDUETIME = 5000; // 5 sec
        private const int DEFAULTPERIODICTTIME = 60000; // 1 min

        private List<CassandraEndpoint> configuredEndpoints;
        private Queue<CassandraEndpoint> availableEndpoints;
        private HashSet<CassandraEndpoint> blackListedEndpoints;
        private Timer endpointRecoveryTimer;

        public RoundRobinEndpointManager()
        {
            logger = LoggerHelper.CreateLogger(this.GetType());
            this.configuredEndpoints = new List<CassandraEndpoint>();
            this.availableEndpoints = new Queue<CassandraEndpoint>(this.configuredEndpoints);
            this.blackListedEndpoints = new HashSet<CassandraEndpoint>();
            this.endpointRecoveryTimer = new Timer(new TimerCallback(this.EndpointRecoveryMethod), null, Timeout.Infinite, Timeout.Infinite);
        }

        #region IEndpointManager Members

        public void AddEndpoint(CassandraEndpoint cassandraEndpoint)
        {
            this.availableEndpoints.Enqueue(cassandraEndpoint);
            this.configuredEndpoints.Add(cassandraEndpoint);
        }

        public List<CassandraEndpoint> Endpoints
        {
            get { return new List<CassandraEndpoint>(this.configuredEndpoints); }
        }

        public CassandraEndpoint Retrieve()
        {
            CassandraEndpoint borrowedEndpoint = null;
            do
            {
                borrowedEndpoint = null;
                lock (this.availableEndpoints)
                {
                    if (this.availableEndpoints.Count > 0)
                    {
                        borrowedEndpoint = this.availableEndpoints.Dequeue();
                    }
                    else
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (CassandraEndpoint endpoint in this.configuredEndpoints)
                        {
                            if (sb.Length > 0)
                            {
                                sb.Append("','");
                            }
                            sb.Append(endpoint.ToString());
                        }
                        logger.Fatal(logger.StringFormatInvariantCulture("All endpoints ['{0}'] are blacklisted, cluster is down?", sb.ToString()));
                    }
                }
            } while ((borrowedEndpoint != null) && (this.IsBlackListed(borrowedEndpoint)));

            if (borrowedEndpoint != null)
            {
                lock (this.availableEndpoints)
                {
                    this.availableEndpoints.Enqueue(borrowedEndpoint);
                }
            }
            return borrowedEndpoint;
        }

        public void Ban(CassandraEndpoint endpoint)
        {
            lock (this.blackListedEndpoints)
            {
                if (!this.blackListedEndpoints.Contains(endpoint))
                {
                    this.blackListedEndpoints.Add(endpoint);
                }
            }

            logger.Warn(logger.StringFormatInvariantCulture("Endpoint '{0}' was banned.", endpoint.ToString()));
        }

        public IConnectionFactory ConnectionFactory
        {
            set;
            private get;
        }
        #endregion

        private bool IsBlackListed(CassandraEndpoint borrowedEndpoint)
        {
            bool isBlackListed = false;
            if (this.blackListedEndpoints.Count > 0)
            {
                lock (this.blackListedEndpoints)
                {
                    isBlackListed = this.blackListedEndpoints.Contains(borrowedEndpoint);
                    this.startTimer(DEFAULTDUETIME);
                }
            }
            this.logger.Debug(logger.StringFormatInvariantCulture("Endpoint: '{0}' blacklisted: {1}", borrowedEndpoint, isBlackListed));
            return isBlackListed;
        }

        private void EndpointRecoveryMethod(object state)
        {
            this.logger.Info("Stoping EndpointRecovery Timer.");
            bool wasProperlyFinished = false;
            bool areThereLeftBehind = false;
            // need to stop the time to avoid 2 methods run concurrent
            this.stopTimer();
            try
            {
                this.logger.Debug("Getting blacklisted endpoints.");
                HashSet<CassandraEndpoint> clonedBlackList = null;
                lock (this.blackListedEndpoints)
                {
                    clonedBlackList = new HashSet<CassandraEndpoint>(this.blackListedEndpoints);
                }

                bool isUp = false;
                //now i can work without problems
                foreach (CassandraEndpoint endpoint in clonedBlackList)
                {
                    this.logger.Debug(logger.StringFormatInvariantCulture("Checking on '{0}'", endpoint));
                    CassandraClient cassandraClient = this.ConnectionFactory.Create(endpoint);
                    try
                    {
                        cassandraClient.OpenTransport();
                        string clusterName = cassandraClient.InnerClient.describe_cluster_name();
                        isUp = true;
                        this.logger.Debug(logger.StringFormatInvariantCulture("Endpoint '{0}' s up again", endpoint));
                    }
                    catch
                    {
                        this.logger.Debug(logger.StringFormatInvariantCulture("Endpoint '{0}' still down", endpoint));
                        // this endpoint is still down :(
                        isUp = false;
                    }
                    finally
                    {
                        cassandraClient.CloseTransport();
                    }

                    if (isUp)
                    {
                        lock (this.blackListedEndpoints)
                        {
                            this.blackListedEndpoints.Remove(endpoint);
                        }

                        lock (this.availableEndpoints)
                        {
                            this.availableEndpoints.Enqueue(endpoint);
                        }
                        this.logger.Debug(logger.StringFormatInvariantCulture("Endpoint '{0}' moved to available queue.", endpoint));
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
                logger.Error(ex);
            }
            finally
            {
                if (wasProperlyFinished && areThereLeftBehind)
                {
                    this.logger.Debug("There are endpoints still blacklisted.");
                    // i should reactive myself again 
                    this.startTimer(DEFAULTPERIODICTTIME);
                }
                else if (!wasProperlyFinished)
                {
                    this.logger.Debug("I crashed, so i will reactivate sooner.");
                    // damn, i want my revenge!
                    this.startTimer(DEFAULTDUETIME);
                }
                else
                {
                    // i get to here, im done... im gonna go to the beach! :)
                    this.logger.Debug("Work is done!.");
                }
            }
        }

        private void stopTimer()
        {
            this.endpointRecoveryTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void startTimer(int dueTime)
        {
            this.endpointRecoveryTimer.Change(dueTime, Timeout.Infinite);
        }
    }
}
