using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aquiles.Core.Client;
using Apache.Cassandra;
using CassandraClient = Apache.Cassandra.Cassandra.Client;
using Aquiles.Core.Model;
using Aquiles.Core.Connection.Pooling;
using Aquiles.Core.Cluster.Impl;
using Aquiles.Core;
using Aquiles.Core.Exceptions;
using Thrift;
using Thrift.Transport;
using System.IO;
using System.Reflection;
using System.Net.Sockets;

namespace Aquiles.Cassandra10.Client
{
    public class DefaultClient : AbstractClient
    {
        private bool sendKeyspace = false;

        private CassandraClient cassandraClient;
        public CassandraClient CassandraClient
        {
            get
            {
                return this.cassandraClient;
            }
            set
            {
                this.cassandraClient = value;
            }
        }

        internal TcpClient TcpClient
        {
            set;
            get;
        }

        private string keyspaceName;
        public override string KeyspaceName
        {
            get 
            { 
                return this.keyspaceName; 
            }
            set 
            { 
                this.keyspaceName = value;
                this.sendKeyspace = !String.IsNullOrEmpty(this.keyspaceName);
            }
        }
        public override void Open()
        {
            if (this.disposed)
                throw new ObjectDisposedException("DefaultClient");

            if (this.CassandraClient != null)
            {
                this.CassandraClient.InputProtocol.Transport.Open();
                if (!this.CassandraClient.InputProtocol.Transport.Equals(this.CassandraClient.OutputProtocol.Transport))
                {
                    this.CassandraClient.OutputProtocol.Transport.Open();
                }
            }
            else
            {
                throw new AquilesException("Cannot open a client when no cassandra client was associated with it.");
            }
        }

        public override void Close()
        {
            if (this.disposed)
                throw new ObjectDisposedException("DefaultClient");

            this.CassandraClient.InputProtocol.Transport.Close();
            if (!this.CassandraClient.InputProtocol.Transport.Equals(this.CassandraClient.OutputProtocol.Transport))
            {
                this.CassandraClient.OutputProtocol.Transport.Close();
            }
        }

        public override bool IsOpen()
        {
            if (this.disposed)
                throw new ObjectDisposedException("DefaultClient");

            if ((this.CassandraClient != null)
                && (this.CassandraClient.InputProtocol != null)
                && (this.CassandraClient.OutputProtocol != null)
                && (this.CassandraClient.InputProtocol.Transport != null)
                && (this.CassandraClient.OutputProtocol.Transport != null))
            {
                return this.CassandraClient.InputProtocol.Transport.IsOpen && this.CassandraClient.OutputProtocol.Transport.IsOpen;
            }
            return false;
        }

        public override string getClusterName()
        {
            if (this.disposed)
                throw new ObjectDisposedException("DefaultClient");

            return this.CassandraClient.describe_cluster_name();
        }

        public override Object Execute(Delegate executionBlock)
        {
            if (this.disposed)
                throw new ObjectDisposedException("DefaultClient");

            long startedTicks = DateTime.Now.Ticks;


            if (this.sendKeyspace)
            {
                try
                {
                    this.sendKeyspace = false;
                    this.cassandraClient.set_keyspace(this.keyspaceName);
                }
                catch (Exception ex)
                {
                    throw this.buildException(ex);
                }
            }

            try
            {
                return executionBlock.DynamicInvoke(this.CassandraClient);
            }
            //catch (TargetException ex)
            //{
            //    throw new ExecutionBlockException(ex.Message, ex) { IsConnectionStillHealthy = true, ShouldRetry = false };
            //}
            //catch (MemberAccessException ex)
            //{
            //    throw new ExecutionBlockException(ex.Message, ex) { IsConnectionStillHealthy = true, ShouldRetry = false };
            //}
            catch (TargetInvocationException ex)
            {
                Exception inner = ex.InnerException;
                throw this.buildException(inner);
            }
            finally
            {
                TimeSpan duration = new TimeSpan(DateTime.Now.Ticks - startedTicks);
            }
        }

