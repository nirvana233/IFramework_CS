/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-05-03
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;

namespace IFramework.Serialization
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public class ListStringConverter<T>: StringConverter<List<T>>
    {
        ObjectStringConverter<T> c = Get(typeof(T)) as ObjectStringConverter<T>;

        private static int ReadItem(string value, int start)
        {
            int depth = 0;

            for (int i = start; i < value.Length; i++)
            {
                char data = value[i];
                if (data == StringConverter.leftBound || data == StringConverter.midLeftBound)
                {
                    depth++;
                }
                else if (data == StringConverter.rightBound || data == StringConverter.midRightBound)
                {
                    depth--;
                }
                if (depth == 0)
                {
                    for (int j = i; j < value.Length; j++)
                    {
                        if (value[j] == StringConverter.dot)
                        {
                            return j;
                        }
                    }
                    return value.Length - 1;
                }
            }
            return -1;

        }

        public static bool ReadArray(string self, Action<string> call)
        {
            try
            {
                if (!self.StartsWith(StringConverter.midLeftBound.ToString())) goto ERR;
                if (!self.EndsWith(StringConverter.midRightBound.ToString())) goto ERR;
                self = self.Remove(0, 1);
                self = self.Remove(self.Length - 1, 1);
                self = self.Replace(" ", "").Replace("\r\n", "\n").Replace("\n", "");
                int start = 0;
                while (start < self.Length)
                {
                    int end = ReadItem(self, start);
                    if (end == -1)
                    {
                        break;
                    }
                    string inner = self.Substring(start, end + 1 - start);
                    if (inner.EndsWith(StringConverter.dot.ToString()))
                    {
                        inner = inner.Remove(inner.Length - 1, 1);
                    }
                    call.Invoke(inner);
                    start = end + 1;
                }
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        ERR:
            return false;
        }
        private List<T> list = new List<T>();
        public override string ConvertToString(List<T> t)
        {
            if (t == null || t.Count == 0) return string.Empty;
            string result = string.Empty;
            for (int i = 0; i < t.Count; i++)
            {
                if (i == t.Count - 1)
                {
                    result = result.Append($"{c.ConvertToString(t[i])}");
                }
                else
                {
                    result = result.Append($"{c.ConvertToString(t[i])}{ dot}");
                }
            }
            return $"{midLeftBound}{result}{midRightBound}";
        }
        public override bool TryConvert(string self, out List<T> result)
        {
            list.Clear();

            bool bo = ReadArray(self, (inner) =>
            {
                T value;
                if (c.TryConvert(inner, out value))
                {
                    list.Add(value);
                }
            });
            if (bo)
                result = new List<T>(list);
            else
                result = MakeDefault();
            return bo;
        }
    }
}
