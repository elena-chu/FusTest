using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ws.Extensions.Patterns
{
    public interface IWrapper<T>
    {
        T Value { get; }
    }
}
