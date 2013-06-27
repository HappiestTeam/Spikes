using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aquiles.Exceptions;
using System.Globalization;

namespace Aquiles.Connection.Factory
{
    internal static class ConnectionFactoryHelper
    {
        public static IConnectionFactory Create(ConnectionFactoryType factoryType)
        {
            IConnectionFactory connectionFactory = null;
            switch (factoryType)
            {
                case ConnectionFactoryType.DEFAULT:
                    connectionFactory = new DefaultTransportConnectionFactory();
                    break;
                case ConnectionFactoryType.BUFFERED:
                    connectionFactory = new BufferedTransportConnectionFactory();
                    break;
                default:
                    throw new AquilesException(String.Format(CultureInfo.CurrentCulture, "No implementation found for '{0}'", factoryType));
            }

            return connectionFactory;
        }
    }
}
