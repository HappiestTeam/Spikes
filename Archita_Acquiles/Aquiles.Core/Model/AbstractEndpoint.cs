using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace Aquiles.Core.Model
{
    public abstract class AbstractEndpoint : IEndpoint
    {
        #region IEndpoint Members

        public DateTime UpDateTime
        {
            get;
            set;
        }

        public string Address
        {
            get;
            set;
        }

        public int Port
        {
            get;
            set;
        }

        public int Timeout
        {
            get;
            set;
        }

        #endregion


        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture, "{0}:{1}-{2}", this.Address, this.Port, this.Timeout);
        }
    }
}
