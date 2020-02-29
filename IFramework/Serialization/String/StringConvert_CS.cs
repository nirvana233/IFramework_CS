/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-04-07
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Text;

namespace IFramework.Serialization
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释

    public class IntStringConverter : StringConverter<int>
    {
        public override bool TryConvert(string self, out int result)
        {
            return int.TryParse(self, out result);
        }
    }
    public class ShortStringConverter : StringConverter<short>
    {
        public override bool TryConvert(string self, out short result)
        {
            return short.TryParse(self, out result);
        }
    }
    public class LongStringConverter : StringConverter<long>
    {
        public override bool TryConvert(string self, out long result)
        {
            return long.TryParse(self, out result);
        }
    }
    public class UInt16StringConverter : StringConverter<UInt16>
    {
        public override bool TryConvert(string self, out UInt16 result)
        {
            return UInt16.TryParse(self, out result);
        }
    }
    public class UInt32StringConverter : StringConverter<UInt32>
    {
        public override bool TryConvert(string self, out UInt32 result)
        {
            return UInt32.TryParse(self, out result);
        }
    }
    public class UInt64StringConverter : StringConverter<UInt64>
    {
        public override bool TryConvert(string self, out UInt64 result)
        {
            return UInt64.TryParse(self, out result);
        }
    }
    public class FloatStringConverter : StringConverter<float>
    {
        public override bool TryConvert(string self, out float result)
        {
            return float.TryParse(self, out result);
        }
    }
    public class DoubleStringConverter : StringConverter<double>
    {
        public override bool TryConvert(string self, out double result)
        {
            return double.TryParse(self, out result);
        }
    }
    public class DecimalStringConverter : StringConverter<decimal>
    {
        public override bool TryConvert(string self, out decimal result)
        {
            return decimal.TryParse(self, out result);
        }
    }
    public class StringStringConverter : StringConverter<string>
    {
        public override bool TryConvert(string self, out string result)
        {
            result = self;
            return true;
        }
    }
    public class BoolStringConverter : StringConverter<bool>
    {
        public override bool TryConvert(string self, out bool result)
        {
            return bool.TryParse(self, out result);
        }
    }
    public class CharStringConverter : StringConverter<char>
    {
        public override bool TryConvert(string self, out char result)
        {
            return char.TryParse(self, out result);
        }
    }
    public class ByteStringConverter : StringConverter<byte>
    {
        public override bool TryConvert(string self, out byte result)
        {
            return byte.TryParse(self, out result);
        }
    }
    public class DateTimeStringConverter : StringConverter<DateTime>
    {
        public override bool TryConvert(string self, out DateTime result)
        {
            return DateTime.TryParse(self, out result);
        }
    }
    public class ByteArrayStringConverter : StringConverter<byte[]>
    {
        public override string ConvertToString(byte[] t)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < t.Length; i++)
                sb.Append(t[i].ToString("X2"));
            return sb.ToString();
        }
        public override bool TryConvert(string self, out byte[] result)
        {
            if (self.Length % 2 != 0) throw new System.Exception("Parse Err Color");
            result = new byte[self.Length / 2];
            for (int i = 0; i < result.Length; i++)
                result[i] = byte.Parse(self.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
            return true;
        }
    }

    public class EnumStringConverter<T> : StringConverter<T> where T : struct
    {
        public override string ConvertToString(T t)
        {
            if (typeof(T).IsEnum)
            {
                ulong ul;
                try
                {
                    ul = Convert.ToUInt64(t as Enum);
                }
                catch (OverflowException)
                {
                    unchecked
                    {
                        ul = (ulong)Convert.ToInt64(t as Enum);
                    }
                }
                return ul.ToString();
            }
            else
                throw new Exception("This Type is Not Enum "+ typeof(T));
        }
        public override bool TryConvert(string self, out T result)
        {
            return Enum.TryParse(self, out result);
        }
    }

#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

}
