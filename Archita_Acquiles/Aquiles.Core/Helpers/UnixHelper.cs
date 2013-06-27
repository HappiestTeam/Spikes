using System;
using System.Collections.Generic;

using System.Text;

namespace Aquiles.Helpers
{
    public class UnixHelper
    {
        private static Int64 MULTIPLIERCONSTANT = 1000;
        private static DateTimePrecise preciseDatetime = new DateTimePrecise(10);

        private UnixHelper() { }

        static readonly DateTime unitStartDateTime = new DateTime(1970, 1, 1).ToUniversalTime();

        public static long UnixTimestamp
        {
            get 
            {
                TimeSpan difference = preciseDatetime.UtcNow - unitStartDateTime;
                return Convert.ToInt64(difference.TotalMilliseconds * MULTIPLIERCONSTANT); 
            }
        }
    }
}
