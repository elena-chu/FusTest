using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ws.Extensions.Mvvm.ViewModels
{
    public class ViewModelBase : BindableBase
    {
        //public object Model { get; protected set; }
    }

    public class ViewModelBase<T> : ViewModelBase
    {
        public ViewModelBase(T model)
        {
            Model = model;
        }

        public /*new*/ T Model { get; protected set; }
    }

}
