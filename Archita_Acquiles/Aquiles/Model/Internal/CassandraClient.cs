using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apache.Cassandra;
using System.Globalization;

namespace Aquiles.Model.Internal
{
    internal class CassandraClient
    {
        internal Cassandra.Client InnerClient
        {
            get;
            private set;
        }

        internal CassandraEndpoint Endpoint
        {
            get;
            private set;
        }
        
        public CassandraClient(Cassandra.Client cassandraClient, CassandraEndpoint endpoint)
        {
            this.InnerClient = cassandraClient;
            this.Endpoint = endpoint;
        }

        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture, "{0}-{1}", this.InnerClient.ToString(), this.Endpoint.ToString());
        }

        public void OpenTransport()
        {
            this.InnerClient.InputProtocol.Transport.Open();
            if (!this.InnerClient.InputProtocol.Transport.Equals(this.InnerClient.OutputProtocol.Transport))
            {
                this.InnerClient.OutputProtocol.Transport.Open();
            }
        }

        public void CloseTransport()
        {
            this.InnerClient.InputProtocol.Transport.Close();
            if (!this.InnerClient.InputProtocol.Transport.Equals(this.InnerClient.OutputProtocol.Transport))
            {
                this.InnerClient.OutputProtocol.Transport.Close();
            }
        }

        public bool IsOpened
        {
            get
            {
                if ( (this.InnerClient != null) 
                    && (this.InnerClient.InputProtocol != null) 
                    && (this.InnerClient.OutputProtocol != null)
                    && (this.InnerClient.InputProtocol.Transport != null) 
                    && (this.InnerClient.OutputProtocol.Transport != null) )
                {
                    return this.InnerClient.InputProtocol.Transport.IsOpen && this.InnerClient.OutputProtocol.Transport.IsOpen;
                }
                return false;
            }
        }
    }
}
