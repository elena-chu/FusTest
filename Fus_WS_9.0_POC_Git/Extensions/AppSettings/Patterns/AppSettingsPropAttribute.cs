using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ws.Extensions.AppSettings.Patterns
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class AppSettingsPropAttribute : Attribute
    {
        public string Name { get; }

        public AppSettingsPropAttribute(string name)
        {
            Name = name;
        }
    }

    interface IData { }

    interface Int1<T> where T : IData
    {
        IEnumerable<T> Items { get; }
    }

    interface Int2<out T> where T : IData
    {
        IEnumerable<T> Items { get; }
    }

    class Data : IData
    {
    }

    class Class1 : Int1<Data>
    {
        public IEnumerable<Data> Items
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }

    class Class2 : Int2<Data>
    {
        public IEnumerable<Data> Items
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
