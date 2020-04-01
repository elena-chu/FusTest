using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ws.Extensions.Patterns
{
    public class Wrapper<T> : IWrapper<T>
    {
        public T Value { get; set; }

        public Wrapper()
        {
            try
            {
                Value = Activator.CreateInstance<T>();
            }
            catch
            { /* ignore exception */ }
        }

        public Wrapper(T value)
        {
            Value = value;
        }
    }
}
