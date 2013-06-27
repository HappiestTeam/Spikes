using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Aquiles.Helpers.Encoders;
using Aquiles.Core.Cluster.Impl;
using Aquiles.Core.Diagnostics.Impl;
using Aquiles.Core.Connection.Pooling.Impl;
using Aquiles.Core.Connection.Factory;
using Aquiles.Core.Client;
using Aquiles.Core.Connection.EndpointManager;
using Aquiles.Core.Model;
using Aquiles.Core.Model.Impl;

namespace Aquiles.Core.Test
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class DefaultClusterTest
    {

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void MyTestInitialize()
        {
        }


        // Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
        }

        #endregion

        [TestMethod()]
        public void DefaultClusterTestOne()
        {
            TraceLogger logger = new TraceLogger();
            DefaultCluster cluster = new DefaultCluster();
            cluster.Logger = logger;
            cluster.MaximumRetries = 1;
            cluster.Name = "TestCluster";
            SizeControlledClientPool pool = new SizeControlledClientPool();
            pool.Name = "TestPool";
            pool.ClientFactory = new MyClientFactory();
            pool.DueTime = 2000;
            MyEndpointManager endpointManager = new MyEndpointManager();
            endpointManager.Name = "Endpoint";
            endpointManager.Endpoints = new List<IEndpoint>();
            endpointManager.Endpoints.Add(new DefaultEndpoint() { Address = "127.0.0.1", Port = 9160, Timeout = 1000, UpDateTime=DateTime.Now });
            pool.EndpointManager = endpointManager;
            pool.Logger = logger;
            pool.MagicNumber = 7;
            pool.MaximumClientsToSupport = 10;
            pool.MaximumRetriesToPollClient = 1;
            pool.MinimumClientsToKeep = 1;
            pool.PeriodicTime = 2000;
            cluster.PoolManager = pool;

            for (int i = 0; i < 100; i++)
            {
                IClient client = cluster.Borrow();
                cluster.Release(client);
            }
        }
    }

    public class MyClientFactory : IConnectionFactory
    {
        #region IConnectionFactory Members

        public Client.IClient Create(Model.IEndpoint endpoint, Connection.Pooling.IClientPool ownerPool)
        {
            return new MyClient() { Endpoint = endpoint, OwnerPool = ownerPool };
        }

        #endregion
    }

    public class MyClient : AbstractClient
    {
        public override void ResetToDefaultConnectionConfig()
        {
            // DO NOTHING
        }

        public override void OverrideConnectionConfig(ConnectionConfig config)
        {
            // DO NOTHING
        }

        public override string getClusterName()
        {
            return String.Empty;
        }

        public override object Execute(Delegate executionBlock)
        {
            // DO NOTHING
            return null;
        }

        public override void Close()
        {
            // DO NOTHING
        }

        public override bool IsOpen()
        {
            return true;
        }

        public override void Open()
        {
            // DO NOTHING
        }

        public override string KeyspaceName
        {
            get;
            set;
        }
    }

    public class MyEndpointManager : IEndpointManager
    {
        #region IEndpointManager Members

        public List<Model.IEndpoint> Endpoints
        {
            set;
            get;
        }

        public string Name
        {
            set;
            get;
        }

        public Model.IEndpoint Pick()
        {
            return this.Endpoints[0];
        }

        public void Ban(Model.IEndpoint endpoint, DateTime connectionCreationTime)
        {
            // do nothing
        }

        #endregion
    }
}
