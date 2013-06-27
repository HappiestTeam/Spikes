using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aquiles
{
    internal class UnixHelper
    {
        private UnixHelper() { }

        static readonly DateTime unitStartDateTime = new DateTime(1970, 1, 1);

        public static long UnixTimestamp
        {
            get { return Convert.ToInt64((DateTime.UtcNow - unitStartDateTime).TotalMilliseconds); }
        }
    }
}
