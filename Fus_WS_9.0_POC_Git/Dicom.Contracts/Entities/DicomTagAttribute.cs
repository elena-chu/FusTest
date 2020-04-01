using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ws.Dicom.Interfaces.Entities
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class DicomTagAttribute : Attribute
    {
        public static readonly DicomTagAttribute Default = new DicomTagAttribute(0);

        public uint Value { get; }

        public DicomTagAttribute(uint value)
        {
            Value = value;
        }
    }
}
