using System;
using System.Configuration;
using System.Linq;
using System.Reflection;

namespace Ws.Extensions.AppSettings.Patterns
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class AppSettingsClassAttribute : Attribute
    {
        public Type SettingsType { get; }

        public AppSettingsClassAttribute(Type settingsType)
        {
            SettingsType = settingsType;
        }

        public static void Save(object vm) => SaveOrLoad(vm, true);

        public static void Load(object vm) => SaveOrLoad(vm, false);

        private static void SaveOrLoad(object vm, bool save)
        {
            var vmType = vm.GetType();

            var classAttr = vmType.GetCustomAttribute<AppSettingsClassAttribute>();
            if (classAttr == null)
                return;

            var defaultSettingsPi = classAttr.SettingsType.GetProperty(
                "Default", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            if (defaultSettingsPi == null)
                return;
            var defaultSettings = defaultSettingsPi.GetValue(null, null) as ApplicationSettingsBase;
            if (defaultSettings == null)
                return;

            var propsInfo = vm.GetType().GetProperties().Where(
                prop => IsDefined(prop, typeof(AppSettingsPropAttribute), true));

            foreach (var pi in propsInfo)
            {
                try
                {
                    var propAttr = pi.GetCustomAttribute<AppSettingsPropAttribute>();
                    var settingsPi = classAttr.SettingsType.GetProperty(propAttr.Name);

                    if (save)
                    {
                        var propValue = pi.GetValue(vm);
                        settingsPi.SetValue(defaultSettings, propValue);
                    }
                    else
                    {
                        var propValue = settingsPi.GetValue(defaultSettings);
                        pi.SetValue(vm, propValue);
                    }
                }
                catch
                { /* ignore exception */ }
            }

            if (save)
                defaultSettings.Save();
        }
    }
}