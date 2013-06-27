using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aquiles.Core.Client
{
    public class ConnectionConfig
    {
        public int? ReceiveTimeout
        {
            get;
            set;
        }

        public int? SendTimeout
        {
            get;
            set;
        }
    }
}
