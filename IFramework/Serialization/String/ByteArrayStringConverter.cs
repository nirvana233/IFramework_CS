/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-04-07
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.Text;

namespace IFramework.Serialization
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    [System.Runtime.InteropServices.ComVisible(false)]

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

#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

}
