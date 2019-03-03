using System;
using System.Collections;
using System.Collections.Generic;
#if NETSTANDARD11
using System.Linq;
#endif
using System.Reflection;
using System.Runtime.Serialization;
using System.Collections.Concurrent;

namespace Mixpanel.MessageProperties
{
    internal static class PropertiesDigger
    {
        public static IEnumerable<ObjectProperty> Get(object obj, PropertyOrigin propertyOrigin)
        {
            switch (obj)
            {
                case null:
                    yield break;

                case IDictionary<string, object> dic:
                    foreach (KeyValuePair<string, object> entry in dic)
                    {
                        yield return new ObjectProperty(
                            entry.Key, 
                            PropertyNameSource.Default, 
                            propertyOrigin, 
                            entry.Value);
                    }
                    yield break;

                case IDictionary dic:
                    foreach (DictionaryEntry entry in dic)
                    {
                        if (!(entry.Key is string stringKey))
                        {
                            continue;
                        }

                        yield return new ObjectProperty(
                            stringKey, 
                            PropertyNameSource.Default, 
                            propertyOrigin, 
                            entry.Value);
                    }
                    yield break;

                default:
                    foreach (var propertyInfo in GetObjectPropertyInfos(obj.GetType()))
                    {
                        yield return new ObjectProperty(
                            propertyInfo.PropertyName,
                            propertyInfo.PropertyNameSource,
                            propertyOrigin,
                            propertyInfo.PropertyInfo.GetValue(obj, null));
                    }
                    yield break;

            }
        }

        private static readonly ConcurrentDictionary<Type, List<ObjectPropertyInfo>>
            PropertyInfosCache = new ConcurrentDictionary<Type, List<ObjectPropertyInfo>>();

        private static List<ObjectPropertyInfo> GetObjectPropertyInfos(Type type)
        {
            return PropertyInfosCache.GetOrAdd(type, t =>
            {
#if NETSTANDARD11
                bool isDataContract = t.GetTypeInfo().GetCustomAttribute<DataContractAttribute>() != null;
                var infos = t.GetRuntimeProperties().Where(x => x.CanRead).ToArray();
#else
                bool isDataContract = t.GetCustomAttribute<DataContractAttribute>() != null;
                var infos = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
#endif
                var res = new List<ObjectPropertyInfo>(infos.Length);

                foreach (var info in infos)
                {
                    if (info.GetCustomAttribute<IgnoreDataMemberAttribute>() != null)
                        continue;

                    DataMemberAttribute dataMemberAttr = null;
                    if (isDataContract)
                    {
                        dataMemberAttr = info.GetCustomAttribute<DataMemberAttribute>();
                        if (dataMemberAttr == null)
                            continue;
                    }

                    var mixpanelNameAttr = info.GetCustomAttribute<MixpanelNameAttribute>();
                    if (mixpanelNameAttr != null)
                    {
                        var isMixpanelNameEmpty = string.IsNullOrWhiteSpace(mixpanelNameAttr.Name);
                        res.Add(new ObjectPropertyInfo(
                            isMixpanelNameEmpty ? info.Name : mixpanelNameAttr.Name,
                            isMixpanelNameEmpty ? PropertyNameSource.Default : PropertyNameSource.MixpanelName,
                            info));
                        continue;
                    }

                    if (dataMemberAttr != null)
                    {
                        var isDataMemberNameEmpty = string.IsNullOrWhiteSpace(dataMemberAttr.Name);
                        res.Add(new ObjectPropertyInfo(
                            isDataMemberNameEmpty ? info.Name : dataMemberAttr.Name,
                            isDataMemberNameEmpty ? PropertyNameSource.Default : PropertyNameSource.DataMember,
                            info));
                        continue;
                    }

                    res.Add(new ObjectPropertyInfo(info.Name, PropertyNameSource.Default, info));
                }
                return res;
            });
        }

        sealed class ObjectPropertyInfo
        {
            public string PropertyName { get; }
            public PropertyNameSource PropertyNameSource { get; }
            public PropertyInfo PropertyInfo { get; }

            public ObjectPropertyInfo(
                string propertyName, 
                PropertyNameSource propertyNameSource, 
                PropertyInfo propertyInfo)
            {
                PropertyName = propertyName;
                PropertyNameSource = propertyNameSource;
                PropertyInfo = propertyInfo;
            }
        }
    }
}
