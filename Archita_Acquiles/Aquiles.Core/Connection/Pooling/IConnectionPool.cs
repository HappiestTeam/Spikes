using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aquiles.Core.Model;
using Aquiles.Core.Client;

namespace Aquiles.Core.Connection.Pooling
{
    public interface IClientPool
    {
        string Name {
            get;
        }
        IClient Borrow();
        void Release(IClient client);
        void Invalidate(IClient client);

        void Attach(IClient client);
        void Detach(IClient client);
    }
}
