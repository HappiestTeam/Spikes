using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aquiles.Logging;
using Aquiles.Model.Internal;
using Aquiles.Connection.Pooling;
using Aquiles.Exceptions;
using Apache.Cassandra;
using Thrift;
using System.Net.Sockets;
using System.IO;
using System.Globalization;

namespace Aquiles.Impl
{
    internal class DefaultAquilesConnection : IAquilesConnection
    {
        private LoggerHelper logger;

        private CassandraClient cassandraClient;
        private IAquilesCluster cluster;
        private bool isClientHealthy;

        private DefaultAquilesConnection()
        {
            this.isClientHealthy = true;
            this.logger = LoggerHelper.CreateLogger(this.GetType());
        }


        internal DefaultAquilesConnection(IAquilesCluster cluster) 
            : this()
        {
            this.cluster = cluster;
        }

        #region AquilesConnection Members
        public void Open()
        {
            long startedTicks = DateTime.Now.Ticks;
            this.cassandraClient = this.cluster.ConnectionPool.Borrow();
            TimeSpan duration = new TimeSpan(DateTime.Now.Ticks - startedTicks);
            logger.Info(logger.StringFormatInvariantCulture("Getting a connection took {0} ms.", duration.Milliseconds));
            if (this.cassandraClient != null)
            {
                this.logger.Debug("Connection opened");
            }
            else
            {
                string message = String.Format(CultureInfo.CurrentCulture, "Couldn't retrieve a client from the pool whose cluster parent is '{0}'.", this.cluster.FriendlyName);
                this.logger.Fatal(message);
                throw new AquilesException(message);
            }
        }

        public void Close()
        {
            long startedTicks = DateTime.Now.Ticks;
            if (this.isClientHealthy)
            {
                this.cluster.ConnectionPool.Release(this.cassandraClient);
                this.logger.Debug("Connection released.");
            }
            else
            {
                this.cluster.ConnectionPool.Invalidate(this.cassandraClient);
                this.logger.Debug("Connection returned as invalid.");
            }
            this.cassandraClient = null;
            TimeSpan duration = new TimeSpan(DateTime.Now.Ticks - startedTicks);
            logger.Info(logger.StringFormatInvariantCulture("Releasing / invalidating a connection took {0} ms.", duration.Milliseconds));
        }

        public void Execute(IAquilesCommand command)
        {
            if (command != null)
            {
                this.ExecuteCommand(command);
            }
            else
            {
                throw new ArgumentNullException("command");
            }
        }

        private void ExecuteCommand(IAquilesCommand command)
        {
            long startedTicks = DateTime.Now.Ticks;
            command.ValidateInput(cluster.KeySpaces);

            bool isClientStillHealthy = false;
            bool openedInScope = false;
            bool shouldRetry = false;
            do
            {
                openedInScope = this.OpenCassandraClientConnection(openedInScope);

                try
                {
                    this.logger.Debug("executing command.");
                    command.Execute(this.cassandraClient.InnerClient);
                    isClientStillHealthy = true;
                    shouldRetry = false;
                }
                catch (NotFoundException ex)
                {
                    //WTF?? "not found" should be trapped.
                    string error = BuildExceptionMessage(ex);
                    logger.Error(error, ex);
                    // despite this command failed, the connection is still usable.
                    isClientStillHealthy = true;
                    throw new AquilesException(error, ex);
                }
                catch (InvalidRequestException ex)
                {
                    //WTF?? "invalid request" should checked on the command method "ValidateRequest"
                    string error = BuildExceptionMessage(ex, ex.Why);
                    logger.Error(error, ex);
                    // despite this command failed, the connection is still usable.
                    isClientStillHealthy = true;
                    throw new AquilesException(error, ex);
                }
                catch (UnavailableException ex)
                {
                    //WTF?? Cassandra cannot ensure object replication state?
                    string error = BuildExceptionMessage(ex);
                    logger.Error(error, ex);
                    throw new AquilesException(error, ex);
                }
                catch (TimedOutException ex)
                {
                    //WTF?? Cassandra timeout?
                    string error = BuildExceptionMessage(ex);
                    logger.Error(error, ex);
                    throw new AquilesException(error, ex);
                }
                catch (TApplicationException ex)
                {
                    // client thrift version does not match server version?
                    string error = BuildExceptionMessage(ex);
                    logger.Error(error, ex);
                    throw new AquilesException(error, ex);
                }
                catch (AuthenticationException ex)
                {
                    // invalid credentials
                    string error = BuildExceptionMessage(ex, ex.Why);
                    logger.Error(error, ex);
                    // despite this command failed, the connection is still usable.
                    isClientStillHealthy = true;
                    throw new AquilesException(error, ex);
                }
                catch (AuthorizationException ex)
                {
                    // user does not have access to keyspace
                    string error = BuildExceptionMessage(ex, ex.Why);
                    logger.Error(error, ex);
                    // despite this command failed, the connection is still usable.
                    isClientStillHealthy = true;
                    throw new AquilesException(error, ex);
                }
                catch (IOException ex)
                {
                    // i got the client, it was opened but the node crashed or something
                    if (openedInScope)
                    {
                        logger.Warn(logger.StringFormatInvariantCulture("Exception '{0}', retrying on a different connection.", ex), ex);
                        shouldRetry = true;
                    }
                    else
                    {
                        string error = BuildExceptionMessage(ex);
                        logger.Error(error, ex);
                        // despite this command failed, the connection is still usable.
                        throw new AquilesException(error, ex);
                    }
                }
                finally
                {
                    this.isClientHealthy = isClientStillHealthy;
                    if (openedInScope)
                    {
                        this.logger.Debug("connection opened in scope, closing.");
                        this.Close();
                    }
                }
            } while (shouldRetry);
            TimeSpan duration = new TimeSpan(DateTime.Now.Ticks - startedTicks);
            logger.Info(logger.StringFormatInvariantCulture("Command '{0}' took {1} ms.", command, duration.Milliseconds));
        }

        private bool OpenCassandraClientConnection(bool openedInScope)
        {
            if (this.cassandraClient == null)
            {
                this.logger.Debug("connection not opened.");
                this.Open();
                openedInScope = true;
            }
            return openedInScope;
        }

        #endregion

        #region IDisposable Members
        private bool disposed = false;
        public void Dispose()
        {
			this.Dispose(true);
			GC.SuppressFinalize(this);
        }

        private void Dispose(Boolean forceDispose) {
            if (!this.disposed)
            {
                this.disposed = true;
                if (forceDispose)
                {
                    if (this.cassandraClient != null)
                    {
                        this.Close();
                    }
                }
            }
        }

        ~DefaultAquilesConnection()
        {
            this.Dispose(false);
        }
        #endregion

        private static string BuildExceptionMessage(Exception ex)
        {
            return BuildExceptionMessage(ex, null);
        }
        private static string BuildExceptionMessage(Exception ex, string why)
        {
            if (String.IsNullOrEmpty(why))
            {
                return String.Format(CultureInfo.CurrentCulture, "Exception '{0}' during executing command. See inner exception for further details.", ex.Message);
            }
            else
            {
                return String.Format(CultureInfo.CurrentCulture, "Exception '{0}' during executing command: '{1}'. See inner exception for further details.", ex.Message, why);
            }
        }
    }
}
