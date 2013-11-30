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
        public IDictionary<string, object> Get(object obj)
        {
            var props = new Dictionary<string, object>();
            if (obj == null) return props;

            if (obj is IDictionary<string, object>)
            {
                var dic = obj as IDictionary<string, object>;
                foreach (KeyValuePair<string, object> pair in dic)
                {
                    props.Add(pair.Key, pair.Value);
                }
            }
            else if (obj is IDictionary)
            {
                var dic = obj as IDictionary;
                foreach (DictionaryEntry entry in dic)
                {
                    var keyS = entry.Key as string;
                    if (keyS == null) continue;
                    props[keyS] = entry.Value;
                }
            }
            else
            {
                foreach (var propertyInfo in GetPropertyInfos(obj.GetType()))
                {
                    props.Add(propertyInfo.Item1, propertyInfo.Item2.GetValue(obj, null));
                }
            }

            return props;
        }

        private static readonly ConcurrentDictionary<Type, List<Tuple<string, PropertyInfo>>>
            PropertyInfosCache = new ConcurrentDictionary<Type, List<Tuple<string, PropertyInfo>>>();

        private List<Tuple<string, PropertyInfo>> GetPropertyInfos(Type type)
        {
            return PropertyInfosCache.GetOrAdd(type, t =>
            {
                bool isDataContract = t.GetCustomAttribute<DataContractAttribute>() != null;
                var infos = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                var res = new List<Tuple<string, PropertyInfo>>(infos.Length);

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
                        res.Add(Tuple.Create(
                            string.IsNullOrWhiteSpace(mixpanelNameAttr.Name)
                                ? info.Name
                                : mixpanelNameAttr.Name,
                            info));
                        continue;
                    }

                    if (dataMemberAttr != null)
                    {
                        res.Add(Tuple.Create(
                            string.IsNullOrWhiteSpace(dataMemberAttr.Name)
                                ? info.Name
                                : dataMemberAttr.Name,
                            info));
                        continue;
                    }

                    res.Add(Tuple.Create(info.Name, info));
                }
                return res;
            });
        }
    }
}
