namespace IFramework
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public static class LogExtension
    {
        public static void Log(this string message, int lev = 0, params object[] paras)
        {
            IFramework.Log.L(message, lev, paras);
        }
        public static void Warning(this string message, int lev = 0, params object[] paras)
        {
            IFramework.Log.W(message, lev, paras);
        }
        public static void Err(this string message, int lev = 0, params object[] paras)
        {
            IFramework.Log.E(message, lev, paras);
        }

        public static void LogFormat(this string message, string format, int lev = 0, params object[] paras)
        {
            IFramework.Log.LF(message, format, lev, paras);
        }
        public static void WarningFormat(this string message, string format, int lev = 0, params object[] paras)
        {
            IFramework.Log.WF(message, format, lev, paras);
        }
        public static void ErrFormat(this string message, string format, int lev = 0, params object[] paras)
        {
            IFramework.Log.EF(message, format, lev, paras);
        }

      
    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

}
