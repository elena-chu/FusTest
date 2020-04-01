using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ws.Extensions.Mvvm.ViewModels
{
    public class BindableWrapper<T> : BindableBase
    {
        public T Value;

        public BindableWrapper(T value)
        {
            Value = value;
        }
    }
}
