/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-05-03
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;

namespace IFramework.Serialization
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public class DictionaryStringConverter<Key, Value> : StringConverter<Dictionary<Key, Value>>
    {
        const string keyChar = "k";
        const string valueChar = "v";
        Dictionary<Key, Value> dic = new Dictionary<Key, Value>();
        StringConverter<Key> k = Get(typeof(Key)) as StringConverter<Key>;
        StringConverter<Value> v = Get(typeof(Value)) as StringConverter<Value>;
        private void Read(string self, Dictionary<Key, Value> result)
        {
            Key lastKey = default(Key);
            ObjectStringConverter<Key>.ReadObject(self, (fieldName, inner) =>
            {
                if (fieldName == keyChar)
                {
                    k.TryConvert(inner, out lastKey);
                }
                else if (fieldName == valueChar)
                {
                    Value value;
                    if (v.TryConvert(inner, out value))
                    {
                        result.Add(lastKey, value);
                    }
                    else
                    {
                        result.Add(lastKey, default(Value));
                    }
                }
            });
        }
        public override bool TryConvert(string self, out Dictionary<Key, Value> result)
        {
            dic.Clear();
            bool bo = ListStringConverter<Key>.ReadArray(self, (inner) =>
            {
                if (inner.EndsWith(dot.ToString()))
                {
                    inner = inner.Remove(inner.Length - 1, 1);
                }
                Read(inner, dic);
            });
            if (bo)
            {
                result = new Dictionary<Key, Value>(dic);
            }
            else
            {
                result = MakeDefault();
            }
            return bo;
        }
   
        public override string ConvertToString(Dictionary<Key, Value> t)
        {
            if (t == null || t.Count == 0) return string.Empty;
            int count = 0;
            string result = string.Empty;
            foreach (var item in t)
            {
                var _key = item.Key;
                var _value = item.Value;
                
                count++;
                result = result.Append($"{leftBound}{keyChar}{colon}{k.ConvertToString(_key)}{dot}{valueChar}{colon}{v.ConvertToString(_value)}{rightBound}");
                if (count!=t.Count)
                {
                    result = result.Append(dot.ToString());
                }
            }
            return $"{midLeftBound}{result}{midRightBound}";
        }
    }
}
