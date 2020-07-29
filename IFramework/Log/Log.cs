namespace IFramework
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public class Log
    {
        public static int lev_L = 0;
        public static int lev_W = 0;
        public static int lev_E = 0;
        public static bool enable = true;
        public static bool enable_L = true;
        public static bool enable_W = true;
        public static bool enable_E = true;
        public static ILoger loger { get; set; }
        public static ILogRecorder recorder { get; set; }
        static Log()
        {
            loger = new CSLogger();
        }

        public static void L(object message, int lev = 0, params object[] paras)
        {
            if (!enable) return;
            if (!enable_L) return;
            if (lev_L > lev) return;
            if (loger!=null)
                loger.Log(LogType.Log, message, paras);
            if (recorder!=null)
                recorder.Log(LogType.Log, message, paras);
        }
        public static void W(object message, int lev = 0, params object[] paras)
        {
            if (!enable) return;
            if (!enable_W) return;
            if (lev_W > lev) return;
            if (loger!=null)
                loger.Log(LogType.Warning, message, paras);
            if (recorder != null)
                recorder.Log(LogType.Warning, message, paras);
        }
        public static void E(object message, int lev = 0, params object[] paras)
        {
            if (!enable) return;
            if (!enable_E) return;

            if (lev_E > lev) return;
            if (loger != null)
                loger.Log(LogType.Error, message, paras);
            if (recorder != null)
                recorder.Log(LogType.Error, message, paras);

        }

        public static void LF(object message, string format, int lev = 0, params object[] paras)
        {
            if (!enable) return;
            if (!enable_L) return;
            if (lev_L > lev) return;
            if (loger != null)
                loger.LogFormat(LogType.Log, format, message, paras);
            if (recorder != null)
                recorder.LogFormat(LogType.Log, format, message, paras);

        }
        public static void WF(object message, string format, int lev = 0, params object[] paras)
        {
            if (!enable) return;
            if (!enable_W) return;
            if (lev_W > lev) return;
            if (loger != null)
                loger.LogFormat(LogType.Warning, format, message, paras);
            if (recorder != null)
                recorder.LogFormat(LogType.Warning, format, message, paras);
        }
        public static void EF(object message, string format, int lev = 0, params object[] paras)
        {
            if (!enable) return;
            if (!enable_E) return;
            if (lev_E > lev) return;
            if (loger != null)
                loger.LogFormat(LogType.Error, format, message, paras);
            if (recorder != null)
                recorder.LogFormat(LogType.Error, format, message, paras);
        }

    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
}