        public override void OverrideConnectionConfig(ConnectionConfig config)
        {
            if (config.ReceiveTimeout != null && config.ReceiveTimeout.HasValue)
            {
                this.TcpClient.ReceiveTimeout = config.ReceiveTimeout.Value;
            }

            if (config.SendTimeout != null && config.SendTimeout.HasValue)
            {
                this.TcpClient.SendTimeout = config.SendTimeout.Value;
            }
        }

        public override void ResetToDefaultConnectionConfig()
        {
            this.TcpClient.ReceiveTimeout = this.TcpClient.SendTimeout = this.Endpoint.Timeout;
        }

        private ExecutionBlockException buildException(Exception exception)
        {
            Type exceptionType = exception.GetType();
            if (exceptionType.Equals(typeof(NotFoundException)))
            {
                //WTF?? "not found" should be trapped.
                string error = BuildExceptionMessage(exception);
                // despite this command failed, the connection is still usable.
                return new ExecutionBlockException(error, exception) { IsClientHealthy = true, ShouldRetry = false };
            } 
            else if (exceptionType.Equals(typeof(InvalidRequestException)))
            {
                InvalidRequestException exception2 = (InvalidRequestException) exception;
                string error = BuildExceptionMessage(exception, exception2.Why);
                // despite this command failed, the connection is still usable.
                return new ExecutionBlockException(error, exception) { IsClientHealthy = true, ShouldRetry = false };
            }
            else if (exceptionType.Equals(typeof(UnavailableException)))
            {
                //WTF?? Cassandra cannot ensure object replication state?
                string error = BuildExceptionMessage(exception);
                return new ExecutionBlockException(error, exception) { IsClientHealthy = true, ShouldRetry = true };
            }
            else if (exceptionType.Equals(typeof(TimedOutException)))
            {
                //WTF?? Cassandra timeout?
                string error = BuildExceptionMessage(exception);
                return new ExecutionBlockException(error, exception) { IsClientHealthy = true, ShouldRetry = true };
            }
            else if (exceptionType.Equals(typeof(TApplicationException)))
            {
                // client thrift version does not match server version?
                string error = BuildExceptionMessage(exception);
                return new ExecutionBlockException(error, exception) { IsClientHealthy = false, ShouldRetry = false };
            }
            else if (exceptionType.Equals(typeof(AuthenticationException)))
            {
                AuthenticationException exception2 = (AuthenticationException) exception;
                // invalid credentials
                string error = BuildExceptionMessage(exception, exception2.Why);
                // despite this command failed, the connection is still usable.
                return new ExecutionBlockException(error, exception2) { IsClientHealthy = true, ShouldRetry = false };
            }
            else if (exceptionType.Equals(typeof(AuthorizationException)))
            {
                AuthorizationException exception2 = (AuthorizationException) exception;
                // user does not have access to keyspace
                string error = BuildExceptionMessage(exception2, exception2.Why);
                // despite this command failed, the connection is still usable.
                return new ExecutionBlockException(error, exception2) { IsClientHealthy = true, ShouldRetry = false };
            }
            else if (exceptionType.Equals(typeof(TTransportException)))
            {
                // user does not have access to keyspace
                string error = String.Format("{0} This might happen when the transport configured on the client does not match the server configuration.", BuildExceptionMessage(exception));
                // this connection is screwed
                return new ExecutionBlockException(error, exception) { IsClientHealthy = false, ShouldRetry = true };
            }
            else if (exceptionType.Equals(typeof(IOException)))
            {
                // i got the client, it was opened but the node crashed or something
                string error = BuildExceptionMessage(exception);
                return new ExecutionBlockException(error, exception) { IsClientHealthy = false, ShouldRetry = true };
            }
            else
            {
                return new ExecutionBlockException("Unhandled exception.", exception);
            }

        }
    }
}
