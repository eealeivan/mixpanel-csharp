using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Runtime.Serialization;

namespace Mixpanel
{
    internal class PropertiesDigger
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
                GetPropertiesFromObject(obj, props);
            }

            return props;
        }

        private void GetPropertiesFromObject(object obj, Dictionary<string, object> props)
        {
            foreach (var propertyInfo in GetPropertyInfos(obj.GetType()))
            {
                props.Add(propertyInfo.Item1, propertyInfo.Item2.GetValue(obj, null));
            }
        }

        private IEnumerable<Tuple<string, PropertyInfo>> GetPropertyInfos(Type type)
        {
            bool isDataContract = type.GetCustomAttribute<DataContractAttribute>() != null;

            var infos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var info in infos)
            {
                if(info.GetCustomAttribute<IgnoreDataMemberAttribute>() != null)
                    continue;

                var mixpanelPropAttr = info.GetCustomAttribute<MixpanelPropertyAttribute>();
                if (mixpanelPropAttr != null)
                {
                    yield return Tuple.Create(
                        string.IsNullOrWhiteSpace(mixpanelPropAttr.Name) ? info.Name : mixpanelPropAttr.Name, 
                        info);
                    continue;
                }

                var dataMemberAttr = info.GetCustomAttribute<DataMemberAttribute>();
                if (isDataContract && dataMemberAttr == null)
                {
                    continue;
                }

                if (dataMemberAttr != null)
                {
                    yield return Tuple.Create(
                        string.IsNullOrWhiteSpace(dataMemberAttr.Name) ? info.Name : dataMemberAttr.Name,
                        info);
                    continue;
                }

                yield return Tuple.Create(info.Name, info);
            }
        }
    }
}
