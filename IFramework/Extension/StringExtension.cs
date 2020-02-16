using System.Linq;
using System.Text;

namespace IFramework
{
    /// <summary>
    /// string静态扩展
    /// </summary>
    public static partial class StringExtension
    {
        /// <summary>
        /// 替换第一个符合的字符串
        /// </summary>
        /// <param name="self"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <param name="startAt"></param>
        /// <returns></returns>
        public static string ReplaceFirst(this string self, string oldValue, string newValue, int startAt = 0)
        {
            var index = self.IndexOf(oldValue, startAt);
            if (index < 0)
            {
                return self;
            }
            return (self.Substring(0, index) + newValue + self.Substring(index + oldValue.Length));
        }
        /// <summary>
        /// 移除字符串
        /// </summary>
        /// <param name="self"></param>
        /// <param name="targets"></param>
        /// <returns></returns>
        public static string RemoveString(this string self, params string[] targets)
        {
            return targets.Aggregate(self, (current, t) => current.Replace(t, string.Empty));
        }
        /// <summary>
        /// 分割 并且去掉空格
        /// </summary>
        /// <param name="self"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string[] SplitAndTrim(this string self, params char[] separator)
        {
            var res = self.Split(separator);
            for (var i = 0; i < res.Length; i++)
            {
                res[i] = res[i].Trim();
            }
            return res;
        }
        /// <summary>
        /// 截取俩字符串中间的字符
        /// </summary>
        /// <param name="self"></param>
        /// <param name="front"></param>
        /// <param name="behind"></param>
        /// <returns></returns>
        public static string Cut(this string self, string front, string behind)
        {
            var startIndex = self.IndexOf(front) + front.Length;
            var endIndex = self.IndexOf(behind);
            if (startIndex < 0 || endIndex < 0)
                return self;
            return self.Substring(startIndex, endIndex - startIndex);
        }
        /// <summary>
        /// 截取指定字符串后的字符
        /// </summary>
        /// <param name="self"></param>
        /// <param name="front"></param>
        /// <returns></returns>
        public static string CutAfter(this string self, string front)
        {
            var startIndex = self.IndexOf(front) + front.Length;
            return startIndex < 0 ? self : self.Substring(startIndex);
        }
        /// <summary>
        /// 截取指定字符串前的字符
        /// </summary>
        /// <param name="str"></param>
        /// <param name="behind"></param>
        /// <returns></returns>
        public static string CutBefore(this string str, string behind)
        {
            int index = str.LastIndexOf(behind);
            return str.Substring(0, index);
        }
        /// <summary>
        /// 第一个字符大写
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static string UpperFirst(this string self)
        {
            return char.ToUpper(self[0]) + self.Substring(1);
        }
        /// <summary>
        /// 第一个字符小写
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static string LowerFirst(this string self)
        {
            return char.ToLower(self[0]) + self.Substring(1);
        }
        /// <summary>
        /// 字符串结尾转Unix编码
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static string ToUnixLineEndings(this string self)
        {
            return self.Replace("\r\n", "\n").Replace("\r", "\n");
        }
        /// <summary>
        /// 在字符串前拼接字符串
        /// </summary>
        /// <param name="self"></param>
        /// <param name="toPrefix"></param>
        /// <returns></returns>
        public static string AppendHead(this string self, string toPrefix)
        {
            return new StringBuilder(toPrefix).Append(self).ToString();
        }


        /// <summary>
        /// 拼接字符串
        /// </summary>
        /// <param name="self"></param>
        /// <param name="toAppend"></param>
        /// <returns></returns>
        public static string Append(this string self, string toAppend)
        {
            return new StringBuilder(self).Append(toAppend).ToString();
        }
        /// <summary>
        /// 拼接字符串
        /// </summary>
        /// <param name="self"></param>
        /// <param name="toAppend"></param>
        /// <returns></returns>
        public static string Append(this string self, string[] toAppend)
        {
            return new StringBuilder(self).Append(toAppend).ToString();
        }
        /// <summary>
        /// 在字符串之间加空格，根据大小写
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static string AddSpacedBetwwenWord(this string self)
        {
            var sb = new StringBuilder(self.Length * 2);
            sb.Append(char.ToUpper(self[0]));
            for (var i = 1; i < self.Length; i++)
            {
                if (char.IsUpper(self[i]) && self[i - 1] != ' ')
                {
                    sb.Append(' ');
                }
                sb.Append(self[i]);
            }
            return sb.ToString();
        }
    }

}
