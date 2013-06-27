using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CassandraClient = Apache.Cassandra.Cassandra.Client;

namespace Aquiles.Cassandra10
{
    public delegate object ExecutionBlock(CassandraClient client);
}
