using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aquiles.Exceptions;
using System.Globalization;

namespace Aquiles.Connection.Pooling
{
    internal static class ConnectionPoolHelper
    {
        public static IConnectionPool Create(ConnectionPoolType poolType) {
            IConnectionPool connectionPool = null;
            switch (poolType)
            {
                case ConnectionPoolType.NOPOOL:
                    connectionPool = new NoConnectionPool();
                    break;
                case ConnectionPoolType.SIZECONTROLLEDPOOL:
                    connectionPool = new SizeControlledConnectionPool();
                    break;
                default:
                    throw new AquilesException(String.Format(CultureInfo.CurrentCulture, "No implementation found for '{0}'", poolType));
            }

            return connectionPool;
        }
    }
}
