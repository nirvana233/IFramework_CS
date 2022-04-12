/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-05-03
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Reflection;

namespace IFramework.Serialization
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public class ObjectStringConverter<T>: StringConverter<T>
    {
        private static int ReadField(string value, int start, out int colinIndex)
        {
            colinIndex = value.IndexOf(StringConverter.colon, start);
            if (colinIndex < 0) return -1;
            int depth = 0;
            for (int i = colinIndex + 1; i < value.Length; i++)
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
                else
                {
                    if (data == StringConverter.dot && depth == 0)
                    {
                        return i;
                    }
                    else
                    {
                        continue;
                    }
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
            return value.Length - 1;
        }

        public static bool ReadObject(string value, Action<string, string> call)
        {
            try
            {
                if (!value.StartsWith(StringConverter.leftBound.ToString())) goto ERR;
                if (!value.EndsWith(StringConverter.rightBound.ToString())) goto ERR;
                value = value.Remove(0, 1);
                value = value.Remove(value.Length - 1, 1);
                value = value.Replace(" ", "").Replace("\r\n", "\n").Replace("\n", "");
                int start = 0;
                while (start < value.Length)
                {
                    int colinIndex = 0;
                    int end = ReadField(value, start, out colinIndex);
                    if (end == -1)
                    {
                        break;
                    }
                    string fieldName = value.Substring(start, colinIndex - start);
                    string inner = value.Substring(colinIndex + 1, end - colinIndex);
                    if (inner.EndsWith(StringConverter.dot.ToString()))
                    {
                        inner = inner.Remove(inner.Length - 1, 1);
                    }

                    call?.Invoke(fieldName, inner);

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

        private string WriteString(string name,string innner,bool last)
        {
            if (last)
            {
                return $"{name}{colon}{innner}";
            }
            else
            {
                return $"{name}{colon}{innner}{ dot}";
            }
        }
        public override string ConvertToString(T t)
        {
            if (t == null) return string.Empty;
            var fields = typeof(T).GetFields();
            string result = string.Empty;
            for (int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                if (field.IsNotSerialized) continue;
                if (field.IsStatic) continue;
                StringConverter c = Get(field.FieldType);
                result = result.Append(WriteString(field.Name, c.ConvertToString(field.GetValue(t)), i == fields.Length - 1));
            }

            var ps = typeof(T).GetProperties();
            for (int i = 0; i < ps.Length; i++)
            {
                var p = ps[i];
                if (!p.CanRead) continue;
                if (!p.CanWrite) continue;
                StringConverter c = Get(p.PropertyType);
                result = result.Append(WriteString(p.Name, c.ConvertToString(p.GetValue(t)), i == ps.Length - 1));
            }
            return $"{leftBound}{result}{rightBound}";
        }

        public override bool TryConvert(string self, out T result)
        {
            object _obj = Activator.CreateInstance<T>();
            bool bo = ReadObject(self, (fieldName, inner) =>
            {
                SetMember(fieldName,inner,_obj);
            });
            if (bo)
                result = (T)_obj;
            else
                result = MakeDefault();
            return bo;
        }
        private void SetMember(string fieldName,string inner,object _obj)
        {
            var membders = typeof(T).GetMember(fieldName);
            if (membders != null && membders.Length == 1)
            {
                var membder = membders[0];
                if (membder is FieldInfo)
                {
                    FieldInfo f = membder as FieldInfo;
                    StringConverter c = Get(f.FieldType);
                    object value;
                    if (c.TryConvertObject(inner, out value))
                    {
                        f.SetValue(_obj, value);
                    }
                }
                else if (membder is PropertyInfo)
                {
                    PropertyInfo p = membder as PropertyInfo;
                    StringConverter c = Get(p.PropertyType);
                    object value;
                    if (c.TryConvertObject(inner, out value))
                    {
                        p.SetValue(_obj, value);
                    }
                }
            }
        }

    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
}
