using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aquiles.Core
{
    public interface IFactory<T>
    {
        T Create();
    }
}
