using Ws.Extensions.Patterns;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ws.Extensions.Configuration.Patterns
{
    public class Configuration<T> : Wrapper<T>
    {
        public Configuration(IEnumerable<string> jsonFiles)
        {
            ConfigurationBuilder builder = new ConfigurationBuilder();

            foreach (var file in jsonFiles)
                builder.AddJsonFile(file, true);

            var config = builder.Build();

            var settings = config.Get<T>();

            if (settings != null)
                Value = settings;

            //if (Value == null)
            //{
            //    try
            //    {
            //        Value = Activator.CreateInstance<T>();
            //    }
            //    catch
            //    { /* ignore exception */ }
            //}
        }
    }
}
