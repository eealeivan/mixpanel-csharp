using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace Mixpanel.Core
{
    internal sealed class PropertiesDigger
    {
        public IDictionary<string, Tuple<PropertyNameSource, object>> Get(object obj)
        {
            var props = new Dictionary<string, Tuple<PropertyNameSource, object>>();
            if (obj == null) return props;

            if (obj is IDictionary<string, object>)
            {
                var dic = obj as IDictionary<string, object>;
                foreach (KeyValuePair<string, object> pair in dic)
                {
                    props[pair.Key] = Tuple.Create(PropertyNameSource.Default, pair.Value);
                }
            }
            else if (obj is IDictionary)
            {
                var dic = obj as IDictionary;
                foreach (DictionaryEntry entry in dic)
                {
                    var keyS = entry.Key as string;
                    if (keyS == null) continue;
                    props[keyS] = Tuple.Create(PropertyNameSource.Default, entry.Value);
                }
            }
            else
            {
                foreach (var propertyInfo in GetPropertyInfos(obj.GetType()))
                {
                    props[propertyInfo.Item1] =
                        Tuple.Create(propertyInfo.Item2, propertyInfo.Item3.GetValue(obj, null));
                }
            }

            return props;
        }

        private static readonly ConcurrentDictionary<Type, List<Tuple<string, PropertyNameSource, PropertyInfo>>>
            PropertyInfosCache = new ConcurrentDictionary<Type, List<Tuple<string, PropertyNameSource, PropertyInfo>>>();

        private List<Tuple<string, PropertyNameSource, PropertyInfo>> GetPropertyInfos(Type type)
        {
            return PropertyInfosCache.GetOrAdd(type, t =>
            {
                bool isDataContract = t.GetCustomAttribute<DataContractAttribute>() != null;
                var infos = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                var res = new List<Tuple<string, PropertyNameSource, PropertyInfo>>(infos.Length);

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
                        res.Add(Tuple.Create(
                            isMixpanelNameEmpty ? info.Name : mixpanelNameAttr.Name,
                            isMixpanelNameEmpty ? PropertyNameSource.Default : PropertyNameSource.MixpanelName,
                            info));
                        continue;
                    }

                    if (dataMemberAttr != null)
                    {
                        var isDataMemberNameEmpty = string.IsNullOrWhiteSpace(dataMemberAttr.Name);
                        res.Add(Tuple.Create(
                            isDataMemberNameEmpty ? info.Name : dataMemberAttr.Name,
                            isDataMemberNameEmpty ? PropertyNameSource.Default : PropertyNameSource.DataMember,
                            info));
                        continue;
                    }

                    res.Add(Tuple.Create(info.Name, PropertyNameSource.Default, info));
                }
                return res;
            });
        }
    }
}
