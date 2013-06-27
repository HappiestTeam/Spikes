using System;
using System.Collections.Generic;
using System.Text;

namespace Aquiles.Core.Model
{
    public interface IEndpoint
    {
        DateTime UpDateTime
        {
            get;
            set;
        }

        string Address
        {
            get;
        }

        int Port
        {
            get;
        }

        int Timeout
        {
            get;
        }
    }

}
