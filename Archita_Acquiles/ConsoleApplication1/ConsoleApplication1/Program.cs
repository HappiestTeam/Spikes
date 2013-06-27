using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Apache.Cassandra;
using Thrift;
using Aquiles.Core;
using Aquiles.Core.Configuration;
using Aquiles.Helpers;
using Aquiles.Helpers.Encoders;
using Aquiles.Core.Cluster;

using Aquiles.Cassandra10;



namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            const string KEYSPACENAME = "las";

            //byte[] key = ByteEncoderHelper.UTF8Encoder.ToByteArray("aravind.d");
            //byte[] userName = ByteEncoderHelper.UTF8Encoder.ToByteArray("aravind.d");
            //byte[] firstName = ByteEncoderHelper.UTF8Encoder.ToByteArray("Aravind");
            //byte[] lastName = ByteEncoderHelper.UTF8Encoder.ToByteArray("Dhakshinamoorthy");
            //byte[] password = ByteEncoderHelper.UTF8Encoder.ToByteArray("test1234");
            //byte[] email = ByteEncoderHelper.UTF8Encoder.ToByteArray("aravind.d@happiestminds.com");
                      
            //ColumnParent columnParent = new ColumnParent();
            //Column userNameColumn = new Column()
            //{
            //    Name = ByteEncoderHelper.UTF8Encoder.ToByteArray("user_name"),
            //    Timestamp = UnixHelper.UnixTimestamp,
            //    Value = userName
            //};

            //Column firstNameColumn = new Column()
            //{
            //    Name = ByteEncoderHelper.UTF8Encoder.ToByteArray("first_name"),
            //    Timestamp = UnixHelper.UnixTimestamp,
            //    Value = firstName
            //};

            //Column lastNameColumn = new Column()
            //{
            //    Name = ByteEncoderHelper.UTF8Encoder.ToByteArray("last_name"),
            //    Timestamp = UnixHelper.UnixTimestamp,
            //    Value = lastName
            //};

            //Column passwordColumn = new Column()
            //{
            //    Name = ByteEncoderHelper.UTF8Encoder.ToByteArray("password"),
            //    Timestamp = UnixHelper.UnixTimestamp,
            //    Value = password
            //};

            //Column emailColumn = new Column()
            //{
            //    Name = ByteEncoderHelper.UTF8Encoder.ToByteArray("email"),
            //    Timestamp = UnixHelper.UnixTimestamp,
            //    Value = email
            //};

            //columnParent.Column_family = "user";

            //ICluster cluster = Aquiles.Cassandra10.AquilesHelper.RetrieveCluster("Test Cluster");
            //cluster.Execute(new Aquiles.Cassandra10.ExecutionBlock(delegate(Cassandra.Client  client)
            //{
            //    client.insert(key, columnParent, userNameColumn, ConsistencyLevel.ONE);
            //    client.insert(key, columnParent, firstNameColumn, ConsistencyLevel.ONE);
            //    client.insert(key, columnParent, lastNameColumn, ConsistencyLevel.ONE);
            //    client.insert(key, columnParent, passwordColumn, ConsistencyLevel.ONE);
            //    client.insert(key, columnParent, emailColumn, ConsistencyLevel.ONE);

            //    return null;
            //}), KEYSPACENAME);

             //Fetch inserted data
            byte[] key = ByteEncoderHelper.UTF8Encoder.ToByteArray("archita");
            ICluster cluster = Aquiles.Cassandra10.AquilesHelper.RetrieveCluster("Test Cluster");
            byte[] userName = ByteEncoderHelper.UTF8Encoder.ToByteArray("aravind.d");

            ColumnPath columnPath = new ColumnPath()
            {
                Column = ByteEncoderHelper.UTF8Encoder.ToByteArray("user_name"),
                Column_family = "user",
            };

            ColumnOrSuperColumn columnOrSuperColumn = null;

            cluster.Execute(new ExecutionBlock(delegate(Cassandra.Client client)
            {
                columnOrSuperColumn = client.get(key, columnPath, ConsistencyLevel.ONE);
                return columnOrSuperColumn;
            }), KEYSPACENAME);
           
            if (columnOrSuperColumn.Column.Value.SequenceEqual<byte>(userName))
            {
                Console.WriteLine("Value found");
                Console.WriteLine(System.Text.Encoding.Default.GetString(columnOrSuperColumn.Column.Value));
                Console.WriteLine("Success ! ");
            }
            else
            {
                Console.WriteLine("Value was not found");
                Console.WriteLine("Failure ! ");
            }
            Console.ReadLine();


        }
    }
}
