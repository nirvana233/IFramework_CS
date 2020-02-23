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
    public static partial class StringConvert
    {
        static StringConvert()
        {
            onTryConverts = new List<OnTryConvert>();
            onConvertToStrings = new List<OnConvertToString>();
        }
        public delegate bool OnConvertToString(ref string result, object obj, Type type);
        public delegate bool OnTryConvert(string str,  Type type,ref object result);
        public static event OnTryConvert onTryConvert
        {
            add
            {
                if (!onTryConverts.Contains(value))
                {
                    onTryConverts.Add(value);
                }
            }
            remove
            {
                if (onTryConverts.Contains(value))
                {
                    onTryConverts.Remove(value);
                }
            }
        }
        public static event OnConvertToString onConvertToString
        {
            add
            {
                if (!onConvertToStrings.Contains(value))
                {
                    onConvertToStrings.Add(value);
                }
            }
            remove
            {
                if (onConvertToStrings.Contains(value))
                {
                    onConvertToStrings.Remove(value);
                }
            }
        }

        private static List<OnTryConvert> onTryConverts;
        private static List<OnConvertToString> onConvertToStrings;
        public const string dot = ",";
        public const string leftBound = "[ ";
        public const string rightBound = " ]";
        public const char colon = ':';
        public static string ConvertToString<T>(this T self)
        {
            return self.ConvertToString(typeof(T));
        }
        public static string ConvertToString(this object self, Type type)
        {
            if (self == null) return string.Empty;
            else if (type == typeof(string)) return ((string)self).ConvertToString();
            else if (type == typeof(int)) return ((int)self).ConvertToString();
            else if (type == typeof(float)) return ((float)self).ConvertToString();
            else if (type == typeof(double)) return ((double)self).ConvertToString();
            else if (type == typeof(decimal)) return ((decimal)self).ConvertToString();
            else if (type == typeof(bool)) return ((bool)self).ConvertToString();
            else if (type == typeof(char)) return ((char)self).ConvertToString();
            else if (type == typeof(long)) return ((long)self).ConvertToString();
            else if (type == typeof(short)) return ((short)self).ConvertToString();
            else if (type == typeof(byte)) return ((byte)self).ConvertToString();
            else if (type == typeof(ushort)) return ((ushort)self).ConvertToString();
            else if (type == typeof(uint)) return ((uint)self).ConvertToString();
            else if (type == typeof(ulong)) return ((ulong)self).ConvertToString();
            else if (type == typeof(DateTime)) return ((DateTime)self).ConvertToString();
            else if (type == typeof(byte[])) return ((byte[])self).ConvertToString();
            else if(onConvertToStrings .Count>0)
            {
                string result=string.Empty;
                for (int i = 0; i < onConvertToStrings.Count; i++)
                {
                    if (onConvertToStrings[i].Invoke(ref result, self, type))
                    {
                        return result;
                    }
                }
                throw new Exception(string.Format("Can't Convert Type {0} ToString", type));
            }
            else
                throw new Exception(string.Format("Can't Convert Type {0} ToString", type));
        }

        public static bool TryConvert<T>(this string self, out T t)
        {
            object obj = default(object);
            if (self.TryConvert(typeof(T), ref obj))
            {
                t = (T)obj;
                return true;
            }
            else
            {
                t = default(T);
                return false;
            }
        }
        public static bool TryConvert(this string self, Type type, ref object obj)
        {

            if (string.IsNullOrEmpty(self)) return false;
            if (type == typeof(object))
            {
                Type t = self.GetType();
                if (t == typeof(object))
                    return false;
                return TryConvert(self, t, ref obj);
            }
            else if (type == typeof(string))
            {
                obj = self;
                return true;
            }
            else if (type == typeof(int))
            {
                int res;
                if (!self.TryConvert(out res)) return false;
                obj = res;
                return true;
            }
            else if (type == typeof(float))
            {
                float res;
                if (!self.TryConvert(out res)) return false;
                obj = res;
                return true;
            }
            else if (type == typeof(double))
            {
                double res;
                if (!self.TryConvert(out res)) return false;
                obj = res;
                return true;
            }
            else if (type == typeof(decimal))
            {
                decimal res;
                if (!self.TryConvert(out res)) return false;
                obj = res;
                return true;
            }
            else if (type == typeof(bool))
            {
                bool res;
                if (!self.TryConvert(out res)) return false;
                obj = res;
                return true;
            }
            else if (type == typeof(char))
            {
                char res;
                if (!self.TryConvert(out res)) return false;
                obj = res;
                return true;
            }
            else if (type == typeof(long))
            {
                long res;
                if (!self.TryConvert(out res)) return false;
                obj = res;
                return true;
            }
            else if (type == typeof(short))
            {
                short res;
                if (!self.TryConvert(out res)) return false;
                obj = res;
                return true;
            }
            else if (type == typeof(byte))
            {
                byte res;
                if (!self.TryConvert(out res)) return false;
                obj = res;
                return true;
            }
            else if (type == typeof(UInt16))
            {
                UInt16 res;
                if (!self.TryConvert(out res)) return false;
                obj = res;
                return true;
            }
            else if (type == typeof(UInt32))
            {
                UInt32 res;
                if (!self.TryConvert(out res)) return false;
                obj = res;
                return true;
            }
            else if (type == typeof(UInt64))
            {
                UInt64 res;
                if (!self.TryConvert(out res)) return false;
                obj = res;
                return true;
            }
            else if (type == typeof(DateTime))
            {
                DateTime res;
                if (!self.TryConvert(out res)) return false;
                obj = res;
                return true;
            }
            else if (type == typeof(byte[]))
            {
                byte[] res;
                if (!self.TryConvert(out res)) return false;
                obj = res;
                return true;
            }
            else if (onTryConverts.Count >0)
            {
                for (int i = 0; i < onTryConverts.Count; i++)
                {
                    if (onTryConverts[i].Invoke(self, type,ref obj))
                    {
                        return true;
                    }
                }
                throw new Exception(string.Format("Can't Convert String {0} To Type {1}", self, type));
            }
            else
                throw new Exception(string.Format("Can't Convert String {0} To Type {1}", self, type));
        }
    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

}
