using System.Text.RegularExpressions;

namespace IFramework
{
    /// <summary>
    /// 正则
    /// </summary>
    public static partial class StringExtension_Regex
    {
        /// <summary>
        /// 是否是邮箱
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool IsMail(this string self)
        {
            if (string.IsNullOrEmpty(self)) return false;
            return Regex.IsMatch(self, @"^\s*([A-Za-z0-9_-]+(\.\w+)*@(\w+\.)+\w{2,5})s*$");
        }
        /// <summary>
        /// 是否是电话
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool IsCellPhoneNumber(this string self)
        {
            if (string.IsNullOrEmpty(self)) return false;
            return Regex.IsMatch(self, @"^[1]+[3,5]+\d{9}");
        }
        /// <summary>
        /// 是否是年龄
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool IsAge(this string self)
        {
            if (string.IsNullOrEmpty(self)) return false;
            return Regex.IsMatch(self, @"^\d{1,2}$");
        }
        /// <summary>
        /// 是否是密码
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool IsPassWord(this string self)
        {
            if (string.IsNullOrEmpty(self)) return false;
            return new Regex(@"^[a-zA-Z]\w{5,15}$").IsMatch(self);
        }
        /// <summary>
        /// 是否包含中文
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool HasChinese(this string self)
        {
            if (string.IsNullOrEmpty(self)) return false;
            return Regex.IsMatch(self, @"[\u4e00-\u9fa5]");
        }
        /// <summary>
        /// 是否是网址
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool IsUrl(this string self)
        {
            if (string.IsNullOrEmpty(self)) return false;
            return Regex.IsMatch(self, @"[a-zA-z]+://[^\s]*");
        }
        /// <summary>
        /// 是否是IPV4
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool IsIPv4(this string self)
        {
            if (string.IsNullOrEmpty(self)) return false;
            return Regex.IsMatch(self, @"((25[0 - 5] | 2[0 - 4]\d | ((1\d{ 2})| ([1 - 9] ?\d)))\.){ 3} (25[0 - 5] | 2[0 - 4]\d | ((1\d{ 2})| ([1 - 9] ?\d)))");
        }
        /// <summary>
        /// 是否是合法字段名称
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool IsLegalFieldName(this string self)
        {
            if (string.IsNullOrEmpty(self)) return false;
            return Regex.IsMatch(self, @"^[_a-zA-Z][_a-zA-Z0-9]*$");
        }
        /// <summary>
        /// 移除中文
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static string RemoveChinese(this string self)
        {
            if (Regex.IsMatch(self, @"[\u4e00-\u9fa5]"))
            {
                string retValue = string.Empty;
                var strsStrings = self.ToCharArray();
                for (int index = 0; index < strsStrings.Length; index++)
                {
                    if (strsStrings[index] >= 0x4e00 && strsStrings[index] <= 0x9fa5) continue;
                    retValue += strsStrings[index];
                }
                return retValue;
            }
            else
                return self;
        }
        /// <summary>
        /// 移除字母
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static string Removeletters(this string self)
        {
            return Regex.Replace(self, "[a-zA_Z]", "", RegexOptions.IgnoreCase);
        }
        /// <summary>
        /// 移除数字
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static string RemoveNumbers(this string self)
        {
            return Regex.Replace(self, "[0,9]", "", RegexOptions.IgnoreCase);
        }
        /// <summary>
        /// 移除数字
        /// </summary>
        /// <param name="self"></param>
        /// <param name="minLen"></param>
        /// <param name="maxLen"></param>
        /// <returns></returns>
        public static string RemoveNumbers(this string self, int minLen, int maxLen)
        {
            return Regex.Replace(self, string.Format("[0,9]{2}{0},{1}{3}", minLen, maxLen,"{","}"), "", RegexOptions.IgnoreCase);
        }
        /// <summary>
        /// 移除数字
        /// </summary>
        /// <param name="self"></param>
        /// <param name="minLen"></param>
        /// <returns></returns>
        public static string RemoveNumbers(this string self, int minLen)
        {
            return Regex.Replace(self, string.Format("[0,9]{1}{0},{2}", minLen, "{", "}"), "", RegexOptions.IgnoreCase);
        }
        /// <summary>
        /// 留下数字
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static string RemoveNotNumber(this string self)
        {
            return Regex.Replace(self, @"[^\d]*", "", RegexOptions.IgnoreCase);
        }

    }

}
