using Dicom;
using Ws.Dicom.Interfaces.Entities;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ws.Dicom.Persistency.Fo.Entities
{
    public static class DicomDatasetExtensions
    {
        private static readonly ILogger _logger = Log.ForContext<DicomDataset>();

        public static void FillObject<T>(this DicomDataset ds, T obj)
        {
            var propsInfo = typeof(T).GetProperties();
            //.Where(prop => Attribute.IsDefined(prop, typeof(DicomTagAttribute), true));

            foreach (var propInfo in propsInfo)
            {

                var tagAttr = (DicomTagAttribute)Attribute.GetCustomAttribute(propInfo, typeof(DicomTagAttribute), true);
                if (tagAttr == null)
                    continue;

                DicomTag tag = tagAttr.Value;

                string stringVal;
                if (!ds.TryGetString(tag, out stringVal) || string.IsNullOrWhiteSpace(stringVal))
                    continue;

                var realType = Nullable.GetUnderlyingType(propInfo.PropertyType);
                if (realType == null)
                    realType = propInfo.PropertyType;

                if (realType == typeof(string))
                {
                    propInfo.SetValue(obj, stringVal);
                    continue;
                }

                MethodInfo method = typeof(DicomDataset).GetMethod(nameof(ds.GetSingleValueOrDefault));
                MethodInfo generic = method.MakeGenericMethod(new Type[] { realType });

                object defaultValue = null;
                if (realType.GetTypeInfo().IsValueType)
                    defaultValue = Activator.CreateInstance(realType);

                var propValue = generic.Invoke(ds, new object[] { tag, defaultValue });
                propInfo.SetValue(obj, propValue);
            }
        }

        public static T CreateObject<T>(this DicomDataset ds, Func<T> factory)
        {
            T obj = factory();
            ds.FillObject(obj);

            List<ValidationResult> results = new List<ValidationResult>();
            if (!Validator.TryValidateObject(obj, new ValidationContext(obj), results, true))
            {
                _logger.Error("Failed to create dicom object from a dataset: {@results}");
                return default(T);
            }

            return obj;
        }

        public static T CreateObject<T>(this DicomDataset ds) where T : new() => ds.CreateObject(() => new T());


        public static void Fill<T>(this DicomDataset ds)
        {
            var propsInfo = typeof(T).GetProperties();
            //.Where(prop => Attribute.IsDefined(prop, typeof(DicomTagAttribute), true));

            foreach (var propInfo in propsInfo)
            {
                var tagAttr = (DicomTagAttribute)Attribute.GetCustomAttribute(propInfo, typeof(DicomTagAttribute), true);
                if (tagAttr == null)
                    continue;

                DicomTag tag = tagAttr.Value;

                if (!ds.Contains(tag))
                    ds.AddOrUpdate(tag, string.Empty);
            }
        }

        private static DateTime? GetDateAndTime(this DicomDataset ds, DicomTag dateTag, DicomTag timeTag)
        {
            DateTime date;
            if (!ds.TryGetSingleValue(dateTag, out date))
                return null;

            DateTime time;
            if (ds.TryGetSingleValue(timeTag, out time))
            {
                date = date.AddHours(time.Hour).AddMinutes(time.Minute).AddSeconds(time.Second);
            }

            return date;
        }
    }
}
